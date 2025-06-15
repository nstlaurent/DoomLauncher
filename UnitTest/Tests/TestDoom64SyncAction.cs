using DoomLauncher.Interfaces;
using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.DataSources;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestDoom64SyncAction
    {
        private IDataSourceAdapter database;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
        }

        [TestMethod]
        public void ApplyToGameFile_IdentifiesDoom64FileFromMapInfo()
        {
            var syncAction = new Doom64SyncAction(database);
            
            GameFile file = new GameFile { GameFileID = 1 };
            var mapInfoData = new string[] { "classtype = something" };

            var result = syncAction.ApplyToGameFile(file, null, mapInfoData);

            Assert.IsFalse(result.Failed);
            Assert.IsTrue(file.IsDoom64);
        }

        [TestMethod]
        public void ApplyToGameFile_IdentifiesNonDoom64FileFromMapInfo()
        {
            var syncAction = new Doom64SyncAction(database);

            GameFile file = new GameFile { GameFileID = 1 };
            var mapInfoData = new string[] { "somethingElse = something" };

            var result = syncAction.ApplyToGameFile(file, null, mapInfoData);

            Assert.IsFalse(result.Failed);
            Assert.IsFalse(file.IsDoom64);
        }

        [TestMethod]
        public void ApplyToGameFile_LinksDoom64GameFileWithDoom64IWad()
        {
            var syncAction = new Doom64SyncAction(database);
            GameFile file = new GameFile { GameFileID = 1 };
            var mapInfoData = new string[] { "classtype = something" };

            IGameFile doom64IWad = database.GetGameFile("doom64.zip");
            if (doom64IWad != null)
            {
                database.DeleteGameFile(doom64IWad);
            }
            IGameFile newDoom64IWad = new GameFile { GameFileID = 777, FileName = "doom64.zip", IWadID = 666 };
            database.InsertGameFile(newDoom64IWad);

            var result = syncAction.ApplyToGameFile(file, null, mapInfoData);

            Assert.IsFalse(result.Failed);
            Assert.AreEqual(666, file.IWadID);
        }
    }
}
