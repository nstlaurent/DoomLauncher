using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace DoomLauncher.Adapters.Launch
{
    public class PlayDemoLaunchFeature : LaunchFeature
    {
        private readonly String _playDemoFile;

        public PlayDemoLaunchFeature(string playDemoFile)
        {
            _playDemoFile = playDemoFile;
        }

        public LaunchResult CreateParam(ISourcePort sourcePort)
        {
            FileInfo fi = new FileInfo(_playDemoFile);

            if (!fi.Exists)
            {
                return LaunchResult.Failure($"Failed to find demo file {_playDemoFile}");
            }
            else
            {
                var paramString = sourcePort.PlayDemoParameter(new SpData(_playDemoFile));
                return LaunchResult.Success(paramString);
            }
        }
    }
}
