using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System.Collections.Generic;

namespace DoomLauncher.Adapters.Launch
{
    public class ExtraParametersLaunchFeature : ILaunchFeature
    {
        private readonly string _extraParameters; // Never null
        private readonly bool _extraParametersOnly;

        public ExtraParametersLaunchFeature(string extraParameters, bool extraParametersOnly)
        {
            _extraParameters = extraParameters ?? "";
            _extraParametersOnly = extraParametersOnly;
        }

        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            if (_extraParametersOnly)
                return LaunchParameters.ExclusiveParam(_extraParameters);
            else
                return LaunchParameters.Param(_extraParameters);
        }
    }
}
