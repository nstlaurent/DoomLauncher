using System;

namespace DoomLauncher.Interfaces
{
    public interface ITextFileParser
    {
        string Title { get; set; }
        string Author { get; set; }
        DateTime? ReleaseDate { get; set; }
        string Description { get; set; }
    }
}
