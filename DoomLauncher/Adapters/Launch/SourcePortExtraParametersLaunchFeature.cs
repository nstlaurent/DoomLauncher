using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System.Collections.Generic;

namespace DoomLauncher.Adapters.Launch
{
    public class SourcePortExtraParametersLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            return LaunchParameters.Param(sourcePort.ExtraParameters);
        }
    }
}
