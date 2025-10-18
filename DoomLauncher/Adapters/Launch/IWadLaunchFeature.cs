using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher.Adapters.Launch
{
    public class IWadLaunchFeature : ILaunchFeature
    {
        private readonly IGameFile _iwad;
        private readonly bool _extractFiles;

        public IWadLaunchFeature(IGameFile iwad, bool extractFiles = true)
        {
            _iwad = iwad;
            _extractFiles = extractFiles;
        }

        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            if (!gameFile.ArchiveExists(directories.GameFileDirectory))
            {
                return LaunchParameters.Failure($"Couldn't find game file at {directories.GameFileDirectory.GetFullPath()}");
            }

            string extractedFileName;
            try
            {
                extractedFileName = GetExtractedFileName(sourcePort, directories);
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

            var paramString = sourcePort.GetFlavor().IwadParameter(new SpData(extractedFileName, gameFile, addFiles));

            return LaunchParameters.Param(paramString).WithVariableReplacement("iwad", Path.GetFileNameWithoutExtension(_iwad.FileNameNoPath));
        }

        private string GetExtractedFileName(ISourcePortData sourcePortData, IDirectoriesConfiguration directories)
        {
            using (IArchiveReader reader = _iwad.OpenGameFile(directories.GameFileDirectory))
            {
                IArchiveEntry firstMatchingEntry = GetFirstIWadEntry(reader, sourcePortData);

                if (firstMatchingEntry != null)
                {
                    if (firstMatchingEntry.ExtractRequired)
                    { 
                        string extractFile = Path.Combine(directories.TempDirectory.GetFullPath(), firstMatchingEntry.Name);
                        if (_extractFiles)
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
            return GameFilesLaunchFeature.GetRelevantEntries(reader, sourcePortData, Util.SplitString(specificFiles)).FirstOrDefault();
        }
    }
}
