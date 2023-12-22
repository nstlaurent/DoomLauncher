using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    public class NuggetDoomSourcePort : BoomSourcePort
    {
        public NuggetDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() => CheckFileNameWithoutExtension("nugget-doom");        
        public override bool LoadSaveGameSupported() => true;
    }
}
