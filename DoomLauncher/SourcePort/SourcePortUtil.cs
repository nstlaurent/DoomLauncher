using DoomLauncher.Interfaces;
using System.Linq;

namespace DoomLauncher.SourcePort
{
    public static class SourcePortUtil
    {
        public static ISourcePort CreateSourcePort(ISourcePortData sourcePortData)
        {
            ISourcePort[] sourcePorts = new ISourcePort[]
            {
                new ZDoomSourcePort(sourcePortData),
                new StatdumpSourcePort(sourcePortData),
                new LevelstatSourcePort(sourcePortData),
                new DoomsdaySourcePort(sourcePortData),
                new HelionSourcePort(sourcePortData),
                new GenericSourcePort(sourcePortData)
            };

            return sourcePorts.First(x => x.Supported());
        }
    }
}
