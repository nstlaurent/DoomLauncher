using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class AppConfiguration
    {
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
                SplitTopBottom = Convert.ToInt32(GetValue(config, "SplitTopBottom"));
                SplitLeftRight = Convert.ToInt32(GetValue(config, "SplitLeftRight"));
                AppWidth = Convert.ToInt32(GetValue(config, "AppWidth"));
                AppHeight = Convert.ToInt32(GetValue(config, "AppHeight"));
                AppX = Convert.ToInt32(GetValue(config, "AppX"));
                AppY = Convert.ToInt32(GetValue(config, "AppY"));
                WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), GetValue(config, "WindowState"));
                ColumnConfig = GetValue(config, "ColumnConfig");
                ScreenshotPreviewSize = Convert.ToInt32(GetValue(config, "ScreenshotPreviewSize"));

                DateParseFormats = SplitString(GetValue(config, "DateParseFormats"));
                ScreenshotCaptureDirectories = SplitString(GetValue(config, "ScreenshotCaptureDirectories"));
            }
            catch(Exception ex)
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
            ScreenshotDirectory = SetChildDirectory(gameFileDir, "Screenshots");
            TempDirectory = SetChildDirectory(gameFileDir, "Temp");
            DemoDirectory = SetChildDirectory(gameFileDir, "Demos");
            SaveGameDirectory = SetChildDirectory(gameFileDir, "SaveGames");
            GameFileDirectory = new LauncherPath(gameFileDir);
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
            ColumnConfig = GetValue(config, "ColumnConfig");
        }

        private string[] SplitString(string item)
        {
            if (!string.IsNullOrEmpty(item))
                return item.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                return new string[] { };
        }

        private void VerifyPaths(bool throwErrors)
        {
            VerifyPath(GameFileDirectory, throwErrors);
            VerifyPath(ScreenshotDirectory, throwErrors);
            VerifyPath(TempDirectory, throwErrors);
            VerifyPath(DemoDirectory, throwErrors);
            VerifyPath(SaveGameDirectory, throwErrors);
        }

        private void VerifyPath(LauncherPath path, bool throwErrors)
        {
            if (throwErrors && !Directory.Exists(path.GetFullPath()))
                throw new DirectoryNotFoundException(path.GetPossiblyRelativePath());
        }

        public IDataSourceAdapter DataSourceAdapter { get; set; }
        public LauncherPath GameFileDirectory { get; private set; }
        public LauncherPath ScreenshotDirectory { get; private set; }
        public LauncherPath SaveGameDirectory { get; private set; }
        public LauncherPath TempDirectory { get; private set; }
        public LauncherPath DemoDirectory { get; private set; }
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
    }
}
