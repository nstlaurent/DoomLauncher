using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameConfSyncAction
    {
        private DbDataSourceAdapter database;

        [TestInitialize]
        public void Initialize()
        {
            database = (DbDataSourceAdapter)TestUtil.CreateAdapter();
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.DataAccess.ExecuteNonQuery("delete from IWads");
        }

        [TestMethod]
        public void ApplyToGameFile_SetsTitle()
        {
            GameFile gameFile = new GameFile 
            { 
                GameFileID = 1,
                Title = "Wrong Title",
            };

            Tree files = new Tree("root", new Tree("GAMECONF", 
                "{\"data\": { \"title\": \"Bingo Bongo\" }} "));
            var reader = new TreeReader(files);
            var action = new GameConfSyncAction(database);

            action.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual("Bingo Bongo", gameFile.Title);
        }

        [TestMethod]
        public void ApplyToGameFile_SetsAuthor()
        {
            GameFile gameFile = new GameFile
            {
                GameFileID = 1,
                Author = "Wrong Author",
            };

            Tree files = new Tree("root", new Tree("GAMECONF",
                "{\"data\": { \"author\": \"Bob the Dog\" }} "));
            var reader = new TreeReader(files);
            var action = new GameConfSyncAction(database);

            action.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual("Bob the Dog", gameFile.Author);
        }

        [TestMethod]
        public void ApplyToGameFile_SetsDescription()
        {
            GameFile gameFile = new GameFile
            {
                GameFileID = 1,
                Description = "Wrong Description",
            };

            Tree files = new Tree("root", new Tree("GAMECONF",
                "{\"data\": { \"description\": \"A fancy dog.\" }} "));
            var reader = new TreeReader(files);
            var action = new GameConfSyncAction(database);

            action.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual("A fancy dog.", gameFile.Description);
        }

        [TestMethod]
        public void ApplyToGameFile_SetsIwad()
        {
            GameFile gameFile = new GameFile
            {
                GameFileID = 1,
            };

            database.InsertIWad(new IWadData { Name = "doom2.zip" });
            var iwadId = database.GetIWads().FirstOrDefault(x => x.Name == "doom2.zip").IWadID;

            Tree files = new Tree("root", new Tree("GAMECONF",
                "{\"data\": { \"iwad\": \"doom2.wad\" }} "));
            var reader = new TreeReader(files);
            var action = new GameConfSyncAction(database);

            action.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual(iwadId, gameFile.IWadID);
        }

        [TestMethod]
        public void ApplyToGameFile_IgnoresAbsentGAMECONF()
        {
            GameFile gameFile = new GameFile
            {
                GameFileID = 1,
                Title = "Old Title",
            };

            Tree files = new Tree("root", new Tree("WADINFO",
                "{\"data\": { \"title\": \"Eviternity\" }} "));
            var reader = new TreeReader(files);
            var action = new GameConfSyncAction(database);

            action.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual("Old Title", gameFile.Title);
        }

    }
}
