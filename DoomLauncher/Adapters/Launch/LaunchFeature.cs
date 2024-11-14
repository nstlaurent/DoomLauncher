using DoomLauncher.SourcePort;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Adapters.Launch
{
    public interface LaunchFeature
    {
        LaunchResult CreateParam(ISourcePort sourcePort); 
    }

}
