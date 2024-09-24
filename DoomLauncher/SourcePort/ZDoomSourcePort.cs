using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher.SourcePort
{
    public class ZDoomSourcePort : GenericSourcePort
    {
        private static readonly string[] DirectoryNames = new string[] { "GZDoom", "VKDoom" };

        private static string UserDirectoryBase => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static string[] UserSaveGameDirectories => DirectoryNames.Select(x => UserSaveGameDirectory(x)).ToArray();
        public static string[] UserScreenshotDirectories => DirectoryNames.Select(x => UserScreenshotDirectory(x)).ToArray();

        public static string UserSaveGameDirectory(string name) => Path.Combine(UserDirectoryBase, "Saved Games", name);
        public static string UserScreenshotDirectory(string name) => Path.Combine(UserDirectoryBase, "Pictures", "Screenshots", name);

        public ZDoomSourcePort(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override bool Supported() =>
            CheckFileNameContains("zdoom.exe") ||
            CheckFileNameWithoutExtension("zandronum") ||
            CheckFileNameWithoutExtension("vkdoom");

        public override string LoadSaveParameter(SpData data) =>
            $"-loadgame \"{data.Value}\"";

        public override bool StatisticsSupported() => true;

        public override string[] GetScreenshotDirectories() => UserScreenshotDirectories;

        public override string[] GetSaveGameDirectories() => UserSaveGameDirectories;

        public override IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats) =>
            new ZDoomStatsReader(gameFile, m_sourcePortData.Directory.GetFullPath(), existingStats);

        public override string WarpParameter(SpData data) => GetMapParameter(data.Value);
    }
}
