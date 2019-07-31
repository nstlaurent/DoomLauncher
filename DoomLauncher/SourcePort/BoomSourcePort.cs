using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class BoomSourcePort : GenericSourcePort
    {
        public BoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            string exe = m_sourcePortData.Executable.ToLower();
            return exe.Contains("prboom") || exe.Contains("glboom");
        }

        public override bool StatisticsSupported()
        {
            return true;
        }

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats)
        {
            return new BoomStatsReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "levelstat.txt"));
        }
    }
}
