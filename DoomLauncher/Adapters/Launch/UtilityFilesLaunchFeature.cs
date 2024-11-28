using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace DoomLauncher.Adapters.Launch
{
    public class UtilityFilesLaunchFeature : ILaunchFeature
    {
        private readonly List<SpecificFilesForm.SpecificFilePath> _pathFiles;

        public UtilityFilesLaunchFeature(List<SpecificFilesForm.SpecificFilePath> pathFiles)
        {
            _pathFiles = new List<SpecificFilesForm.SpecificFilePath>(pathFiles);
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            ISourcePortFlavor sourcePortFlavor = new GenericSourcePortFlavor(sourcePort);
            StringBuilder sb = new StringBuilder();

            try
            {
                List<string> files = new List<string>();
                foreach (var pathFile in _pathFiles)
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
                return LaunchParameters.Param(sb.ToString());
            }
            catch (FileNotFoundException)
            {
                var errorMessage = string.Format("The game file was not found: {0}", gameFile.FileName);
                return LaunchParameters.Failure(errorMessage);
            }
            catch (InvalidDataException)
            {
                var errorMessage = string.Format("The game file does not appear to be a valid zip file: {0}", gameFile.FileName);
                return LaunchParameters.Failure(errorMessage);
            }
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
    }
}
