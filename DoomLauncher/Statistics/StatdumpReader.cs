using DoomLauncher.Interfaces;

namespace DoomLauncher.Statistics
{
    public class StatdumpReader : MultiLineStatReader
    {
        private static string s_statRegex = @"=+\w+(/\w+)?=+Time:\d+:\d+\(par:\d+:\d+\)\w+\(\w+\):Kills:\d+(/\d+\(-?\d+%\))?Items:\d+(/\d+\(-?\d+%\))?Secrets:\d+(/\d+)?";
        private static ParseItem[] s_regexItems = new ParseItem[]
        {
            new ParseItem(@"\w+(/\w+)?", string.Empty, "MapName"),
            new ParseItem(@"\d+:\d+", string.Empty, "LevelTime"),
            new ParseItem(@"\d+:\d+", string.Empty, null), //remove par
            new ParseItem(@"\w+\(\w+\):", string.Empty, null), //remove player name
            new ParseItem(@":\d+", ":", "KillCount"),
            new ParseItem(@"Kills/\d+", "Kills/", "TotalKills"),
            new ParseItem(@":\d+", ":", "ItemCount"),
            new ParseItem(@"Items/\d+", "Items/", "TotalItems"),
            new ParseItem(@":\d+", ":", "SecretCount"),
            new ParseItem(@"Secrets/\d+", "Secrets/", "TotalSecrets")
        };

        public StatdumpReader(IGameFile gameFile, string statFile) : base(gameFile, statFile, s_statRegex, s_regexItems)
        {
            ErrorOnParseItem = false;
        }

        public override string LaunchParameter { get { return $"-statdump \"{StatFile}\""; } }
    }
}
