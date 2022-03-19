using System;

namespace DoomLauncher.Adapters
{
    public class PlaySession
    {
        public PlaySession(GameFilePlayAdapter adapter, IStatisticsReader statisticsReader, DateTime start)
        {
            Adapter = adapter;
            StatisticsReader = statisticsReader;
            Start = start;
        }

        public GameFilePlayAdapter Adapter { get; }
        public IStatisticsReader StatisticsReader { get; }
        public DateTime Start { get; set; }
    }
}
