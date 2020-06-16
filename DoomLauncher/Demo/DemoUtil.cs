using System.Linq;

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
