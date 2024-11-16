using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.IO;

namespace DoomLauncher.Adapters.Launch
{
    public class PlayDemoLaunchFeature : LaunchFeature
    {
        private readonly String _playDemoFile;

        public PlayDemoLaunchFeature(string playDemoFile)
        {
            _playDemoFile = playDemoFile;
        }

        public LaunchResult CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            FileInfo fi = new FileInfo(_playDemoFile);

            if (!fi.Exists)
            {
                return LaunchResult.Failure($"Failed to find demo file {_playDemoFile}");
            }
            else
            {
                var paramString = sourcePort.GetFlavor().PlayDemoParameter(new SpData(_playDemoFile));
                return LaunchResult.Param(paramString);
            }
        }
    }
}
