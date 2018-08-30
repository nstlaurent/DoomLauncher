using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private ProgressBarForm m_progressBarSync;

        private async Task SyncLocalDatabase(string[] fileNames)
        {
            m_progressBarSync = CreateProgressBar("Updating...", ProgressBarStyle.Continuous);
            ProgressBarStart(m_progressBarSync);

            SyncLibraryHandler handler = await Task.Run(() => ExecuteSyncHandler(fileNames));

            ProgressBarEnd(m_progressBarSync);
            SyncLocalDatabaseComplete(handler);
        }

        void SyncLocalDatabaseComplete(SyncLibraryHandler handler)
        {
            SetIWadGameFiles();

            UpdateLocal();
            HandleTabSelectionChange();

            if (handler != null &&
                (handler.InvalidFiles.Length > 0 || m_zdlInvalidFiles.Count > 0))
            {
                DisplayInvalidFilesError(handler.InvalidFiles.Union(m_zdlInvalidFiles));
            }
            else if (m_launchFile != null)
            {
                IGameFile launchFile = DataSourceAdapter.GetGameFile(Path.GetFileName(m_launchFile));
                m_launchFile = null;
                if (launchFile != null)
                    HandlePlay(new IGameFile[] { launchFile });
            }
        }

        private void SetIWadGameFiles()
        {
            IEnumerable<IIWadData> iwads = DataSourceAdapter.GetIWads();
            List<IGameFile> gameFileDataUpdate = new List<IGameFile>();

            if (iwads.Any())
            {
                IEnumerable<IGameFile> gameFiles = DataSourceAdapter.GetGameFiles();
                foreach (IIWadData iwad in iwads)
                {
                    IGameFile find = gameFiles.FirstOrDefault(x => x.FileName.ToLower() == iwad.FileName.ToLower().Replace(".wad", ".zip"));
                    if (find != null)
                    {
                        if (!find.IWadID.HasValue) //this should mean the file was just added so we should set the pre-defined title
                        {
                            FillIwadData(find);
                            gameFileDataUpdate.Add(find);
                            find.IWadID = iwad.IWadID;
                            DataSourceAdapter.UpdateGameFile(find, new GameFileFieldType[] { GameFileFieldType.IWadID });
                        }

                        if (!iwad.GameFileID.HasValue)
                        {
                            iwad.GameFileID = find.GameFileID;
                            DataSourceAdapter.UpdateIWad(iwad);
                        }                   
                    }
                    else
                    {
                        Util.ThrowDebugException("This should not happen");
                    }
                }
            }
        }

        private void DisplayInvalidFilesError(IEnumerable<InvalidFile> invalidFiles)
        {
            StringBuilder sb = new StringBuilder();

            foreach (InvalidFile file in invalidFiles)
            {
                sb.Append(file.FileName);
                sb.Append(":\t\t");
                sb.Append(file.Reason);
                sb.Append('\n');
            }

            ShowTextBoxForm("Processing Errors",
                "The information on these files may be incomplete.\n\nFor ZDL files adding the missing Source Port/IWAD name and re-adding will update the information.\n\nFor zip archive/pk3 errors: Doom Launcher uses a zip library that has very strict zip rules that not all applications respect.\n\nVerify the zip by opening it with your favorite zip application. Create a new zip file and extract the files from the original zip into the newly created one. Then add the newly created zip to Doom Launcher.", 
                sb.ToString(), true);
        }

        private SyncLibraryHandler ExecuteSyncHandler(string[] files)
        {
            SyncLibraryHandler handler = null;

            try
            {
                handler = new SyncLibraryHandler(DataSourceAdapter, DirectoryDataSourceAdapter,
                    AppConfiguration.GameFileDirectory, AppConfiguration.TempDirectory);
                handler.SyncFileChange += syncHandler_SyncFileChange;
                handler.GameFileDataNeeded += syncHandler_GameFileDataNeeded;

                handler.Execute(files);

                if (m_pendingZdlFiles != null)
                {
                    SyncPendingZdlFiles();
                    m_pendingZdlFiles = null;
                }
            }
            catch(Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }

            return handler;
        }

        private void SyncPendingZdlFiles()
        {
            foreach(IGameFile gameFile in m_pendingZdlFiles)
            {
                IGameFile libraryGameFile = DataSourceAdapter.GetGameFile(gameFile.FileName);

                if (libraryGameFile != null)
                {
                    libraryGameFile.SettingsSkill = gameFile.SettingsSkill;
                    libraryGameFile.SettingsMap = gameFile.SettingsMap;
                    libraryGameFile.SettingsExtraParams = gameFile.SettingsExtraParams;
                    libraryGameFile.SourcePortID = gameFile.SourcePortID;
                    libraryGameFile.IWadID = gameFile.IWadID;
                    libraryGameFile.SettingsSkill = gameFile.SettingsSkill;
                    libraryGameFile.SettingsFiles = gameFile.SettingsFiles;

                    if (string.IsNullOrEmpty(libraryGameFile.Comments))
                        libraryGameFile.Comments = gameFile.Comments;

                    DataSourceAdapter.UpdateGameFile(libraryGameFile);
                }
            }
        }
            
        private void FillIwadData(IGameFile gameFile)
        {
            FileInfo fi = new FileInfo(gameFile.FileName);

            if (string.IsNullOrEmpty(gameFile.Title))
            {
                switch (gameFile.FileName.Replace(fi.Extension, string.Empty).ToUpper())
                {
                    case "DOOM1":
                        gameFile.Title = "Doom Shareware";
                        break;
                    case "DOOM":
                        gameFile.Title = "The Ultimate Doom";
                        break;
                    case "DOOM2":
                        gameFile.Title = "Doom II: Hell on Earth";
                        break;
                    case "PLUTONIA":
                        gameFile.Title = "Final Doom: The Plutonia Experiment";
                        break;
                    case "TNT":
                        gameFile.Title = "Final Doom: TNT: Evilution";
                        break;
                    case "FREEDOOM1":
                        gameFile.Title = "Freedoom: Phase 1";
                        break;
                    case "FREEDOOM2":
                        gameFile.Title = "Freedoom: Phase 2";
                        break;
                    case "CHEX":
                        gameFile.Title = "Chex Quest";
                        break;
                    case "CHEX3":
                        gameFile.Title = "Chex Quest 3";
                        break;
                    case "HACX":
                        gameFile.Title = "Hacx: Twitch 'n Kill";
                        break;
                    case "HERETIC1":
                        gameFile.Title = "Heretic Shareware";
                        break;
                    case "HERETIC":
                        gameFile.Title = "Heretic: Shadow of the Serpent Riders";
                        break;
                    case "HEXEN":
                        gameFile.Title = "Hexen: Beyond Heretic";
                        break;
                    case "STRIFE0":
                        gameFile.Title = "Strife Demo";
                        break;
                    case "STRIFE1":
                        gameFile.Title = "Strife: Quest for the Sigil";
                        break;
                    default:
                        gameFile.Title = gameFile.FileName.Replace(fi.Extension, string.Empty);
                        break;
                }

                DataSourceAdapter.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.Title });
            }
        }

        void syncHandler_SyncFileChange(object sender, EventArgs e)
        {
            SyncLibraryHandler handler = sender as SyncLibraryHandler;

            if (handler != null)
            {
                if (InvokeRequired)
                    Invoke(new Action<SyncLibraryHandler>(ProgressBarUpdate), new object[] { handler });
                else
                    ProgressBarUpdate(handler);
            }
        }

        void syncHandler_GameFileDataNeeded(object sender, EventArgs e)
        {
            SyncLibraryHandler handler = sender as SyncLibraryHandler;

            if (handler != null)
            {
                if (InvokeRequired)
                    Invoke(new Action<SyncLibraryHandler>(HandleGameFileDataNeeded), new object[] { handler });
                else
                    HandleGameFileDataNeeded(handler);
            }
        }

        void HandleGameFileDataNeeded(SyncLibraryHandler handler)
        {
            if (CurrentDownloadFile != null && CurrentDownloadFile.FileName == handler.CurrentGameFile.FileName)
            {
                handler.CurrentGameFile.Title = CurrentDownloadFile.Title;
                handler.CurrentGameFile.Author = CurrentDownloadFile.Author;
                handler.CurrentGameFile.ReleaseDate = CurrentDownloadFile.ReleaseDate;
            }
        }

        void ProgressBarUpdate(SyncLibraryHandler handler)
        {
            if (m_progressBarSync != null)
            {
                m_progressBarSync.Maximum = handler.SyncFileCount;
                m_progressBarSync.Value = handler.SyncFileCurrent;
                m_progressBarSync.DisplayText = string.Format("Reading {0}...", handler.CurrentSyncFileName);
            }
        }

        void ProgressBarForm_Cancelled(object sender, EventArgs e)
        {
            this.Enabled = true;
            this.BringToFront();
        }

        private void SyncIWads(string[] files)
        {
            IEnumerable<string> iwads = DataSourceAdapter.GetIWads().Select(x => x.Name);
            IEnumerable<string> iwadsToAdd = files.Except(iwads);

            foreach (string file in iwadsToAdd)
            {
                try
                {
                    DataSourceAdapter.InsertIWad(new IWadData() { FileName = file, Name = file });
                }
                catch (Exception ex)
                {
                    Util.DisplayUnexpectedException(this, ex);
                }
            }
        }

        private async void HandleSyncStatus()
        {
            IEnumerable<string> dsFiles = DirectoryDataSourceAdapter.GetGameFileNames();
            IEnumerable<string> dbFiles = DataSourceAdapter.GetGameFileNames();
            IEnumerable<string> diff = dsFiles.Except(dbFiles);

            SyncStatusForm form = ShowSyncStatusForm("Sync Status", "Files that exist in the GameFiles directory but not the Database:", diff,
                new string[] { "Do Nothing", "Add to Library", "Delete" });
            Task task = HandleSyncStatusGameFilesOption((SyncFileOption)form.SelectedOptionIndex, form.GetSelectedFiles());
            await task;

            diff = dbFiles.Except(dsFiles);

            form = ShowSyncStatusForm("Sync Status", "Files that exist in the Database but not the GameFiles directory:", diff,
                new string[] { "Do Nothing", "Find in idgames", "Delete" });
            task = HandleSyncStatusLibraryOptions((SyncFileOption)form.SelectedOptionIndex, form.GetSelectedFiles());
            await task;
        }

        private async Task HandleSyncStatusGameFilesOption(SyncFileOption option, IEnumerable<string> files)
        {
            ProgressBarForm form = CreateProgressBar(string.Empty, ProgressBarStyle.Marquee);

            switch (option)
            {
                case SyncFileOption.Add:
                    m_progressBarSync = CreateProgressBar("Updating...", ProgressBarStyle.Continuous);
                    ProgressBarStart(m_progressBarSync);

                    SyncLibraryHandler handler = await Task.Run(() => ExecuteSyncHandler(files.ToArray()));

                    ProgressBarEnd(m_progressBarSync);
                    SyncLocalDatabaseComplete(handler);
                    break;

                case SyncFileOption.Delete:
                    form.DisplayText = "Deleting...";
                    ProgressBarStart(form);

                    await Task.Run(() => DeleteLocalGameFiles(files));

                    ProgressBarEnd(form);
                    break;

                default:
                    break;
            }
        }

        private void DeleteLocalGameFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                try
                {
                    File.Delete(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), file));
                }
                catch
                {
                    //failed, nothing to do
                }
            }
        }

        private async Task HandleSyncStatusLibraryOptions(SyncFileOption option, IEnumerable<string> files)
        {
            ProgressBarForm form = new ProgressBarForm();
            form.ProgressBarStyle = ProgressBarStyle.Marquee;

            switch (option)
            {
                case SyncFileOption.Add:
                    form.DisplayText = "Searching...";
                    ProgressBarStart(form);

                    List<IGameFile> gameFiles = await Task.Run(() => FindIdGamesFiles(files));
                    foreach (IGameFile gameFile in gameFiles)
                        m_downloadHandler.Download(IdGamesDataSourceAdapter, gameFile as IGameFileDownloadable);

                    ProgressBarEnd(form);
                    DisplayFilesNotFound(files, gameFiles);

                    if (gameFiles.Count > 0)
                        DisplayDownloads();

                    break;

                case SyncFileOption.Delete:
                    form.DisplayText = "Deleting...";
                    ProgressBarStart(form);

                    await Task.Run(() => DeleteLibraryGameFiles(files));

                    ProgressBarEnd(form);
                    UpdateLocal();
                    break;

                default:
                    break;
            }
        }

        private void DisplayFilesNotFound(IEnumerable<string> files, List<IGameFile> gameFiles)
        {
            IEnumerable<string> filesNotFound = files.Except(gameFiles.Select(x => x.FileName));

            if (filesNotFound.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (string file in filesNotFound)
                {
                    sb.Append(file);
                    sb.Append(Environment.NewLine);
                }

                TextBoxForm form = new TextBoxForm(true, MessageBoxButtons.OK);
                form.Text = "Files Not Found";
                form.HeaderText = "The following files were not found in the idgames database:";
                form.DisplayText = sb.ToString();
                form.ShowDialog(this);
            }
        }

        private List<IGameFile> FindIdGamesFiles(IEnumerable<string> files)
        {
            List<IGameFile> gameFiles = new List<IGameFile>();

            foreach (string file in files)
            {
                IGameFile gameFile = IdGamesDataSourceAdapter.GetGameFile(file);
                if (gameFile != null)
                    gameFiles.Add(gameFile);
            }

            return gameFiles;
        }

        private void DeleteLibraryGameFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                IGameFile gameFile = DataSourceAdapter.GetGameFile(file);
                if (gameFile != null && gameFile.GameFileID.HasValue)
                    DeleteGameFileAndAssociations(gameFile);
            }
        }

        private SyncStatusForm ShowSyncStatusForm(string title, string header, IEnumerable<string> files, IEnumerable<string> dropDownOptions)
        {
            SyncStatusForm form = new SyncStatusForm();
            form.Text = title;
            form.SetHeaderText(header);
            form.SetData(files, dropDownOptions);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
            return form;
        }
    }
}
