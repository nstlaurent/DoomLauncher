using DoomLauncher.GameStores.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSteamFileUtils
    {
        [TestMethod]
        public void TryGetInstallDir()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamLibrary1\steamapps\appmanifest_2280.acf");
            var success = SteamFileUtils.TryGetInstallDir(path, out var installDir);
            
            Assert.IsTrue(success);
            Assert.AreEqual("TestDoom", installDir);
        }

        [TestMethod]
        public void TryGetLibraryPaths()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall\config\libraryfolders.vdf");
            var success = SteamFileUtils.TryGetLibraryPaths(path, out var libraryPaths);

            Assert.IsTrue(success);
            Assert.AreEqual(2, libraryPaths.Count);
            Assert.AreEqual(@"Resources\TestSteamLibrary1", libraryPaths[0]);
            Assert.AreEqual(@"Resources\TestSteamLibrary2", libraryPaths[1]);
        }
    }
}
