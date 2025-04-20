using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.Handlers.Sync
{
    public class IdGamesTextInfo
    {
        public static readonly IdGamesTextInfo EMPTY = new IdGamesTextInfo(null, null, null, null, null);

        public IdGamesTextInfo(string title, string author, DateTime? releaseDate, string description, string game)
        {
            Title = title ?? "";
            Author = author ?? "";
            ReleaseDate = releaseDate;
            Description = description ?? "";
            Game = game ?? "";

        }

        public int QualityScore 
        {
            get
            {
                var fields = new List<object>() { Title, Author, ReleaseDate, Description, Game };
                return fields.Where(x => x != null && !(x is string s && string.IsNullOrWhiteSpace(s))).Count();
            }
        }

        // Preserve the first non null element
        public IdGamesTextInfo Combine(IdGamesTextInfo other) => 
            new IdGamesTextInfo(
                !string.IsNullOrWhiteSpace(Title) ? Title : other.Title,
                !string.IsNullOrWhiteSpace(Author) ? Author : other.Author, 
                ReleaseDate ?? other.ReleaseDate,
                !string.IsNullOrWhiteSpace(Description) ? Description : other.Description,
                !string.IsNullOrWhiteSpace(Game) ? Game : other.Game);

        public string Title { get; }
        public string Author { get; }
        public DateTime? ReleaseDate { get; }
        public string Description { get; }

        public string Game { get; }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is IdGamesTextInfo info
                && (Title, Author, ReleaseDate, Description, Game).Equals(
                    (info.Title, info.Author, info.ReleaseDate, info.Description, info.Game));
        }

        public override int GetHashCode() => 
            (Title, Author, ReleaseDate, Description, Game).GetHashCode();
    }

}
