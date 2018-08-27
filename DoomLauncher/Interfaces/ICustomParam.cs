using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public interface ICustomParam
    {
        int CustomParamID { get; set; }
        int GameFileID { get; set; }
        string FileName { get; set; }
        string Parameter { get; set; }
    }
}
