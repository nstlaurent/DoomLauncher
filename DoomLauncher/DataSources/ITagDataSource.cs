using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface ITagDataSource
    {
        int TagID { get; set; }
        string Name { get; set; }
        bool HasTab { get; set; }
        bool HasColor { get; set; }
        int? Color { get; set; }
    }
}
