using DoomLauncher.Interfaces;
using DoomLauncher.Statistics;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class StatdumpSourcePort : GenericSourcePort
    {
        public StatdumpSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {
        
        }

        public override bool Supported() =>
            CheckFileNameWithoutExtension("chocolate-doom") ||
            CheckFileNameWithoutExtension("cndoom") ||
            CheckFileNameWithoutExtension("crl-doom") ||
            CheckFileNameWithoutExtension("inter-doom");

        public override bool StatisticsSupported() => true;

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats) =>
            new StatdumpReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "stats.txt"));
    }
}
