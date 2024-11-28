using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public class SourcePortExtraParametersLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            return LaunchParameters.Param(sourcePort.ExtraParameters);
        }
    }
}
