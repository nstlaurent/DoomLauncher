using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.Steam;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSteamLoader
    {
        [TestMethod]
        public void LoadFromPath_FindsExpectedWads()
        {
            // Assuming Steam configured in TestSteamInstall

            // Assuming fixtures:

            // SteamGame.ULTIMATE_DOOM:
            // TestSteamLibrary1 / steamapps / common / TestDoom

            // SteamGame.HERETIC:
            // TestSteamLibrary1 / steamapps / common / TestHeretic
            // TestSteamLibrary2 / steamapps / common / TestHeretic

            // SteamGame.HEXEN:
            // TestSteamLibrary2 / steamapps / common / TestHexen

            var steam = SteamLoader.LoadFromPath(@"Resources\TestSteamInstall");

            var iwads = steam.GetInstalledIWads();
            Assert.AreEqual(6, iwads.Count);
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom2.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("plutonia.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("tnt.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("heretic.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("hexen.wad")));

            var pwads = steam.GetInstalledPWads();
            Assert.AreEqual(4, pwads.Count);
            Assert.IsTrue(pwads.Exists(x => x.Contains("nerve.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("id1.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("masterlevels.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("sigil.wad")));
        }
    }
}
