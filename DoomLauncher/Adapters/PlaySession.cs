using System;
using static DoomLauncher.GameLauncher;

namespace DoomLauncher.Adapters
{
    public class PlaySession
    {
        public PlaySession(GameLaunchInfo launchInfo, IStatisticsReader statisticsReader, DateTime start)
        {
            GameLaunchInfo = launchInfo;
            StatisticsReader = statisticsReader;
            Start = start;
        }

        public GameLaunchInfo GameLaunchInfo { get; }
        public IStatisticsReader StatisticsReader { get; }
        public DateTime Start { get; set; }
    }
}
