using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.TextFileParsers
{
    public class IdGamesTextInfo
    {
        public static readonly IdGamesTextInfo EMPTY = new IdGamesTextInfo(null, null, null, null);

        public IdGamesTextInfo(string title, string author, DateTime? releaseDate, string description)
        {
            Title = title ?? "";
            Author = author ?? "";
            ReleaseDate = releaseDate;
            Description = description ?? "";
        }

        public int QualityScore 
        {
            get
            {
                var fields = new List<object>() { Title, Author, ReleaseDate, Description };
                return fields.Where(x => x != null && !(x is string s && string.IsNullOrWhiteSpace(s))).Count();
            }
        }

        // Preserve the first non null element
        public IdGamesTextInfo Combine(IdGamesTextInfo other) => 
            new IdGamesTextInfo(
                !string.IsNullOrWhiteSpace(Title) ? Title : other.Title,
                !string.IsNullOrWhiteSpace(Author) ? Author : other.Author, 
                ReleaseDate ?? other.ReleaseDate,
                !string.IsNullOrWhiteSpace(Description) ? Description : other.Description);

        public string Title { get; }
        public string Author { get; }
        public DateTime? ReleaseDate { get; }
        public string Description { get; }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is IdGamesTextInfo info
                && (Title, Author, ReleaseDate, Description).Equals((info.Title, info.Author, info.ReleaseDate, info.Description));
        }

        public override int GetHashCode() => 
            (Title, Author, ReleaseDate, Description).GetHashCode();
    }

}
