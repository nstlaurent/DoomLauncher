using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestIntendedIwadSyncAction
    {
        private IDataSourceAdapter database;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
        }

        [TestCleanup]
        public void Cleanup()
        {
            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from IWads");
        }

        [TestMethod]
        public void ApplyToGameFile_FindsIWadMatchingIntendedGame()
        {
            var gameFile = new GameFile()
            {
                FileName = "blah.wad",
                IntendedGame = IWadInfo.Heretic
            };

            var expectedIwad = CreateIWad(IWadInfo.Heretic);

            var syncAction = new IntendedIwadSyncAction(database);

            Tree files = new Tree("root", new Tree("entry1" ));
            var reader = new TreeReader(files);

            syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual(expectedIwad.IWadID, gameFile.IWadID);
        }

        [TestMethod]
        public void ApplyToGameFile_FindsBackupIWadMatchingIntendedGameIfIwadIsMissing()
        {
            var gameFile = new GameFile()
            {
                FileName = "diamond.wad",
                IntendedGame = IWadInfo.Doom2
            };

            var backupIWad = CreateIWad(IWadInfo.FreeDoom2);

            var syncAction = new IntendedIwadSyncAction(database);

            Tree files = new Tree("root", new Tree("entry1"));
            var reader = new TreeReader(files);

            syncAction.ApplyToGameFile(gameFile, reader, new string[0]);

            Assert.AreEqual(backupIWad.IWadID, gameFile.IWadID);
        }

        private IIWadData CreateIWad(IWadInfo iwadInfo)
        {
            var fileName = iwadInfo.GameName + ".wad";
            var gameFile0 = new GameFile()
            {
                FileName = fileName,
                Title = iwadInfo.Title
            };
            database.InsertGameFile(gameFile0);
            var gameFile1 = database.GetGameFile(fileName);

            var iwad0 = new IWadData()
            {
                FileName = fileName,
                Name = iwadInfo.Title,
                GameFileID = gameFile1.GameFileID
            };
            database.InsertIWad(iwad0);
            var iwad = database.GetIWad(gameFile1.GameFileID ?? -1);

            gameFile1.IWadID = iwad.IWadID;
            database.UpdateGameFile(gameFile1);

            return iwad;
        }

    }
}
