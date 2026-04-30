using DoomLauncher.Interfaces;
using System;
using System.Linq;
using System.Text;
using System.IO;

namespace DoomLauncher.Handlers.Sync
{
    public class TextFileSyncAction : ISyncAction
    {
        private readonly Func<string, IdGamesTextInfo> m_parseTextFile;

        public TextFileSyncAction(Func<string, IdGamesTextInfo> parseTextFile)
        {
            m_parseTextFile = parseTextFile;
        }

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            var textInfos = from entry in reader.Entries
                            where isTxtFile(entry.FullName) || entry.FullName.ToLower().Equals("wadinfo")
                            let info = m_parseTextFile(entry.ReadString(Encoding.UTF7))
                            orderby info.QualityScore descending
                            select info;


            var bestInfo = textInfos.Aggregate(IdGamesTextInfo.EMPTY, (a, b) => a.Combine(b));

            if (!string.IsNullOrEmpty(bestInfo.Title))
                gameFile.Title = bestInfo.Title;

            if (!string.IsNullOrEmpty(bestInfo.Author))
                gameFile.Author = bestInfo.Author;

            if (bestInfo.ReleaseDate != null)
                gameFile.ReleaseDate = bestInfo.ReleaseDate;

            if (!string.IsNullOrEmpty(bestInfo.Description))
                gameFile.Description = bestInfo.Description;

            if (!string.IsNullOrEmpty(bestInfo.Game))
                gameFile.IntendedGame = SelectGame(bestInfo.Game);

            if (string.IsNullOrWhiteSpace(gameFile.Title))
                gameFile.Title = GetUserFriendlyFilename(gameFile.FileNameNoPath);

            return SyncResult.EMPTY;
        }

        private string GetUserFriendlyFilename(string filename)
        {
            var words = Path.GetFileNameWithoutExtension(filename).Replace("_", " ").Replace("-", " ").Split();
            var capitalisedWords = words.Select(word => string.Concat(word[0].ToString().ToUpper(), word.Substring(1)));
            return string.Join(" ", capitalisedWords.ToArray());
        }

        private static IWadInfo SelectGame(string gameName)
        {
            string name = gameName.ToUpper();
            if (name == "ULTIMATEDOOM")
                return IWadInfo.Doom;
            else if (gameName.StartsWith("DOOM64"))
                return IWadInfo.Doom64;
            else if (gameName.StartsWith("DOOM2"))
                return IWadInfo.Doom2;
            else
                return IWadInfo.FromGameName(gameName);
        }

        private bool isTxtFile(string filename)
        {
            try
            {
                return Path.GetExtension(filename).Equals(".txt", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false; // Path.GetExtension is a bit of a stickler, but we just need yes or no.
            }
        }
    }
}
