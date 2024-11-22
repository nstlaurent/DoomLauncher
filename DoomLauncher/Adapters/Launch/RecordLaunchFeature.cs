using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.IO;

namespace DoomLauncher.Adapters.Launch
{
    public class RecordLaunchFeature : LaunchFeature
    {
        private readonly LauncherPath _tempDirectory;

        public RecordLaunchFeature(LauncherPath tempDirectory)
        {
            _tempDirectory = tempDirectory;
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            var recordedFileName = Path.Combine(_tempDirectory.GetFullPath(), Guid.NewGuid().ToString());
            var paramString = sourcePort.GetFlavor().RecordParameter(new SpData(recordedFileName));
            return LaunchParameters.ParamWithRecording(paramString, recordedFileName);
        }
    }
}
