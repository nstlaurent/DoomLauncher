using DoomLauncher.Interfaces;
using DoomLauncher.Statistics;
namespace DoomLauncher
{
    public class BoomStatsReader : MultiLineStatReader
    {
        private static string s_statRegex = @"\S+-\d+:\d+.\d+\(\d+:\d+\)K:\d+/\d+I:\d+/\d+S:\d+/\d+";
        private static ParseItem[] s_regexItems = new[]
        {
            new ParseItem(@"\S+-", "-", "MapName"),
            new ParseItem(@"\S+:\S+\(", "-(", "LevelTime"),
            new ParseItem(@"\d+/", "/", "KillCount"),
            new ParseItem(@"K:\d+", "K:", "TotalKills"),
            new ParseItem(@"\d+/", "/", "ItemCount"),
            new ParseItem(@"I:\d+", "I:", "TotalItems"),
            new ParseItem(@"\d+/", "/", "SecretCount"),
            new ParseItem(@"S:\d+", "S:", "TotalSecrets")
        };

        public BoomStatsReader(IGameFile gameFile, string statFile) : base(gameFile, statFile, s_statRegex, s_regexItems)
        {
            RemoveNewLines = false;
        }

        public override string LaunchParameter { get { return "-levelstat"; } }
    }
}
