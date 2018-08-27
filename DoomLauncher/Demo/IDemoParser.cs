using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Demo
{
    public interface IDemoParser
    {
        bool CanParse();
        string[] GetRequiredFiles();    
    }
}
