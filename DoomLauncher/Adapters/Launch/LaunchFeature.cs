using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public interface LaunchFeature
    {
        LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile); 
    }

}
