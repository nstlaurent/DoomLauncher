using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace DoomLauncher.Adapters.Launch
{
    public class AdditionalFilesLaunchFeature : ILaunchFeature
    {
        private List<IGameFile> _additionalFiles;
        private List<string> _specificFiles;

        public AdditionalFilesLaunchFeature(List<IGameFile> additionalFiles, List<string> specificFiles)
        {
            _additionalFiles = (additionalFiles != null) ? new List<IGameFile>(additionalFiles) : new List<IGameFile>();
            _specificFiles = (specificFiles != null) ? new List<string>(specificFiles) : new List<string>();
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePortData, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            var filesToUse = new List<IGameFile>(_additionalFiles);

            if (isGameFileIwad)
                filesToUse.Remove(gameFile);
            else if (!filesToUse.Contains(gameFile))
                filesToUse.Add(gameFile);

            List<string> launchFiles = new List<string>();
            foreach (IGameFile file in filesToUse)
            {
                if (!file.ArchiveExists(gameFileDirectory))
                {
                    return LaunchParameters.Failure($"Couldn't find additional file at {new LauncherPath(file.FileName).GetFullPath()}");
                }
                launchFiles.AddRange(GetExtractedFileNames(sourcePortData, file, gameFileDirectory, tempDirectory));
            }

            // Build launch string
            // -file "file1", "file2",.. -deh "deh1", "deh2",.. 

            StringBuilder sb = new StringBuilder();
            BuildLaunchString(sb, sourcePortData.GetFlavor(), launchFiles);
            return LaunchParameters.Param(sb.ToString());
        }

        private void BuildLaunchString(StringBuilder sb, ISourcePortFlavor sourcePort, List<string> files)
        {
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
        }

        public List<string> GetExtractedFileNames(ISourcePortData sourcePortData, IGameFile gameFile, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            List<string> launchFiles = new List<string>();
            if (gameFile.IsDirectory())
            {
                launchFiles.Add(gameFile.FileName);
                return launchFiles;
            }

            try
            {
                using (IArchiveReader reader = gameFile.OpenGameFile(gameFileDirectory))
                {
                    IEnumerable<IArchiveEntry> relevantEntries = GetRelevantEntries(reader, sourcePortData);

                    foreach (IArchiveEntry entry in relevantEntries)
                    {
                        if (entry.ExtractRequired)
                        {
                            string extractFile = Path.Combine(tempDirectory.GetFullPath(), entry.Name);
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


        private IEnumerable<IArchiveEntry> GetRelevantEntries(IArchiveReader reader, ISourcePortData sourcePortData)
        {
            // Go with the specific files if available, otherwise pick out the SourcePort's allowed extensions
            if (_specificFiles != null && _specificFiles.Count > 0)
            {
                return reader.Entries.Where(entry => _specificFiles.Contains(entry.FullName));
            }
            else
            {
                return reader.Entries.Where(x => EntryMatchesSourcePortExtensions(x, sourcePortData));
            }
        }

        private bool EntryMatchesSourcePortExtensions(IArchiveEntry entry, ISourcePortData sourcePortData)
        {
            string[] extensions = sourcePortData.SupportedExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return !string.IsNullOrEmpty(entry.Name) && entry.Name.Contains('.')
                        && extensions.Any(y => y.Equals(Path.GetExtension(entry.Name), StringComparison.OrdinalIgnoreCase));
        }
    }
}
