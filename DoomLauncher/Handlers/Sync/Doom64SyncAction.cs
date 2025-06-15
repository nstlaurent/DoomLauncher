using DoomLauncher.Interfaces;
using System.Text.RegularExpressions;

namespace DoomLauncher.Handlers.Sync {
    public class Doom64SyncAction : ISyncAction
    {
        private static readonly Regex ClassTypeRegex = new Regex(@"classtype\s*="); // Only Doom64 files have classtype in the MAPINFO

        private readonly IDataSourceAdapter m_database;

        public Doom64SyncAction(IDataSourceAdapter database)
        {
            m_database = database;
        }

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            var isDoom64 = IsDoom64Wad(mapInfoData);
            file.IsDoom64 = isDoom64;

            if (isDoom64)
            {
                var doom64Iwad = m_database.GetGameFile("doom64.zip");
                if (doom64Iwad != null)
                {
                    file.IWadID = doom64Iwad.IWadID;
                }
            }

            return SyncResult.EMPTY;
        }

        private bool IsDoom64Wad(string[] mapInfoData)
        {
            foreach (string data in mapInfoData)
            {
                MatchCollection matches = ClassTypeRegex.Matches(data);
                if (matches.Count > 0)
                    return true;
            }
            return false;
        }
    }
}
