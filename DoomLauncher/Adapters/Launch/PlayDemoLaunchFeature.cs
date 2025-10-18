using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.Adapters.Launch
{
    public class PlayDemoLaunchFeature : ILaunchFeature
    {
        private readonly string _playDemoFile;

        public PlayDemoLaunchFeature(string playDemoFile)
        {
            _playDemoFile = playDemoFile;
        }

        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            FileInfo fi = new FileInfo(_playDemoFile);

            if (!fi.Exists)
            {
                return LaunchParameters.Failure($"Failed to find demo file {_playDemoFile}");
            }
            else
            {
                var paramString = sourcePort.GetFlavor().PlayDemoParameter(new SpData(_playDemoFile, gameFile, addFiles));
                return LaunchParameters.Param(paramString);
            }
        }
    }
}
