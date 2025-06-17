using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestIWadTitlesSyncAction
    {
        [TestMethod]
        public void ApplyToGameFile_ReplacesTitleOfIWads()
        {
            IGameFile gameFile = new GameFile
            {
                FileName = "heretic.zip",
                Title = "Wrong title"
            };

            var action = new IWadTitlesSyncAction();
            action.ApplyToGameFile(gameFile, null, null);

            Assert.AreEqual("Heretic: Shadow of the Serpent Riders", gameFile.Title);

        }

        [TestMethod]
        public void ApplyToGameFile_DoesntReplaceTitleOfNonIWads()
        {
            IGameFile gameFile = new GameFile
            {
                FileName = "floom.zip",
                Title = "blah"
            };

            var action = new IWadTitlesSyncAction();
            action.ApplyToGameFile(gameFile, null, null);

            Assert.AreEqual("blah", gameFile.Title);

        }

    }
}
