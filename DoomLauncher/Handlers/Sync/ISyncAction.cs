using DoomLauncher.Interfaces;

namespace DoomLauncher.Handlers.Sync
{
    public interface ISyncAction
    {
        SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData);
    }

    class EmptySyncAction : ISyncAction
    {
        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            return SyncResult.EMPTY;
        }
    }

    public static class SyncActionExtensions
    {
        public static ISyncAction OnlyIf(this ISyncAction action, bool condition) =>
            condition ? action : new EmptySyncAction();
    }
}
