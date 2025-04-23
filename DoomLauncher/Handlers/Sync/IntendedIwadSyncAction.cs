using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System.Linq;

namespace DoomLauncher.Handlers.Sync
{
    public class IntendedIwadSyncAction : ISyncAction
    {
        private readonly IIWadDataSourceAdapter m_database;

        public IntendedIwadSyncAction(IIWadDataSourceAdapter database)
        {
            m_database = database;
        }

        public void ApplyIntendedGame(IGameFile gameFile)
        {
            if (gameFile.IntendedGame != null)
            {
                var iwadsFromDb = m_database.GetIWads();
                var matchingIWad = iwadsFromDb.FirstOrDefault(iwad => iwad.FileName == gameFile.IntendedGame?.FileName);

                if (matchingIWad == null && gameFile.IntendedGame?.BackupGame != null)
                {
                    // If the intended game is not found, try the backup game
                    matchingIWad = iwadsFromDb.FirstOrDefault(iwad => iwad.FileName == gameFile.IntendedGame.BackupGame.FileName);
                }
                if (matchingIWad != null)
                {
                    gameFile.IWadID = matchingIWad.IWadID;
                }
            }
        }

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            ApplyIntendedGame(gameFile);
            return SyncResult.EMPTY;
        }
    }
}
