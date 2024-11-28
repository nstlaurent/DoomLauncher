using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.IO;
using System.Linq;

namespace DoomLauncher.Adapters.Launch
{
    public class IWadLaunchFeature : ILaunchFeature
    {
        private readonly IGameFile _iwad;

        public IWadLaunchFeature(IGameFile iwad)
        {
            _iwad = iwad;
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            if (!gameFile.ArchiveExists(gameFileDirectory))
            {
                return LaunchParameters.Failure($"Couldn't find game file at {gameFileDirectory.GetFullPath()}");
            }

            string extractedFileName;
            try
            {
                extractedFileName = GetExtractedFileName(sourcePort, gameFileDirectory, tempDirectory);
            }
            catch (FileNotFoundException)
            {
                return LaunchParameters.Failure($"File not found: {gameFile.FileName}");
            }
            catch (IOException)
            {
                return LaunchParameters.Failure($"File in use: {gameFile.FileName}");
            }
            catch (Exception e)
            {
                return LaunchParameters.Failure($"There was an issue with the IWad: {gameFile.FileName}." +
                    $"{Environment.NewLine}{Environment.NewLine}{e.Message}");
            }

            if (extractedFileName == null)
            {
                return LaunchParameters.Failure("Failed to find any IWAD files in the select IWAD archive.\n" +
                    "View the IWAD and click 'Select Individual Files...' to ensure the IWAD file is selected.");
            }

            var paramString = sourcePort.GetFlavor().IwadParameter(new SpData(extractedFileName));

            return LaunchParameters.Param(paramString).WithVariableReplacement("iwad", Path.GetFileNameWithoutExtension(_iwad.FileNameNoPath));
        }

        private string GetExtractedFileName(ISourcePortData sourcePortData, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            using (IArchiveReader reader = _iwad.OpenGameFile(gameFileDirectory))
            {
                IArchiveEntry firstMatchingEntry = GetFirstIWadEntry(reader, sourcePortData);

                if (firstMatchingEntry != null)
                {
                    if (firstMatchingEntry.ExtractRequired)
                    { 
                        string extractFile = Path.Combine(tempDirectory.GetFullPath(), firstMatchingEntry.Name);
                        firstMatchingEntry.ExtractToFileForceOverwrite(extractFile);
                        return extractFile;
                    }
                    else
                    {
                        return firstMatchingEntry.FullName;
                    }
                }
                else return null;
            }
        }

        private IArchiveEntry GetFirstIWadEntry(IArchiveReader reader, ISourcePortData sourcePortData)
        {
            var specificFiles = _iwad.SettingsSpecificFiles;

            // Go with the IWAD's specific files if available, otherwise pick out the SourcePort's allowed extensions
            if (!string.IsNullOrEmpty(specificFiles))
            {
                return reader.Entries.Where(entry => specificFiles.Contains(entry.FullName)).FirstOrDefault();
            }
            else
            {
                return reader.Entries.Where(x => EntryMatchesSourcePortExtensions(x, sourcePortData)).FirstOrDefault();
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
