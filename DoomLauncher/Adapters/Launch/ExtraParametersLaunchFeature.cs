using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Adapters.Launch
{
    public class ExtraParametersLaunchFeature : LaunchFeature
    {
        private readonly string _extraParameters;

        public ExtraParametersLaunchFeature(string extraParameters)
        {
            _extraParameters = extraParameters ?? "";
        }

        public LaunchResult CreateParam(ISourcePort sourcePort)
        {
            return LaunchResult.Success(_extraParameters);
        }
    }
}
