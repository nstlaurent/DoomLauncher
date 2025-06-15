using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestDoom64TitlePicSyncAction
    {
        [TestMethod]
        public void ApplyGameFile_FindsDoom64TitlePic()
        {
            var gameFile = new GameFile()
            {
                GameFileID = 11,
                FileName = "horizon.zip"
            };
            var syncAction = new Doom64TitlePicSyncAction();
            var reader = ArchiveReader.Create(Path.Combine("Resources", "doom64_with_titlepic.zip"));

            var result = syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.IsTrue(result.GetTitlePic(gameFile, out _));
        }

        [TestMethod]
        public void ApplyGameFile_DoesntFindOtherImages()
        {
            var gameFile = new GameFile()
            {
                GameFileID = 44,
                FileName = "blah.zip"
            };
            var syncAction = new Doom64TitlePicSyncAction();
            var reader = ArchiveReader.Create(Path.Combine("Resources", "doom64_without_titlepic.zip"));

            var result = syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.IsFalse(result.GetTitlePic(gameFile, out _));
        }
    }
}
