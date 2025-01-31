using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.GameStores;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameStoreFiles
    {
        [TestMethod]
        public void Combine_AddsLists()
        {
            var gameStoreFiles1 = new GameStoreFiles(new List<string> { "a.wad", "b.wad" }, new List<string> { "x.wad" }, null);
            var gameStoreFiles2 = new GameStoreFiles(new List<string> { "c.wad" }, new List<string> { "y.wad", "z.wad" }, null);

            var result = gameStoreFiles1.Combine(gameStoreFiles2);

            Assert.IsTrue(new List<string> { "a.wad", "b.wad", "c.wad" }.SequenceEqual(result.InstalledIWads));
            Assert.IsTrue(new List<string> { "x.wad", "y.wad", "z.wad" }.SequenceEqual(result.InstalledPWads));
        }

        [TestMethod]
        public void Combine_AddsListsIgnoringDuplicates()
        {
            var gameStoreFiles1 = new GameStoreFiles(new List<string> { @"base\a.wad", "b.wad" }, new List<string> { @"base\x.wad" }, null);
            var gameStoreFiles2 = new GameStoreFiles(new List<string> { @"release\a.wad" }, new List<string> { @"release\x.wad", "z.wad" }, null);

            var result = gameStoreFiles1.Combine(gameStoreFiles2);


            Assert.IsTrue(new List<string> { @"base\a.wad", "b.wad" }.SequenceEqual(result.InstalledIWads));
            Assert.IsTrue(new List<string> { @"base\x.wad", "z.wad" }.SequenceEqual(result.InstalledPWads));
        }
    }
}
