using DoomLauncher.Interfaces;
using System;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class CrispyDoomSourcePort : ChocolateDoomSourcePort
    {
        public CrispyDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return Path.GetFileNameWithoutExtension(m_sourcePortData.Executable).Equals("crispy-doom", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
