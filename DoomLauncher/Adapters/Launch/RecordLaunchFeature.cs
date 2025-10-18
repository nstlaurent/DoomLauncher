using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.Adapters.Launch
{
    public class RecordLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            var recordedFileName = Path.Combine(directories.TempDirectory.GetFullPath(), Guid.NewGuid().ToString());
            var paramString = sourcePort.GetFlavor().RecordParameter(new SpData(recordedFileName, gameFile, addFiles));

            return LaunchParameters.Param(paramString).WithRecordedFileName(recordedFileName);
        }
    }
}
