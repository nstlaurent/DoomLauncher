using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public interface LaunchFeature
    {
        LaunchResult CreateParam(ISourcePortData sourcePort, IGameFile gameFile); 
    }

}
