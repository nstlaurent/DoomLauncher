using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;

namespace DoomLauncher.Adapters.Launch
{
    public class LoadSaveLaunchFeature : LaunchFeature
    {
        private readonly string _loadSaveFile;

        public LoadSaveLaunchFeature(string loadSaveFile)
        {
            _loadSaveFile = loadSaveFile;
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            string paramString;
            if (sourcePort.GetFlavor().LoadSaveGameSupported())
                paramString = sourcePort.GetFlavor().LoadSaveParameter(new SpData(_loadSaveFile));
            else
                paramString = "";

            return LaunchParameters.Param(paramString);
        }
    }
}
