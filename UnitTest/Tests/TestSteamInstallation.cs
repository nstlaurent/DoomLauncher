using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.Steam;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSteamInstallation
    {
        [TestMethod]
        public void FindExpectedWads_ReturnsEmptyIfNoLibrariesFound()
        {
            var steam = new SteamInstallation(@"C:\whatevs", new List<SteamLibrary>());
            var iwads = steam.GetInstalledIWads();
            var pwads = steam.GetInstalledPWads();

            Assert.AreEqual(0, iwads.Count);
            Assert.AreEqual(0, pwads.Count);
        }

        [TestMethod]
        public void GetInstalledWads_ReturnsWadsWithoutDuplicates()
        {
            var ultimateDoom = new SteamInstalledGame(SteamGame.ULTIMATE_DOOM, 
                @"C:\SteamLib1\Ultimate Doom",
                new List<string>() { @"C:\SteamLib1\Ultimate Doom\doom.wad", @"C:\SteamLib1\Ultimate Doom\doom2.wad" },
                new List<string>() { @"C:\SteamLib1\Ultimate Doom\nerve.wad", @"C:\SteamLib1\Ultimate Doom\id1.wad" },
                null);

            var doom2Lib1 = new SteamInstalledGame(SteamGame.DOOM2, 
                @"C:\SteamLib1\Doom2",
                new List<string>() { @"C:\SteamLib1\Doom2\doom2.wad" },
                new List<string>(),
                null);

            var doom2Lib2 = new SteamInstalledGame(SteamGame.DOOM2, 
                @"C:\SteamLib2\Doom2",
                new List<string>() { @"C:\SteamLib2\Doom2\doom2.wad" },
                new List<string>(), 
                null);

            var strife = new SteamInstalledGame(SteamGame.STRIFE,
                @"C:\SteamLib2\Strife",
                new List<string>() { @"C:\SteamLib2\Strife\strife.wad" },
                new List<string>(),
                null);

            var doom64 = new SteamInstalledGame(SteamGame.DOOM64,
                @"C:\SteamLib1\Doom64",
                new List<string>() { @"C:\SteamLib1\Doom64\doom64.wad" },
                new List<string>(),
                @"C:\SteamLib1\Doom64\Doom64_x64.exe");

            var library1 = new SteamLibrary(@"C:\SteamLib1", 
                new List<SteamInstalledGame>() { ultimateDoom, doom2Lib1, doom64 });

            var library2 = new SteamLibrary(@"C:\SteamLib2",
                new List<SteamInstalledGame>() { doom2Lib2, strife });

            var steam = new SteamInstallation(@"C:\Steam", 
                new List<SteamLibrary>() { library1, library2 });

            var iwads = steam.GetInstalledIWads();
            var pwads = steam.GetInstalledPWads();
            var doom64Exe = steam.GetInstalledDoom64Exe();

            Assert.AreEqual(4, iwads.Count);
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom2.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("strife.wad")));
            Assert.IsTrue(iwads.Exists(x => x.Contains("doom64.wad")));

            Assert.AreEqual(2, pwads.Count);
            Assert.IsTrue(pwads.Exists(x => x.Contains("nerve.wad")));
            Assert.IsTrue(pwads.Exists(x => x.Contains("id1.wad")));

            Assert.IsNotNull(doom64Exe);
            Assert.AreEqual(@"C:\SteamLib1\Doom64\Doom64_x64.exe", doom64Exe);
        }
    }
}
