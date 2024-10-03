using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DoomLauncher.Config;
using DoomLauncher.WindowsVersion;
using Microsoft.Win32;
using DoomLauncher.DataSources;

namespace DoomLauncher
{
    public static class ProgramInit
    {
        public static bool Init()
        {
            bool success = false;
            CleanOldLibraries();
            CheckInteropUpdate();

            if (VerifyDatabase())
            {
                string dataSource = Path.Combine(LauncherPath.GetDataDirectory(), DbDataSourceAdapter.DatabaseFileName);
                IDataSourceAdapter dataSourceAdapter = DbDataSourceAdapter.CreateAdapter();
                DataCache.Instance.Init(dataSourceAdapter);

                BackupDatabase(dataSource);
                CreateSendToLink();
                KillRunningApps();

                success = VerifyGameFilesDirectory();
            }

            return success;
        }

        public static void CheckSystemTheme()
        {
            try
            {
                var adapter = DataCache.Instance.DataSourceAdapter;
                if (DataCache.Instance.AppConfiguration.ConfigurationColorTheme != ColorThemeType.System &&
                    adapter.GetSourcePorts().Any())
                {
                    DataCache.Instance.AppConfiguration.ColorTheme = DataCache.Instance.AppConfiguration.ConfigurationColorTheme;
                    return;
                }

                ColorThemeType theme;
                switch (WindowsVersionInfo.GetTheme())
                {
                    case WindowsTheme.Light:
                        theme = ColorThemeType.Default;
                        ColorTheme.Current = new DefaultTheme();
                        break;
                    case WindowsTheme.Dark:
                    case WindowsTheme.Unknown:
                    default:
                        theme = ColorThemeType.Dark;
                        ColorTheme.Current = new DarkTheme();
                        break;
                }

                DataCache.Instance.AppConfiguration.ColorTheme = theme;
            }
            catch
            {

            }
        }

        private static bool VerifyGameFilesDirectory()
        {
            if (Directory.Exists(DataCache.Instance.AppConfiguration.GameFileDirectory.GetFullPath()))
                return true;

            bool check = false;

            try
            {
                InitGameFilesDebug();
                check = InitFileCheck("GameFiles", "GameFiles_", true);

                if (!check)
                {
                    MessageBox.Show("Initialization failure. Could not find DoomLauncher GameFiles directory. Please update your settings to continue.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    DialogResult result;
                    bool success = false;

                    do
                    {
                        success = ShowSettings(true, out result);
                    } while (result != DialogResult.Cancel && !success);

                    check = success;
                }
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(null, ex);
            }

            return check;
        }

        private static bool ShowSettings(bool allowCancel, out DialogResult result)
        {
            SettingsForm form = new SettingsForm(DataCache.Instance.DataSourceAdapter, DataCache.Instance.AppConfiguration);
            form.SetCancelAllowed(allowCancel);
            form.StartPosition = FormStartPosition.CenterParent;
            result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    DataCache.Instance.AppConfiguration.Refresh();
                }
                catch (DirectoryNotFoundException ex)
                {
                    MessageBox.Show(string.Format("The directory {0} was not found. DoomLauncher will not operate correctly with invalid paths. " +
                        "Make sure the directory you are setting contains all folders required (Demos, SaveGames, Screenshots, Temp)", ex.Message),
                        "Invalid Directory",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    Util.DisplayUnexpectedException(null, ex);
                    return false;
                }
            }

            return true;
        }

        [Conditional("DEBUG")]
        private static void InitGameFilesDebug()
        {
            try
            {
                string basePath = "GameFiles";

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(Path.Combine(basePath, "Demos"));
                    Directory.CreateDirectory(Path.Combine(basePath, "SaveGames"));
                    Directory.CreateDirectory(Path.Combine(basePath, "Screenshots"));
                    Directory.CreateDirectory(Path.Combine(basePath, "Temp"));
                }
            }
            catch
            {
                // For local debug code only, just catch if we testing in debug outside of dev
            }
        }

        private static void BackupDatabase(string dataSource)
        {
            FileInfo fi = new FileInfo(dataSource);

            if (fi.Exists)
            {
                Directory.CreateDirectory(Path.Combine(LauncherPath.GetDataDirectory(), "Backup"));
                string backupName = GetBackupFileName(fi);

                FileInfo fiBackup = new FileInfo(backupName);
                if (!fiBackup.Exists)
                    fi.CopyTo(backupName);

                CleanupBackupDirectory();
            }
        }

        private static void CleanupBackupDirectory()
        {
            string[] files = Directory.GetFiles(Path.Combine(LauncherPath.GetDataDirectory(), "Backup"), "*.sqlite");
            List<FileInfo> filesInfo = new List<FileInfo>();
            Array.ForEach(files, x => filesInfo.Add(new FileInfo(x)));
            List<FileInfo> filesInfoOrdered = filesInfo.OrderBy(x => x.CreationTime).ToList();

            while (filesInfoOrdered.Count > 10)
            {
                filesInfoOrdered.First().Delete();
                filesInfoOrdered.RemoveAt(0);
            }
        }

        private static string GetBackupFileName(FileInfo fi)
        {
            string end = DateTime.Now.ToString("yyyy_MM_dd") + fi.Extension;
            return Path.Combine(fi.DirectoryName, "Backup", fi.Name.Replace(fi.Extension, end));
        }

        private static bool VerifyDatabase()
        {
            bool check = false;
            try
            {
                if (File.Exists(Path.Combine(LauncherPath.GetDataDirectory(), DbDataSourceAdapter.DatabaseFileName)))
                {
                    check = true;
                    // Still attempt to delete the init database here for people that manually update (not installed)
                    if (File.Exists(DbDataSourceAdapter.InitDatabaseFileName))
                        File.Delete(DbDataSourceAdapter.InitDatabaseFileName);
                    return check;
                }

                check = InitFileCheck(DbDataSourceAdapter.DatabaseFileName, DbDataSourceAdapter.InitDatabaseFileName, false);

                if (!check)
                {
                    MessageBox.Show("Initialization failure. Could not find DoomLauncher database",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(null, ex);
            }

            return check;
        }

        private static bool InitFileCheck(string initFile, string file, bool directory)
        {
            string dataSource = Path.Combine(Directory.GetCurrentDirectory(), initFile);
            string initDataSource = Path.Combine(Directory.GetCurrentDirectory(), file);

            if (directory)
            {
                DirectoryInfo diInit = new DirectoryInfo(initDataSource);
                DirectoryInfo diSource = new DirectoryInfo(dataSource);

                if (diSource.Exists)
                {
                    if (diInit.Exists)
                        diInit.Delete(true);
                }
                else
                {
                    if (diInit.Exists)
                        diInit.MoveTo(dataSource);
                    else
                        return false;
                }
            }
            else
            {
                FileInfo fiInit = new FileInfo(initDataSource);

                if (File.Exists(dataSource))
                {
                    if (fiInit.Exists)
                        fiInit.Delete();
                }
                else
                {
                    if (fiInit.Exists)
                        fiInit.MoveTo(dataSource);
                    else
                        return false;
                }
            }

            return true;
        }

        private static void CleanOldLibraries()
        {
            // Need to remove the old sqlite interop, only if it wasn't done through the installer
            // The installer doesn't really support x86/x64 folders so the x64 dll is dumped in the main for now
            if (LauncherPath.IsInstalled())
                return;

            try
            {
                string[] libraries = new string[] { "SQLite.Interop.dll", "SQLite.Interop.dll.bak", "7z.dll" };
                foreach (string library in libraries)
                {
                    if (!File.Exists(library))
                        continue;
                    File.Delete(library);
                }
            }
            catch
            {
                // Do not crash if it fails to delete for any reason
            }
        }

        private static void CheckInteropUpdate()
        {
            if (LauncherPath.IsInstalled())
                return;

            string[] dirs = new string[] { "x86", "x64" };
            string updateFile = Path.Combine("GameFiles\\Temp", UpdateControl.AppUpdateFileName);
            if (!File.Exists(updateFile) || AllInteropsExist())
                return;

            using (ZipArchive za = ZipFile.OpenRead(updateFile))
            {
                foreach (string dir in dirs)
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var entries = za.Entries.Where(x => x.FullName.Contains(dir));
                    foreach (var entry in entries)
                        entry.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), dir, entry.Name), true);
                }
            }
        }

        private static bool AllInteropsExist()
        {
            DirectoryInfo[] dirs = new DirectoryInfo[] { new DirectoryInfo("x86"), new DirectoryInfo("x64") };

            foreach (DirectoryInfo dir in dirs)
            {
                if (!dir.Exists)
                    return false;

                FileInfo[] files = dir.GetFiles();
                if (!files.Any(x => x.Name.Equals("SQLite.Interop.dll", StringComparison.OrdinalIgnoreCase)))
                    return false;
                if (!files.Any(x => x.Name.Equals("7z.dll", StringComparison.OrdinalIgnoreCase)))
                    return false;
            }

            return true;
        }

        private static void CreateSendToLink()
        {
            //http://stackoverflow.com/questions/234231/creating-application-shortcut-in-a-directory
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
            dynamic shell = Activator.CreateInstance(t);
            try
            {
                string sendToPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\SendTo");
                var lnk = shell.CreateShortcut(Path.Combine(sendToPath, "DoomLauncher.lnk"));

                // This should always exist, but a user did report not having this folder on Windows 10...
                if (!Directory.Exists(sendToPath))
                    Directory.CreateDirectory(sendToPath);

                try
                {
                    lnk.TargetPath = Path.Combine(Directory.GetCurrentDirectory(), Util.GetExecutableNoPath());
                    lnk.IconLocation = string.Format(Path.Combine(Directory.GetCurrentDirectory(), "DoomLauncher.ico"));
                    lnk.Save();
                }
                catch
                {
                    // Do not crash just for failing to create SendTo link
                }
                finally
                {
                    Marshal.FinalReleaseComObject(lnk);
                }
            }
            catch
            {
                // Do not crash just for failing to create SendTo link
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        private static void KillRunningApps()
        {
            try
            {
                Process currentProc = Process.GetCurrentProcess();
                foreach (Process proc in Process.GetProcessesByName("DoomLauncher").Where(x => x.Id != currentProc.Id))
                {
                    proc.CloseMainWindow();

                    Stopwatch sw = Stopwatch.StartNew();
                    while (sw.Elapsed.TotalSeconds < 10 && !proc.HasExited)
                    {
                        System.Threading.Thread.Sleep(100);
                        proc.Refresh();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Doom Launcher is already running and could not be stopped. It is not recommended to have more than instance running!", "Already Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
