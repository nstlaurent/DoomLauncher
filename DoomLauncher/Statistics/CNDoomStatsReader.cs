using DoomLauncher.Interfaces;
using DoomLauncher.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DoomLauncher
{
    public class CNDoomStatsReader : MultiLineStatReader
    {
        private static string s_statRegex = @"#+\w+#+Time:\d+:\d+.\d+Kills:\d+/\d+#+Items:\d+/\d+Secrets:\d+/\d+";
        private static ParseItem[] s_regexItems = new ParseItem[]
        {
            new ParseItem(@"\w+", string.Empty, "MapName"),
            new ParseItem(@"\d+:\d+.\d+", string.Empty, "LevelTime"),
            new ParseItem(@":\d+", ":", "KillCount"),
            new ParseItem(@"/\d+", "/", "TotalKills"),
            new ParseItem(@":\d+", ":", "ItemCount"),
            new ParseItem(@"/\d+", "/", "TotalItems"),
            new ParseItem(@":\d+", ":", "SecretCount"),
            new ParseItem(@"/\d+", "/", "TotalSecrets")
        };

        public CNDoomStatsReader(IGameFile gameFile, string stdoutFile) : base(gameFile, stdoutFile, s_statRegex, s_regexItems)
        {
            
        }

        public override string LaunchParameter { get { return "-printstats"; } }
    }
}
