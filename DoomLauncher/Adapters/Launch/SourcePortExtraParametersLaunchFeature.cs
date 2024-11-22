using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public class SourcePortExtraParametersLaunchFeature : LaunchFeature
    {
        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            return LaunchParameters.Param(sourcePort.ExtraParameters);
        }
    }
}
