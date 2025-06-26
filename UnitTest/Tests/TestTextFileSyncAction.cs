using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestTextFileSyncAction
    {

        [TestMethod]
        public void ApplyToGameFile_ChoosesHighestQualityAnswers()
        {
            IGameFile gameFile = new GameFile()
            {
                FileName = "banana.wad"
            };
            Tree files = new Tree("root", new Tree("child1.txt"), new Tree("child2.txt"));

            var syncAction = new TextFileSyncAction(str =>
            {
                switch (str)
                {
                    case "child1.txt":
                        return new IdGamesTextInfo("Child Mod", "Radley Bobbikins", DateTime.Parse("4/3/2025"), null, "DOOM2");
                    case "child2.txt":
                        return new IdGamesTextInfo("Wrong name", null, null, "A fine mod.", null);
                    default:
                        throw new Exception($"Unexpected entry {str}");
                }
            });

            syncAction.ApplyToGameFile(gameFile, new TreeReader(files), new string[0]);

            // Child1's fields are preferred, because it is higher quality
            Assert.AreEqual("Child Mod", gameFile.Title);
            Assert.AreEqual("Radley Bobbikins", gameFile.Author);
            Assert.AreEqual(DateTime.Parse("4/3/2025"), gameFile.ReleaseDate);

            // Falls through to Child2 where missing from Child1
            Assert.AreEqual("A fine mod.", gameFile.Description);
        }

        [TestMethod]
        public void ApplyToGameFile_IgnoresNonTextFiles()
        {
            IGameFile gameFile = new GameFile()
            {
                FileName = "something.wad"
            };
            Tree files = new Tree("root", new Tree("lowqual.txt"), new Tree("highqual1.bin"));

            var syncAction = new TextFileSyncAction(str =>
            {
                switch (str)
                {
                    case "lowqual.txt":
                        return new IdGamesTextInfo("Low quality title", null, null, null, null);
                    case "highqual1.bin":
                        return new IdGamesTextInfo("High Quality Mod", "Ernest Wallop", null, "Lots of description", "DOOM2");
                    default:
                        throw new Exception($"Unexpected entry {str}");
                }
            });

            syncAction.ApplyToGameFile(gameFile, new TreeReader(files), new string[0]);

            // The high quality file got ignored because it isn't .txt
            Assert.IsTrue(string.IsNullOrEmpty(gameFile.Author));
            Assert.IsNull(gameFile.ReleaseDate);
            Assert.IsTrue(string.IsNullOrEmpty(gameFile.Description));

            // Low quality text file got used
            Assert.AreEqual("Low quality title", gameFile.Title);
        }

        [TestMethod]
        public void ApplyToGameFile_ChecksWADINFO()
        {
            IGameFile gameFile = new GameFile()
            {
                FileName = "potato.wad"
            };
            Tree files = new Tree("root", new Tree("WADINFO"), new Tree("wadinfo"));

            var syncAction = new TextFileSyncAction(str =>
            {
                switch (str)
                {
                    case "WADINFO":
                        return new IdGamesTextInfo("Wadinfo title", null, null, null, "HERETIC");
                    case "wadinfo":
                        return new IdGamesTextInfo(null, "Wadinfo author", null, "WadInfo description", null);
                    default:
                        throw new Exception($"Unexpected entry {str}");
                }
            });

            syncAction.ApplyToGameFile(gameFile, new TreeReader(files), new string[0]);

            Assert.AreEqual("Wadinfo title", gameFile.Title);
            Assert.AreEqual("Wadinfo author", gameFile.Author);
            Assert.AreEqual("WadInfo description", gameFile.Description);
        }

        [TestMethod]
        public void ApplyToGameFile_NullOrEmptyDataDoesntOverwriteGameFile()
        {
            IGameFile gameFile = new GameFile()
            {
                FileName = "imps.wad",
                Author = "Fredericus",
                Title = "Too Many Imps",
                ReleaseDate = DateTime.Parse("1/7/2021"),
                Description = "A wad with too many imps",
            };
            Tree files = new Tree("root", new Tree("empty.txt"));

            var syncAction = new TextFileSyncAction(str =>
                new IdGamesTextInfo(null, null, null, null, null));

            syncAction.ApplyToGameFile(gameFile, new TreeReader(files), new string[0]);

            Assert.AreEqual("Too Many Imps", gameFile.Title);
            Assert.AreEqual("Fredericus", gameFile.Author);
            Assert.AreEqual(DateTime.Parse("1/7/2021"), gameFile.ReleaseDate);
            Assert.AreEqual("A wad with too many imps", gameFile.Description);
        }
    }
}