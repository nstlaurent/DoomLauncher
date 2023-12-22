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

        public override bool Supported() =>
            CheckFileNameContains("prboom") || CheckFileNameContains("glboom");


        public override bool StatisticsSupported() => true;

        public override bool LoadSaveGameSupported() => false;

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats) =>
            new BoomStatsReader(gameFile, Path.Combine(m_sourcePortData.Directory.GetFullPath(), "levelstat.txt"));
 
    }
}
