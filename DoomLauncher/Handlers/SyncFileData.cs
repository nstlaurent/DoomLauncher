using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    class SyncFileData
    {
        public SyncFileData(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; set; }
        public bool Selected { get; set; }
    }
}
