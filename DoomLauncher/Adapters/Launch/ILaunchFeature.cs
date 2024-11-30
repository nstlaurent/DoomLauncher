using DoomLauncher.Config;
using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public interface ILaunchFeature
    {
        LaunchParameters CreateParameter(IGameFile gameFile, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories); 
    }

}
