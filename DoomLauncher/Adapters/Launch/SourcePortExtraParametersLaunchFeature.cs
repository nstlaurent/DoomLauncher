using DoomLauncher.Config;
using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public class SourcePortExtraParametersLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParameter(IGameFile gameFile, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            return LaunchParameters.Param(sourcePort.ExtraParameters);
        }
    }
}
