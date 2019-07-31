using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class CNDoomSourcePort : GenericSourcePort
    {
        public CNDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return Path.GetFileNameWithoutExtension(m_sourcePortData.Executable).Equals("CNDOOM", StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool StatisticsSupported()
        {
            return true;
        }

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats)
        {
            return new CNDoomStatsReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "stdout.txt"));
        }
    }
}
