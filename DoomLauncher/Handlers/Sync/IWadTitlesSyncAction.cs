using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Handlers.Sync
{
    public class IWadTitlesSyncAction : ISyncAction
    {
        private readonly IIWadDataSourceAdapter m_database;

        public IWadTitlesSyncAction(IIWadDataSourceAdapter database)
        {
            m_database = database;
        }

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            var iwadsFromDb = m_database.GetIWads();
            var firstMatchingIwad = iwadsFromDb.FirstOrDefault(iwad => iwad.FileName == gameFile.IntendedGame.FileName);

            IWadInfo info = IWadInfo.FromFileName(gameFile.FileName);
            // if ()

            return SyncResult.EMPTY;
        }
    }
}
