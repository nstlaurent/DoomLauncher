using System.Collections.Generic;

namespace DoomLauncher.Steam
{
    public class SteamGame
    {
        public readonly static SteamGame ULTIMATE_DOOM = new SteamGame(2280, "DOOM + DOOM II", 
            new List<string> { @"rerelease\doom.wad", @"rerelease\doom2.wad", @"rerelease\plutonia.wad", @"rerelease\tnt.wad", 
                               @"base\doom.wad", @"base\doom2\doom2.wad", @"base\plutonia\plutonia.wad", @"base\tnt\tnt.wad" }, 
            new List<string> { @"rerelease\id1.wad", @"rerelease\nerve.wad", @"rerelease\masterlevels.wad", @"rerelease\sigil.wad" },
            null);

        public readonly static SteamGame DOOM2 = new SteamGame(2300, "DOOM II", 
            new List<string> { "base\\doom2.wad" }, 
            new List<string>(), 
            null);

        public readonly static SteamGame FINAL_DOOM = new SteamGame(2290, "Final DOOM", 
            new List<string> { "base\\plutonia.wad", "base\\tnt.wad" }, 
            new List<string>(),
            null);

        public readonly static SteamGame HERETIC = new SteamGame(2390, "Heretic: Shadow of the Serpent Riders", 
            new List<string> { "base\\heretic.wad" }, 
            new List<string>(),
            null);

        public readonly static SteamGame HEXEN = new SteamGame(2360, "Hexen: Beyond Heretic",
            new List<string> { "base\\hexen.wad" },
            new List<string>(), 
            null);

        public readonly static SteamGame STRIFE = new SteamGame(317040, "Strife: Veteran Edition",
            new List<string> { "strife1.wad" },
            new List<string>(), 
            null);

        public readonly static SteamGame DOOM64 = new SteamGame(1148590, "Doom 64",
            new List<string> { "doom64.wad" },
            new List<string>(), 
            "doom64_x64.exe");

        public static readonly List<SteamGame> GAMES_IN_PRIORITY_ORDER = new List<SteamGame>() 
        { 
            ULTIMATE_DOOM, DOOM2, FINAL_DOOM, HERETIC, HEXEN, STRIFE, DOOM64
        };

        public int GameId { get; }

        public string Name { get; }

        public List<string> ExpectedIWadFiles { get; }

        public List<string> ExpectedPWadFiles { get; }

        public string ExpectedDoom64Exe { get; }

        private SteamGame(int gameId, string name, List<string> expectedIWadFiles, List<string> expectedPwadFiles, string expectedDoom64Exe)
        {
            GameId = gameId;
            Name = name;
            ExpectedIWadFiles = expectedIWadFiles;
            ExpectedPWadFiles = expectedPwadFiles;
            ExpectedDoom64Exe = expectedDoom64Exe;
        }

        public override bool Equals(object obj)
        {
            if (obj is SteamGame)
            {
                SteamGame other = (SteamGame)obj;
                return other.GameId == GameId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GameId;
        }
    }
}
