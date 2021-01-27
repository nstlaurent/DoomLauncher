using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    public class EternitySourcePort : GenericSourcePort
    {
        public EternitySourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() => CheckFileNameWithoutExtension("eternity");
        public override bool LoadSaveGameSupported() => true;
    }
}
