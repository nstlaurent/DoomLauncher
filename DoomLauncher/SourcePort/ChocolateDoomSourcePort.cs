using DoomLauncher.Interfaces;
using DoomLauncher.Statistics;
using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class ChocolateDoomSourcePort : GenericSourcePort
    {
        public ChocolateDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return Path.GetFileNameWithoutExtension(m_sourcePortData.Executable).Equals("chocolate-doom", StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool StatisticsSupported()
        {
            return true;
        }

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats)
        {
            return new ChocolateDoomStatsReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "stats.txt"));
        }
    }
}
