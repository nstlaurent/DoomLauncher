using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.GameStores;
using System.Linq;
using static DoomLauncher.GameStores.StoreGameLoader;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSteamLoader
    {
        
        [TestMethod]
        public void LoadFromPath_FindsExpectedWads()
        {
            // Assuming fixtures:

            // SteamGame.ULTIMATE_DOOM:
            // TestSteamLibrary1 / steamapps / common / TestDoom


            GameFinder finder = _ => @"Resources\TestSteamLibrary1\steamapps\common\TestDoom";

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
        public void LoadFromPath_ReturnsEmptyFilesIfDirectoryNotFound()
        {
            var gameStoreFiles = StoreGameLoader.LoadStoreGame(StoreGame.ULTIMATE_DOOM, @"Resources\DoesNotExist");
            Assert.AreEqual(gameStoreFiles, GameStoreFiles.EMPTY);
        }
    }
}
