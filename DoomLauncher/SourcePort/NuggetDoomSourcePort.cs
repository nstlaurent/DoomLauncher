using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    public class NuggetDoomSourcePort : BoomSourcePort
    {
        public NuggetDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return CheckFileNameWithoutExtension("nugget-doom");
        }
    }
}
