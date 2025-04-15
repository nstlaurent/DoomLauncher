using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DoomLauncher.Handlers.Sync
{
    public class IdGamesTextFileParser
    {
        private static readonly string s_fullRegex = @"\s*{0}\s*:[^\r\n]*";
        private static readonly string s_fullRegexDescription = @"\s*{0}\s*:[^=]*";
        private static readonly string s_regex = @"\s*{0}\s*:\s*";
        private readonly string[] m_dateParseFormats;

        public IdGamesTextFileParser(string[] dateParseFormats)
        {
            m_dateParseFormats = dateParseFormats.ToArray();
        }

        public IdGamesTextInfo Parse(string text)
        {
            var title = FindValue(text, "Title", s_fullRegex, false);
            var author = FindValue(text, "Authors*", s_fullRegex, false);
            var releaseDate = ParseReleaseDate(text);
            var description = FindValue(text, "Description", s_fullRegexDescription, false).Replace("\r\n", "\n");
            var game = FindValue(text, "Game", s_fullRegex, false);

            return new IdGamesTextInfo(title, author, releaseDate, description, game);
        }

        private DateTime? ParseReleaseDate(string text)
        {
            string[] dateItems = new string[] { "Date Finished", "Release date" };

            string date = string.Empty;

            foreach (string item in dateItems)
            {
                date = FindValue(text, item, s_fullRegex, true).Replace("th", string.Empty).Trim();
                if (!string.IsNullOrEmpty(date)) break;
            }

            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt;
                date = date.Replace(".", "/");
                if (ParseDate1(date, m_dateParseFormats, out dt))
                    return dt;
                else if (ParseDate2(date, m_dateParseFormats, out dt))
                    return dt;
                else if (ParseDate3(date, m_dateParseFormats, out dt))
                    return dt;
            }
            return null;
        }

        private bool ParseDate1(string date, string[] dateParseFormats, out DateTime dt)
        {
            if (DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return true;
            }   
            else if (DateTime.TryParseExact(date, dateParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return true;

            return false;
        }

        private bool ParseDate2(string date, string[] dateParseFormats, out DateTime dt)
        {
            if (date.Length > 0 && !char.IsNumber(date.Last())) date = date.Substring(0, date.Length - 1);
            if (date.Length > 0 && !char.IsNumber(date.First())) date = date.Substring(1);

            return ParseDate1(date, dateParseFormats, out dt);
        }

        private bool ParseDate3(string date, string[] dateParseFormats, out DateTime dt)
        {
            while (date.Length > 0 && !char.IsNumber(date.Last())) date = date.Substring(0, date.Length - 1);
            while (date.Length > 0 && !char.IsNumber(date.First())) date = date.Substring(1);
            return ParseDate1(date, dateParseFormats, out dt);
        }

        private string FindValue(string text, string category, string regexFull, bool ignoreCase)
        {
            Match m = new Regex(string.Format(regexFull, category), RegexOptions.IgnoreCase).Match(text);

            if (m.Success)
                return m.Value.Substring((new Regex(string.Format(s_regex, category), RegexOptions.IgnoreCase).Match(m.Value).Value.Length));

            return string.Empty;
        }
    }
}
