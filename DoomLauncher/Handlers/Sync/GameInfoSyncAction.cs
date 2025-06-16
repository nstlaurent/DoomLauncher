using DoomLauncher.Interfaces;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoomLauncher.Handlers.Sync
{
    /// <summary>
    /// https://zdoom.org/wiki/GAMEINFO
    /// </summary>
    public class GameInfoSyncAction : ISyncAction
    {
        private static readonly Regex STARTUPTITLE_REGEX = new Regex(@"\s*STARTUPTITLE\s*=\s*"".*"""); //@"\s*{0}\s*:[^=]*";
        private static readonly Regex SUBTRACT_REGEX = new Regex(@"\s*STARTUPTITLE\s*=\s*");

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            // Normally it's GAMEINFO, but I've seen GAMEINFO.txt in the wild. 
            var entry = reader.Entries.FirstOrDefault(x => x.Name.ToLower().StartsWith("gameinfo"));
            if (entry != null)
            {
                var text = entry.ReadString(Encoding.UTF7);
                Match m = STARTUPTITLE_REGEX.Match(text);
                if (m.Success)
                {
                    var fullStatement = m.Value;
                    var assignment = SUBTRACT_REGEX.Match(fullStatement);
                    if (assignment.Success)
                    {
                        var title = fullStatement.Substring(assignment.Length + 1, fullStatement.Length - assignment.Length - 2).Trim();
                        if (!string.IsNullOrEmpty(title))
                            gameFile.Title = title;
                    }
                }
            }

            return SyncResult.EMPTY;
        }
    }
}
