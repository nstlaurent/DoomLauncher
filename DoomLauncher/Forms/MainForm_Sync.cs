using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private async Task<SyncResult> SyncLocalDatabase(string[] fileNames, FileManagement fileManagement, bool updateViews, ITagData tag = null)
        {
            var pg = ProgressBarStart(ProgressBarType.Sync);
            pg.Text = $"Syncing {fileNames.Count()} files...";
            SyncResult syncResult =  await Task.Run(() => ExecuteSyncHandler(fileNames, fileManagement, tag));
            ProgressBarEnd(ProgressBarType.Sync);
            SyncLocalDatabaseComplete(syncResult, updateViews);
            return syncResult;
        }

        void SyncLocalDatabaseComplete(SyncResult syncResult, bool updateViews)
        {
            if (updateViews)
            {
                UpdateLocal();
                HandleTabSelectionChange();

                foreach (IGameFile updateGameFile in syncResult.UpdatedGameFiles)
                    UpdateDataSourceViews(updateGameFile);

                IGameFileView view = GetCurrentViewControl();
                IGameFile selectedFile = view.SelectedItem;
                if (selectedFile != null && syncResult.UpdatedGameFiles.Contains(selectedFile))
                    HandleSelectionChange(view, true);
            }

            if (syncResult != null && syncResult.FailedTitlePicFiles.Count > 0)
            {
                StringBuilder sb = new StringBuilder("The following files had title images but failed to convert to a valid image:");
                sb.AppendLine();
                sb.AppendLine();
                foreach (IGameFile gameFile in syncResult.FailedTitlePicFiles)
                    sb.Append(gameFile.FileNameNoPath);

                MessageBox.Show(this, sb.ToString(), "Image Conversion Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (syncResult != null &&
                (syncResult.InvalidFiles.Count > 0 || m_zdlInvalidFiles.Count > 0))
            {
                DisplayInvalidFilesError(syncResult.InvalidFiles.Union(m_zdlInvalidFiles));
            }
            else if (m_launchFile != null)
            {
                IGameFile launchFile = DataSourceAdapter.GetGameFile(Path.GetFileName(m_launchFile));
                m_launchFile = null;
                if (launchFile != null)
                    HandlePlay(new IGameFile[] { launchFile });
            }
        }

        private void DisplayInvalidFilesError(IEnumerable<InvalidFile> invalidFiles)
        {
            StringBuilder sb = new StringBuilder();

            foreach (InvalidFile file in invalidFiles)
            {
                sb.Append(file.FileName);
                sb.Append(": ");
                sb.Append(file.Reason);
                sb.Append(Environment.NewLine);
            }

            ShowTextBoxForm("Processing Errors",
                "The information on these files may be incomplete.\n\nFor ZDL files adding the missing Source Port/IWAD name and re-adding will update the information.\n\nFor zip archive/pk3 errors: Doom Launcher uses a zip library that has very strict zip rules that not all applications respect.\n\nVerify the zip by opening it with your favorite zip application. Create a new zip file and extract the files from the original zip into the newly created one. Then add the newly created zip to Doom Launcher.",
                sb.ToString(), false);
        }

        private SyncResult ExecuteSyncHandler(string[] files, FileManagement fileManagement, ITagData tag = null)
        {
            SyncLibraryHandler handler = null;
            SyncResult syncResult = SyncResult.EMPTY;

            try
            {
                var syncActions = new List<ISyncAction>()
                {
                    // Lower on the list is higher priority
                    new TextFileSyncAction(new IdGamesTextFileParser(AppConfiguration.DateParseFormats).Parse),
                    new Doom64SyncAction(DataSourceAdapter),
                    new MapStringSyncAction(AppConfiguration.TempDirectory),
                    new GameInfoSyncAction(),
                    new StartupImageSyncAction(),
                    new TitlePicSyncAction(DataSourceAdapter, 
                                            DataCache.Instance.DefaultPalette,
                                            DataCache.Instance.HexenPalette,
                                            DataCache.Instance.HereticPalette).OnlyIf(AppConfiguration.AutomaticallyPullTitlpic),
                    new Doom64TitlePicSyncAction(),
                    new IWadTitlesSyncAction(),
                    new GameConfSyncAction(DataSourceAdapter),
                    new KnownWadsSyncAction(DataSourceAdapter)
                };

                handler = new SyncLibraryHandler(DataSourceAdapter, DirectoryDataSourceAdapter, AppConfiguration, 
                    fileManagement, syncActions);

                handler.SyncFileChanged += syncHandler_SyncFileChanged;
                handler.GameFileDataNeeded += syncHandler_GameFileDataNeeded;

                syncResult = handler.SyncManyFiles(files);
                SyncTitlePics(syncResult);

                if (m_pendingZdlFiles != null)
                {
                    SyncPendingZdlFiles();
                    m_pendingZdlFiles = null;
                }

                if (tag != null)
                    TagSyncFiles(syncResult, tag);
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }

            return syncResult;
        }

        private void SyncTitlePics(SyncResult syncResult)
        {
            foreach (IGameFile gameFile in syncResult.AddedOrUpdatedFiles)
            {
                if (!syncResult.GetTitlePic(gameFile, out Image image))
                    continue;

                image = image.ScaleDoomImage();

                var screenshots = DataSourceAdapter.GetFiles(gameFile, FileType.Screenshot);
                if (ScreenshotHandler.FindScreenshot(screenshots, image, out MemoryStream imageStream))
                    continue;

                if (imageStream == null)
                    continue;

                ScreenshotHandler.InsertScreenshot(gameFile, imageStream, screenshots, out _);
                imageStream?.Dispose();
            }
        }

        private void TagSyncFiles(SyncResult syncResult, ITagData tag)
        {
            DataCache.Instance.AddGameFileTag(syncResult.AddedGameFiles, tag, out _);
            DataCache.Instance.AddGameFileTag(syncResult.UpdatedGameFiles, tag, out _);
            DataCache.Instance.TagMapLookup.Refresh(new ITagData[] { tag });
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

        void syncHandler_SyncFileChanged(SyncLibraryHandler.SyncProgressEvent e)
        {
            if (InvokeRequired)
                Invoke(new Action<SyncLibraryHandler.SyncProgressEvent>(ProgressBarUpdate), new object[] { e });
            else
                ProgressBarUpdate(e);
        }

        void syncHandler_GameFileDataNeeded(SyncLibraryHandler.GameFileDataNeededEvent e)
        {
            if (InvokeRequired)
                Invoke(new Action<SyncLibraryHandler.GameFileDataNeededEvent>(HandleGameFileDataNeeded), new object[] { e });
            else
                HandleGameFileDataNeeded(e);
        }

        void HandleGameFileDataNeeded(SyncLibraryHandler.GameFileDataNeededEvent e)
        {
            if (CurrentDownloadFile != null && CurrentDownloadFile.FileName == e.CurrentGameFile.FileName)
            {
                e.CurrentGameFile.Title = CurrentDownloadFile.Title;
                e.CurrentGameFile.Author = CurrentDownloadFile.Author;
                e.CurrentGameFile.ReleaseDate = CurrentDownloadFile.ReleaseDate;
            }
        }

        void ProgressBarUpdate(SyncLibraryHandler.SyncProgressEvent e)
        {
            if (m_progressBars.TryGetValue(ProgressBarType.Sync, out var progressBar))
            {
                progressBar.Maximum = e.SyncFileCount;
                progressBar.Value = e.SyncFileCurrent;
                progressBar.DisplayText = string.Format("Reading {0}...", e.CurrentSyncFileName);
            }
        }

        void ProgressBarForm_Cancelled(object sender, EventArgs e)
        {
            Enabled = true;
            BringToFront();
        }

        private void SyncIWads(IEnumerable<IGameFile> gameFiles)
        {
            foreach (var gameFile in gameFiles)
            {
                DataSourceAdapter.InsertIWad(new IWadData() { GameFileID = gameFile.GameFileID.Value, FileName = gameFile.FileName, Name = gameFile.FileName });
                var iwad = DataSourceAdapter.GetIWads().OrderBy(x => x.IWadID).LastOrDefault();

                IWadInfo wadInfo = IWadInfo.GetIWadInfo(gameFile.FileName);
                gameFile.Title = wadInfo == null ? Path.GetFileNameWithoutExtension(gameFile.FileName).ToUpper() : wadInfo.Title;
                DataSourceAdapter.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.Title });

                if (iwad != null)
                {
                    gameFile.IWadID = iwad.IWadID;
                    DataSourceAdapter.UpdateGameFile(gameFile, new[] { GameFileFieldType.IWadID });
                }
                
            }

            ThumbnailManager.SetIWads(DataSourceAdapter.GetGameFileIWads().ToList());

            UpdateLocal();
            HandleTabSelectionChange();
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
            switch (option)
            {
                case SyncFileOption.Add:
                    ProgressBarStart(ProgressBarType.Sync);

                    SyncResult syncResult = await Task.Run(() => ExecuteSyncHandler(files.ToArray(), FileManagement.Managed));

                    ProgressBarEnd(ProgressBarType.Sync);
                    SyncLocalDatabaseComplete(syncResult, true);
                    break;

                case SyncFileOption.Delete:
                    ProgressBarStart(ProgressBarType.Delete);

                    await Task.Run(() => DeleteLocalGameFiles(files));

                    ProgressBarEnd(ProgressBarType.Delete);
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
            switch (option)
            {
                case SyncFileOption.Add:
                    ProgressBarStart(ProgressBarType.Search);

                    List<IGameFile> gameFiles = await Task.Run(() => FindIdGamesFiles(files));
                    if (gameFiles == null)
                    {
                        MessageBox.Show(this, "Failed to connect to id games.", "Connection Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        foreach (IGameFile gameFile in gameFiles)
                            m_downloadHandler.Download(IdGamesDataSourceAdapter, gameFile as IGameFileDownloadable);
                    }

                    ProgressBarEnd(ProgressBarType.Search);

                    if (gameFiles != null)
                    {
                        DisplayFilesNotFound(files, gameFiles);

                        if (gameFiles.Count > 0)
                            DisplayDownloads();
                    }

                    break;

                case SyncFileOption.Delete:
                    ProgressBarStart(ProgressBarType.Delete);

                    await Task.Run(() => DeleteLibraryGameFiles(files));

                    ProgressBarEnd(ProgressBarType.Delete);
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
                form.Title = "Files Not Found";
                form.HeaderText = "The following files were not found in the idgames database:";
                form.DisplayText = sb.ToString();
                form.ShowDialog(this);
            }
        }

        private List<IGameFile> FindIdGamesFiles(IEnumerable<string> files)
        {
            List<IGameFile> gameFiles = new List<IGameFile>();

            try
            {
                foreach (string file in files)
                {
                    IGameFile gameFile = IdGamesDataSourceAdapter.GetGameFile(file);
                    if (gameFile != null)
                        gameFiles.Add(gameFile);
                }
            }
            catch
            {
                return null;
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

        private async Task HandleResync(bool pullTitlepic)
        {
            IGameFileView view = GetCurrentViewControl();
            if (view == null)
                return;

            bool? setPullTitlepic = null;
            if (!pullTitlepic)
                setPullTitlepic = false;
            var allGameFiles = SelectedItems(view);

            AddFileType addFileType = view.DoomLauncherParent is IWadTabViewCtrl ? AddFileType.IWad : AddFileType.GameFile;

            var managed = allGameFiles.Where(x => !x.IsUnmanaged()).Select(x => Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), x.FileName)).ToArray();
            if (managed.Length > 0)
                await HandleAddGameFiles(addFileType, managed, overrideManagement: FileManagement.Managed,
                    overridePullTitlepic: setPullTitlepic);

            var unmanaged = allGameFiles.Where(x => x.IsUnmanaged()).Select(x => x.FileName).ToArray();
            if (unmanaged.Length > 0)
                await HandleAddGameFiles(addFileType, unmanaged, overrideManagement: FileManagement.Unmanaged,
                    overridePullTitlepic: setPullTitlepic);
        }
    }
}
