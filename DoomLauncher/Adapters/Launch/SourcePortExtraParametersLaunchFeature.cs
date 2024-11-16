using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public class SourcePortExtraParametersLaunchFeature : LaunchFeature
    {
        public LaunchResult CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            return LaunchResult.Param(sourcePort.ExtraParameters);
        }
    }
}
