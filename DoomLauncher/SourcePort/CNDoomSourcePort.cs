using DoomLauncher.Interfaces;
using System;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class CNDoomSourcePort : ChocolateDoomSourcePort
    {
        public CNDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return Path.GetFileNameWithoutExtension(m_sourcePortData.Executable).Equals("cndoom", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
