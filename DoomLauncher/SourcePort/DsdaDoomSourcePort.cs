using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    public class DsdaDoomSourcePort : BoomSourcePort
    {
        public DsdaDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() => CheckFileNameWithoutExtension("dsda-doom");
    }
}
