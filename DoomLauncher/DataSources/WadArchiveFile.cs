using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    class WadArchiveFile : FileData
    {
        public override bool IsUrl { get { return true; } }
    }
}
