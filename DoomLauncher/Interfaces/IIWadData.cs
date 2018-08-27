using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface IIWadData
    {
        int IWadID { get; set; }
        string Name { get; set; }
        string FileName { get; set; }
        int? GameFileID { get; set; }
    }
}
