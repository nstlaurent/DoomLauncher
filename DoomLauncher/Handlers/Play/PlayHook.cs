
using DoomLauncher.Interfaces;

namespace DoomLauncher.Handlers.Play
{
    public interface PlayHook
    {
        void BeforePlay(IGameFile gamefile, ISourcePortData sourcePort);
        void AfterPlay(IGameFile gamefile, ISourcePortData sourcePort);
    }
}
