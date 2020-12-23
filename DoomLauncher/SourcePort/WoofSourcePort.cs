using DoomLauncher.Interfaces;
using System;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class WoofSourcePort : ChocolateDoomSourcePort
    {
        public WoofSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return Path.GetFileNameWithoutExtension(m_sourcePortData.Executable).Equals("woof", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
