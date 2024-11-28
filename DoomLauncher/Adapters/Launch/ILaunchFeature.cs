using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public interface ILaunchFeature
    {
        LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory); 
    }

}
