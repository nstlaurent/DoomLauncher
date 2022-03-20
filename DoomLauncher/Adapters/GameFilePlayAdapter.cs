using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DoomLauncher
{
    public class GameFilePlayAdapter
    {
        public event EventHandler ProcessExited;

        public GameFilePlayAdapter()
        {
            AdditionalFiles = Array.Empty<IGameFile>();
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
                if (!AssertGameFile(gameFile, gameFileDirectory, tempDirectory, sourcePort, sb)) return null;
                if (!HandleGameFileIWad(IWad, sourcePort, sb, gameFileDirectory, tempDirectory)) return null;
            }

            List<string> launchFiles = new List<string>();

            foreach (IGameFile loadFile in loadFiles)
            {
                if (!AssertGameFile(loadFile, gameFileDirectory, tempDirectory, sourcePort, sb)) return null;
                if (!HandleGameFile(loadFile, launchFiles, gameFileDirectory, tempDirectory, sourcePortData, true)) return null;
            }

            launchFiles = SortParameters(launchFiles).ToList();
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

            IStatisticsReader statsReader = sourcePort.CreateStatisticsReader(gameFile, Array.Empty<IStatsData>());
            if (SaveStatistics && statsReader != null && !string.IsNullOrEmpty(statsReader.LaunchParameter))
                sb.Append(" " + statsReader.LaunchParameter);

            if (!string.IsNullOrEmpty(LoadSaveFile) && sourcePort.LoadSaveGameSupported())
                sb.Append(" " + sourcePort.LoadSaveParameter(new SpData(LoadSaveFile)));

            return sb.ToString();
        }

        private bool AssertGameFile(IGameFile gameFile, LauncherPath gameFileDirectory, LauncherPath tempDirectory, ISourcePort sourcePort, StringBuilder sb)
        {
            if (gameFile.IsDirectory())
                return AssertDirectory(gameFile.FileName);

            if (!AssertFile(gameFileDirectory.GetFullPath(), gameFile.FileName, "game file"))
                return false;

            return true;
        }

        private bool AssertDirectory(string dir)
        {
            if (Directory.Exists(dir))
                return true;

            LastError = $"Directory {dir} does not exist.";
            return false;
        }

        private bool HandleGameFileIWad(IGameFile gameFile, ISourcePort sourcePort, StringBuilder sb, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            try
            {
                using (IArchiveReader reader = CreateArchiveReader(gameFile, gameFileDirectory))
                {
                    IArchiveEntry entry = reader.Entries.First();
                    string extractFile = Path.Combine(tempDirectory.GetFullPath(), entry.Name);
                    ExtractEntryFile(entry, extractFile);

                    if (!entry.ExtractRequired)
                        extractFile = entry.FullName;

                    sb.Append(sourcePort.IwadParameter(new SpData(extractFile, gameFile, AdditionalFiles)));
                }
            }
            catch (FileNotFoundException)
            {
                LastError = $"File not found: {gameFile.FileName}";
                return false;
            }
            catch (IOException)
            {
                LastError = $"File in use: {gameFile.FileName}";
                return false;
            }
            catch (Exception e)
            {
                LastError = $"There was an issue with the IWad: {gameFile.FileName}." +
                    $"{Environment.NewLine}{Environment.NewLine}{e.Message}";
                return false;
            }

            return true;
        }

        private void ExtractEntryFile(IArchiveEntry entry, string extractFile)
        {
            try
            {
                if (ExtractFiles && entry.ExtractRequired)
                    entry.ExtractToFile(extractFile, true);
            }
            catch (IOException)
            {
                if (IgnoreExtractError)
                    return;
                throw;
            }
        }

        private bool HandleGameFile(IGameFile gameFile, List<string> launchFiles, LauncherPath gameFileDirectory, LauncherPath tempDirectory, 
            ISourcePortData sourcePort, bool checkSpecific)
        {
            if (gameFile.IsDirectory())
            {
                launchFiles.Add(gameFile.FileName);
                return true;
            }

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
        public bool HandleGameFile(IGameFile gameFile, StringBuilder sb, LauncherPath tempDirectory,
            ISourcePort sourcePort, List<SpecificFilesForm.SpecificFilePath> pathFiles)
        {
            try
            {
                List<string> files = new List<string>();

                foreach(var pathFile in pathFiles)
                {
                    if (File.Exists(pathFile.ExtractedFile))
                    {
                        using (IArchiveReader reader = ArchiveReader.Create(pathFile.ExtractedFile))
                        {
                            var entry = reader.Entries.FirstOrDefault(x => x.FullName == pathFile.InternalFilePath);
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
            List<string> dehFiles = new List<string>();

            if (files.Count > 0)
            {
                sb.Append(sourcePort.FileParameter(new SpData()));
                var dehExtensions = Util.GetDehackedExtensions();

                foreach (string str in files)
                {
                    FileInfo fi = new FileInfo(str);
                    if (!dehExtensions.Contains(fi.Extension.ToLower()))
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

        private List<string> GetFilesFromGameFileSettings(IGameFile gameFile, LauncherPath gameFileDirectory, LauncherPath tempDirectory, 
            bool checkSpecific, string[] extensions)
        {
            List<string> files = new List<string>();

            using (IArchiveReader reader = CreateArchiveReader(gameFile, gameFileDirectory))
            {
                IEnumerable<IArchiveEntry> entries;
                if (checkSpecific && SpecificFiles != null && SpecificFiles.Length > 0)
                {
                    entries = reader.Entries;
                }
                else
                {
                    entries = reader.Entries.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains('.') &&
                        extensions.Any(y => y.Equals(Path.GetExtension(x.Name), StringComparison.OrdinalIgnoreCase)));
                }

                foreach (IArchiveEntry entry in entries)
                {
                    bool useFile = true;
                    if (checkSpecific && SpecificFiles != null && SpecificFiles.Length > 0)
                        useFile = SpecificFiles.Contains(entry.FullName);

                    if (useFile)
                    {
                        if (entry.ExtractRequired)
                        {
                            string extractFile = Path.Combine(tempDirectory.GetFullPath(), entry.Name);
                            if (ExtractFiles)
                                entry.ExtractToFile(extractFile, true);
                            files.Add(extractFile);
                        }
                        else
                        {
                            files.Add(entry.FullName);
                        }
                    }
                }
            }

            return files;
        }

        private IArchiveReader CreateArchiveReader(IGameFile gameFile, LauncherPath gameFileDirectory)
        {
            string file = Path.Combine(gameFileDirectory.GetFullPath(), gameFile.FileName);

            // If the unmanaged file is a pk3 then ArchiveReader.Create will read it as a zip and try to unpack
            // Return FileArchiveReader instead so the pk3 will be added as a file
            // Zip extensions are ignored in this case since Doom Launcher's base functionality revovles around reading zip contents
            // SpecificFilesForm will also read zip files explicitly to allow user to select files in the archive
            if (!gameFile.IsDirectory() && gameFile.IsUnmanaged() && !Path.GetExtension(gameFile.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                return new FileArchiveReader(file);

            return ArchiveReader.Create(file);
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
        public string LoadSaveFile { get; set; }

        public ISourcePortData SourcePort { get; private set; }
        public IGameFile GameFile { get; private set; }
        public string RecordedFileName { get; private set; }
        public string PlayDemoFile { get; set; }

        public bool ExtractFiles { get; set; }
        public bool IgnoreExtractError { get; set; }

        void proc_Exited(object sender, EventArgs e)
        {
            ProcessExited?.Invoke(this, new EventArgs());
        }

        // Take .deh and .bex files and put them together so they cane be put in the same parameter
        private IEnumerable<string> SortParameters(IEnumerable<string> parameters)
        {
            List<string> dehFiles = new List<string>();
            var dehExtensions = Util.GetDehackedExtensions();

            foreach (string file in parameters)
            {
                foreach (string deh in dehExtensions)
                {
                    if (Path.GetExtension(file).Equals(deh, StringComparison.OrdinalIgnoreCase))
                        dehFiles.Add(file);
                }
            }

            return parameters.Except(dehFiles).Union(dehFiles).ToList();
        }
    }
}
