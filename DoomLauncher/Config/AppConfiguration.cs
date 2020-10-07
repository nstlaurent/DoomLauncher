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

        public static string SplitTopBottomName => "SplitTopBottom";
        public static string SplitLeftRightName => "SplitLeftRight";
        public static string AppWidthName => "AppWidth";
        public static string AppHeightName => "AppHeight";
        public static string AppXName => "AppX";
        public static string AppYName => "AppY";
        public static string WindowStateName => "WindowState";
        public static string ColumnConfigName => "ColumnConfig";
        public static string ScreenshotPreviewSizeName => "ScreenshotPreviewSize";
        public static string ItemsPerPageName => "ItemsPerPage";
        public static string LastSelectedTab => "LastSelectedTab";

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
                int iValue = -1;
                int.TryParse(value, out iValue);
                return iValue;
            }
            else if (type == typeof(bool))
            {
                bool bValue = false;
                bool.TryParse(value, out bValue);
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

                IdGamesUrl = GetValue(config, "IdGamesUrl");
                ApiPage = GetValue(config, "ApiPage");
                MirrorUrl = GetValue(config, "MirrorUrl");
                CleanTemp = Convert.ToBoolean(GetValue(config, "CleanTemp"));

                SetChildDirectories(config);
                SplitTopBottom = Convert.ToInt32(GetValue(config, SplitTopBottomName));
                SplitLeftRight = Convert.ToInt32(GetValue(config, SplitLeftRightName));
                AppWidth = Convert.ToInt32(GetValue(config, AppWidthName));
                AppHeight = Convert.ToInt32(GetValue(config, AppHeightName));
                AppX = Convert.ToInt32(GetValue(config, AppXName));
                AppY = Convert.ToInt32(GetValue(config, AppYName));
                WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), GetValue(config, WindowStateName));
                ColumnConfig = GetValue(config, ColumnConfigName);
                ScreenshotPreviewSize = Convert.ToInt32(GetValue(config, ScreenshotPreviewSizeName));
                FileManagement = (FileManagement)Enum.Parse(typeof(FileManagement), GetValue(config, "FileManagement"));
                ItemsPerPage = Convert.ToInt32(GetValue(config, ItemsPerPageName));
                DeleteScreenshotsAfterImport = Convert.ToBoolean(GetValue(config, "DeleteScreenshotsAfterImport"));
                LastSelectedTabIndex = Convert.ToInt32(GetValue(config, LastSelectedTab));

                var newType = (GameFileViewType)Enum.Parse(typeof(GameFileViewType), GetValue(config, "GameFileViewType"));
                if (newType != GameFileViewType)
                {
                    GameFileViewType = newType;
                    GameFileViewTypeChanged?.Invoke(this, EventArgs.Empty);
                }

                DateParseFormats = Util.SplitString(GetValue(config, "DateParseFormats"));
                ScreenshotCaptureDirectories = Util.SplitString(GetValue(config, "ScreenshotCaptureDirectories"));
            }
            catch
            {
                if (throwErrors)
                    throw;
            }

            VerifyPaths(throwErrors);
        }

        private static string GetValue(IEnumerable<IConfigurationData> config, string name)
        {
            return config.First(x => x.Name == name).Value;
        }

        private void SetChildDirectories(IEnumerable<IConfigurationData> config)
        {
            string gameFileDir = GetValue(config, "GameFileDirectory");
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
            ColumnConfig = GetValue(config, ColumnConfigName);
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
        public int SplitTopBottom { get; private set; }
        public int SplitLeftRight { get; private set; }
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
    }
}
