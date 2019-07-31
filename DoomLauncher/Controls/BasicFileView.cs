using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public abstract class BasicFileView : UserControl, IFileAssociationView
    {
        protected ContextMenuStrip m_menu;
        private IFileData[] m_files = new IFileData[] { };

        public abstract void SetData(IGameFile gameFile);

        protected void SetData(DataGridView dgvMain, IGameFile gameFile)
        {
            if (GameFile != null && GameFile.GameFileID.HasValue)
            {
                IEnumerable<IFileData> files = DataSourceAdapter.GetFiles(gameFile, FileType);
                List<ISourcePortData> sourcePorts = Util.GetSourcePortsData(DataSourceAdapter);

                var items = from file in files
                            join sp in sourcePorts on file.SourcePortID equals sp.SourcePortID
                            select new { Description = file.Description, DateCreated = file.DateCreated, SourcePortName = sp.Name, FileData = file };

                dgvMain.DataSource = items.ToList();
                dgvMain.ContextMenuStrip = m_menu;

                m_files = files.ToArray();
            }
            else
            {
                dgvMain.DataSource = null;
                m_files = new IFileData[] { };
            }
        }

        public abstract void ClearData();

        public bool Delete()
        {
            List<IFileData> selectedFiles = GetSelectedFiles();

            if (selectedFiles.Count > 0 &&
                MessageBox.Show(this, "Delete selected file(s)?", "Confirm", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                foreach (IFileData file in selectedFiles)
                {
                    if (file.IsUrl)
                    {
                        //can't delete a remote file...
                    }
                    else
                    {
                        FileInfo fi = new FileInfo(Path.Combine(DataDirectory.GetFullPath(), file.FileName));
                        DataSourceAdapter.DeleteFile(file);
                        try
                        {
                            fi.Delete();
                        }
                        catch { }
                    }
                }

                return true;
            }

            return false;
        }

        public IGameFile GameFile { get; set; }

        public bool New()
        {
            GameFile = DataSourceAdapter.GetGameFile(GameFile.FileName); //todo: refactor may have broke this, or no longer needed
            return CreateFileAssociation(this, DataSourceAdapter, DataDirectory, FileType, GameFile, null).Count > 0;
        }

        public static List<IFileData> CreateFileAssociation(IWin32Window parent, IDataSourceAdapter adapter, LauncherPath directory, FileType type, IGameFile gameFile, ISourcePortData sourcePort)
        {
            List<IFileData> fileDataList = new List<IFileData>();
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog(parent) == DialogResult.OK)
            {
                FileDetailsEditForm detailsForm = new FileDetailsEditForm();
                detailsForm.Initialize(adapter);
                detailsForm.StartPosition = FormStartPosition.CenterParent;

                foreach (string file in dialog.FileNames)
                {
                    detailsForm.Description = Path.GetFileName(file);
                    if (sourcePort != null)
                        detailsForm.SourcePort = sourcePort;

                    if (detailsForm.ShowDialog(parent) == DialogResult.OK && detailsForm.SourcePort != null)
                    {
                        FileInfo fi = new FileInfo(file);
                        IFileData fileData = CreateNewFileDataSource(detailsForm, fi, type, gameFile);

                        fi.CopyTo(Path.Combine(directory.GetFullPath(), fileData.FileName));

                        adapter.InsertFile(fileData);
                        var fileSearch = adapter.GetFiles(gameFile, FileType.Demo).FirstOrDefault(x => x.FileName == fileData.FileName);
                        if (fileSearch != null) fileData = fileSearch;
                        fileDataList.Add(fileData);
                    }
                    else if (detailsForm.SourcePort == null)
                    {
                        MessageBox.Show(parent, "A source port must be selected.", "Error", MessageBoxButtons.OK);
                    }
                }
            }

            return fileDataList;
        }

        private static FileData CreateNewFileDataSource(FileDetailsEditForm detailsForm, FileInfo fi, FileType type, IGameFile gameFile)
        {
            FileData fileData = new FileData();
            fileData.FileName = string.Concat(Guid.NewGuid(), fi.Extension);
            fileData.FileTypeID = type;
            fileData.GameFileID = gameFile.GameFileID.Value;
            fileData.Description = detailsForm.Description;
            fileData.SourcePortID = detailsForm.SourcePort.SourcePortID;
            return fileData;
        }

        public bool Edit()
        {
            List<IFileData> selectedFiles = GetSelectedFiles();

            if (selectedFiles.Count > 0)
            {
                IFileData file = selectedFiles.First();
                FileDetailsEditForm form = new FileDetailsEditForm();
                form.Initialize(DataSourceAdapter, file);
                form.StartPosition = FormStartPosition.CenterParent;

                if (form.ShowDialog(this) == DialogResult.OK && form.SourcePort != null && !file.IsUrl)
                {
                    file.SourcePortID = form.SourcePort.SourcePortID;
                    file.Description = form.Description;
                    DataSourceAdapter.UpdateFile(file);
                    return true;
                }
                else if (form.SourcePort == null)
                {
                    MessageBox.Show(this, "A source port must be selected.", "Error", MessageBoxButtons.OK);
                }
            }

            return false;
        }

        public void CopyToClipboard()
        {
            List<IFileData> selectedFiles = GetSelectedFiles();

            if (selectedFiles.Count > 0)
            {
                StringCollection paths = new StringCollection();

                foreach (IFileData file in selectedFiles)
                {
                    if (file.IsUrl)
                    {
                        Process.Start(file.FileName);
                    }
                    else
                    {
                        FileInfo fi = new FileInfo(Path.Combine(DataDirectory.GetFullPath(), file.FileName));
                        if (fi.Exists)
                            paths.Add(Path.Combine(DataDirectory.GetFullPath(), file.FileName));

                        if (paths.Count > 0)
                            Clipboard.SetFileDropList(paths);
                    }
                }
            }
        }

        public void CopyAllToClipboard()
        {
            StringCollection paths = new StringCollection();

            foreach (IFileData dsFile in Files)
            {
                if (!dsFile.IsUrl)
                {
                    FileInfo fi = new FileInfo(Path.Combine(DataDirectory.GetFullPath(), dsFile.FileName));

                    if (fi.Exists)
                    {
                        paths.Add(Path.Combine(DataDirectory.GetFullPath(), dsFile.FileName));
                    }

                    if (paths.Count > 0)
                    {
                        Clipboard.SetFileDropList(paths);
                    }
                }
            }
        }

        public virtual void View()
        {
            List<IFileData> selectedFiles = GetSelectedFiles();

            if (selectedFiles.Count > 0)
            {
                IFileData file = selectedFiles.First();

                if (file.IsUrl)
                {
                    Process.Start(file.FileName);
                }
                else
                {
                    if (File.Exists(Path.Combine(DataDirectory.GetFullPath(), file.FileName)))
                        Process.Start(Path.Combine(DataDirectory.GetFullPath(), file.FileName));
                }
            }
        }

        public bool MoveFileOrderUp()
        {
            return SetFilePriority(true);
        }

        public bool MoveFileOrderDown()
        {
            return SetFilePriority(false);
        }

        public bool SetFileOrderFirst()
        {
            List<IFileData> selectedFiles = GetSelectedFiles();

            if (selectedFiles.Count > 0)
            {
                IFileData file = selectedFiles.First();
                if (!file.IsUrl)
                {
                    List<IFileData> files = Files.ToList();
                    files.Remove(file);
                    files.Insert(0, file);
                    SetFilePriorities(files);

                    foreach (IFileData fileUpdate in files)
                        DataSourceAdapter.UpdateFile(fileUpdate);

                    return true;
                }
            }

            return false;
        }

        private bool SetFilePriority(bool up) //todo fix for new file
        {
            List<IFileData> selectedFiles = GetSelectedFiles();

            if (selectedFiles.Count > 0)
            {
                IFileData file = selectedFiles.First();

                if (!file.IsUrl)
                {
                    List<IFileData> files = Files.ToList();
                    SetFilePriorities(files);

                    if (up && file.FileOrder > 0)
                        file.FileOrder--;

                    if (!up && file.FileOrder < files.Count - 1)
                        file.FileOrder++;

                    files.Remove(file);
                    files.Insert(file.FileOrder, file);

                    SetFilePriorities(files);

                    foreach (IFileData fileUpdate in files)
                        DataSourceAdapter.UpdateFile(fileUpdate);

                    return true;
                }
            }

            return false;
        }

        private void SetFilePriorities(List<IFileData> files)
        {
            int priority = 0;

            foreach (IFileData file in files)
            {
                file.FileOrder = priority++;
            }
        }

        public IDataSourceAdapter DataSourceAdapter { get; set; }
        public LauncherPath DataDirectory { get; set; }

        protected virtual IFileData[] Files
        {
            get { return m_files; }
        }

        public void SetContextMenu(ContextMenuStrip menu)
        {
            m_menu = menu;
        }

        protected abstract List<IFileData> GetSelectedFiles();

        public FileType FileType { get; set; }
        public virtual bool DeleteAllowed { get { return true; } }
        public virtual bool CopyAllowed { get { return true; } }
        public virtual bool NewAllowed { get { return true; } }
        public virtual bool EditAllowed { get { return true; } }
        public virtual bool ViewAllowed { get { return true; } }
        public virtual bool ChangeOrderAllowed { get { return true; } }
    }
}
