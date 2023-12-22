using DoomLauncher.Interfaces;

namespace DoomLauncher.SourcePort
{
    class FdwlSourcePort : BoomSourcePort
    {
        public FdwlSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() => CheckFileNameWithoutExtension("fdwl");
        public override bool LoadSaveGameSupported() => true;
    }
}
