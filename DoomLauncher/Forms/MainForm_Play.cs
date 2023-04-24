using DoomLauncher.Adapters;
using DoomLauncher.DataSources;
using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private readonly List<PlaySession> m_activeSessions = new List<PlaySession>();

        private FilterForm m_filterForm;

        private void HandlePlay()
        {
            if (GetCurrentViewControl() != null)
                HandlePlay(SelectedItems(GetCurrentViewControl()));
        }

        private bool AssertFile(string file)
        {
            bool exists;
            if (Util.IsDirectory(file))
                exists = Directory.Exists(file);
            else
                exists = File.Exists(file);

            if (!exists)
                MessageBox.Show(this, string.Format("The file {0} does not exist.", file), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return exists;
        }

        private void HandlePlay(IEnumerable<IGameFile> gameFiles, ISourcePortData sourcePort = null, string map = null, bool autoPlay = false)
        {
            LaunchData launchData = GetLaunchFiles(gameFiles, checkActiveSessions: true);

            if (!launchData.Success)
            {
                if (!string.IsNullOrEmpty(launchData.ErrorTitle))
                    MessageBox.Show(this, launchData.ErrorDescription, launchData.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (launchData.GameFile == null)
            {
                var iwad = DataSourceAdapter.GetIWad((int)AppConfiguration.GetTypedConfigValue(ConfigType.DefaultIWad, typeof(int)));
                if (iwad != null)
                {
                    GameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.GameFileID, iwad.GameFileID.Value.ToString()));
                    launchData.GameFile = DataSourceAdapter.GetGameFiles(options).FirstOrDefault();
                }
            }

            if (launchData.GameFile == null)
                return;

            SetupPlayForm(launchData.GameFile);
            if (sourcePort != null) 
                m_currentPlayForm.SelectedSourcePort = sourcePort;
            if (map != null)
                m_currentPlayForm.SelectedMap = map;

            if (autoPlay || m_currentPlayForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    HandlePlaySettings(m_currentPlayForm, m_currentPlayForm.SelectedGameProfile);
                    if (m_currentPlayForm.SelectedSourcePort != null)
                        StartPlay(launchData.GameFile, m_currentPlayForm.SelectedSourcePort, m_currentPlayForm.ScreenFilter);
                    ctrlSummary.PauseSlideshow();
                }
                catch (IOException)
                {
                    MessageBox.Show(this, "The file is in use and cannot be launched. Please close any programs that may be using the file and try again.", "File In Use",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                HandleSelectionChange(GetCurrentViewControl(), true);
            }
        }

        private LaunchData GetLaunchFiles(IEnumerable<IGameFile> gameFiles, bool checkActiveSessions)
        {
            IGameFile gameFile = null;
            if (gameFiles != null)
            {
                if (gameFiles.Count() > 1)
                {
                    gameFile = PromptUserMainFile(gameFiles, out bool accepted);  //ask user which file to tie all stats to
                    if (!accepted)
                        return new LaunchData(string.Empty, string.Empty);
                }
                else
                {
                    gameFile = gameFiles.FirstOrDefault();
                }
            }

            if (checkActiveSessions && m_activeSessions.Any() && !AppConfiguration.AllowMultiplePlaySessions)
                return new LaunchData("Already Playing", "There is already a game in progress. Please exit that game first.\n\nCheck the 'Allow Multiple Play Sessions' in the settings to enable this feature.");

            if (!DataSourceAdapter.GetSourcePorts().Any())
                return new LaunchData("No Source Ports", "You must have at least one source port configured to play! Click the settings menu on the top left and select Source Ports to configure.");

            if (!DataSourceAdapter.GetIWads().Any())
                return new LaunchData("No IWADs", "You must have at least one IWAD configured to play! Click the settings menu on the top left and select IWads to configure.");

            if (gameFile != null && GetCurrentViewControl() != null)
            {
                ITabView tabView = m_tabHandler.TabViewForControl(GetCurrentViewControl());
                if (tabView != null)
                    gameFile = DataSourceAdapter.GetGameFile(gameFile.FileName); //this file came from the grid, which does not have all info populated to save performance

                if (gameFiles.Count() > 1) //for when the user selected more than one file
                {
                    HandleMultiSelectPlay(gameFile, gameFiles.Except(new IGameFile[] { gameFile })); //sets SettingsFiles with all the other game files
                    List<IGameFile> gameFilesList = new List<IGameFile>() { gameFile };
                    Array.ForEach(gameFiles.Skip(1).ToArray(), x => gameFilesList.Add(x));
                    gameFiles = gameFilesList;
                }
            }

            return new LaunchData(gameFile, (GameFile)gameFile, gameFiles);
        }

        private IGameFile PromptUserMainFile(IEnumerable<IGameFile> gameFiles, out bool accepted)
        {
            accepted = false;

            FileSelectForm form = new FileSelectForm();
            ITabView tabView = m_tabHandler.TabViews.FirstOrDefault(x => x.Key.Equals(TabKeys.LocalKey));
            form.Initialize(DataSourceAdapter, tabView, gameFiles);
            form.StartPosition = FormStartPosition.CenterParent;
            form.SetDisplayText("Please select the main file that all data will be associated with. (Screenshots, demos, save games, etc.)");
            form.MultiSelect = false;
            form.ShowSearchControl(false);

            if (form.ShowDialog(this) == DialogResult.OK && form.SelectedFiles.Length > 0)
            {
                accepted = true;
                return form.SelectedFiles[0];
            }

            return gameFiles.First();
        }

        private void HandleMultiSelectPlay(IGameFile firstGameFile, IEnumerable<IGameFile> gameFiles)
        {
            //If the user selected multiple files
            //Take all the files after the first and set them as additional files to the first

            StringBuilder sbAdditionalFiles = new StringBuilder();

            foreach(IGameFile gameFile in gameFiles)
            {
                sbAdditionalFiles.Append(gameFile.FileName);
                sbAdditionalFiles.Append(';');
            }

            firstGameFile.SettingsFiles = sbAdditionalFiles.ToString();
        }

        private void HandlePlaySettings(PlayForm form, IGameProfile gameProfile)
        {
            if (form.RememberSettings && gameProfile != null)
            {
                form.UpdateGameProfile(gameProfile);

                form.GameFile.SettingsGameProfileID = form.SelectedGameProfile.GameProfileID;
                DataSourceAdapter.UpdateGameFile(form.GameFile, new GameFileFieldType[] { GameFileFieldType.SettingsGameProfileID });

                if (gameProfile is IGameFile gameFile)
                {
                    DataSourceAdapter.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.SourcePortID, GameFileFieldType.IWadID, GameFileFieldType.SettingsMap,
                    GameFileFieldType.SettingsSkill, GameFileFieldType.SettingsFiles, GameFileFieldType.SettingsExtraParams, GameFileFieldType.SettingsSpecificFiles, GameFileFieldType.SettingsStat,
                    GameFileFieldType.SettingsFilesIWAD, GameFileFieldType.SettingsFilesSourcePort, GameFileFieldType.SettingsSaved, GameFileFieldType.SettingsLoadLatestSave });
                }
                else
                {
                    DataSourceAdapter.UpdateGameProfile(gameProfile);
                }
            }
        }

        private void SetupPlayForm(IGameFile gameFile)
        {
            m_currentPlayForm = new PlayForm(AppConfiguration, DataSourceAdapter);
            m_currentPlayForm.SaveSettings += m_currentPlayForm_SaveSettings;
            m_currentPlayForm.OnPreviewLaunchParameters += m_currentPlayForm_OnPreviewLaunchParameters;
            m_currentPlayForm.StartPosition = FormStartPosition.CenterParent;

            m_currentPlayForm.Initialize(GetAdditionalTabViews(), gameFile, m_activeSessions.Any());
            m_currentPlayForm.SetGameProfile(GetGameProfile(gameFile));
        }

        private IGameProfile GetGameProfile(IGameFile gameFile)
        {
            if (gameFile.SettingsGameProfileID.HasValue)
            {
                var profile = DataSourceAdapter.GetGameProfiles(gameFile.GameFileID.Value).FirstOrDefault(x => x.GameProfileID == gameFile.SettingsGameProfileID.Value);
                if (profile != null)
                    return profile;
            }

            return (GameFile)gameFile;
        }

        private void m_currentPlayForm_OnPreviewLaunchParameters(object sender, EventArgs e)
        {
            GameFilePlayAdapter playAdapter = CreatePlayAdapter(m_currentPlayForm, playAdapter_ProcessExited, AppConfiguration);
            playAdapter.ExtractFiles = false;
            if (m_currentPlayForm.SettingsValid(out string err))
                ShowLaunchParameters(playAdapter, m_currentPlayForm.GameFile, m_currentPlayForm.SelectedSourcePort);
            else
                MessageBox.Show(this, err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void m_currentPlayForm_SaveSettings(object sender, EventArgs e)
        {
            HandlePlaySettings(m_currentPlayForm, m_currentPlayForm.SelectedGameProfile);
        }

        private List<ITabView> GetAdditionalTabViews()
        {
            List<ITabView> views = new List<ITabView>();
            views.AddRange(m_tabHandler.TabViews.Where(x => TabKeys.LocalKey.Equals(x.Key)));
            views.AddRange(m_tabHandler.TabViews.Where(x => x is TagTabView));
            return views;
        }

        private bool StartPlay(IGameFile gameFile, ISourcePortData sourcePort, bool screenFilter)
        {
            GameFilePlayAdapter playAdapter = CreatePlayAdapter(m_currentPlayForm, playAdapter_ProcessExited, AppConfiguration);
            m_saveGames = Array.Empty<IFileData>();

            if (AppConfiguration.CopySaveFiles)
                CopySaveGames(gameFile, sourcePort);
            CreateFileDetectors(sourcePort);

            if (m_currentPlayForm.PreviewLaunchParameters)
                ShowLaunchParameters(playAdapter, gameFile, sourcePort);

            bool isGameFileIwad = IsGameFileIwad(gameFile);

            IStatisticsReader statisticsReader = null;
            if (m_currentPlayForm.SaveStatistics)
                statisticsReader = SetupStatsReader(sourcePort, gameFile);

            if (playAdapter.Launch(AppConfiguration.GameFileDirectory, AppConfiguration.TempDirectory, 
                gameFile, sourcePort, isGameFileIwad))
            {
                m_activeSessions.Add(new PlaySession(playAdapter, statisticsReader, DateTime.Now));

                if (gameFile != null)
                {
                    gameFile.LastPlayed = DateTime.Now;
                    DataSourceAdapter.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.LastPlayed });
                    UpdateDataSourceViews(gameFile);
                }
            }
            else
            {
                MessageBox.Show(this, playAdapter.LastError, "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (screenFilter)
            {
                m_filterForm = new FilterForm(Screen.FromControl(this), m_currentPlayForm.GetFilterSettings());
                m_filterForm.Show(this);
            }

            return true;
        }

        private void CopySaveGames(IGameFile gameFile, ISourcePortData sourcePort)
        {
            if (gameFile != null)
            {
                HandleCopySaveGames(gameFile, sourcePort);
            }
            else if (IsGameFileIwad(gameFile))
            {
                gameFile = GetGameFileForIWad(gameFile);
                HandleCopySaveGames(gameFile, sourcePort);
            }
        }

        private void HandleCopySaveGames(IGameFile gameFile, ISourcePortData sourcePort)
        {
            m_saveGames = DataSourceAdapter.GetFiles(gameFile, FileType.SaveGame).Where(x => x.SourcePortID == sourcePort.SourcePortID).ToArray();
            SaveGameHandler saveGameHandler = new SaveGameHandler(DataSourceAdapter, AppConfiguration.SaveGameDirectory);
            saveGameHandler.CopySaveGamesToSourcePort(sourcePort, m_saveGames);
        }

        private void ShowLaunchParameters(GameFilePlayAdapter playAdapter, IGameFile gameFile, ISourcePortData sourcePort)
        {
            TextBoxForm form = new TextBoxForm
            {
                Text = "Launch Parameters",
                StartPosition = FormStartPosition.CenterParent
            };

            string launchParameters = playAdapter.GetLaunchParameters(AppConfiguration.GameFileDirectory,
                AppConfiguration.TempDirectory, gameFile, sourcePort, IsGameFileIwad(gameFile));

            if (launchParameters != null)
            {
                launchParameters = launchParameters.Replace(@" -", string.Concat(Environment.NewLine, " -"));
                launchParameters = launchParameters.Replace("\" \"", string.Concat("\"", Environment.NewLine, " \""));
                if (launchParameters.StartsWith(Environment.NewLine)) launchParameters = launchParameters.Substring(Environment.NewLine.Length);
                string individualFiles = string.Empty;

                if (m_currentPlayForm.SpecificFiles != null && m_currentPlayForm.SpecificFiles.Length > 0)
                    individualFiles = Environment.NewLine + string.Format("Selected Files: {0}", string.Join(", ", m_currentPlayForm.SpecificFiles));

                string sourcePortParams = string.Empty;
                if (!string.IsNullOrEmpty(sourcePort.ExtraParameters))
                    sourcePortParams = string.Concat(Environment.NewLine, Environment.NewLine, "Paramters from source port: ", sourcePort.ExtraParameters);

                form.DisplayText = string.Concat(launchParameters, Environment.NewLine, Environment.NewLine, 
                    string.Format("Supported Extensions: {0}", sourcePort.SupportedExtensions),
                    individualFiles,
                    sourcePortParams,
                    Environment.NewLine, Environment.NewLine,
                    "*** If files appear to be missing check the 'Select Individual Files' option and supported extensions options in the Source Port form of the selected source port.");
            }
            else
            {
                form.DisplayText = "Failed to generate launch parameters";
            }

            form.SelectDisplayText(0, 0);
            form.ShowDialog(this);
        }

        private IStatisticsReader SetupStatsReader(ISourcePortData sourcePort, IGameFile gameFile)
        {
            IStatisticsReader statisticsReader = CreateStatisticsReader(sourcePort, gameFile);

            if (statisticsReader != null)
            {
                statisticsReader.NewStastics += m_statsReader_NewStastics;
                statisticsReader.Start();
            }

            return statisticsReader;
        }

        private void CreateFileDetectors(ISourcePortData sourcePortData)
        {
            ISourcePort sourcePort = SourcePortUtil.CreateSourcePort(sourcePortData);
            CreateScreenshotDetectors(sourcePortData, sourcePort);
            CreateSaveGameDetectors(sourcePortData, sourcePort);
        }

        private void CreateSaveGameDetectors(ISourcePortData sourcePortData, ISourcePort sourcePort)
        {
            m_saveFileDetectors = CreateDefaultSaveGameDetectors();
            m_saveFileDetectors.Add(CreateSaveGameDetector(sourcePortData.GetSavePath().GetFullPath()));

            string saveDir = sourcePort.GetSaveGameDirectory();
            if (!string.IsNullOrEmpty(saveDir) && Directory.Exists(saveDir))
                m_saveFileDetectors.Add(CreateSaveGameDetector(saveDir));

            Array.ForEach(m_saveFileDetectors.ToArray(), x => x.StartDetection());
        }

        private void CreateScreenshotDetectors(ISourcePortData sourcePortData, ISourcePort sourcePort)
        {
            m_screenshotDetectors = CreateDefaultScreenshotDetectors();
            m_screenshotDetectors.Add(CreateScreenshotDetector(sourcePortData.Directory.GetFullPath()));

            string screenshotDir = sourcePort.GetScreenshotDirectory();
            if (!string.IsNullOrEmpty(screenshotDir) && Directory.Exists(screenshotDir))
                m_screenshotDetectors.Add(CreateScreenshotDetector(screenshotDir));

            Array.ForEach(m_screenshotDetectors.ToArray(), x => x.StartDetection());
        }

        private GameFilePlayAdapter CreatePlayAdapter(PlayForm form, EventHandler processExited, AppConfiguration appConfig)
        {
            GameFilePlayAdapter playAdapter = new GameFilePlayAdapter();
            playAdapter.IWad = form.SelectedIWad;
            playAdapter.Map = form.SelectedMap;
            playAdapter.Skill = form.SelectedSkill;
            playAdapter.Record = form.Record;
            playAdapter.SpecificFiles = form.SpecificFiles;
            playAdapter.AdditionalFiles = form.GetAdditionalFiles().ToArray();
            playAdapter.PlayDemo = form.PlayDemo;
            playAdapter.ExtraParameters = form.ExtraParameters;
            playAdapter.SaveStatistics = form.SaveStatistics;
            playAdapter.IgnoreExtractError = AppConfiguration.AllowMultiplePlaySessions && m_activeSessions.Any();

            if (form.LoadLatestSave)
            {
                if (!AppConfiguration.CopySaveFiles)
                {
                    MessageCheckBox message = new MessageCheckBox("Copy Save Files Disabled",
                        "Copy save files is disabled and the load latest save feature may not function.\n\nSelect the check box below to enable this setting.",
                        "Enable Setting", SystemIcons.Warning);
                    message.StartPosition = FormStartPosition.CenterParent;
                    message.ShowDialog(this);
                    if (message.Checked)
                        AppConfiguration.EnableCopySaveFiles();
                }
                playAdapter.LoadSaveFile = GetLoadLatestSave(form.GameFile, form.SelectedSourcePort);
            }

            playAdapter.ProcessExited += processExited;
            if (form.SelectedDemo != null)
                playAdapter.PlayDemoFile = Path.Combine(appConfig.DemoDirectory.GetFullPath(), form.SelectedDemo.FileName);
            return playAdapter;
        }

        private string GetLoadLatestSave(IGameFile gameFile, ISourcePortData sourcePortData)
        {
            var saveFile = DataSourceAdapter.GetFiles(gameFile, FileType.SaveGame).Where(x => x.SourcePortID == sourcePortData.SourcePortID)
                .OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (saveFile != null)
                return Path.Combine(sourcePortData.GetSavePath().GetFullPath(), saveFile.OriginalFileName);

            return string.Empty;
        }

        private IStatisticsReader CreateStatisticsReader(ISourcePortData sourcePort, IGameFile gameFile)
        {
            List<IStatsData> existingStats = new List<IStatsData>();
            if (gameFile != null && gameFile.GameFileID.HasValue)
                existingStats = DataSourceAdapter.GetStats(gameFile.GameFileID.Value).ToList();

            return SourcePortUtil.CreateSourcePort(sourcePort).CreateStatisticsReader(gameFile, existingStats);
        }

        void m_statsReader_NewStastics(object sender, NewStatisticsEventArgs e)
        {
            if (e.Statistics == null || !(sender is IStatisticsReader statisticsReader))
                return;

            PlaySession session = m_activeSessions.FirstOrDefault(x => statisticsReader.Equals(x.StatisticsReader));
            if (session == null || session.Adapter.GameFile == null)
                return;

            e.Statistics.MapName = e.Statistics.MapName.ToUpper();
            e.Statistics.GameFileID = session.Adapter.GameFile.GameFileID.Value;
            e.Statistics.SourcePortID = m_currentPlayForm.SelectedSourcePort.SourcePortID;

            if (e.Update)
            {
                IStatsData stats = DataSourceAdapter.GetStats(e.Statistics.GameFileID).LastOrDefault(x => x.MapName == e.Statistics.MapName);
                if (stats != null)
                    DataSourceAdapter.DeleteStats(stats.StatID);
            }

            DataSourceAdapter.InsertStats(e.Statistics);
        }

        private bool IsGameFileIwad(IGameFile gameFile)
        {
            return DataSourceAdapter.GetGameFileIWads().Any(x => x.GameFileID.Value == gameFile.GameFileID.Value);
        }

        void playAdapter_ProcessExited(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object>(HandleProcessExited), new object[] { sender });
            }
            else
            {
                HandleProcessExited(sender);
            }
        }

        private void HandleProcessExited(object sender)
        {
            if (m_filterForm != null)
            {
                m_filterForm.Close();
                m_filterForm = null;
            }

            GameFilePlayAdapter adapter = sender as GameFilePlayAdapter;
            PlaySession session = m_activeSessions.FirstOrDefault(x => x.Adapter.Equals(adapter));
            DateTime dtExit = DateTime.Now;
            Directory.SetCurrentDirectory(m_workingDirectory);

            if (adapter.SourcePort != null)
            {
                IGameFile gameFile = adapter.GameFile;

                if (gameFile != null && session != null)
                    SetMinutesPlayed(session, dtExit);

                if (!string.IsNullOrEmpty(adapter.RecordedFileName))
                    HandleRecordedDemo(adapter, gameFile);

                HandleDetectorFiles(adapter, gameFile);

                if (session != null && session.StatisticsReader != null)
                {
                    IStatisticsReader statsReader = session.StatisticsReader;
                    statsReader.Stop();

                    if (statsReader.ReadOnClose)
                        statsReader.ReadNow();

                    if (statsReader.Errors.Length > 0)
                        HandleStatReaderErrors(statsReader);
                }
            }

            if (session != null)
                m_activeSessions.Remove(session);

            IGameFileView view = GetCurrentViewControl();
            view.UpdateGameFile(adapter.GameFile);
            HandleSelectionChange(view, true);
        }

        private IGameFile GetGameFileForIWad(IGameFile gameFile)
        {
            return DataSourceAdapter.GetGameFileIWads().FirstOrDefault(x => x.GameFileID.Value == gameFile.GameFileID.Value);
        }

        private void SetMinutesPlayed(PlaySession session, DateTime dtExit)
        {
            IGameFile gameFile = session.Adapter.GameFile;
            gameFile.MinutesPlayed += Convert.ToInt32(dtExit.Subtract(session.Start).TotalMinutes);
            DataSourceAdapter.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.MinutesPlayed });
            UpdateDataSourceViews(gameFile);
        }

        private void HandleStatReaderErrors(IStatisticsReader m_statsReader)
        {
            TextBoxForm form = new TextBoxForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = "Statistic Reader Errors";
            form.HeaderText = string.Concat("The following errors were reported by the statistics reader.", 
                Environment.NewLine, "The statistics may be incomplete or missing.");

            StringBuilder sb = new StringBuilder();
            foreach(string error in m_statsReader.Errors)
            {
                sb.Append(error);
                sb.Append(Environment.NewLine);
            }

            form.DisplayText = sb.ToString();
            form.ShowDialog(this);
        }

        private void HandleDetectorFiles(GameFilePlayAdapter adapter, IGameFile gameFile)
        {
            ScreenshotHandler.HandleNewScreenshots(adapter.SourcePort, gameFile, GetNewScreenshots());
            SaveGameHandler savegameHandler = new SaveGameHandler(DataSourceAdapter, AppConfiguration.SaveGameDirectory);

            savegameHandler.HandleNewSaveGames(adapter.SourcePort, gameFile, GetNewSaveGames(m_saveFileDetectors, m_saveGames));
            savegameHandler.HandleUpdateSaveGames(adapter.SourcePort, gameFile, m_saveGames);
            savegameHandler.HandleDeleteSaveGames(GetDeletedSaveGames(m_saveFileDetectors), m_saveGames);
        }

        private void HandleRecordedDemo(GameFilePlayAdapter adapter, IGameFile gameFile)
        {
            DirectoryInfo di = new DirectoryInfo(AppConfiguration.TempDirectory.GetFullPath());
            FileInfo fiTemp = new FileInfo(adapter.RecordedFileName);
            FileInfo fi = di.GetFiles().FirstOrDefault(x => x.Name.Contains(fiTemp.Name));

            if (fi != null && fi.Exists)
            {
                DemoHandler demoHandler = new DemoHandler(DataSourceAdapter, AppConfiguration.DemoDirectory);
                demoHandler.HandleNewDemo(adapter.SourcePort, gameFile, fi.FullName,
                    m_currentPlayForm.RecordDescriptionText);
            }
            else
            {
                MessageBox.Show(this, "Could not find the demo file. Does this source port support recording?", "Demo Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<INewFileDetector> CreateDefaultScreenshotDetectors()
        {
            List<INewFileDetector> ret = new List<INewFileDetector>();

            foreach (string dir in AppConfiguration.ScreenshotCaptureDirectories)
            {
                if (Directory.Exists(dir))
                    ret.Add(CreateScreenshotDetector(dir));
            }

            return ret;
        }

        private List<INewFileDetector> CreateDefaultSaveGameDetectors()
        {
            return new List<INewFileDetector>();
        }

        private INewFileDetector CreateScreenshotDetector(string dir)
        {
            return new NewFileDetector(new string[] { ".png", ".jpg", ".bmp" }, dir, true); //future - should be configurable
        }

        private INewFileDetector CreateSaveGameDetector(string dir)
        {
            return new NewFileDetector(new string[] { ".zds", ".dsg", ".esg", ".hsg" }, dir, true); //future - should be configurable
        }

        private string[] GetNewScreenshots()
        {
            IEnumerable<string> newFiles = new string[] { };
            m_screenshotDetectors.ForEach(x => newFiles = newFiles.Union(x.GetNewFiles()));
            return newFiles.ToArray();
        }

        private static string[] GetNewSaveGames(List<INewFileDetector> saveFileDetectors, IFileData[] existingSaves)
        {
            IEnumerable<string> newFiles = new string[] { };
            saveFileDetectors.ForEach(x => newFiles = newFiles.Union(x.GetNewFiles()));

            IEnumerable<string> modifiedFiles = new string[] { };
            saveFileDetectors.ForEach(x => modifiedFiles = modifiedFiles.Union(x.GetModifiedFiles()));

            //modified files uses full path, m_saveGames does not. This section checks for modified files that were not part of the gamefile's save games
            //e.g save0.zds was a save game for this gamefile. User overwrites save1.zds for this gamefile. We now need to keep track of save1.zds as well.
            IEnumerable<string> saveFiles = existingSaves.Select(x => x.OriginalFileName);
            List<string> ret = newFiles.ToList();
            foreach(string modifiedFile in modifiedFiles)
            {
                FileInfo fi = new FileInfo(modifiedFile);
                if (!saveFiles.Contains(fi.Name) && !ret.Contains(fi.Name))
                    ret.Add(modifiedFile);
            }

            return ret.ToArray();
        }

        private static string[] GetDeletedSaveGames(List<INewFileDetector> saveFileDetectors)
        {
            IEnumerable<string> deletedFiles = new string[] { };
            saveFileDetectors.ForEach(x => deletedFiles = deletedFiles.Union(x.GetDeletedFiles()));
            return deletedFiles.ToArray();
        }
    }
}
