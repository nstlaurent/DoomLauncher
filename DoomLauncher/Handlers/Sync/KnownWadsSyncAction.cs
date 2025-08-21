using DoomLauncher.Interfaces;
using System;
using System.Linq;

namespace DoomLauncher.Handlers.Sync
{
    public class KnownWadsSyncAction : ISyncAction
    {
        private readonly IDataSourceAdapter m_database;

        public KnownWadsSyncAction(IDataSourceAdapter database)
        {
            m_database = database;
        }

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            var baseName = file.FileNameBase.ToLower();
            
            switch (baseName)
            {
                case "hexdd":
                    file.Title = "Hexen: Deathkings of the Dark Citadel";
                    file.IWadID = m_database.GetIWads().FirstOrDefault(iwad => iwad.FileNameBase.Equals("hexen", StringComparison.OrdinalIgnoreCase))?.IWadID;
                    break;
            };
            return SyncResult.EMPTY;
        }
    }
}
