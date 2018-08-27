using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class NewStatisticsEventArgs : EventArgs
    {
        public NewStatisticsEventArgs(IStatsData stats)
        {
            Statistics = stats;
            Update = false;
        }

        public NewStatisticsEventArgs(IStatsData stats, bool update)
        {
            Statistics = stats;
            Update = update;
        }

        public IStatsData Statistics { get; set; }
        public bool Update { get; set; }
    }
}
