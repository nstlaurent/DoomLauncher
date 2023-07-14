using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class AppConfiguration
    {
        public event EventHandler GameFileViewTypeChanged;
        public event EventHandler VisibleViewsChanged;
        public event EventHandler ColorThemeChanged;

        public static string SplitTopBottomName => "SplitTopBottom";
        public static string SplitLeftRightName => "SplitLeftRight";
        public static string SplitTagSelectName => "SplitTagSelect";
        public static string AppWidthName => "AppWidth";
        public static string AppHeightName => "AppHeight";
        public static string AppXName => "AppX";
        public static string AppYName => "AppY";
        public static string WindowStateName => "WindowState";
        public static string ColumnConfigName => "ColumnConfig";
        public static string ScreenshotPreviewSizeName => "ScreenshotPreviewSize";
        public static string ItemsPerPageName => "ItemsPerPage";
        public static string LastSelectedTabIndexName => "LastSelectedTabIndex";
        public static string TagSelectPinnedName => "TagSelectPinned";
        public static string ShowTabHeadersName => "ShowTabHeaders";
        public static string CopySaveFilesName => "CopySaveFiles";
        public static string AllowMultiplePlaySessionsName => "AllowMultiplePlaySessions";
        public static string AutomaticallyPullTitlpicName => "AutomaticallyPullTitlpic";
        public static string VisibleViewsName => "VisibleViews";
        public static string ShowPlayDialogName => "ShowPlayDialog";
        public static string ImportScreenshotsName => "ImportScreenshots";

        public AppConfiguration(IDataSourceAdapter adapter)
        {
            DataSourceAdapter = adapter;
            Refresh(false);
        }

        public void Refresh()
        {
            Refresh(true);
        }

        public string GetConfigValue(ConfigType ct)
        {
            IConfigurationData config = DataSourceAdapter.GetConfiguration().FirstOrDefault(x => x.Name == ct.ToString("g"));

            if (config != null)
                return config.Value;

            return null;
        }

        public object GetTypedConfigValue(ConfigType ct, Type type)
        {
            return GetValueFromConfig(GetConfigValue(ct), type);
        }

        public LauncherPath PathForFileType(FileType type)
        {
            switch(type)
            {
                case FileType.Demo:
                    return DemoDirectory;
                case FileType.SaveGame:
                    return SaveGameDirectory;
                case FileType.Screenshot:
                    return ScreenshotDirectory;
                case FileType.Thumbnail:
                    return ThumbnailDirectory;
                default:
                    throw new ArgumentException($"Invalid FileType {type}");
            }
        }

        private static object GetValueFromConfig(string value, Type type)
        {
            if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(int))
            {
                if (!int.TryParse(value, out int iValue))
                    return -1;
                return iValue;
            }
            else if (type == typeof(bool))
            {
                if (!bool.TryParse(value, out bool bValue))
                    return false;
                return bValue;
            }
            else
            {
                throw new NotSupportedException(string.Format("Type {0} not supported ", type.ToString()));
            }
        }

        private void Refresh(bool throwErrors)
        {
            try
            {
                IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();

                IdGamesUrl = GetValue(config, "IdGamesUrl", string.Empty);
                ApiPage = GetValue(config, "ApiPage", string.Empty);
                MirrorUrl = GetValue(config, "MirrorUrl", string.Empty);
                CleanTemp = Convert.ToBoolean(GetValue(config, "CleanTemp", "true"));

                SetChildDirectories(config);
                SplitTopBottom = Convert.ToDouble(GetValue(config, SplitTopBottomName, "475"));
                SplitLeftRight = Convert.ToDouble(GetValue(config, SplitLeftRightName, "680"));
                SplitTagSelect = Convert.ToDouble(GetValue(config, SplitTagSelectName, "300"));
                AppWidth = Convert.ToInt32(GetValue(config, AppWidthName, "1024"));
                AppHeight = Convert.ToInt32(GetValue(config, AppHeightName, "768"));
                AppX = Convert.ToInt32(GetValue(config, AppXName, "0"));
                AppY = Convert.ToInt32(GetValue(config, AppYName, "0"));
                WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), GetValue(config, WindowStateName, FormWindowState.Normal.ToString("g")));
                ColumnConfig = GetValue(config, ColumnConfigName, string.Empty);
                ScreenshotPreviewSize = Convert.ToInt32(GetValue(config, ScreenshotPreviewSizeName, "0"));
                FileManagement = (FileManagement)Enum.Parse(typeof(FileManagement), GetValue(config, "FileManagement", FileManagement.Managed.ToString("g")));
                ItemsPerPage = Convert.ToInt32(GetValue(config, ItemsPerPageName, "30"));
                DeleteScreenshotsAfterImport = Convert.ToBoolean(GetValue(config, "DeleteScreenshotsAfterImport", "false"));
                LastSelectedTabIndex = Convert.ToInt32(GetValue(config, LastSelectedTabIndexName, "0"));
                TagSelectPinned = Convert.ToBoolean(GetValue(config, TagSelectPinnedName, "false"));
                ShowTabHeaders = Convert.ToBoolean(GetValue(config, ShowTabHeadersName, "true"));
                CopySaveFiles = Convert.ToBoolean(GetValue(config, CopySaveFilesName, "true"));
                AllowMultiplePlaySessions = Convert.ToBoolean(GetValue(config, AllowMultiplePlaySessionsName, "false"));
                AutomaticallyPullTitlpic = Convert.ToBoolean(GetValue(config, AutomaticallyPullTitlpicName, "true"));
                ShowPlayDialog = Convert.ToBoolean(GetValue(config, ShowPlayDialogName, "true"));
                ImportScreenshots = Convert.ToBoolean(GetValue(config, ImportScreenshotsName, "false"));

                List<EventHandler> events = new List<EventHandler>();
                if (Enum.TryParse(GetValue(config, "ColorThemeType", "Default"), out ColorThemeType colorThemeType))
                {
                    if (colorThemeType != ColorTheme)
                        AddEvent(events, ColorThemeChanged);
                    ColorTheme = colorThemeType;
                }

                var newType = (GameFileViewType)Enum.Parse(typeof(GameFileViewType), GetValue(config, "GameFileViewType", GameFileViewType.TileViewCondensed.ToString("g")));
                if (newType != GameFileViewType)
                {
                    GameFileViewType = newType;
                    AddEvent(events, GameFileViewTypeChanged);
                }

                var newVisibleViews = Util.SplitString(GetValue(config, VisibleViewsName, "Recent;Untagged;IWads;Id Games"));
                if (VisibleViews == null || (newVisibleViews.Length != VisibleViews.Count))
                {
                    VisibleViews = newVisibleViews.ToHashSet(StringComparer.OrdinalIgnoreCase);
                    AddEvent(events, VisibleViewsChanged);
                }

                DateParseFormats = Util.SplitString(GetValue(config, "DateParseFormats", "dd/M/yy;dd/MM/yyyy;dd MMMM yyyy;"));
                ScreenshotCaptureDirectories = Util.SplitString(GetValue(config, "ScreenshotCaptureDirectories", string.Empty));

                foreach (var invokeEvent in events)
                    invokeEvent?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                if (throwErrors)
                    throw;
            }

            VerifyPaths(throwErrors);
        }

        private static void AddEvent(List<EventHandler> events, EventHandler eventAdd)
        {
            if (eventAdd != null)
                events.Add(eventAdd);
        }

        private static string GetValue(IEnumerable<IConfigurationData> config, string name, string defaultValue)
        {
            var item = config.FirstOrDefault(x => x.Name == name);
            if (item == null)
                return defaultValue;
            return item.Value;
        }

        private void SetChildDirectories(IEnumerable<IConfigurationData> config)
        {
            string gameFileDir = GetValue(config, "GameFileDirectory", "GameFiles");
            GameFileDirectory = GetGameFileDir(gameFileDir);
            gameFileDir = GameFileDirectory.GetFullPath();

            ScreenshotDirectory = SetChildDirectory(gameFileDir, "Screenshots");
            TempDirectory = SetChildDirectory(gameFileDir, "Temp");
            DemoDirectory = SetChildDirectory(gameFileDir, "Demos");
            SaveGameDirectory = SetChildDirectory(gameFileDir, "SaveGames");
            ThumbnailDirectory = SetChildDirectory(gameFileDir, "Thumbnails");
        }

        private static LauncherPath GetGameFileDir(string gameFileDir)
        {
            if (gameFileDir == "GameFiles\\")
            {
                string dataDir = LauncherPath.GetDataDirectory();
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                    Directory.CreateDirectory(Path.Combine(dataDir, gameFileDir));
                }

                return new LauncherPath(Path.Combine(dataDir, gameFileDir));
            }

            return new LauncherPath(gameFileDir);
        }

        private LauncherPath SetChildDirectory(string gameFileDirectory, string childDirectory)
        {
            if (!childDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())) //might be some older code that is still not using Path.Combine...
                childDirectory = childDirectory + Path.DirectorySeparatorChar;

            return new LauncherPath(Path.Combine(gameFileDirectory, childDirectory));
        }

        public void RefreshColumnConfig()
        {
            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            ColumnConfig = GetValue(config, ColumnConfigName, string.Empty);
        }

        public void EnableCopySaveFiles()
        {
            CopySaveFiles = true;
            IConfigurationData config = DataSourceAdapter.GetConfiguration().FirstOrDefault(x => x.Name == CopySaveFilesName);
            if (config != null)
            {
                config.Value = "true";
                DataSourceAdapter.UpdateConfiguration(config);
            }
        }

        private void VerifyPaths(bool throwErrors)
        {
            VerifyPath(GameFileDirectory, throwErrors);
            VerifyPath(ScreenshotDirectory, throwErrors);
            VerifyPath(TempDirectory, throwErrors);
            VerifyPath(DemoDirectory, throwErrors);
            VerifyPath(SaveGameDirectory, throwErrors);
            VerifyPath(ThumbnailDirectory, throwErrors);
        }

        private void VerifyPath(LauncherPath path, bool throwErrors)
        {
            bool exists = Directory.Exists(path.GetFullPath());

            if (throwErrors && !exists)
                throw new DirectoryNotFoundException(path.GetPossiblyRelativePath());

            if (!exists)
                Directory.CreateDirectory(path.GetFullPath());
        }

        public IDataSourceAdapter DataSourceAdapter { get; set; }
        public LauncherPath GameFileDirectory { get; private set; }
        public LauncherPath ScreenshotDirectory { get; private set; }
        public LauncherPath SaveGameDirectory { get; private set; }
        public LauncherPath TempDirectory { get; private set; }
        public LauncherPath DemoDirectory { get; private set; }
        public LauncherPath ThumbnailDirectory { get; private set; }
        public string IdGamesUrl { get; private set; }
        public string ApiPage { get; private set; }
        public string MirrorUrl { get; private set; }
        public bool CleanTemp { get; private set; }
        public double SplitTopBottom { get; private set; }
        public double SplitLeftRight { get; private set; }
        public double SplitTagSelect { get; set; }
        public int AppWidth { get; private set; }
        public int AppHeight { get; private set; }
        public int AppX { get; private set; }
        public int AppY { get; private set; }
        public FormWindowState WindowState { get; private set; }
        public string[] ScreenshotCaptureDirectories { get; private set; }
        public string[] DateParseFormats { get; private set; }
        public string ColumnConfig { get; private set; }
        public int ScreenshotPreviewSize { get; private set; }
        public FileManagement FileManagement { get; private set; }
        public GameFileViewType GameFileViewType { get; private set; }
        public int ItemsPerPage { get; set; }
        public bool DeleteScreenshotsAfterImport { get; set; }
        public int LastSelectedTabIndex { get; set; }
        public bool TagSelectPinned { get; set; }
        public bool ShowTabHeaders { get; set; } = true;
        public bool CopySaveFiles { get; set; }
        public bool AllowMultiplePlaySessions { get; set; }
        public bool AutomaticallyPullTitlpic { get; set; }
        public bool ShowPlayDialog { get; set; }
        public bool ImportScreenshots { get; set; }
        public HashSet<string> VisibleViews { get; set; }
        public ColorThemeType ColorTheme { get; set; }
    }
}
