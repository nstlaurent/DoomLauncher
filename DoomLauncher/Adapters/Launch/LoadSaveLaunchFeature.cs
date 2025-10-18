using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System.Collections.Generic;

namespace DoomLauncher.Adapters.Launch
{
    public class LoadSaveLaunchFeature : ILaunchFeature
    {
        private readonly string _loadSaveFile;

        public LoadSaveLaunchFeature(string loadSaveFile)
        {
            _loadSaveFile = loadSaveFile;
        }

        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            if (!string.IsNullOrEmpty(_loadSaveFile) && sourcePort.GetFlavor().LoadSaveGameSupported())
            {
                var paramString = sourcePort.GetFlavor().LoadSaveParameter(new SpData(_loadSaveFile, gameFile, addFiles));
                return LaunchParameters.Param(paramString);
            }
            else
                return LaunchParameters.EMPTY;
        }
    }
}
