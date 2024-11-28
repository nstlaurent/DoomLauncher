using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;

namespace DoomLauncher.Adapters.Launch
{
    public class LoadSaveLaunchFeature : ILaunchFeature
    {
        private readonly string _loadSaveFile;

        public LoadSaveLaunchFeature(string loadSaveFile)
        {
            _loadSaveFile = loadSaveFile;
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            if (!string.IsNullOrEmpty(_loadSaveFile) && sourcePort.GetFlavor().LoadSaveGameSupported())
            {
                var paramString = sourcePort.GetFlavor().LoadSaveParameter(new SpData(_loadSaveFile));
                return LaunchParameters.Param(paramString);
            }
            else
                return LaunchParameters.EMPTY;
        }
    }
}
