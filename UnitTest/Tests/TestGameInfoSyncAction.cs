using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameInfoSyncAction
    {
        [TestMethod]
        public void ApplyToGameFile_FindsTitleInGAMEINFO()
        {
            var gameFile = new GameFile()
            {
                FileName = "blah.wad"
            };

            var syncAction = new GameInfoSyncAction();

            Tree files = new Tree("root", new Tree("GAMEINFO", "IWAD = \"blah.wad\"\nSTARTUPTITLE  = \"The Big Tree\"\nSOMETHING_ELSE = blah"));
            var reader = new TreeReader(files);

            var result = syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual("The Big Tree", gameFile.Title);
        }

        [TestMethod]
        public void ApplyToGameFile_FindsTitleInGAMEINFOTxt()
        {
            var gameFile = new GameFile()
            {
                FileName = "blah.wad"
            };

            var syncAction = new GameInfoSyncAction();

            Tree files = new Tree("root", new Tree("GAMEINFO.txt", "IWAD = \"blah.wad\"\nSTARTUPTITLE  = \"The Big Banana\"\nSOMETHING_ELSE = blah"));
            var reader = new TreeReader(files);

            var result = syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual("The Big Banana", gameFile.Title);
        }

        [TestMethod]
        public void ApplyToGameFile_DoesntFindsTitleInAFileCalledSomethingElse()
        {
            var gameFile = new GameFile()
            {
                FileName = "blah.wad"
            };

            var syncAction = new GameInfoSyncAction();

            Tree files = new Tree("root", new Tree("GAMEINF", "IWAD = \"blah.wad\"\nSTARTUPTITLE  = \"The Big Banana\"\nSOMETHING_ELSE = blah"));
            var reader = new TreeReader(files);

            var result = syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.IsTrue(string.IsNullOrEmpty(gameFile.Title));
        }
    }
}
