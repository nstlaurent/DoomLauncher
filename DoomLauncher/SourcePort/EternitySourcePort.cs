using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    public class EternitySourcePort : ChocolateDoomSourcePort
    {
        public EternitySourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return CheckFileNameWithoutExtension("eternity");
        }
    }
}
