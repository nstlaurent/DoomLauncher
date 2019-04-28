using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.SourcePort
{
    public static class SourcePortUtil
    {
        public static ISourcePort CreateSourcePort(ISourcePortData sourcePortData)
        {
            ISourcePort[] sourcePorts = new ISourcePort[]
            {
                new ZDoomSourcePort(sourcePortData),
                new BoomSourcePort(sourcePortData),
                new DoomsdaySourcePort(sourcePortData),
                new CNDoomSourcePort(sourcePortData),
                new GenericSourcePort(sourcePortData)
            };

            return sourcePorts.First(x => x.Supported());
        }
    }
}
