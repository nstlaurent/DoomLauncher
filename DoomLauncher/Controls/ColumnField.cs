using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class ColumnField
    {
        public ColumnField(string datakey, string title)
        {
            DataKey = datakey;
            Title = title;
        }

        public string DataKey { get; set; }
        public string Title { get; set; }
    }
}
