using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    class ZDoomSourcePort : GenericSourcePort
    {
        private static string UserDirectoryBase => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string UserSaveGameDirectory => Path.Combine(UserDirectoryBase, "Saved Games", "GZDoom");
        public static string UserScreenshotDirectory => Path.Combine(UserDirectoryBase, "Pictures", "Screenshots", "GZDoom");

        public ZDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported()
        {
            if (CheckFileNameContains("zdoom.exe"))
                return true;

            return CheckFileNameWithoutExtension("zandronum");
        }

        public override string LoadSaveParameter(SpData data) =>
            $"-loadgame \"{data.Value}\"";

        public override bool StatisticsSupported() => true;

        public override bool LoadSaveGameSupported() => true;

        public override string GetScreenshotDirectory() => UserScreenshotDirectory;

        public override string GetSaveGameDirectory() => UserSaveGameDirectory;

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats) =>
            new ZDoomStatsReader(gameFile, m_sourcePortData.Directory.GetFullPath(), existingStats);

        public override string WarpParameter(SpData data) => GetMapParameter(data.Value);
    }
}
