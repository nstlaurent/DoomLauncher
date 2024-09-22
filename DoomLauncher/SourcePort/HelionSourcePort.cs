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

        public override string WarpParameter(SpData data) => GetMapParameter(data.Value);

        public override string LoadSaveParameter(SpData data) =>
            $"-loadgame \"{data.Value}\"";

        public override bool Supported() => CheckFileNameWithoutExtension("Helion");

        public override bool StatisticsSupported() => true;

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats) =>
            new LevelstatReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "levelstat.txt"));
    }
}
