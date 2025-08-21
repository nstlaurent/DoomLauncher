using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestKnownWadsSyncAction
    {
        IDataSourceAdapter database;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
        }

        [TestCleanup]
        public void Cleanup()
        {
            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from IWads");
        }

        [TestMethod]
        public void ApplyToGameFile_DoesNothingToUnknownGameFile()
        {
            var action = new KnownWadsSyncAction(database);

            var gameFile = new GameFile()
            {
                FileName = "Unknown.zip",
                Title = "MyTitle"
            };

            action.ApplyToGameFile(gameFile, null, null);

            Assert.AreEqual("MyTitle", gameFile.Title);
            Assert.IsNull(gameFile.IWadID);
        }

        [TestMethod]
        public void ApplyToGameFile_SetsTheTitleForHexDD()
        {
            var action = new KnownWadsSyncAction(database);

            var gameFile = new GameFile()
            {
                FileName = "hexdd.zip",
                Title = "Replace me"
            };

            action.ApplyToGameFile(gameFile, null, null);

            Assert.AreEqual("Hexen: Deathkings of the Dark Citadel", gameFile.Title);
        }

        [TestMethod]
        public void ApplyToGameFile_SetsTheHexenIWadForHexDD()
        {
            var action = new KnownWadsSyncAction(database);

            IIWadData hexenIwad = new IWadData() { FileName = "hexen.wad" };
            database.InsertIWad(hexenIwad);
            hexenIwad = database.GetIWads().FirstOrDefault(iwad => iwad.FileNameBase.Equals("hexen"));
            Assert.IsNotNull(hexenIwad);
            Assert.IsNotNull(hexenIwad.IWadID);

            var gameFile = new GameFile()
            {
                FileName = "hexdd.zip",
                Title = "Replace me"
            };

            action.ApplyToGameFile(gameFile, null, null);

            Assert.AreEqual(hexenIwad.IWadID, gameFile.IWadID);
        }
    }
}
