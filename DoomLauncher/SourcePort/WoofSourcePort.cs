using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    public class WoofSourcePort : BoomSourcePort
    {
        public WoofSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() => CheckFileNameWithoutExtension("woof");
        public override bool LoadSaveGameSupported() => true;
    }
}
