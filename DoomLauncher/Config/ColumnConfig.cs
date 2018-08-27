using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{

    public class ColumnConfig
    {
        public ColumnConfig() { }
        public ColumnConfig(string parent, string column, int width, SortOrder sort)
        {
            Parent = parent;
            Column = column;
            Width = width;
            Sort = sort;
        }

        public string Parent { get; set; }
        public string Column { get; set; }
        public int Width { get; set; }
        public SortOrder Sort { get; set; }
    }
}
