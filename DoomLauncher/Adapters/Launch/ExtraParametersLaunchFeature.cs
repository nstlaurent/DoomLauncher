using DoomLauncher.Interfaces;

namespace DoomLauncher.Adapters.Launch
{
    public class ExtraParametersLaunchFeature : LaunchFeature
    {
        private readonly string _extraParameters;
        private readonly bool _extraParametersOnly;

        public ExtraParametersLaunchFeature(string extraParameters, bool extraParametersOnly)
        {
            _extraParameters = extraParameters;
            _extraParametersOnly = extraParametersOnly;
        }

        public LaunchResult CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            if (_extraParametersOnly)
                return LaunchResult.ExclusiveParam(_extraParameters);
            else
                return LaunchResult.Param(_extraParameters);
        }
    }
}
