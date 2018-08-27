using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Demo
{
    public static class DemoUtil
    {
        public static IDemoParser GetDemoParser(string demoFile)
        {
            IDemoParser[] parsers = new IDemoParser[]
            {
                new CldDemoParser(demoFile)
            };

            return parsers.FirstOrDefault(x => x.CanParse());
        }
    }
}
