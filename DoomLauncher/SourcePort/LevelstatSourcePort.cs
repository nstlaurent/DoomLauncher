using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public class LevelstatSourcePort : GenericSourcePort
    {
        public LevelstatSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() =>
            CheckFileNameContains("crispy-doom") // + crispy-doom-truecolor
            || CheckFileNameContains("crispy-heretic") // + crispy-heretic-truecolor
            || CheckFileNameWithoutExtension("so-doom")
            || CheckFileNameWithoutExtension("inter-heretic")
            || CheckFileNameContains("boom-plus.exe") // prboom-plus + glboom-plus
            || CheckFileNameWithoutExtension("dsda-doom")
            || CheckFileNameWithoutExtension("fdwl")
            || CheckFileNameWithoutExtension("woof")
            || CheckFileNameWithoutExtension("nugget-doom")
            || CheckFileNameWithoutExtension("cherry-doom");

        public override bool StatisticsSupported() => true;

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats) =>
            new LevelstatReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "levelstat.txt"));
    }
}
