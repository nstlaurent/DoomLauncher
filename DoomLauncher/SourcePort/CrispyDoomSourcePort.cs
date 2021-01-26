using DoomLauncher.Interfaces;

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
            return CheckFileNameWithoutExtension("crispy-doom");
        }
    }
}
