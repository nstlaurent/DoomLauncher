using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class HelionSourcePort : GenericSourcePort
    {
        public HelionSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            return CheckFileNameWithoutExtension("Helion");
        }

        public override bool StatisticsSupported()
        {
            return true;
        }

        public override bool LoadSaveGameSupported()
        {
            return true;
        }

        public override string LoadSaveParameter(SpData data)
        {
            return $"-loadgame \"{data.Value}\"";
        }

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats)
        {
            return new BoomStatsReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "levelstat.txt"));
        }
    }
}
