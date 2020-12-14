using DoomLauncher.Controls;
using DoomLauncher.DataSources;
using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using PresentationControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm : Form
    {
        public bool ShouldShowToolTip { get; private set; } = true;

        private readonly string m_workingDirectory;
        private bool m_playInProgress = false, m_idGamesLoaded;
        private IGameFile m_lastSelectedItem;
        private PlayForm m_currentPlayForm;
        private DownloadView m_downloadView;
        private DownloadHandler m_downloadHandler;
        private List<INewFileDetector> m_screenshotDetectors;
        private List<INewFileDetector> m_saveFileDetectors;
        private IFileData[] m_saveGames;
        private IStatisticsReader m_statsReader;
        private TabHandler m_tabHandler;
        private VersionHandler m_versionHandler;
        private readonly SplashScreen m_splash;
        private readonly UpdateControl m_updateControl = new UpdateControl();
        private readonly TagSelectControl m_tagSelectControl = new TagSelectControl();
        private Popup m_tagPopup;

        private string m_launchFile;
        private Dictionary<ITabView, GameFileSearchField[]> m_savedTabSearches = new Dictionary<ITabView, GameFileSearchField[]>();

        public MainForm(string launchFile)
        {
            Load += MainForm_Load;
            m_launchFile = launchFile;

            m_splash = new SplashScreen();
            m_splash.StartPosition = FormStartPosition.CenterScreen;
            m_splash.Show();
            m_splash.Invalidate();

            InitializeComponent();
            InitIcons();
            ClearSummary();

            m_workingDirectory = LauncherPath.GetDataDirectory();
        }

        private void InitIcons()
        {
            btnSearch.Image = Icons.Search;
            btnPlay.Image = Icons.Play;
            toolStripDropDownButton1.Image = Icons.Bars;
            btnDownloads.Image = Icons.Download;
            btnTags.Image = Icons.Tags;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            HandleLoad();
        }

        private async void HandleLoad()
        {
            bool success = false;

            if (VerifyDatabase())
            {
                string dataSource = Path.Combine(LauncherPath.GetDataDirectory(), DbDataSourceAdapter.DatabaseFileName);
                DataSourceAdapter = DbDataSourceAdapter.CreateAdapter();
                DataCache.Instance.Init(DataSourceAdapter);

                BackupDatabase(dataSource);
                CreateSendToLink();
                KillRunningApps();

                if (VerifyGameFilesDirectory())
                {
                    await Initialize();
                    success = true;
                }
            }

            if (!success)
                Close();

            await CheckFirstInit();
            UpdateLocal();

            SetupSearchFilters();

            InitWindow();
            HandleTabSelectionChange();
            InvokeHideSplashScreen();

            Task.Run(() => CheckForAppUpdate());
        }

        private void InvokeHideSplashScreen()
        {
            if (InvokeRequired)
                Invoke(new Action(HideSplashScreen));
            else
                HideSplashScreen();
        }

        private void HideSplashScreen()
        {
            m_splash?.Close();
        }

        private void InitWindow()
        {
            //Only set location and window state if the location is valid, either way we always set Width, Height, and splitter values
            if (ValidatePosition(AppConfiguration))
            {
                WindowState = AppConfiguration.WindowState;

                if (WindowState != FormWindowState.Maximized)
                {
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(AppConfiguration.AppX, AppConfiguration.AppY);
                }
            }

            // Save the height and set after splitter, otherwise splitter resizing will be incorrect
            int saveWidth = Width;
            int saveHeight = Height;

            Width = AppConfiguration.AppWidth;
            Height = AppConfiguration.AppHeight;

            splitTopBottom.SplitterDistance = AppConfiguration.SplitTopBottom;
            splitLeftRight.SplitterDistance = AppConfiguration.SplitLeftRight;
            splitTagSelect.SplitterDistance = AppConfiguration.SplitTagSelect;

            // If the app was closed in the maximized state then the width and height are maxed out
            // This causes the window to take up the full screen even when set to normal state
            if (WindowState == FormWindowState.Maximized)
            {
                Width = saveWidth;
                Height = saveHeight;
            }
        }

        private void KillRunningApps()
        {
            try
            {
                Process currentProc = Process.GetCurrentProcess();
                foreach (Process proc in Process.GetProcessesByName("DoomLauncher").Where(x => x.Id != currentProc.Id))
                    proc.CloseMainWindow();
            }
            catch
            {
                MessageBox.Show(this, "Doom Launcher is already running and could not be stopped. It is not recommended to have more than instance running!", "Already Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public IGameFileView GetCurrentViewControl()
        {
            ITabView view = GetCurrentTabView();

            if (view != null)
                return view.GameFileViewControl;

            return null;
        }

        private void ctrlAssociationView_FileAdded(object sender, EventArgs e)
        {
            IGameFileView view = GetCurrentViewControl();
            view.UpdateGameFile(view.SelectedItem);
            HandleSelectionChange(GetCurrentViewControl(), true);
        }

        void ctrlAssociationView_FileDeleted(object sender, EventArgs e)
        {
            IGameFileView view = GetCurrentViewControl();
            view.UpdateGameFile(view.SelectedItem);
            HandleSelectionChange(GetCurrentViewControl(), true);
        }

        void ctrlAssociationView_FileOrderChanged(object sender, EventArgs e)
        {
            IGameFileView view = GetCurrentViewControl();
            view.UpdateGameFile(view.SelectedItem);
            HandleSelectionChange(view, true);
        }

        void DownloadView_UserPlay(object sender, EventArgs e)
        {
            if (m_downloadView.SelectedItem != null)
            {
                IGameFile gameFile = DataSourceAdapter.GetGameFile(m_downloadView.SelectedItem.Key.ToString());
                if (gameFile != null)
                    HandlePlay(new IGameFile[] { gameFile });
            }
        }

        void ctrlView_SelectionChange(object sender, EventArgs e)
        {
            HandleSelectionChange(sender, false);
        }

        void ctrlView_RowDoubleClicked(object sender, EventArgs e)
        {
            HandleRowDoubleClicked(sender as IGameFileView);
        }

        void ctrlView_GridKeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPress(e);
        }

        private void HandleRowDoubleClicked(IGameFileView ctrl)
        {
            if (ctrl != null)
            {
                ITabView tabView = m_tabHandler.TabViewForControl(ctrl);

                if (tabView != null && tabView is IdGamesTabViewCtrl)
                    HandleDownload(AppConfiguration.TempDirectory);
                else if (tabView != null && tabView.IsPlayAllowed)
                    HandlePlay();
            }
        }

        private void HandleSearch()
        {
            if (GetCurrentViewControl() != null)
            {
                ITabView tabView = m_tabHandler.TabViewForControl(GetCurrentViewControl());

                if (tabView != null && tabView.IsSearchAllowed)
                {
                    if (string.IsNullOrEmpty(ctrlSearch.SearchText.Trim()))
                    {
                        tabView.SetGameFiles();
                        UpdateSavedTabSearch(tabView, null);
                    }
                    else
                    {
                        var searchFields = Util.SearchFieldsFromSearchCtrl(ctrlSearch);
                        UpdateSavedTabSearch(tabView, searchFields);
                        tabView.SetGameFiles(searchFields);
                    }
                }
            }
        }

        private void UpdateSavedTabSearch(ITabView tabView, GameFileSearchField[] searchFields)
        {
            if (searchFields == null)
                m_savedTabSearches.Remove(tabView);
            else
                m_savedTabSearches[tabView] = searchFields;
        }

        private void CleanTempDirectory()
        {
            DirectoryInfo dir = new DirectoryInfo(AppConfiguration.TempDirectory.GetFullPath());

            if (dir.Exists)
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        //failed, nothing to do
                    }
                }
            }
        }

        private void viewTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleViewTextFile();
        }

        private void openZipFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleOpenZipFile();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandlePlay();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            HandlePlay();
        }

        private void playNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandlePlay();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleDelete();
        }

        private void ToolStripDropDownButton1_DropDownOpened(object sender, EventArgs e)
        {
            ShouldShowToolTip = false;
        }

        private void ToolStripDropDownButton1_DropDownClosed(object sender, EventArgs e)
        {
            ShouldShowToolTip = true;
        }

        private void HandleRename()
        {
            IGameFile gameFile = SelectedItems(GetCurrentViewControl()).FirstOrDefault();

            if (gameFile != null)
            {
                bool success = false;
                TextBoxForm form = new TextBoxForm(false, MessageBoxButtons.OKCancel);
                form.SetMaxLength(48);
                form.DisplayText = gameFile.FileName;
                form.StartPosition = FormStartPosition.CenterParent;
                form.Text = string.Format("Rename {0}", gameFile.FileName);

                int idx = form.DisplayText.IndexOf('.');
                if (idx != -1)
                    form.SelectDisplayText(0, idx);

                while (!success && form.ShowDialog(this) == DialogResult.OK)
                {
                    success = RenameGameFile(gameFile, form.DisplayText);

                    idx = form.DisplayText.IndexOf('.');
                    if (idx != -1)
                        form.SelectDisplayText(0, idx);
                }
            }
        }

        private bool RenameGameFile(IGameFile gameFile, string fileName)
        {
            string error = null;

            bool valid = VerifyFileName(fileName);

            try
            {
                if (valid)
                {
                    if (!string.IsNullOrEmpty(fileName) && fileName != gameFile.FileName)
                        error = HandleRenameFile(gameFile, fileName, error);
                    else
                        error = "The new file name must be different and not empty.";
                }
                else
                {
                    error = "The entered file name is invalid.";
                }
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }

            if (error != null)
            {
                MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private string HandleRenameFile(IGameFile gameFile, string fileName, string error)
        {
            string oldFileName = gameFile.FileName;
            FileInfo fi = new FileInfo(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), gameFile.FileName));

            if (fi.Exists)
            {
                FileInfo fiCheck = new FileInfo(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), fileName));

                if (!fiCheck.Exists)
                {
                    fi.MoveTo(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), fileName));

                    IGameFile gameFileUpdate = DataSourceAdapter.GetGameFile(gameFile.FileName); //Need to to populate all info before updating
                    gameFile.FileName = fileName;
                    gameFileUpdate.FileName = fileName;
                    DataSourceAdapter.UpdateGameFile(gameFileUpdate, new GameFileFieldType[] { GameFileFieldType.Filename });
                    UpdateAdditinalFileReferences(oldFileName, fileName);
                    HandleSelectionChange(GetCurrentViewControl(), true);
                }
                else
                {
                    error = string.Format("The file {0} already exists.", fileName);
                }
            }
            else
            {
                error = string.Format("Could not find {0} to rename.", gameFile.FileName);
            }

            return error;
        }

        private void UpdateAdditinalFileReferences(string oldFileName, string newFileName)
        {
            GameFileFieldType[] updateFields = new GameFileFieldType[] { GameFileFieldType.SettingsFiles };
            GameFileGetOptions options = new GameFileGetOptions(new GameFileFieldType[] { GameFileFieldType.GameFileID, GameFileFieldType.SettingsFiles });
            var gameFiles = DataSourceAdapter.GetGameFiles(options).Where(x => x.SettingsFiles.Length > 0);

            foreach (var databaseGameFile in gameFiles)
            {
                string[] files = Util.SplitString(databaseGameFile.SettingsFiles);
                if (files.Contains(oldFileName))
                {
                    databaseGameFile.SettingsFiles = GetRenamedAdditionalFileSetting(files, oldFileName, newFileName);
                    DataSourceAdapter.UpdateGameFile(databaseGameFile, updateFields);
                }
            }

            var sourcePorts = DataSourceAdapter.GetSourcePorts();

            foreach (var databaseSourcePort in sourcePorts)
            {
                string[] files = Util.SplitString(databaseSourcePort.SettingsFiles);
                if (files.Contains(oldFileName))
                {
                    databaseSourcePort.SettingsFiles = GetRenamedAdditionalFileSetting(files, oldFileName, newFileName);
                    DataSourceAdapter.UpdateSourcePort(databaseSourcePort);
                }
            }
        }

        private static string GetRenamedAdditionalFileSetting(string[] files, string oldFileName, string newFileName)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == oldFileName)
                {
                    files[i] = newFileName;
                    break;
                }
            }

            return string.Join(";", files);
        }

        private static bool VerifyFileName(string fileName)
        {
            return fileName.Except(Path.GetInvalidFileNameChars()).Count() == fileName.Distinct().Count() &&
                fileName.Except(Path.GetInvalidPathChars()).Count() == fileName.Distinct().Count();
        }

        private void HandleViewTextFile()
        {
            if (GetCurrentViewControl() == null)
                return;

            IGameFile[] items = SelectedItems(GetCurrentViewControl());

            foreach (IGameFile item in items)
            {
                if (item != null && AssertFile(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), item.FileName)))
                {
                    using (IArchiveReader reader = ArchiveReader.Create(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), item.FileName)))
                    {
                        var entries = reader.Entries.Where(x => x.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase));

                        if (entries.Any())
                        {
                            var entry = entries.First();
                            if (entries.Count() > 1)
                            {
                                entry = null;
                                SimpleFileSelectForm form = new SimpleFileSelectForm();
                                form.Initialize(GetSortedTextFiles(item, entries));
                                form.StartPosition = FormStartPosition.CenterParent;
                                if (form.ShowDialog(this) == DialogResult.OK)
                                    entry = entries.FirstOrDefault(x => x.Name == form.SelectedFile);
                            }

                            if (entry != null)
                            {
                                string extractedFileName = Path.Combine(AppConfiguration.TempDirectory.GetFullPath(), entry.Name);
                                entry.ExtractToFile(extractedFileName, true);
                                Process.Start(extractedFileName);
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, "No text file found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private IEnumerable<string> GetSortedTextFiles(IGameFile item, IEnumerable<IArchiveEntry> entries)
        {
            FileInfo fi = new FileInfo(item.FileName);
            string baseFile = fi.Name.Replace(fi.Extension, string.Empty);

            var find = entries.Select(x => x.Name).ToList();
            var first = find.FirstOrDefault(x => x.StartsWith(baseFile, StringComparison.InvariantCultureIgnoreCase));

            if (first != null)
            {
                find.Remove(first);
                find.Insert(0, first);
            }

            return find;
        }

        private void HandleOpenZipFile()
        {
            if (GetCurrentViewControl() == null)
                return;

            IGameFile[] items = SelectedItems(GetCurrentViewControl());
            IGameFile lastFile = null;

            try
            {
                foreach (IGameFile item in items)
                {
                    if (item != null && AssertFile(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), item.FileName)) &&
                        Util.GetReadablePkExtensions().Contains(Path.GetExtension(item.FileName)))
                    {
                        lastFile = item;
                        Process.Start(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), item.FileName));
                    }
                }
            }
            catch
            {
                string filename = lastFile == null ? "the file" : lastFile.FileNameNoPath; 
                MessageBox.Show(this, $"Could not open {filename}", "Cannot Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleDelete()
        {
            if (GetCurrentViewControl() != null)
            {
                MessageCheckBox messageBox = null;
                ITabView tabView = m_tabHandler.TabViewForControl(GetCurrentViewControl());
                bool showDialog = true;
                bool update = false;

                if (tabView != null && tabView.IsDeleteAllowed && tabView.IsLocal)
                {
                    IGameFile[] items = SelectedItems(GetCurrentViewControl());

                    foreach (IGameFile gameFile in items)
                    {
                        if (showDialog)
                        {
                            messageBox = new MessageCheckBox("Confirm", string.Format("Delete {0} and all associated data?", gameFile.FileName),
                                string.Format("Do this for all {0} items", items.Length), SystemIcons.Question, MessageBoxButtons.OKCancel);
                            messageBox.SetShowCheckBox(items.Length > 1);
                        }

                        if (gameFile != null && (!showDialog || messageBox.ShowDialog(this) == DialogResult.OK))
                        {
                            try
                            {
                                DeleteGameFileAndAssociations(gameFile);
                                update = true;
                            }
                            catch
                            {
                                MessageBox.Show(this, string.Format("The file {0} appears to be in use and could not be deleted.", gameFile.FileName),
                                    "In Use", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        if (messageBox != null && messageBox.Checked)
                            showDialog = false;
                        if (messageBox != null && messageBox.Checked && messageBox.DialogResult == DialogResult.Cancel)
                            break;
                    }
                }

                if (update)
                {
                    GetCurrentViewControl().SelectedItem = null;
                    UpdateLocal();
                    HandleSelectionChange(GetCurrentViewControl(), true);
                }
            }
        }

        private void DeleteGameFileAndAssociations(IGameFile gameFile)
        {
            DeleteLocalFileAssociations(gameFile);

            IIWadData iwadFind = DataSourceAdapter.GetIWad(gameFile.GameFileID.Value);
            if (iwadFind != null)
                DataSourceAdapter.DeleteIWad(iwadFind);
            //note: appears sqlite we re-use deleted auto-inc ids so we have to be careful and delete everything
            if (!gameFile.IsUnmanaged())
                DirectoryDataSourceAdapter.DeleteGameFile(gameFile);
            DataSourceAdapter.DeleteGameFile(gameFile);
            if (gameFile.GameFileID.HasValue)
                DataSourceAdapter.DeleteStatsByFile(gameFile.GameFileID.Value);

            var tagMapping = DataSourceAdapter.GetTagMappings(gameFile.GameFileID.Value);
            tagMapping.ToList().ForEach(x => DataSourceAdapter.DeleteTagMapping(x));

            var profiles = DataSourceAdapter.GetGameProfiles(gameFile.GameFileID.Value);
            profiles.ToList().ForEach(x => DataSourceAdapter.DeleteGameProfile(x.GameProfileID));

            DataCache.Instance.TagMapLookup.RemoveGameFile(gameFile);
        }

        private void DeleteLocalFileAssociations(IGameFile gameFIle)
        {
            IEnumerable<IFileData> files = DataSourceAdapter.GetFiles(gameFIle);

            foreach (IFileData file in files)
            {
                string path = DirectoryForFileType(file.FileTypeID).GetFullPath();
                FileInfo fi = new FileInfo(Path.Combine(path, file.FileName));

                if (fi.Exists)
                    fi.Delete();

                DataSourceAdapter.DeleteFile(file);
            }
        }

        private LauncherPath DirectoryForFileType(FileType fileTypeID)
        {
            switch (fileTypeID)
            {
                case FileType.Screenshot:
                    return AppConfiguration.ScreenshotDirectory;
                case FileType.Demo:
                    return AppConfiguration.DemoDirectory;
                case FileType.SaveGame:
                    return AppConfiguration.SaveGameDirectory;
                case FileType.Thumbnail:
                    return AppConfiguration.ThumbnailDirectory;
                default:
                    throw new NotImplementedException();
            }
        }

        private void HandleSelectionChange(object sender, bool forceChange)
        {
            if (!(sender is IGameFileView))
                return;

            IGameFile item = null;
            IGameFile[] items = SelectedItems(GetCurrentViewControl());
            if (items.Length > 0)
                item = items.First();

            if (!forceChange && !AssertCurrentViewItem(item))
            {
                if (item == null)
                {
                    m_lastSelectedItem = null;
                    ctrlAssociationView.ClearData();
                    ClearSummary();
                    btnPlay.Enabled = false;
                }
                return;
            }

            if (GetCurrentTabView() is IdGamesTabViewCtrl)
                ctrlAssociationView.SetButtonsAllButtonsEnabled(false);
            else
                ctrlAssociationView.SetData(item);

            if (item != null)
            {
                IEnumerable<IStatsData> stats = new IStatsData[] { };

                if (item.GameFileID.HasValue)
                    stats = DataSourceAdapter.GetStats(item.GameFileID.Value);

                SetSummary(item);
                ctrlSummary.SetStatistics(item, stats);
            }
            else
            {
                btnPlay.Enabled = false;
                ctrlAssociationView.ClearData();
            }

            if (forceChange)
                GetCurrentViewControl().RefreshData();
        }

        private bool AssertCurrentViewItem(IGameFile item)
        {
            if (item == null || (m_lastSelectedItem != null && m_lastSelectedItem.Equals(item)))
            {
                return false;
            }

            m_lastSelectedItem = item;
            return true;
        }

        private void SetSummary(IGameFile item)
        {
            ctrlSummary.SetTitle(item.Title);
            ctrlSummary.SetDescription(item.Description);
            ctrlSummary.TagText = BuildTagText(item);
            ctrlSummary.SetTimePlayed(item.MinutesPlayed);
 
            if (!string.IsNullOrEmpty(item.Comments))
                ctrlSummary.SetComments(item.Comments);
            else
                ctrlSummary.ClearComments();

            List<string> imagePaths = new List<string>();
            if (item.GameFileID.HasValue)
            {
                foreach (var screenshot in DataSourceAdapter.GetFiles(item, FileType.Screenshot))
                    imagePaths.Add(Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), screenshot.FileName));
            }

            if (imagePaths.Count > 0)
                SetPreviewImages(imagePaths);
            else
                ctrlSummary.SetPreviewImage(DataCache.Instance.DefaultImage);
        }

        private void ClearSummary()
        {
            ctrlSummary.SetTitle(string.Empty);
            ctrlSummary.SetDescription(string.Empty);
            ctrlSummary.ClearPreviewImage();
            ctrlSummary.TagText = string.Empty;
            ctrlSummary.SetTimePlayed(0);
            ctrlSummary.ClearComments();
        }

        private void SetPreviewImages(List<string> imagePaths)
        {
            try
            {
                ctrlSummary.SetPreviewImages(imagePaths);
            }
            catch
            {
                ctrlSummary.SetPreviewImage(DataCache.Instance.DefaultImage);
            }
        }

        private string BuildTagText(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
            {
                IEnumerable<ITagData> tags = GetTagsFromFile(gameFile);

                if (tags.Any())
                    return string.Join(", ", tags.Select(x => x.Name));
            }

            return "N/A";
        }

        private IEnumerable<ITagData> GetTagsFromFile(IGameFile gameFile)
        {
            return DataCache.Instance.TagMapLookup.GetTags(gameFile);
        }

        private IGameFile[] SelectedItems(IGameFileView ctrl)
        {
            return ctrl.SelectedItems;
        }

        private void SetSelectedItem(IGameFileView ctrl, IGameFile gameFile)
        {
            IGameFile ctrlItem = ctrl.DataSource.FirstOrDefault(x => x.Equals(gameFile));

            if (ctrlItem != null)
                ctrl.SelectedItem = ctrlItem;
        }

        private void UpdateDataSourceViews(IGameFile gameFile)
        {
            foreach (ITabView tab in m_tabHandler.TabViews)
                tab.UpdateDataSourceFile(gameFile);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            HandleSearch();
        }

        private IGameFile CurrentDownloadFile { get; set; }

        private void HandleDownload(LauncherPath directory)
        {
            ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x is IdGamesTabViewCtrl);
            bool displayDownloads = false;

            if (tabView != null)
            {
                IGameFile[] dsItems = SelectedItems(tabView.GameFileViewControl);
                bool showAlreadyDownloading = true;
                bool doForAll = false;
                bool download = true;

                try
                {
                    foreach (IGameFile dsItem in dsItems)
                    {
                        IGameFileDownloadable dlItem = dsItem as IGameFileDownloadable;

                        if (dsItem != null && dlItem != null)
                        {
                            GameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.GameFileID,
                                ((IdGamesGameFile)dsItem).id.ToString()));

                            IGameFile dsItemFull = tabView.Adapter.GetGameFiles(options).FirstOrDefault();
                            dlItem = dsItemFull as IGameFileDownloadable;

                            if (!doForAll)
                                download = PromptUserDownload(dsItems, ref showAlreadyDownloading, ref doForAll, dlItem, dsItemFull, dsItems.Length > 1);

                            if (dlItem != null && download)
                            {
                                CurrentDownloadFile = dsItemFull;
                                dlItem.DownloadCompleted += dlItem_DownloadCompleted;
                                m_downloadHandler.DownloadDirectory = directory;
                                m_downloadHandler.Download(tabView.Adapter, dlItem);
                                displayDownloads = true;
                            }
                        }
                    }
                }
                catch (WebException)
                {
                    ShowBadConnectionError();
                }
            }

            if (displayDownloads)
                DisplayDownloads();
        }

        private void ShowBadConnectionError()
        {
            MessageBox.Show(this, "Unable to reach server. Lost connection?", "Bad Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool PromptUserDownload(IGameFile[] dsItems, ref bool showAlreadyDownloading, ref bool doForAll,
            IGameFileDownloadable dlItem, IGameFile dsItemFull, bool showCheckBox)
        {
            if (showAlreadyDownloading && m_downloadHandler.IsDownloading(dlItem))
            {
                MessageCheckBox messageBox = ShowAlreadyDownloading(dlItem, showCheckBox);
                showAlreadyDownloading = !messageBox.Checked;
            }

            if (!doForAll)
            {
                IGameFile dsCheck = DataSourceAdapter.GetGameFile(dsItemFull.FileName);

                if (dsCheck != null)
                {
                    MessageCheckBox messageBox = ShowAlreadyExists(dsItems, dsCheck, showCheckBox);
                    doForAll = messageBox.Checked;
                    if (messageBox.DialogResult == DialogResult.Cancel)
                        return false;
                }
            }

            return true;
        }

        private MessageCheckBox ShowAlreadyExists(IGameFile[] dsItems, IGameFile dsCheck, bool showCheckBox)
        {
            MessageCheckBox messageBox = new MessageCheckBox("Already Exists", string.Format("The file {0} already exists in the library. Continue Download?", dsCheck.FileName),
                string.Format("Do this for all {0} items", dsItems.Length), SystemIcons.Warning, MessageBoxButtons.OKCancel);
            messageBox.SetShowCheckBox(showCheckBox);
            messageBox.ShowDialog(this);
            return messageBox;
        }

        private MessageCheckBox ShowAlreadyDownloading(IGameFileDownloadable dlItem, bool showCheckBox)
        {
            MessageCheckBox messageBox = new MessageCheckBox("Already Downloading", string.Format("The file {0} is already downloading", dlItem.FileName),
                "Do not show this message again", SystemIcons.Error, MessageBoxButtons.OK);
            messageBox.SetShowCheckBox(showCheckBox);
            messageBox.ShowDialog(this);
            return messageBox;
        }

        private void HandleEdit()
        {
            IGameFileView ctrl = GetCurrentViewControl();

            if (ctrl != null)
            {
                ITabView tabView = m_tabHandler.TabViewForControl(ctrl);
                IGameFile[] gameFiles = SelectedItems(ctrl);

                if (CheckEdit(tabView, gameFiles))
                {
                    IGameFile gameFile = DataSourceAdapter.GetGameFile(gameFiles.First().FileName);
                    IEnumerable<ITagData> tags = GetTagsFromFile(gameFile);

                    GameFileEditForm form = new GameFileEditForm();
                    form.SetCopyFromFileAllowed(DataSourceAdapter, m_tabHandler.TabViews.FirstOrDefault(x => x.Key.Equals(TabKeys.LocalKey)));
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.EditControl.SetShowCheckBoxes(false);
                    form.EditControl.SetDataSource(gameFile, tags);

                    if (gameFiles.Length > 1)
                    {
                        form.Text = "*** Multiple Edit";
                        form.EditControl.SetShowCheckBoxes(true);
                        form.EditControl.SetCheckBoxesChecked(false);
                    }

                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        foreach (IGameFile updateGameFile in gameFiles)
                        {
                            form.EditControl.UpdateDataSource(updateGameFile);
                            if (form.TagsChanged || form.EditControl.TagsChanged)
                                DataCache.Instance.UpdateGameFileTags(new IGameFile[] { updateGameFile }, form.EditControl.TagData);
                            tabView.Adapter.UpdateGameFile(updateGameFile, Util.DefaultGameFileUpdateFields);
                            UpdateDataSourceViews(updateGameFile);
                        }

                        if (gameFiles.Any())
                            HandleSelectionChange(ctrl, true);
                    }
                }
            }
        }

        private static bool CheckEdit(ITabView tabView, IGameFile[] gameFiles)
        {
            return gameFiles.Length > 0 && tabView != null && tabView.IsEditAllowed;
        }

        private void HandleViewWebPage()
        {
            IGameFileView ctrl = GetCurrentViewControl();

            if (ctrl != null)
            {
                IGameFile dsItem = SelectedItems(ctrl).FirstOrDefault();

                if (dsItem != null && dsItem is IdGamesGameFile)
                {
                    try
                    {
                        GameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.GameFileID,
                                ((IdGamesGameFile)dsItem).id.ToString()));
                        IdGamesGameFile dsItemFull = IdGamesDataSourceAdapter.GetGameFiles(options).FirstOrDefault() as IdGamesGameFile;

                        if (dsItemFull != null)
                            Process.Start(string.Format("{0}?file={1}{2}", AppConfiguration.IdGamesUrl, dsItemFull.dir, dsItemFull.FileName));
                    }
                    catch (WebException)
                    {
                        ShowBadConnectionError();
                    }
                }
            }
        }

        private void dlItem_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            HandleDownloadComplete(sender, e);
        }

        private async void HandleDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                IGameFileDownloadable dlFile = sender as IGameFileDownloadable;

                if (e.Error != null)
                {
                    MessageBox.Show(this, e.Error.Message + "\n\nIf this error keeps occuring try chaning your mirror in the Settings menu.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (dlFile != null)
                        await WriteDownloadFile(dlFile);
                }

                try
                {
                    IDisposable dlDispose = sender as IDisposable;
                    if (dlDispose != null)
                        dlDispose.Dispose();
                }
                catch (Exception ex)
                {
                    Util.DisplayUnexpectedException(this, ex);
                }
            }
        }

        private async Task WriteDownloadFile(IGameFileDownloadable dlFile)
        {
            try
            {
                FileInfo fi = new FileInfo(Path.Combine(AppConfiguration.TempDirectory.GetFullPath(), dlFile.FileName));
                fi.CopyTo(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), dlFile.FileName), true);
                fi.Delete();

                await SyncLocalDatabase(new string[] { fi.Name }, FileManagement.Managed, true);
            }
            catch (IOException)
            {
                MessageBox.Show(this, string.Format("The file {0} is in use and cannot be written.", dlFile.FileName),
                    "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleDownload(AppConfiguration.TempDirectory);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleTabSelectionChange();
        }

        private ITabView m_lastSelectedTabView;

        private void HandleTabSelectionChange()
        {
            if (tabControl.SelectedTab != null)
            {
                if (m_lastSelectedTabView != null)
                    m_lastSelectedTabView.GameFileViewControl.SetVisible(false);

                ITabView tabView = GetCurrentTabView();

                if (tabView != null)
                {
                    m_lastSelectedTabView = tabView;
                    btnSearch.Enabled = tabView.IsSearchAllowed;
                    btnPlay.Enabled = tabView.IsPlayAllowed;
                    chkAutoSearch.Enabled = tabView.IsAutoSearchAllowed;

                    if (tabView is IdGamesTabViewCtrl && !m_idGamesLoaded)
                    {
                        tabView.SetGameFiles();
                        m_idGamesLoaded = true;
                    }

                    tabView.GameFileViewControl.Focus();
                    tabView.GameFileViewControl.SetVisible(true);
                    AppConfiguration.LastSelectedTabIndex = tabControl.SelectedIndex;
                    if (tabControl.SelectedTab != null)
                        lblSelectedTag.Text = tabControl.SelectedTab.Text;
                    HandleSelectionChange(tabView.GameFileViewControl, false);
                }
            }
        }

        private ITabView GetCurrentTabView()
        {
            return tabControl.SelectedTab.Controls[0] as ITabView;
        }

        private void sourcePortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleEditSourcePorts(false);
        }

        private void viewWebPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleViewWebPage();
        }

        private void btnDownloads_Click(object sender, EventArgs e)
        {
            DisplayDownloads();
        }

        private void btnTags_Click(object sender, EventArgs e)
        {
            DisplayTags();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DisplayUpdate();
        }

        private void HandleEditSourcePorts(bool initSetup)
        {
            SourcePortViewForm form = new SourcePortViewForm(DataSourceAdapter, AppConfiguration, GetAdditionalTabViews().ToArray(), SourcePortLaunchType.SourcePort);
            form.StartPosition = FormStartPosition.CenterParent;
            form.SourcePortLaunched += form_SourcePortLaunched;

            if (initSetup)
                form.DisplayInitSetupButton();

            form.ShowDialog(this);
            HandleSelectionChange(GetCurrentViewControl(), true);
        }

        private void HandleEditUtilities()
        {
            SourcePortViewForm form = new SourcePortViewForm(DataSourceAdapter, AppConfiguration, GetAdditionalTabViews().ToArray(), SourcePortLaunchType.Utility);
            form.ShowPlayButton(false);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);

            RebuildUtilityToolStrip();
        }

        void form_SourcePortLaunched(object sender, EventArgs e)
        {
            SourcePortViewForm form = sender as SourcePortViewForm;

            if (form != null && form.GetSelectedSourcePort() != null)
                HandlePlay(null, form.GetSelectedSourcePort());
        }

        private void DisplayDownloads()
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            Popup popup = new Popup(m_downloadView)
            {
                Width = dpiScale.ScaleIntX(300),
                Height = m_downloadView.Height
            };
            popup.Show(btnDownloads);
        }

        private void DisplayUpdate()
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            Popup popup = new Popup(m_updateControl)
            {
                Width = dpiScale.ScaleIntX(300),
                Height = m_updateControl.Height
            };
            popup.Show(btnUpdate);
        }

        private void DisplayTags()
        {
            if (m_tagSelectControl.Pinned)
                return;

            DpiScale dpiScale = new DpiScale(CreateGraphics());
            m_tagSelectControl.ClearSelections();
            m_tagPopup = new Popup(m_tagSelectControl)
            {
                Width = dpiScale.ScaleIntX(AppConfiguration.SplitTagSelect),
                Height = Height - PointToClient(btnTags.PointToScreen(btnTags.Location)).Y - btnTags.Height - dpiScale.ScaleIntY(40)
            };
            m_tagPopup.Show(btnTags);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool success = false;
            bool firstRun = true;
            DialogResult result;

            do
            {
                success = ShowSettings(firstRun, out result);
                firstRun = false;
            }
            while (!success);
        }

        private bool ShowSettings(bool allowCancel, out DialogResult result)
        {
            SettingsForm form = new SettingsForm(DataSourceAdapter, AppConfiguration);
            form.SetCancelAllowed(allowCancel);
            form.StartPosition = FormStartPosition.CenterParent;
            result = form.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                try
                {
                    AppConfiguration.Refresh();
                }
                catch (DirectoryNotFoundException ex)
                {
                    MessageBox.Show(this, string.Format("The directory {0} was not found. DoomLauncher will not operate correctly with invalid paths. " +
                        "Make sure the directory you are setting contains all folders required (Demos, SaveGames, Screenshots, Temp)", ex.Message),
                        "Invalid Directory",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    Util.DisplayUnexpectedException(this, ex);
                    return false;
                }

                RefreshConfigItems();
                HandleSelectionChange(GetCurrentViewControl(), true);
            }

            return true;
        }

        private void RefreshConfigItems()
        {
            IdGamesDataSourceAdapter = new IdGamesDataAdapater(AppConfiguration.IdGamesUrl, AppConfiguration.ApiPage, AppConfiguration.MirrorUrl);

            if (m_tabHandler != null)
            {
                ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x is IdGamesTabViewCtrl);
                tabView.Adapter = IdGamesDataSourceAdapter;
            }

            ctrlAssociationView.Initialize(DataSourceAdapter, AppConfiguration);
            SetShowTabHeaders();
        }

        private void CtrlAssociationView_RequestScreenshots(object sender, RequestScreenshotsEventArgs e)
        {
            List<IFileData> screens = DataSourceAdapter.GetFiles(e.GameFile, FileType.Screenshot).ToList();
            ctrlAssociationView.SetScreenshots(screens);
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAddFiles();
        }

        private void HandleAddFiles()
        {
            HandleAddFiles(AddFileType.GameFile, new string[] { "Zip", "WAD", "pk3", "txt", "zdl" }, "Select Game Files");
        }

        private void addIWADsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAddIWads();
        }

        private async Task HandleAddIWads()
        {
            await HandleAddFiles(AddFileType.IWad, new string[] { "WAD" }, "Select IWADs");
        }

        private void UpdateLocal()
        {
            foreach (ITabView tab in m_tabHandler.TabViews)
                UpdateLocalTabData(tab);
        }

        private void UpdateLocalTabData(ITabView tab)
        {
            if (!tab.IsLocal)
                return;
            
            if (m_savedTabSearches.ContainsKey(tab))
                tab.SetGameFiles(m_savedTabSearches[tab]);
            else
                tab.SetGameFiles();
        }

        private IGameFile[] m_pendingZdlFiles;

        private async Task HandleAddGameFiles(AddFileType type, string[] files, ITagData tag = null)
        {
            if (!VerifyAddFiles(type, files))
                return;

            List<string> libraryFiles = new List<string>(files);
            string[] zdlFiles = GetZdlFiles(files).ToArray();
            libraryFiles = libraryFiles.Except(zdlFiles).ToList();

            string[] zdlLibraryFiles = HandleZdlFiles(zdlFiles);
            libraryFiles.AddRange(zdlLibraryFiles);
            //only launch zdl file if it's the only one
            if (m_launchFile == null && zdlLibraryFiles.Length == 1 && libraryFiles.Count == 1)
            {
                FileInfo fi = new FileInfo(zdlLibraryFiles[0]);
                m_launchFile = fi.Name.Replace(fi.Extension, ".zip");
            }

            string[] missingFiles = libraryFiles.Where(x => !File.Exists(x)).ToArray();

            if (missingFiles.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                Array.ForEach(missingFiles, x => sb.Append(x + "\n"));
                MessageBox.Show(this, "The following files were not found and will not be added:" + Environment.NewLine + Environment.NewLine +
                    sb.ToString(), "Files Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                libraryFiles = libraryFiles.Except(missingFiles).ToList();
            }

            if (libraryFiles.Count > 0)
            {
                await HandleCopyFiles(type, libraryFiles.ToArray(), GetUserSelectedFileManagement(), tag);
            }
            else if (m_zdlInvalidFiles.Count > 0)
            {
                DisplayInvalidFilesError(m_zdlInvalidFiles);
            }
        }

        private bool VerifyAddFiles(AddFileType type, string[] files)
        {
            List<string> warnFiles = new List<string>();

            foreach (string file in files)
            {
                IWadInfo info = IWadInfo.GetIWadInfo(file);

                if (type == AddFileType.GameFile && info != null)
                    warnFiles.Add(Path.GetFileName(file));
                else if (type == AddFileType.IWad && info == null)
                    warnFiles.Add(Path.GetFileName(file));
            }

            if (warnFiles.Count > 0)
            {
                StringBuilder warn = new StringBuilder();
                if (type == AddFileType.GameFile)
                    warn.Append("The following file(s) were detected be IWADS and are being added as game files:");
                else
                    warn.Append("The following files(s) were not detected to be IWADS and are being added as IWADS:");

                warn.AppendLine();
                warn.Append(string.Join(", ", warnFiles));
                warn.Append("\n\nContinue?");

                TopMost = true;
                bool ret = MessageBox.Show(this, warn.ToString(), "File Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
                TopMost = false;
                return ret;
            }

            return true;
        }

        private async Task HandleAddFiles(AddFileType type, string[] extensions, string dialogTitle)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = dialogTitle;
            dialog.Multiselect = true;
            dialog.Filter = GetDialogFilter("Game Files", extensions);

            if (dialog.ShowDialog(this) == DialogResult.OK)
                await HandleAddGameFiles(type, dialog.FileNames);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private async Task HandleCopyFiles(AddFileType type, string[] fileNames, FileManagement fileManagement, ITagData tag)
        {
            ProgressBarForm progressBar = CreateProgressBar("Copying...", ProgressBarStyle.Marquee);
            progressBar.Cancelled += m_progressBarFormCopy_Cancelled;

            ProgressBarStart(progressBar);

            FileAddResults fileAddResults = new FileAddResults();

            if (fileManagement == FileManagement.Managed)
                await Task.Run(() => fileAddResults = CopyFiles(fileNames, AppConfiguration.GameFileDirectory.GetFullPath(), progressBar));
            else
                fileAddResults = UnmanagedAddCheck(fileNames, AppConfiguration.GameFileDirectory.GetFullPath());

            string[] files = fileAddResults.GetAllFiles().ToArray();

            ProgressBarEnd(progressBar);

            switch (type)
            {
                case AddFileType.GameFile:
                    await SyncLocalDatabase(files, fileManagement, true, tag);
                    break;
                case AddFileType.IWad:
                    await SyncLocalDatabase(files, fileManagement, false);
                    SyncIWads(fileAddResults);
                    break;
                default:
                    break;
            }

            if (fileAddResults.Errors.Count > 0)
            {
                string start = fileAddResults.Errors.Count > 1 ? string.Concat("Errors:", Environment.NewLine) : string.Empty;
                string tab = fileAddResults.Errors.Count > 1 ? "\t" : string.Empty;
                StringBuilder sb = new StringBuilder(start);
                fileAddResults.Errors.ForEach(x => sb.Append(string.Concat(tab, x.FileName, ": ", x.Error, Environment.NewLine)));
                MessageBox.Show(this, sb.ToString(), "Failed to Add", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private FileAddResults UnmanagedAddCheck(string[] fileNames, string directory)
        {
            FileAddResults results = new FileAddResults();

            string managedError = "File already exists as a managed file.";
            string unmanagedError = "File already exists as an unmanaged file.";

            foreach (string fileName in fileNames)
            {
                string zipName = Path.Combine(directory, Path.GetFileNameWithoutExtension(fileName) + ".zip");
                IGameFile existingGameFile = DataSourceAdapter.GetGameFile(fileName);

                if (File.Exists(zipName))
                {
                    results.Errors.Add(new FileError { FileName = fileName, Error = managedError  });
                }
                else if (existingGameFile != null)
                {
                    if (Path.IsPathRooted(existingGameFile.FileName))
                        results.Errors.Add(new FileError { FileName = fileName, Error = unmanagedError });
                    else
                        results.NewFiles.Add(fileName);
                    //else
                    //    results.Errors.Add(new FileError { FileName = fileName, Error = managedError });
                }
                else
                {
                    results.NewFiles.Add(fileName);
                }
            }

            return results;
        }

        private FileManagement GetUserSelectedFileManagement()
        {
            FileManagement fileManagement = AppConfiguration.FileManagement;

            if (fileManagement == FileManagement.Prompt)
            {
                FileManagementSelect select = new FileManagementSelect
                {
                    StartPosition = FormStartPosition.CenterParent
                };
                TopMost = true;
                select.ShowDialog(this);
                TopMost = false;
                fileManagement = select.GetSelectedFileManagement();
            }

            return fileManagement;
        }

        private List<InvalidFile> m_zdlInvalidFiles = new List<InvalidFile>();

        private string[] HandleZdlFiles(string[] files)
        {
            m_zdlInvalidFiles = new List<InvalidFile>();
            List<string> libraryFiles = new List<string>();
            List<IGameFile> pendingGameFiles = new List<IGameFile>();
            ZdlParser parser = new ZdlParser(DataSourceAdapter.GetSourcePorts(), DataSourceAdapter.GetIWads());

            foreach (string file in files)
            {
                IGameFile[] gameFiles = parser.Parse(File.ReadAllText(file));

                foreach (IGameFile gameFile in gameFiles)
                {
                    FileInfo fi = new FileInfo(gameFile.FileName);

                    libraryFiles.Add(gameFile.FileName);
                    gameFile.FileName = fi.Name.Replace(fi.Extension, ".zip"); //set to name only, doomlauncher uses zip extension
                    pendingGameFiles.Add(gameFile);

                    if (gameFile == gameFiles.First()) //first file is the 'launch' file and rest are 'additional files' as far as we are concerned
                    {
                        StringBuilder sb = new StringBuilder();
                        Array.ForEach(gameFiles.Select(x => Path.GetFileName(x.FileName).Replace(Path.GetExtension(x.FileName), ".zip")).ToArray(),
                            x => sb.Append(x + ';'));
                        sb.Remove(sb.Length - 1, 1);
                        gameFile.SettingsFiles = sb.ToString();
                    }
                }

                if (parser.Errors.Length > 0)
                    m_zdlInvalidFiles.Add(new InvalidFile(file, string.Join(", ", parser.Errors)));
            }

            m_pendingZdlFiles = pendingGameFiles.ToArray();
            return libraryFiles.ToArray();
        }

        private IEnumerable<string> GetZdlFiles(string[] files)
        {
            return files.Where(x => Path.GetExtension(x).Equals(".zdl", StringComparison.OrdinalIgnoreCase));
        }

        private string GetDialogFilter(string name, string[] extensions)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string ext in extensions)
            {
                sb.Append(string.Format("*.{0};", ext.ToLower()));
            }

            sb.Remove(sb.Length - 1, 1);

            return string.Format("{0} ({1})|{1}|All Files (*.*)|*.*", name, sb.ToString());
        }

        private FileAddResults CopyFiles(string[] files, string directory, ProgressBarForm progressBar)
        {
            FileAddResults results = new FileAddResults();
            HashSet<string> addedNames = new HashSet<string>();

            List<string> fileNames = files.ToList();
            fileNames.Sort();
            int count = 0;
            bool promptOverwrite = true;
            bool overwrite = false;

            foreach (string file in fileNames)
            {
                if (progressBar != null)
                    UpdateProgressBar(progressBar, string.Format("Copying {0}...", file), Convert.ToInt32(count / (double)fileNames.Count * 100));
                FileInfo fi = new FileInfo(file);
                string baseName = fi.Name.Replace(fi.Extension, string.Empty);

                if (!IsZipFile(fi) && addedNames.Contains(baseName)) //archive with this name exists, add the file (match .txt with .wad etc)
                    AddEntryToExistingFile(directory, file, fi, baseName);

                if (!addedNames.Contains(baseName))
                    addedNames.Add(baseName);

                string zipName = Path.Combine(directory, fi.Name);
                bool isZip = IsZipFile(fi);

                if (!isZip)
                    zipName = Path.Combine(directory, baseName + ".zip");

                IEnumerable<string> existingFileNames = DataSourceAdapter.GetGameFileNames();

                try
                {
                    string existingFile = existingFileNames.FirstOrDefault(x => Path.GetFileName(x).Equals(fi.Name));

                    if (existingFile != null && Path.IsPathRooted(existingFile))
                    {
                        results.Errors.Add(new FileError { FileName = baseName, Error = "File already exists as an unmanaged file." });
                    }
                    else if (File.Exists(zipName))
                    {
                        if (promptOverwrite)
                        {
                            Tuple<bool, bool> result = PromptCopyFileOverwrite(baseName);
                            overwrite = result.Item1;
                            promptOverwrite = !result.Item2;
                        }

                        if (overwrite)
                        {
                            results.ReplacedFiles.Add(baseName + ".zip");
                            if (isZip)
                                fi.CopyTo(zipName, true);
                            else
                                HandleNonZipReplacement(fi, zipName);
                        }
                    }
                    else
                    {
                        results.NewFiles.Add(baseName + ".zip");

                        if (IsZipFile(fi))
                        {
                            fi.CopyTo(Path.Combine(directory, fi.Name));
                        }
                        else
                        {
                            string newZipName = Path.Combine(directory, baseName + ".zip");
                            AddZipEntry(file, fi.Name, newZipName);
                        }
                    }
                }
                catch (IOException)
                {
                    results.Errors.Add(new FileError { FileName = baseName, Error = "File is in use." });
                }
                catch (Exception ex)
                {
                    results.Errors.Add(new FileError { FileName = baseName, Error = string.Concat("Unknown error: ", ex.HResult) }); //Shouldn't happen
                }

                count++;
            }

            return results;
        }

        private static void AddZipEntry(string file, string name, string newZipName)
        {
            using (ZipArchive za = ZipFile.Open(newZipName, ZipArchiveMode.Create))
            {
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var entry = za.CreateEntry(name);
                    using (var destStream = entry.Open())
                        fileStream.CopyTo(destStream);
                }
            }
        }

        private void HandleNonZipReplacement(FileInfo fi, string zipName)
        {
            string tempFile = Path.Combine(AppConfiguration.TempDirectory.GetFullPath(), fi.Name);
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            fi.CopyTo(tempFile);

            using (ZipArchive za = ZipFile.Open(zipName, File.Exists(zipName) ? ZipArchiveMode.Update : ZipArchiveMode.Create))
            {
                var existingEntries = za.Entries.Where(x => x.Name == fi.Name).ToArray();

                if (existingEntries.Any())
                {
                    foreach (var entry in existingEntries)
                    {
                        entry.Delete();
                        za.CreateEntryFromFile(tempFile, Path.Combine(Path.GetDirectoryName(entry.FullName), fi.Name));
                    }
                }
                else
                {
                    za.CreateEntryFromFile(tempFile, fi.Name);
                }
            }
        }

        private static void AddEntryToExistingFile(string directory, string file, FileInfo fi, string baseName)
        {
            string newZipName = Path.Combine(directory, baseName, ".zip");

            if (File.Exists(newZipName))
            {
                using (ZipArchive za = ZipFile.Open(newZipName, ZipArchiveMode.Update))
                {
                    if (!za.Entries.Any(x => x.Name == fi.Name)) //make sure this file does not already exist in the archive 
                        za.CreateEntryFromFile(file, fi.Name);
                }
            }
        }

        private Tuple<bool, bool> PromptCopyFileOverwrite(string file)
        {
            if (InvokeRequired)
            {
                return (Tuple<bool, bool>)Invoke(new Func<string, Tuple<bool, bool>>(PromptCopyFileOverwrite), new object[] { file });
            }
            else
            {
                MessageCheckBox msg = new MessageCheckBox("Overwrite", string.Format("The file {0} already exists. Overwrite?", file),
                    "Accept For All", SystemIcons.Question, MessageBoxButtons.OKCancel);

                if (msg.ShowDialog() == DialogResult.OK)
                    return new Tuple<bool, bool>(true, msg.Checked);

                return new Tuple<bool, bool>(false, msg.Checked);
            }
        }

        private static bool IsZipFile(FileInfo fi)
        {
            return fi.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase);
        }

        void m_progressBarFormCopy_Cancelled(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateProgressBar(ProgressBarForm form, string text, int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ProgressBarForm, string, int>(UpdateProgressBar), new object[] { form, text, value });
            }
            else
            {
                form.DisplayText = text;
                form.Value = value;
            }
        }

        private void ctrlView_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is IGameFileView ctrl && e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                ITagData tag = null;
                if (m_tabHandler.TabViewForControl(ctrl) is TagTabView tagTabView)
                    tag = tagTabView.TagDataSource;

                if (ctrl.DoomLauncherParent != null && ctrl.DoomLauncherParent is IWadTabViewCtrl)
                    HandleAddGameFiles(AddFileType.IWad, files);
                else
                    HandleAddGameFiles(AddFileType.GameFile, files, tag);
            }
        }

        private void ctrlView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            HandleFormClosing();
        }

        private void MnuLocal_Opening(object sender, CancelEventArgs e)
        {
            RebuildTagToolStrip();

            if (!(GetCurrentViewControl() is IGameFileSortableView sortableView))
                return;

            ToolStripMenuItem sortToolStrip = mnuLocal.Items.Cast<ToolStripItem>().FirstOrDefault(x => x.Text == "Sort By") as ToolStripMenuItem;
            IGameFileView view = GetCurrentViewControl();
            sortToolStrip.Visible = view is GameFileTileViewControl;

            for (int i = 0; i < GameFileViewFactory.DefaultColumnTextFields.Length; i++)
            {
                ColumnField columnField = GameFileViewFactory.DefaultColumnTextFields[i];
                string text = columnField.Title;

                if (columnField.DataKey.Equals(sortableView.GetSortedColumnKey(), StringComparison.InvariantCultureIgnoreCase) && sortableView.GetColumnSort(sortableView.GetSortedColumnKey()) != SortOrder.None)
                {
                    if (sortableView.GetColumnSort(sortableView.GetSortedColumnKey()) == SortOrder.Ascending)
                        text += " ▲";
                    else
                        text += " ▼";
                }

                sortToolStrip.DropDownItems[i].Text = text;
            }
        }

        private void newTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleManageTags();
        }

        private void manageTagsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HandleManageTags();
        }

        private void manageTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleManageTags();
        }

        private void sortToolStripItem_Click(object sender, EventArgs e)
        {
            if (!(GetCurrentViewControl() is IGameFileSortableView sortableView))
                return;

            IGameFileView view = GetCurrentViewControl();
            ToolStripItem strip = sender as ToolStripItem;
            ToolStripMenuItem sortToolStrip = GetSortByToolStrip();

            int index = 0;
            for (int i = 0; i < sortToolStrip.DropDownItems.Count; i++)
            {
                if (sortToolStrip.DropDownItems[i] == strip)
                {
                    index = i;
                    break;
                }
            }

            ColumnField columnField = GameFileViewFactory.DefaultColumnTextFields[index];
            SortOrder sortOrder;

            if (sortableView.GetColumnSort(columnField.DataKey) == SortOrder.Descending)
                sortOrder = SortOrder.Ascending;
            else
                sortOrder = SortOrder.Descending;

            sortableView.SetSortedColumn(columnField.DataKey, sortOrder);
            view.DataSource = GetViewSort(view, view.DataSource);
        }

        private void TabView_DataSourceChanging(object sender, GameFileListEventArgs e)
        {
            if (sender is ITabView tabView)
            {
                if (!(sender is IWadTabViewCtrl))
                    e.GameFiles = RemoveExcludeTags(tabView, e.GameFiles);
                e.GameFiles = GetViewSort(tabView.GameFileViewControl, e.GameFiles);
            }
        }

        private IEnumerable<IGameFile> RemoveExcludeTags(ITabView tabView, IEnumerable<IGameFile> gameFiles)
        {
            ITagData currentTag = null;
            if (tabView is TagTabView tagTabView)
                currentTag = tagTabView.TagDataSource;

            List<IGameFile> gameFilesInclude = new List<IGameFile>(gameFiles.Count());

            foreach (IGameFile gameFile in gameFiles)
            {
                var tags = DataCache.Instance.TagMapLookup.GetTags(gameFile);

                // This tab is for this tag, include it
                if (currentTag != null && currentTag.ExcludeFromOtherTabs && tags.Any(x => x.TagID == currentTag.TagID))
                {
                    gameFilesInclude.Add(gameFile);
                    continue;
                }

                if (!tags.Any(x => x.ExcludeFromOtherTabs))
                    gameFilesInclude.Add(gameFile);
            }

            return gameFilesInclude;
        }

        private IEnumerable<IGameFile> GetViewSort(IGameFileView view, IEnumerable<IGameFile> gameFiles)
        {
            if (!(view is IGameFileSortableView sortableView) || string.IsNullOrEmpty(sortableView.GetSortedColumnKey()))
                return gameFiles;

            ColumnField columnField = GameFileViewFactory.DefaultColumnTextFields.FirstOrDefault(x => x.DataKey.Equals(sortableView.GetSortedColumnKey(), StringComparison.InvariantCultureIgnoreCase));

            if (columnField != null)
            {
                var property = typeof(IGameFile).GetProperty(columnField.DataKey);
                var sort = sortableView.GetColumnSort(sortableView.GetSortedColumnKey());

                if (sort == SortOrder.Ascending)
                    return gameFiles.OrderBy(x => property.GetValue(x));
                else if (sort == SortOrder.Descending)
                    return gameFiles.OrderByDescending(x => property.GetValue(x));
            }

            return gameFiles;
        }

        private void HandleManageTags()
        {
            TagForm form = new TagForm();
            form.Init(DataSourceAdapter);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);

            DataCache.Instance.TagMapLookup.Refresh(new ITagData[] { });

            if (form.TagControl.AddedTags.Length > 0 && GameFileViewFactory.IsUsingColumnView)
            {
                UpdateColumnConfig(); //the ordered tab insert will use this column configuration
                AppConfiguration.RefreshColumnConfig();
            }

            foreach (ITagData tag in form.TagControl.AddedTags)
            {
                if (tag.HasTab)
                    OrderedTagTabInsert(tag);
            }

            foreach (ITagData tag in form.TagControl.EditedTags)
            {
                ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x.Key.Equals(tag.TagID) && x is TagTabView);

                if (tabView != null)
                {
                    if (tag.HasTab)
                        m_tabHandler.UpdateTabTitle(tabView, tag.FavoriteName);
                    else
                        m_tabHandler.RemoveTab(tabView);
                }
                else
                {
                    if (tag.HasTab)
                        OrderedTagTabInsert(tag);
                }
            }

            UpdateTagColumnConfig(form.TagControl.EditedTags);

            foreach (ITagData tag in form.TagControl.DeletedTags)
            {
                ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x.Key.Equals(tag.TagID) && x is TagTabView);

                if (tabView != null)
                    m_tabHandler.RemoveTab(tabView);
            }

            DataCache.Instance.UpdateTags();
            UpdateTabOrder();

            UpdateLocal();
            HandleSelectionChange(GetCurrentViewControl(), false);
        }

        private void UpdateTabOrder()
        {
            int index = TabKeys.KeyNames.Length;

            foreach (var tag in DataCache.Instance.Tags)
            {
                if (!tag.HasTab)
                    continue;

                ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x.Key.Equals(tag.TagID) && x is TagTabView);
                if (tabView == null)
                    continue;

                int checkIndex = m_tabHandler.GetTabIndex(tabView);

                if (checkIndex != -1 && checkIndex != index)
                    m_tabHandler.SetTabIndex(index, tabView);

                index++;
            }
        }

        private void UpdateTagColumnConfig(ITagData[] editedTags)
        {
            ColumnConfig[] columnConfig = DataCache.Instance.GetColumnConfig();

            foreach (ITagData tag in editedTags)
            {
                ITagData previousRevision = DataCache.Instance.PreviousTags.FirstOrDefault(x => x.TagID == tag.TagID);
                if (previousRevision == null)
                    continue;

                var updateColumns = columnConfig.Where(x => x.Parent == previousRevision.Name);
                foreach (var col in updateColumns)
                    col.Parent = tag.Name;
            }

            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            UpdateConfig(config, AppConfiguration.ColumnConfigName, SerializeColumnConfig(columnConfig.ToList()));
            AppConfiguration.RefreshColumnConfig();
        }

        private void OrderedTagTabInsert(ITagData tag)
        {
            int start = m_tabHandler.TabViews.Length - 1;
            int end = 1;

            ITabView[] tabViews = m_tabHandler.TabViews;

            while (start > end && tabViews[start] is TagTabView &&
                tabViews[start].Title.CompareTo(tag.Name) > 0)
            {
                start--;
            }

            m_tabHandler.InsertTab(start + 1, CreateTagTab(GameFileViewFactory.DefaultColumnTextFields, DataCache.Instance.GetColumnConfig(), tag.FavoriteName, tag, true));
        }

        private void utilityToolStripItem_Click(object sender, EventArgs e)
        {
            ToolStripItem strip = sender as ToolStripItem;
            var utilities = DataSourceAdapter.GetUtilities();
            var utility = utilities.FirstOrDefault(x => x.Name == strip.Text);

            if (utility != null)
            {
                LaunchData launchData = GetLaunchFiles(SelectedItems(GetCurrentViewControl()));
                if (launchData.Success)
                {
                    UtilityHandler handler = new UtilityHandler(this, AppConfiguration, utility);
                    if (!handler.RunUtility(launchData.GameFile))
                        MessageBox.Show(this, "The utility was an invalid application or not found.", "Invalid Utility", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void selectTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGameFile gameFile = SelectedItems(GetCurrentViewControl()).FirstOrDefault();

            if (gameFile != null)
            {
                ITagData[] existingTags = DataCache.Instance.TagMapLookup.GetTags(gameFile);

                TagSelectForm form = new TagSelectForm();
                form.StartPosition = FormStartPosition.CenterParent;
                form.TagSelectControl.Init(new TagSelectOptions() { ShowCheckBoxes = true });
                form.TagSelectControl.SetCheckedTags(existingTags);

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    DataCache.Instance.UpdateGameFileTags(new IGameFile[] { gameFile }, form.TagSelectControl.GetCheckedTags());

                    var updatedTags = form.TagSelectControl.GetCheckedTags();

                    var addTags = updatedTags.Except(existingTags);
                    var removeTags = existingTags.Except(updatedTags);

                    foreach (var tag in addTags)
                        DataCache.Instance.AddGameFileTag(new IGameFile[] { gameFile }, tag, out _);

                    foreach (var tag in removeTags)
                        DataCache.Instance.RemoveGameFileTag(new IGameFile[] { gameFile }, tag);

                    var changedTags = addTags.Union(removeTags).ToArray();
                    DataCache.Instance.TagMapLookup.Refresh(changedTags);

                    GetCurrentViewControl().UpdateGameFile(gameFile);
                    HandleSelectionChange(GetCurrentViewControl(), true);
                }
            }
        }

        private void tagToolStripItem_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripItem strip))
                return;

            ITagData tag = DataCache.Instance.Tags.FirstOrDefault(x => x.TagID.Equals(strip.Tag));
            if (tag == null)
                return;

            IGameFile[] gameFiles = SelectedItems(GetCurrentViewControl());
            DataCache.Instance.AddGameFileTag(gameFiles, tag, out List<IGameFile> alreadyTagged);

            DataCache.Instance.TagMapLookup.Refresh(new ITagData[] { tag });
            UpdateTagTabData(tag.TagID);

            foreach (IGameFile gameFile in gameFiles)
                GetCurrentViewControl().UpdateGameFile(gameFile);

            if (alreadyTagged.Count > 0)
            {
                StringBuilder sbError = new StringBuilder(string.Join(", ", alreadyTagged.Select(x => x.FileNameNoPath).ToArray()));
                sbError.Insert(0, "The file(s) ");
                sbError.Append(" already have the tag ");
                sbError.Append(tag.Name);
                MessageBox.Show(this, sbError.ToString(), "Already Tagged", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            HandleSelectionChange(GetCurrentViewControl(), true);
        }

        private void removeTagToolStripItem_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripItem strip))
                return;

            ITagData tag = DataCache.Instance.Tags.FirstOrDefault(x => x.TagID.Equals(strip.Tag));
            if (tag == null)
                return;

            IGameFile[] gameFiles = SelectedItems(GetCurrentViewControl());
            DataCache.Instance.RemoveGameFileTag(gameFiles, tag);

            DataCache.Instance.TagMapLookup.Refresh(new ITagData[] { tag });
            UpdateTagTabData(tag.TagID);

            foreach (IGameFile gameFile in gameFiles)
                GetCurrentViewControl().UpdateGameFile(gameFile);

            HandleSelectionChange(GetCurrentViewControl(), true);
        }

        private void anyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayRandom(null);
        }

        private void unplayedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayRandom(GameFileFieldType.LastPlayed);
        }

        private void unratedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayRandom(GameFileFieldType.Rating);
        }

        private void PlayRandom(GameFileFieldType? field)
        {
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileFieldType[] { GameFileFieldType.GameFileID , GameFileFieldType.LastPlayed, GameFileFieldType.Rating});
            IEnumerable<IGameFile> gameFiles = DataSourceAdapter.GetGameFiles(options);

            if (field != null)
            {
                if (field.Value == GameFileFieldType.LastPlayed)
                    gameFiles = gameFiles.Where(x => !x.LastPlayed.HasValue);
                else if (field.Value == GameFileFieldType.Rating)
                    gameFiles = gameFiles.Where(x => !x.Rating.HasValue);
            }

            if (gameFiles.Any())
            {
                Random rand = new Random(DateTime.Now.Millisecond);
                int index = rand.Next() % gameFiles.Count();

                IGameFile gameFile = gameFiles.ElementAt(index);
                options = new GameFileGetOptions();
                options.SearchField = new GameFileSearchField(GameFileFieldType.GameFileID, gameFile.GameFileID.ToString());

                gameFile = DataSourceAdapter.GetGameFiles(options).FirstOrDefault();

                tabControl.SelectedTab = tabControl.TabPages[1];
                ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x.Title == TabKeys.LocalKey);
                tabView.GameFileViewControl.SelectedItem = gameFile;

                if (gameFile != null)
                    HandlePlay(new IGameFile[] { gameFile });
            }
            else
            {
                MessageBox.Show(this, "No file found that matched the requested parameters!", "None Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTagTabData(int tagID)
        {
            UpdateTagTabData(new int[] { tagID });
        }

        private void UpdateTagTabData(IEnumerable<int> tagIDs)
        {
            foreach (var tagID in tagIDs)
            {
                ITabView tab = m_tabHandler.TabViews.FirstOrDefault(x => x.Key.Equals(tagID) && x is TagTabView);

                if (tab != null)
                    tab.SetGameFiles();
            }

            ITabView untaggedView = m_tabHandler.TabViews.FirstOrDefault(x => x is UntaggedTabView);

            if (untaggedView != null)
                untaggedView.SetGameFiles();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleSyncStatus();
        }

        private void ShowTextBoxForm(string title, string header, string text, bool dialog)
        {
            TextBoxForm form = new TextBoxForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = title;
            form.HeaderText = header;
            form.DisplayText = text;

            if (dialog)
                form.Show(this);
            else
                form.ShowDialog(this);
        }

        private void generateTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGameFile file = SelectedItems(GetCurrentViewControl()).FirstOrDefault();
            file = DataSourceAdapter.GetGameFile(file.FileName); //populate all the date for the file
            if (file != null)
                ShowTextFileGenerator(file);
        }

        private void generateTextFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowTextFileGenerator(null);
        }

        private void ShowTextFileGenerator(IGameFile file)
        {
            TxtGenerator generator = new TxtGenerator();
            generator.SetData(DataSourceAdapter, file);
            generator.StartPosition = FormStartPosition.CenterParent;
            generator.ShowDialog(this);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleRename();
        }

        private string m_typeSearch;
        private DateTime m_typeSearchLastPress;

        private void HandleKeyPress(KeyPressEventArgs e)
        {
            if (GetCurrentViewControl() != null)
            {
                if (m_typeSearch != null && DateTime.Now.Subtract(m_typeSearchLastPress).TotalMilliseconds > 700)
                    m_typeSearch = null;

                if (m_typeSearch == null)
                    m_typeSearch = char.ToLower(e.KeyChar).ToString();
                else
                    m_typeSearch += char.ToLower(e.KeyChar).ToString();

                m_typeSearchLastPress = DateTime.Now;

                if (!SelectItem(GetCurrentViewControl(), m_typeSearch))
                    System.Media.SystemSounds.Beep.Play();
            }
        }

        private bool SelectItem(IGameFileView ctrl, string search)
        {
           bool success = false, isIdGames = false;

            ITabView tabView = m_tabHandler.TabViewForControl(ctrl);
            if (tabView != null && tabView is IdGamesTabViewCtrl)
                isIdGames = true;

            foreach (IGameFile item in GetCurrentViewControl().DataSource)
            {
                if (isIdGames)
                    success = item.Title.ToLower().StartsWith(search);
                else
                    success = item.FileName.ToLower().StartsWith(search);

                if (success)
                {
                    SetSelectedItem(GetCurrentViewControl(), item);
                    break;
                }
            }

            return success;
        }

        private void cumulativeStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCumulativeStatistics();
        }

        private void cumulativeStatisticsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HandleCumulativeStatistics();
        }

        private void HandleCumulativeStatistics()
        {
            if (GetCurrentViewControl() != null)
            {
                List<IStatsData> stats = new List<IStatsData>();
                List<IGameFile> gameFiles = new List<IGameFile>();
                foreach (IGameFile item in GetCurrentViewControl().DataSource)
                {
                    if (item.GameFileID.HasValue)
                    {
                        stats.AddRange(DataSourceAdapter.GetStats(item.GameFileID.Value));
                        gameFiles.Add(item);
                    }
                }

                ITabView tabView = m_tabHandler.TabViewForControl(GetCurrentViewControl());
                string tabText = tabView == null ? string.Empty : tabView.Title;

                CumulativeStats form = new CumulativeStats();
                form.Text = string.Format("Cumulative Stats - {0}", tabText);
                form.SetStatistics(gameFiles, stats);
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this);
            }
        }

        private ProgressBarForm CreateProgressBar(string text, ProgressBarStyle style)
        {
            ProgressBarForm form = new ProgressBarForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.DisplayText = text;

            if (style == ProgressBarStyle.Marquee)
            {
                form.ProgressBarStyle = ProgressBarStyle.Marquee;
            }
            else
            {
                form.Minimum = 0;
                form.Maximum = 100;
            }

            return form;
        }

        private void ProgressBarStart(ProgressBarForm form)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ProgressBarForm>(ProgressBarStart), new object[] { form });
            }
            else
            {
                this.Enabled = false;
                form.Show(this);
            }
        }

        private void ProgressBarEnd(ProgressBarForm form)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ProgressBarForm>(ProgressBarEnd), new object[] { form });
            }
            else
            {
                this.Enabled = true;
                if (form != null)
                    form.Close();
            }
        }

        //private void wadarchiveStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    IGameFileDataSource gameFile = SelectedItems(GetCurrentViewControl()).FirstOrDefault();

        //    if (gameFile != null)
        //    {
        //        string file = Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), gameFile.FileName);
        //        ZipArchive za = ZipFile.OpenRead(file);
        //        ZipArchiveEntry zae = za.Entries.First();
        //        string extractFile = Path.Combine(AppConfiguration.TempDirectory.GetFullPath(), zae.Name);
        //        zae.ExtractToFile(extractFile, true);               

        //        WadArchiveDataAdapter adapter = new WadArchiveDataAdapter();
        //        string test = adapter.Test(Path.Combine(AppConfiguration.TempDirectory.GetFullPath(), extractFile)).FileName;
        //        MessageBox.Show(test);
        //    }
        //}

        private void utilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleEditUtilities();
        }

        private void manageUtilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleEditUtilities();
        }

        private void createShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IGameFile[] gameFiles = SelectedItems(GetCurrentViewControl());
                StringBuilder sbFileNames = new StringBuilder();
                foreach(IGameFile gameFile in gameFiles)
                {
                    string fileName = string.IsNullOrEmpty(gameFile.Title) ? gameFile.FileName : gameFile.Title;
                    Array.ForEach(Path.GetInvalidFileNameChars(), x=> fileName = fileName.Replace(x, ' '));
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Concat(fileName, ".lnk"));

                    IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(filePath) as IWshRuntimeLibrary.IWshShortcut;
                    shortcut.Arguments = gameFile.FileName;
                    shortcut.TargetPath = string.Format(Path.Combine(Directory.GetCurrentDirectory(), Util.GetExecutableNoPath()));
                    shortcut.WindowStyle = 1;
                    shortcut.Description = string.Concat("Doom Launcher - ", gameFile.FileName);
                    shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
                    shortcut.IconLocation = string.Format(Path.Combine(Directory.GetCurrentDirectory(), "DoomLauncher.ico"));
                    shortcut.Save();

                    sbFileNames.Append(fileName);
                    sbFileNames.Append(", ");
                }

                sbFileNames.Length -= 2;
                MessageBox.Show(this, string.Format("Shortcut successfully created for: {0}", sbFileNames.ToString()), "Shortcut Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("Help.pdf");
            }
            catch
            {
                MessageBox.Show(this, "The help document is missing and could not be opened.", "Help Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void createZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCreateZip();
        }

        private async void HandleCreateZip()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "Zip|*.zip";

                if (fileDialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
                {
                    ProgressBarForm progressBar = CreateProgressBar("Creating zip...", ProgressBarStyle.Marquee);
                    ProgressBarStart(progressBar);
                    bool success = false;
                    await Task.Run(() => success = CreateZipFromDirectory(folderDialog.SelectedPath, fileDialog.FileName));
                    ProgressBarEnd(progressBar);

                    if (!success)
                        MessageBox.Show(this, "Failed to create zip file. File may be in use.", "Zip Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static bool CreateZipFromDirectory(string folderPath, string zipFileName)
        {
            try
            {
                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);
                ZipFile.CreateFromDirectory(folderPath, zipFileName);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void addFIlesRecursivelyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                List<string> extensions = Util.GetPkExtenstions().Union(Util.GetDehackedExtensions()).ToList();
                extensions.Add(".wad");
                for (int i = 0; i < extensions.Count; i++)
                    extensions[i] = "*" + extensions[i];

                IEnumerable<string> files = new List<string>();

                foreach (string ext in extensions)
                    files = files.Union(Directory.EnumerateFiles(dialog.SelectedPath, ext, SearchOption.AllDirectories));

                HandleAddGameFiles(AddFileType.GameFile, files.ToArray());
            }
        }

        private AppConfiguration AppConfiguration => DataCache.Instance.AppConfiguration;
        private IDataSourceAdapter DataSourceAdapter { get; set; }

        private IGameFileDataSourceAdapter DirectoryDataSourceAdapter { get; set; }

        private IGameFileDataSourceAdapter IdGamesDataSourceAdapter { get; set; }
    }
}
