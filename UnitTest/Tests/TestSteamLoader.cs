using DoomLauncher.GameStores;
using DoomLauncher.GameStores.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSteamLoader
    {
        [TestMethod]
        public void GetGameFolder_ReturnsGameFolderIfGameIsInstalled()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall");
            var gameFolder = SteamLoader.GetGameFolder(path, StoreGame.ULTIMATE_DOOM);

            Assert.AreEqual(@"Resources\TestSteamLibrary1\steamapps\common\TestDoom", gameFolder);
        }

        [TestMethod]
        public void GetGameFolder_ReturnsNullIfInstallFolderDoesNotExist()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall0");
            var gameFolder = SteamLoader.GetGameFolder(path, StoreGame.ULTIMATE_DOOM);

            Assert.IsNull(gameFolder);
        }

        [TestMethod]
        public void GetGameFolder_ReturnsNullIfNoLibraryPathsFound()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall2");
            var gameFolder = SteamLoader.GetGameFolder(path, StoreGame.ULTIMATE_DOOM);

            Assert.IsNull(gameFolder);
        }

        [TestMethod]
        public void GetGameFolder_ReturnsNullIfGameDirectoryNotFound()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall");
            var gameFolder = SteamLoader.GetGameFolder(path, StoreGame.DOOM64);

            Assert.IsNull(gameFolder);
        }
    }
}
