using System.Collections.Generic;

namespace DoomLauncher.GameStores
{
    public class StoreGame
    {
        public readonly static StoreGame ULTIMATE_DOOM = new StoreGame(2280, 1317223010, "DOOM + DOOM II", 
            new List<string> { @"rerelease\doom.wad", @"rerelease\doom2.wad", @"rerelease\plutonia.wad", @"rerelease\tnt.wad", 
                               @"base\doom.wad", @"base\doom2\doom2.wad", @"base\plutonia\plutonia.wad", @"base\tnt\tnt.wad" }, 
            new List<string> { @"rerelease\id1.wad", @"rerelease\nerve.wad", @"rerelease\masterlevels.wad", @"rerelease\sigil.wad", 
                               @"rerelease\sigil2.wad" },
            null);

        public readonly static StoreGame HERETIC_PLUS_HEXEN = new StoreGame(3286930, 1572667751, "Heretic + Hexen",
            new List<string> { @"base\heretic\heretic.wad", @"base\hexen\hexen.wad" },
            new List<string> { @"base\hexendk\hexdd.wad" }, 
            null);

        public readonly static StoreGame DOOM2 = new StoreGame(2300, null, "DOOM II", 
            new List<string> { "base\\doom2.wad" }, 
            new List<string>(), 
            null);

        public readonly static StoreGame FINAL_DOOM = new StoreGame(2290, null, "Final DOOM", 
            new List<string> { "base\\plutonia.wad", "base\\tnt.wad" }, 
            new List<string>(),
            null);

        public readonly static StoreGame HERETIC = new StoreGame(2390, 1290366318, "Heretic: Shadow of the Serpent Riders", 
            new List<string> { "base\\heretic.wad" }, 
            new List<string>(),
            null);

        public readonly static StoreGame HEXEN = new StoreGame(2360, 1247951670, "Hexen: Beyond Heretic",
            new List<string> { "base\\hexen.wad" },
            new List<string>(), 
            null);

        public readonly static StoreGame STRIFE = new StoreGame(317040, 1432899949, "Strife: Veteran Edition",
            new List<string> { "strife1.wad" },
            new List<string>(), 
            null);

        public readonly static StoreGame DOOM64 = new StoreGame(1148590, 1456611261, "Doom 64",
            new List<string> { "doom64.wad" },
            new List<string>(), 
            "doom64_x64.exe");

        public static readonly List<StoreGame> GAMES_IN_PRIORITY_ORDER = new List<StoreGame>() 
        { 
            ULTIMATE_DOOM, DOOM2, FINAL_DOOM, HERETIC_PLUS_HEXEN, HERETIC, HEXEN, STRIFE, DOOM64
        };

        public int SteamId { get; }

        public int? GogId { get; }

        public string Name { get; }

        public List<string> ExpectedIWadFiles { get; }

        public List<string> ExpectedPWadFiles { get; }

        public string ExpectedDoom64Exe { get; }

        private StoreGame(int steamId, int? gogId, string name, List<string> expectedIWadFiles, List<string> expectedPwadFiles, string expectedDoom64Exe)
        {
            SteamId = steamId;
            GogId = gogId;
            Name = name;
            ExpectedIWadFiles = expectedIWadFiles;
            ExpectedPWadFiles = expectedPwadFiles;
            ExpectedDoom64Exe = expectedDoom64Exe;
        }

        public override bool Equals(object obj)
        {
            if (obj is StoreGame)
            {
                StoreGame other = (StoreGame)obj;
                return other.SteamId == SteamId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return SteamId;
        }
    }
}
