using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
