using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class GameFilePlayAdapter
    {
        public event EventHandler ProcessExited;

        public GameFilePlayAdapter()
        {
            AdditionalFiles = new IGameFile[] { };
            ExtractFiles = true;
        }

        public bool Launch(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePort, bool isGameFileIwad)
        {
            if (!Directory.Exists(sourcePort.Directory.GetFullPath()))
            {
                LastError = string.Concat("The source port directory does not exist:", Environment.NewLine, Environment.NewLine, 
                    sourcePort.Directory.GetPossiblyRelativePath());
                return false;
            }

            GameFile = gameFile;
            SourcePort = sourcePort;

            string launchParameters = GetLaunchParameters(gameFileDirectory, tempDirectory, gameFile, sourcePort, isGameFileIwad);

            if (!string.IsNullOrEmpty(launchParameters))
            {
                Directory.SetCurrentDirectory(sourcePort.Directory.GetFullPath());

                try
                {
                    Process proc = Process.Start(sourcePort.GetFullExecutablePath(), launchParameters);
                    proc.EnableRaisingEvents = true;
                    proc.Exited += proc_Exited;
                }
                catch
                {
                    LastError = "Failed to execute the source port process.";
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetLaunchParameters(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePortData, bool isGameFileIwad)
        {
            ISourcePort sourcePort = SourcePortUtil.CreateSourcePort(sourcePortData);
            StringBuilder sb = new StringBuilder();

            List<IGameFile> loadFiles = AdditionalFiles.ToList();
            if (isGameFileIwad)
                loadFiles.Remove(gameFile);
            else if (!loadFiles.Contains(gameFile))
                loadFiles.Add(gameFile);

            if (IWad != null)
            {
                if (!AssertFile(gameFileDirectory.GetFullPath(), gameFile.FileName, "game file")) return null;
                if (!HandleGameFileIWad(IWad, sourcePort, sb, gameFileDirectory, tempDirectory)) return null;
            }

            List<string> launchFiles = new List<string>();

            foreach (IGameFile loadFile in loadFiles)
            {
                if (!AssertFile(gameFileDirectory.GetFullPath(), loadFile.FileName, "game file")) return null;
                if (!HandleGameFile(loadFile, launchFiles, gameFileDirectory, tempDirectory, sourcePortData, true)) return null;
            }

            string[] extensions = sourcePortData.SupportedExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            launchFiles = SortParameters(launchFiles, extensions).ToList();

            BuildLaunchString(sb, sourcePort, launchFiles);

            if (Map != null)
            {
                sb.Append(sourcePort.WarpParameter(new SpData(Map)));

                if (Skill != null)
                    sb.Append(sourcePort.SkillParameter(new SpData(Skill)));
            }

            if (Record)
            {
                RecordedFileName = Path.Combine(tempDirectory.GetFullPath(), Guid.NewGuid().ToString());
                sb.Append(sourcePort.RecordParameter(new SpData(RecordedFileName)));
            }

            if (PlayDemo && PlayDemoFile != null)
            {
                if (!AssertFile(PlayDemoFile, "", "demo file")) return null;
                sb.Append(sourcePort.PlayDemoParameter(new SpData(PlayDemoFile)));
            }

            if (ExtraParameters != null)
                sb.Append(" " + ExtraParameters);

            if (!string.IsNullOrEmpty(sourcePortData.ExtraParameters))
                sb.Append(" " + sourcePortData.ExtraParameters);

            IStatisticsReader statsReader = sourcePort.CreateStatisticsReader(gameFile, new IStatsData[] { });
            if (SaveStatistics && statsReader != null && !string.IsNullOrEmpty(statsReader.LaunchParameter))
                sb.Append(" " + statsReader.LaunchParameter);

            return sb.ToString();
        }

        private bool HandleGameFileIWad(IGameFile gameFile, ISourcePort sourcePort, StringBuilder sb, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            try
            {
                using (ZipArchive za = ZipFile.OpenRead(Path.Combine(gameFileDirectory.GetFullPath(), gameFile.FileName)))
                {
                    ZipArchiveEntry zae = za.Entries.First();
                    string extractFile = Path.Combine(tempDirectory.GetFullPath(), zae.Name);
                    if (ExtractFiles)
                    {
                        if (File.Exists(extractFile))
                            TrySetFileAttributes(extractFile);
                        zae.ExtractToFile(extractFile, true);
                    }

                    sb.Append(sourcePort.IwadParameter(new SpData(extractFile, gameFile, AdditionalFiles)));
                }
            }
            catch (FileNotFoundException)
            {
                LastError = string.Format("File not found: {0}", gameFile.FileName);
                return false;
            }
            catch (IOException)
            {
                LastError = string.Format("File in use: {0}", gameFile.FileName);
                return false;
            }
            catch(UnauthorizedAccessException)
            {
                LastError = string.Format("Could not overwrite temporary file: {0}", gameFile.FileName);
                return false;
            }
            catch (Exception)
            {
                LastError = string.Format("There was an issue with the IWad: {0}. Corrupted file?", gameFile.FileName);
                return false;
            }

            return true;
        }

        private void TrySetFileAttributes(string file)
        {
            try
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            catch
            {
                //failed, nothing to do
            }
        }

        private bool HandleGameFile(IGameFile gameFile, List<string> launchFiles, LauncherPath gameFileDirectory, LauncherPath tempDirectory, 
            ISourcePortData sourcePort, bool checkSpecific)
        {
            try
            {
                string[] extensions = sourcePort.SupportedExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                launchFiles.AddRange(GetFilesFromGameFileSettings(gameFile, gameFileDirectory, tempDirectory, checkSpecific, extensions));
            }
            catch (FileNotFoundException)
            {
                LastError = string.Format("The game file was not found: {0}", gameFile.FileName);
                return false;
            }
            catch (InvalidDataException)
            {
                LastError = string.Format("The game file does not appear to be a valid zip file: {0}", gameFile.FileName);
                return false;
            }

            return true;
        }

        //This function is currently only used for loading files by utility (which also uses ISourcePort).
        //This uses Util.ExtractTempFile to avoid extracting files with the same name where the user can have the previous file locked.
        //E.g. opening MAP01 from a pk3, and then opening another MAP01 from a different pk3
        public bool HandleGameFile(IGameFile gameFile, StringBuilder sb, LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            ISourcePort sourcePort, List<SpecificFilesForm.SpecificFilePath> pathFiles)
        {
            try
            {
                List<string> files = new List<string>();

                foreach(var pathFile in pathFiles)
                {
                    if (File.Exists(pathFile.ExtractedFile))
                    {
                        using (ZipArchive za = ZipFile.OpenRead(pathFile.ExtractedFile))
                        {
                            var entry = za.Entries.FirstOrDefault(x => x.FullName == pathFile.InternalFilePath);
                            if (entry != null)
                                files.Add(Util.ExtractTempFile(tempDirectory.GetFullPath(), entry));
                        }
                    }
                }

                BuildLaunchString(sb, sourcePort, files);
            }
            catch (FileNotFoundException)
            {
                LastError = string.Format("The game file was not found: {0}", gameFile.FileName);
                return false;
            }
            catch (InvalidDataException)
            {
                LastError = string.Format("The game file does not appear to be a valid zip file: {0}", gameFile.FileName);
                return false;
            }

            return true;
        }

        private void BuildLaunchString(StringBuilder sb, ISourcePort sourcePort, List<string> files)
        {
            string[] dehExt = new string[] { ".deh", ".bex" }; //future - should be configurable
            List<string> dehFiles = new List<string>();

            if (files.Count > 0)
            {
                sb.Append(sourcePort.FileParameter(new SpData()));
                //if (!string.IsNullOrEmpty(sourcePort.FileOption))
                //    sb.Append(string.Concat(" ", sourcePort.FileOption, " ")); //" -file "

                foreach (string str in files)
                {
                    FileInfo fi = new FileInfo(str);
                    if (!dehExt.Contains(fi.Extension.ToLower()))
                        sb.Append(string.Format("\"{0}\" ", str));
                    else
                        dehFiles.Add(str);
                }
            }

            if (dehFiles.Count > 0)
            {
                sb.Append(" -deh ");

                foreach (string str in dehFiles)
                    sb.Append(string.Format("\"{0}\" ", str));
            }
        }

        private List<string> GetFilesFromGameFileSettings(IGameFile gameFile, LauncherPath gameFileDirectory, LauncherPath tempDirectory, bool checkSpecific, string[] extensions)
        {
            List<string> files = new List<string>();

            using (ZipArchive za = ZipFile.OpenRead(Path.Combine(gameFileDirectory.GetFullPath(), gameFile.FileName)))
            {
                var entries = za.Entries.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains('.') &&
                    extensions.Any(y => y.Equals(Path.GetExtension(x.Name), StringComparison.OrdinalIgnoreCase)));

                foreach (ZipArchiveEntry zae in entries)
                {
                    bool useFile = true;

                    if (checkSpecific && SpecificFiles != null && SpecificFiles.Length > 0)
                    {
                        useFile = SpecificFiles.Contains(zae.FullName);
                    }

                    if (useFile)
                    {
                        string extractFile = Path.Combine(tempDirectory.GetFullPath(), zae.Name);
                        if (ExtractFiles)
                            zae.ExtractToFile(extractFile, true);
                        files.Add(extractFile);
                    }
                }
            }

            return files;
        }

        private bool AssertFile(string path, string filename, string displayTypeName)
        {
            FileInfo fi = new FileInfo(Path.Combine(path, filename));

            if (!fi.Exists)
            {
                LastError = string.Format("Failed to find the {0}: {1}", displayTypeName, filename);
                return false;
            }

            return true;
        }

        public string LastError { get; private set; }
        public IGameFile IWad { get; set; }
        public string Map { get; set; }
        public string Skill { get; set; }
        public bool Record { get; set; }
        public bool PlayDemo { get; set; }
        public IGameFile[] AdditionalFiles { get; set; }
        public string ExtraParameters { get; set; }
        public string[] SpecificFiles { get; set; }
        public bool SaveStatistics { get; set; }

        public ISourcePortData SourcePort { get; private set; }
        public IGameFile GameFile { get; private set; }
        public string RecordedFileName { get; private set; }
        public string PlayDemoFile { get; set; }

        public bool ExtractFiles { get; set; }

        void proc_Exited(object sender, EventArgs e)
        {
            ProcessExited?.Invoke(this, new EventArgs());
        }

        private IEnumerable<string> SortParameters(IEnumerable<string> parameters, string[] extensionOrder)
        {
            List<string> ret = new List<string>();
            Array.ForEach(extensionOrder, x => ret.AddRange(parameters.Where(y => y.ToLower().Contains(x.ToLower()))));
            return ret;
        }
    }
}
