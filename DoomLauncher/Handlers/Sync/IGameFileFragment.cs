using DoomLauncher.Interfaces;

namespace DoomLauncher.Handlers.Sync
{
    public interface IGameFileFragment
    {
        SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData);
    }
}
