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
                new BoomSourcePort(sourcePortData),
                new DoomsdaySourcePort(sourcePortData),
                new CNDoomSourcePort(sourcePortData),
                new ChocolateDoomSourcePort(sourcePortData),
                new CrispyDoomSourcePort(sourcePortData),
                new WoofSourcePort(sourcePortData),
                new GenericSourcePort(sourcePortData)
            };

            return sourcePorts.First(x => x.Supported());
        }
    }
}
