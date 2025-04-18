using DoomLauncher.Interfaces;
using System.Text.RegularExpressions;

namespace DoomLauncher.Handlers.Sync {
    public class Doom64GameFileFragment : IGameFileFragment
    {
        private static readonly Regex ClassTypeRegex = new Regex(@"classtype\s*="); // Only Doom64 files have classtype in the MAPINFO

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            file.IsDoom64 = IsDoom64Wad(mapInfoData);
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
