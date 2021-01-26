using DoomLauncher.Interfaces;
using System.Collections.Generic;

namespace DoomLauncher.SourcePort
{
    class ZDoomSourcePort : GenericSourcePort
    {
        public ZDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            string exe = m_sourcePortData.Executable.ToLower();

            if (exe.Contains("zdoom.exe"))
                return true;
            if (exe.Contains("zandronum.exe"))
                return true;

            return false;
        }

        public override string LoadSaveParameter(SpData data)
        {
            return $"-loadgame {data.Value}";
        }

        public override bool StatisticsSupported()
        {
            return true;
        }

        public override bool LoadSaveGameSupported()
        {
            return true;
        }

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats)
        {
            return new ZDoomStatsReader(gameFile, m_sourcePortData.Directory.GetFullPath(), existingStats);
        }
    }
}
