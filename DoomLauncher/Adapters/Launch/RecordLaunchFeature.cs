using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.IO;

namespace DoomLauncher.Adapters.Launch
{
    public class RecordLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            var recordedFileName = Path.Combine(tempDirectory.GetFullPath(), Guid.NewGuid().ToString()); // WRONG WRONG SHOULD BE DEMO DIRECTORY
            var paramString = sourcePort.GetFlavor().RecordParameter(new SpData(recordedFileName));

            return LaunchParameters.Param(paramString).WithRecordedFileName(recordedFileName);
        }
    }
}
