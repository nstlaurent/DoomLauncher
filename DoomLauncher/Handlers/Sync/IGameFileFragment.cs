using DoomLauncher.Interfaces;

namespace DoomLauncher.Handlers.Sync
{
    public interface IGameFileFragment
    {
        SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData);
    }

    class EmptyGameFileFragment : IGameFileFragment
    {
        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            return SyncResult.EMPTY;
        }
    }

    public static class GameFileFragmentExtensions
    {
        public static IGameFileFragment OnlyIf(this IGameFileFragment action, bool condition) =>
            condition ? action : new EmptyGameFileFragment();
    }
}
