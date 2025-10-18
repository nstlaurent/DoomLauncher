using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DoomLauncher.Adapters.Launch
{
    public class GameFilesLaunchFeature : ILaunchFeature
    {
        private readonly List<IGameFile> m_gameFiles;
        private readonly List<string> m_specificFiles;
        private readonly bool m_extractFiles;

        public GameFilesLaunchFeature(List<IGameFile> gameFiles, List<string> specificFiles, bool extractFiles = true)
        {
            m_gameFiles = (gameFiles != null) ? new List<IGameFile>(gameFiles) : new List<IGameFile>();
            m_specificFiles = (specificFiles != null) ? new List<string>(specificFiles) : new List<string>();
            m_extractFiles = extractFiles;
        }

        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            var filesToUse = new List<IGameFile>(m_gameFiles);

            if (isGameFileIwad)
                filesToUse.Remove(gameFile);
            else if (!filesToUse.Contains(gameFile))
                filesToUse.Add(gameFile);

            List<string> launchFiles = new List<string>();
            foreach (IGameFile file in filesToUse)
            {
                if (!file.ArchiveExists(directories.GameFileDirectory))
                {
                    return LaunchParameters.Failure($"Couldn't find additional file at {new LauncherPath(file.FileName).GetFullPath()}");
                }
                launchFiles.AddRange(GetExtractedFileNames(sourcePort, file, directories));
            }

            // Build launch string
            // -file "file1", "file2",.. -deh "deh1", "deh2",.. 

            var launchString = BuildLaunchString(sourcePort.GetFlavor(), launchFiles);
            return LaunchParameters.Param(launchString);
        }

        private string BuildLaunchString(ISourcePortFlavor sourcePort, List<string> files)
        {
            StringBuilder sb = new StringBuilder();
            List<string> dehFiles = new List<string>();

            if (files.Count > 0)
            {
                sb.Append(sourcePort.FileParameter(new SpData()));
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
            return sb.ToString();
        }

        public List<string> GetExtractedFileNames(ISourcePortData sourcePortData, IGameFile gameFile, IDirectoriesConfiguration directories)
        {
            List<string> launchFiles = new List<string>();
            if (gameFile.IsDirectory())
            {
                launchFiles.Add(gameFile.FileName);
                return launchFiles;
            }

            try
            {
                using (IArchiveReader reader = gameFile.OpenGameFile(directories.GameFileDirectory))
                {
                    IEnumerable<IArchiveEntry> relevantEntries = GetRelevantEntries(reader, sourcePortData, m_specificFiles);

                    foreach (IArchiveEntry entry in relevantEntries)
                    {
                        if (entry.ExtractRequired)
                        {
                            string extractFile = Path.Combine(directories.TempDirectory.GetFullPath(), entry.Name);
                            if (m_extractFiles)
                                entry.ExtractToFileForceOverwrite(extractFile);
                            launchFiles.Add(extractFile);
                        }
                        else
                        {
                            launchFiles.Add(entry.FullName);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                string.Format("The game file was not found: {0}", gameFile.FileName);
            }
            catch (InvalidDataException)
            {
                string.Format("The game file does not appear to be a valid zip file: {0}", gameFile.FileName);
            }

            return launchFiles;
        }


        public static IEnumerable<IArchiveEntry> GetRelevantEntries(IArchiveReader reader, ISourcePortData sourcePortData, IList<string> specificFiles)
        {
            // Go with the specific files if available, otherwise pick out the SourcePort's allowed extensions
            if (specificFiles != null && specificFiles.Count > 0)
            {
                // This could be a relative unmanaged file which means the partial path must convert to a full path for comparison
                if (reader is FileArchiveReader && reader.Entries.Any())
                {
                    var filePaths = specificFiles.Select(x => new LauncherPath(x)).ToArray();
                    return reader.Entries.Where(entry => filePaths.Any(filePath => entry.FullName == filePath.GetFullPath()));
                }

                return reader.Entries.Where(entry => specificFiles.Contains(entry.FullName));
            }
            else
            {
                return reader.Entries.Where(x => EntryMatchesSourcePortExtensions(x, sourcePortData));
            }
        }

        private static bool EntryMatchesSourcePortExtensions(IArchiveEntry entry, ISourcePortData sourcePortData)
        {
            string[] extensions = sourcePortData.SupportedExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return !string.IsNullOrEmpty(entry.Name) && entry.Name.Contains('.')
                        && extensions.Any(y => y.Equals(Path.GetExtension(entry.Name), StringComparison.OrdinalIgnoreCase));
        }
    }
}
