using DoomLauncher.Adapters.Launch;
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
    [Flags]
    public enum GameFilePlayAdapterOptions
    {
        None = 0,
        ExtraParamsOnly = 1
    }

    public class GameFilePlayAdapter
    {
        public event EventHandler ProcessExited;

        private readonly GameFilePlayAdapterOptions m_options;

        public GameFilePlayAdapter(GameFilePlayAdapterOptions options = GameFilePlayAdapterOptions.None)
        {
            m_options = options;
            AdditionalFiles = Array.Empty<IGameFile>();
            ExtractFiles = true;
        }

        public bool Launch(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePort, bool isGameFileIwad)
        {
            LastError = string.Empty;
            if (!Directory.Exists(sourcePort.Directory.GetFullPath()))
            {
                LastError = string.Concat("The source port directory does not exist:", Environment.NewLine, Environment.NewLine, 
                    sourcePort.Directory.GetPossiblyRelativePath());
                return false;
            }

            GameFile = gameFile;
            SourcePort = sourcePort;

            string launchParameters = GetLaunchParameters(gameFileDirectory, tempDirectory, gameFile, sourcePort, isGameFileIwad, out var error);
            if (launchParameters == null)
            {
                if (string.IsNullOrEmpty(LastError))
                    LastError = $"Failed to create launch parameters: {error}";
                return false;
            }
       
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

        public string GetLaunchParameters(LauncherPath gameFileDirectory, LauncherPath tempDirectory,
            IGameFile gameFile, ISourcePortData sourcePortData, bool isGameFileIwad, out string error)
        {
            error = string.Empty;

            var launchParameters = new List<LaunchFeature>()
            {
                new MapSkillLaunchFeature(Map, Skill),
                new ExtraParametersLaunchFeature(ExtraParameters, m_options.HasFlag(GameFilePlayAdapterOptions.ExtraParamsOnly)),
                new SourcePortExtraParametersLaunchFeature(),
            };

            ISourcePortFlavor sourcePortFlavor = sourcePortData.GetFlavor();
            StringBuilder sb = new StringBuilder();

            List<IGameFile> loadFiles = AdditionalFiles.ToList();
            if (isGameFileIwad)
                loadFiles.Remove(gameFile);
            else if (!loadFiles.Contains(gameFile))
                loadFiles.Add(gameFile);

            if (IWad != null)
            {
                if (!AssertGameFile(gameFile, gameFileDirectory, tempDirectory, sourcePortFlavor, sb))
                {
                    error = GetFileError(gameFile);
                    return null;
                }

                if (!HandleGameFileIWad(IWad, sourcePortFlavor, sourcePortData, sb, gameFileDirectory, tempDirectory, true))
                {
                    error = GetFileError(IWad);
                    return null;
                }
            }

            List<string> launchFiles = new List<string>();
            foreach (IGameFile loadFile in loadFiles)
            {
                if (!AssertGameFile(loadFile, gameFileDirectory, tempDirectory, sourcePortFlavor, sb))
                {
                    error = GetFileError(loadFile);
                    return null;
                }

                if (!HandleGameFile(loadFile, launchFiles, gameFileDirectory, tempDirectory, sourcePortData, true))
                {
                    error = GetFileError(loadFile);
                    return null;
                }
            }

            launchFiles = SortParameters(launchFiles).ToList();
            BuildLaunchString(sb, sourcePortFlavor, launchFiles);


            if (Record)
                launchParameters.Add(new RecordLaunchFeature(tempDirectory));

            if (PlayDemo)
                launchParameters.Add(new PlayDemoLaunchFeature(PlayDemoFile));

            if (SaveStatistics)
                launchParameters.Add(new StatisticsReaderLaunchFeature());

            if (!string.IsNullOrEmpty(LoadSaveFile))
                launchParameters.Add(new LoadSaveLaunchFeature(LoadSaveFile));

            var paramResult = launchParameters.Aggregate(LaunchResult.EMPTY, 
                (result, param) => result.Combine(param.CreateParam(sourcePortData, gameFile)));

            RecordedFileName = paramResult.RecordedFileName;
            LastError = paramResult.ErrorMessage;
            if (!string.IsNullOrEmpty(LastError))
                return null;

            sb.Append(paramResult.ParamString);

            SetVariableReplacements(gameFile, sb);
            return sb.ToString();
        }

        private void SetVariableReplacements(IGameFile gameFile, StringBuilder sb)
        {
            if (IWad != null)
                sb.Replace("$iwad", Path.GetFileNameWithoutExtension(IWad.FileNameNoPath));
            sb.Replace("$filename", Path.GetFileNameWithoutExtension(gameFile.FileNameNoPath));
        }

        private static string GetFileError(IGameFile gameFile) => $"Failed to add {gameFile.FileNameNoPath}";

        private bool AssertGameFile(IGameFile gameFile, LauncherPath gameFileDirectory, LauncherPath tempDirectory, ISourcePortFlavor sourcePortFlavor, StringBuilder sb)
        {
            if (gameFile.IsDirectory())
                return AssertDirectory(gameFile.FileName);

            if (gameFile.IsUnmanaged())
            {
                var launcherPath = new LauncherPath(gameFile.FileName);
                if (!AssertFile(string.Empty, launcherPath.GetFullPath(), "game file"))
                    return false;
                return true;
            }

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

        private bool HandleGameFileIWad(
            IGameFile gameFile, // Input
            ISourcePortFlavor sourcePortFlavor,  // Input
            ISourcePortData sourcePortData,  // Input
            StringBuilder sb, // Output
            LauncherPath gameFileDirectory, // Input
            LauncherPath tempDirectory,  // Input
            bool checkSpecific) // Input
        {
            try
            {
                string[] extensions = sourcePortData.SupportedExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var file = GetFilesFromGameFileSettings(gameFile, gameFileDirectory, tempDirectory, checkSpecific, extensions, GetIwadSpecificFiles(gameFile)).FirstOrDefault();
                if (file == null)
                {
                    LastError = "Failed to find any IWAD files in the select IWAD archive.\nView the IWAD and click 'Select Individual Files...' to ensure the IWAD file is selected.";
                    return false;
                }

                sb.Append(sourcePortFlavor.IwadParameter(new SpData(file, gameFile, AdditionalFiles)));
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

        private string[] GetIwadSpecificFiles(IGameFile gameFile)
        {
            if (string.IsNullOrEmpty(gameFile.SettingsSpecificFiles))
                return Array.Empty<string>();

            return gameFile.SettingsSpecificFiles.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool HandleGameFile(
            IGameFile gameFile, // Input
            List<string> launchFiles, // Output
            LauncherPath gameFileDirectory, // Input
            LauncherPath tempDirectory, // Input
            ISourcePortData sourcePort, // Input
            bool checkSpecific) // Input
        {
            if (gameFile.IsDirectory())
            {
                launchFiles.Add(gameFile.FileName);
                return true;
            }

            try
            {
                string[] extensions = sourcePort.SupportedExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                launchFiles.AddRange(GetFilesFromGameFileSettings(gameFile, gameFileDirectory, tempDirectory, checkSpecific, extensions, SpecificFiles));
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
            ISourcePortFlavor sourcePortFlavor, List<SpecificFilesForm.SpecificFilePath> pathFiles)
        {
            try
            {
                List<string> files = new List<string>();
                foreach(var pathFile in pathFiles)
                {
                    if (gameFile.IsUnmanaged())
                    {
                        files.Add(pathFile.ExtractedFile);
                        continue;
                    }

                    if (!File.Exists(pathFile.ExtractedFile))
                        continue;

                    using (IArchiveReader reader = ArchiveReader.Create(pathFile.ExtractedFile))
                    {
                        var entry = reader.Entries.FirstOrDefault(x => x.FullName == pathFile.InternalFilePath);
                        if (entry != null)
                            files.Add(Util.ExtractTempFile(tempDirectory.GetFullPath(), entry));
                    }
                }

                BuildLaunchString(sb, sourcePortFlavor, files);
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

        private void BuildLaunchString(StringBuilder sb, ISourcePortFlavor sourcePortFlavor, List<string> files)
        {
            List<string> dehFiles = new List<string>();

            if (files.Count > 0)
            {
                sb.Append(sourcePortFlavor.FileParameter(new SpData()));
                var dehExtensions = Util.GetDehackedExtensions();

                foreach (string str in files)
                {
                    FileInfo fi = new FileInfo(str);
                    if (!dehExtensions.Contains(fi.Extension, StringComparer.OrdinalIgnoreCase))
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
            bool checkSpecific, string[] extensions, string[] specificFiles)
        {
            List<string> files = new List<string>();

            using (IArchiveReader reader = CreateArchiveReader(gameFile, gameFileDirectory))
            {
                IEnumerable<IArchiveEntry> entries;
                if (checkSpecific && specificFiles != null && specificFiles.Length > 0)
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
                    if (checkSpecific && specificFiles != null && specificFiles.Length > 0)
                        useFile = specificFiles.Contains(entry.FullName);

                    if (useFile)
                    {
                        if (entry.ExtractRequired)
                        {
                            string extractFile = Path.Combine(tempDirectory.GetFullPath(), entry.Name);
                            if (ExtractFiles)
                                TryExtractFile(entry, extractFile);
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

        private static void TryExtractFile(IArchiveEntry entry, string extractFile)
        {
            try
            {
                entry.ExtractToFile(extractFile, true);
            }
            catch (Exception ex)
            {
                try
                {
                    if (File.Exists(extractFile))
                    {
                        // Sometimes the read only flag can be set and the file can't be overwritten. This is our temp directory anyway so turn it off.
                        File.SetAttributes(extractFile, File.GetAttributes(extractFile) & ~FileAttributes.ReadOnly);
                        entry.ExtractToFile(extractFile, true);
                    }
                }
                catch
                {
                    throw ex;
                }
            }
        }

        private static IArchiveReader CreateArchiveReader(IGameFile gameFile, LauncherPath gameFileDirectory)
        {
            string file;
            if (gameFile.IsUnmanaged())
                file = new LauncherPath(gameFile.FileName).GetFullPath();
            else
                file = Path.Combine(gameFileDirectory.GetFullPath(), gameFile.FileName);

            // If the unmanaged file is a pk3 then ArchiveReader.Create will read it as a zip and try to unpack
            // Return FileArchiveReader instead so the pk3 will be added as a file
            // Zip extensions are ignored in this case since Doom Launcher's base functionality revovles around reading zip contents
            // SpecificFilesForm will also read zip files explicitly to allow user to select files in the archive
            if (!gameFile.IsDirectory() && gameFile.IsUnmanaged() && !ArchiveUtil.ShouldReadPackagedArchive(gameFile.FileName))
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

        public string LastError { get; private set; } // Output

        public string RecordedFileName { get; private set; } // Output

        public IGameFile IWad { get; set; } // Input
        public string Map { get; set; } // Input
        public string Skill { get; set; } // Input
        public bool Record { get; set; } // Input
        public bool PlayDemo { get; set; } // Input
        public IGameFile[] AdditionalFiles { get; set; } // Input
        public string ExtraParameters { get; set; } // Input
        public string[] SpecificFiles { get; set; } // Input
        public bool SaveStatistics { get; set; } // Input
        public string LoadSaveFile { get; set; } // Input

        public ISourcePortData SourcePort { get; private set; } // Input/Output (but should only be input)
        public IGameFile GameFile { get; private set; } // Input/Output (but should only be input)
        
        public string PlayDemoFile { get; set; } // Input

        public bool ExtractFiles { get; set; } // Input
        public bool IgnoreExtractError { get; set; } // Input

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
