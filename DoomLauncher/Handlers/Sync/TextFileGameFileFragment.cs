using DoomLauncher.Interfaces;
using System;
using System.Linq;
using System.Text;
using System.IO;

namespace DoomLauncher.Handlers.Sync
{
    class TextFileGameFileFragment : IGameFileFragment
    {
        private readonly string[] m_dateParseFormats;

        public TextFileGameFileFragment(string[] dateParseFormats) 
        {
            m_dateParseFormats = dateParseFormats;
        }

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            FillTextFileInfo(file, reader);
            return SyncResult.EMPTY;
        }

        private void FillTextFileInfo(IGameFile gameFile, IArchiveReader reader)
        {
            var textInfos = from entry in reader.Entries
                            where isTxtFile(entry.FullName)
                            let info = ParseIdGamesTextInfo(entry)
                            orderby info.QualityScore descending
                            select info;

            var bestInfo = textInfos.Aggregate(IdGamesTextInfo.EMPTY, (a, b) => a.Combine(b));

            gameFile.Title = bestInfo.Title;
            gameFile.Author = bestInfo.Author;
            gameFile.ReleaseDate = bestInfo.ReleaseDate;
            gameFile.Description = bestInfo.Description;

            if (string.IsNullOrWhiteSpace(gameFile.Title))
                gameFile.Title = GetUserFriendlyFilename(gameFile.FileNameNoPath);
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

        private IdGamesTextInfo ParseIdGamesTextInfo(IArchiveEntry entry)
        {
            string buffer = Encoding.UTF7.GetString(ReadBuffer(entry));
            return new IdGamesTextFileParser(m_dateParseFormats).Parse(buffer);
        }

        private string GetUserFriendlyFilename(string filename)
        {
            var words = Path.GetFileNameWithoutExtension(filename).Replace("_", " ").Replace("-", " ").Split();
            var capitalisedWords = words.Select(word => string.Concat(word[0].ToString().ToUpper(), word.Substring(1)));
            return string.Join(" ", capitalisedWords.ToArray());
        }

        private byte[] ReadBuffer(IArchiveEntry entry)
        {
            byte[] buffer = new byte[entry.Length];
            try
            {
                entry.Read(buffer, 0, Convert.ToInt32(entry.Length));
            }
            catch (Exception)
            {
                // Do not fail because we couldn't read a text file
            }
            return buffer;
        }

    }
}
