using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Adapters.Launch
{
    public class RecordLaunchFeature : LaunchFeature
    {
        private readonly LauncherPath _tempDirectory;

        public RecordLaunchFeature(LauncherPath tempDirectory)
        {
            _tempDirectory = tempDirectory;
        }

        public LaunchResult CreateParam(ISourcePort sourcePort)
        {
            var recordedFileName = Path.Combine(_tempDirectory.GetFullPath(), Guid.NewGuid().ToString());
            var paramString = sourcePort.RecordParameter(new SpData(recordedFileName));
            return LaunchResult.Success(paramString, recordedFileName);
        }
    }
}
