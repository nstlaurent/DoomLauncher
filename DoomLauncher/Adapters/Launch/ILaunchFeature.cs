using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System.Collections.Generic;

namespace DoomLauncher.Adapters.Launch
{
    public interface ILaunchFeature
    {
        LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories); 
    }

}
