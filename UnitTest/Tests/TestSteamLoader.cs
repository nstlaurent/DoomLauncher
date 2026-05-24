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
        public void GetGameFolder()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall");
            var gameFolder = SteamLoader.GetGameFolder(path, StoreGame.ULTIMATE_DOOM);

            Assert.AreEqual(@"Resources\TestSteamLibrary1\steamapps\common\TestDoom", gameFolder);
        }
    }
}
