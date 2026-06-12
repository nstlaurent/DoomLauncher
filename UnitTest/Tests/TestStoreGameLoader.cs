using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.GameStores;
using System.Linq;
using static DoomLauncher.GameStores.StoreGameLoader;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestStoreGameLoader
    {
        
        [TestMethod]
        public void LoadFromPath_FindsExpectedWads()
        {
            // Assuming fixtures:

            // SteamGame.ULTIMATE_DOOM:
            // TestSteamLibrary1 / steamapps / common / TestDoom

            var gameStoreFiles = StoreGameLoader.LoadStoreGame(StoreGame.ULTIMATE_DOOM, @"Resources\TestSteamLibrary1\steamapps\common\TestDoom");

            var iwads = gameStoreFiles.InstalledIWads;
            Assert.AreEqual(4, iwads.Count);
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom2.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("plutonia.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("tnt.wad")));

            var pwads = gameStoreFiles.InstalledPWads;
            Assert.AreEqual(4, pwads.Count);
            Assert.IsTrue(pwads.Exists(x => x.Contains("nerve.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("id1.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("masterlevels.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("sigil.wad")));
        }

        [TestMethod]
        public void LoadAllStoreGames_FindsWadsInExpectedPriorityOrder()
        {
            // Assuming fixtures:

            // SteamGame.HERETIC_PLUS_HEXEN:
            // TestSteamLibrary1 / steamapps / common / TestHereticHexen

            // SteamGame.HEXEN:
            // TestSteamLibrary1 / steamapps / common / Hexen

            var gameStoreFiles = StoreGameLoader.LoadAllStoreGames(new List<GameFinder>() { FindHexenGame });

            var iwads = gameStoreFiles.InstalledIWads;

            // Heretic + Hexen is higher priority, and should get picked up instead of Hexen
            Assert.IsTrue(iwads.Exists(x => x.Contains("Resources\\TestSteamLibrary1\\steamapps\\common\\TestHereticHexen\\hexen.wad")));
            Assert.IsFalse(iwads.Exists(x => x.Contains("Resources\\TestSteamLibrary1\\steamapps\\common\\TestHexen\\hexen.wad")));
        }

        [TestMethod]
        public void LoadFromPath_ReturnsEmptyFilesIfDirectoryNotFound()
        {
            var gameStoreFiles = StoreGameLoader.LoadStoreGame(StoreGame.ULTIMATE_DOOM, @"Resources\DoesNotExist");
            Assert.AreEqual(gameStoreFiles, GameStoreFiles.EMPTY);
        }

        private static string FindHexenGame(StoreGame game)
        {
            if (game.Equals(StoreGame.HERETIC_PLUS_HEXEN))
                return $@"Resources\TestSteamLibrary1\steamapps\common\TestHereticHexen";
            else if (game.Equals(StoreGame.HEXEN))
                return $@"Resources\TestSteamLibrary1\steamapps\common\TestHexen";
            else return null;

        }
    }
}
