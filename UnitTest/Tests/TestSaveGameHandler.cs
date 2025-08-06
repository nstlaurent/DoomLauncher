using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SaveGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSaveGameHandler
    {
        private IDataSourceAdapter database;
        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            SaveGameDirectory = new LauncherPath("SaveGames"),
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("SaveGames");
            Directory.CreateDirectory("SourcePortSaveGames");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("SaveGames"))
                Directory.Delete("SaveGames", true);

            if (Directory.Exists("SourcePortSaveGames"))
                Directory.Delete("SourcePortSaveGames", true);

            if (File.Exists(@"Resources\chocosave1.what"))
                File.Delete(@"Resources\chocosave1.what");

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void InsertSaveGame_NullGameFileFails()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { SourcePortID = 11 };

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, null, @"Resources\chocosave1.dsg");

            Assert.IsNull(saveGame);
        }

        [TestMethod]
        public void InsertSaveGame_UnsavedGameFileFails()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { SourcePortID = 11 };
            var gameFile = new GameFile() { FileName = "Not_saved.zip" };

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");

            Assert.IsNull(saveGame);
        }


        [TestMethod]
        public void InsertSaveGame_CopiesFile()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { Executable = "zdoom.exe" };
            database.InsertSourcePort(sourcePort);
            var gameFile = new GameFile() { FileName = "fname.zip" };
            database.InsertGameFile(gameFile);

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");

            Assert.IsNotNull(saveGame);
            Assert.IsTrue(File.Exists(@"Resources\chocosave1.dsg"));
            Assert.IsTrue(File.Exists(config.SaveGameDirectory.GetFullPath(saveGame.FileName)));
        }

        [TestMethod]
        public void InsertSaveGame_InsertsIntoDatabase()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { Executable = "zdoom.exe" };
            database.InsertSourcePort(sourcePort);
            var gameFile = new GameFile() { FileName = "slayer.zip" };
            database.InsertGameFile(gameFile);

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");
            saveGame = database.GetFiles(gameFile, FileType.SaveGame).FirstOrDefault();

            Assert.IsNotNull(saveGame);
        }

        [TestMethod]
        public void InsertSaveGame_PopulatesTheOriginalFileName()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { Executable = "zdoom.exe" };
            database.InsertSourcePort(sourcePort);
            var gameFile = new GameFile() { FileName = "shotgun.zip" };
            database.InsertGameFile(gameFile);

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");

            Assert.IsNotNull(saveGame);
            Assert.AreEqual("chocosave1.dsg", saveGame.OriginalFileName);
        }

        [TestMethod]
        public void InsertSaveGame_PopulatesTheSourcePort()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { Executable = "zdoom.exe" };
            database.InsertSourcePort(sourcePort);
            var gameFile = new GameFile() { FileName = "shotgun.zip" };
            database.InsertGameFile(gameFile);

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");

            Assert.IsNotNull(saveGame);
            Assert.AreEqual(sourcePort.SourcePortID, saveGame.SourcePortID);
        }

        [TestMethod]
        public void InsertSaveGame_PopulatesTheDescriptionWhereSaveGameTypeIsRecognised()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { Executable = "zdoom.exe" };
            database.InsertSourcePort(sourcePort);
            var gameFile = new GameFile() { FileName = "shotgun.zip" };
            database.InsertGameFile(gameFile);

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");

            Assert.IsNotNull(saveGame);
            var expectedDsgDescription = new DsgSaveGameReader(@"Resources\chocosave1.dsg").GetName();
            Assert.AreEqual(expectedDsgDescription, saveGame.Description);
        }

        [TestMethod]
        public void InsertSaveGame_PopulatesTheDescriptionWhereSaveGameTypeIsNotRecognised()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData() { Executable = "zdoom.exe" };
            database.InsertSourcePort(sourcePort);
            var gameFile = new GameFile() { FileName = "shotgun.zip" };
            database.InsertGameFile(gameFile);
            File.Copy(@"Resources\chocosave1.dsg", @"Resources\chocosave1.what");

            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.what");

            Assert.IsNotNull(saveGame);
            Assert.AreEqual("chocosave1.what", saveGame.Description);
        }

        [TestMethod]
        public void UpdateSaveGameFromSourcePort_CopiesToSaveGameDir()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData()
            {
                Executable = "zdoom.exe",
                AltSaveDirectory = new LauncherPath("SourcePortSaveGames")
            };
            database.InsertSourcePort(sourcePort);

            // Put the original file in the sourceport directory
            File.Copy(@"Resources\chocosave1.dsg", @"SourcePortSaveGames\chocosave1.dsg");
            Assert.IsTrue(File.Exists(@"SourcePortSaveGames\chocosave1.dsg"));

            var gameFile = new GameFile() { FileName = "shotgun.zip" };
            database.InsertGameFile(gameFile);
            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"SourcePortSaveGames\chocosave1.dsg");

            // Touch the save so we know that it has been copied
            var originalFileInfo = TestUtil.TouchFile(@"SourcePortSaveGames\chocosave1.dsg");

            saveGameHandler.UpdateSaveGameFromSourcePort(sourcePort, saveGame);

            var localFileInfo = new FileInfo(saveGame.FullFileName);
            Assert.IsTrue(File.Exists(saveGame.FullFileName));
            Assert.AreEqual(originalFileInfo.LastWriteTime, localFileInfo.LastWriteTime);
        }

        [TestMethod]
        public void UpdateSaveGameFromSourcePort_UpdatesDescriptionInDB()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData()
            {
                Executable = "zdoom.exe",
                AltSaveDirectory = new LauncherPath("SourcePortSaveGames")
            };
            database.InsertSourcePort(sourcePort);

            // Put the original file in the sourceport directory
            File.Copy(@"Resources\chocosave1.dsg", @"SourcePortSaveGames\chocosave1.dsg");
            Assert.IsTrue(File.Exists(@"SourcePortSaveGames\chocosave1.dsg"));

            var gameFile = new GameFile() { FileName = "shotgun.zip" };
            database.InsertGameFile(gameFile);
            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"SourcePortSaveGames\chocosave1.dsg");

            // Set Description to null in DB
            saveGame.Description = null;
            database.UpdateFile(saveGame);
            Assert.IsNull(saveGame.Description);

            // Touch the original file so we know if it was copied or not
            var originalFileInfo = TestUtil.TouchFile(@"SourcePortSaveGames\chocosave1.dsg");

            saveGameHandler.UpdateSaveGameFromSourcePort(sourcePort, saveGame);

            // Description is no longer null in DB
            Assert.IsNotNull(saveGame.Description);
            saveGame = database.GetFiles(gameFile, FileType.SaveGame).First();
            Assert.IsFalse(string.IsNullOrEmpty(saveGame.Description));
        }

        [TestMethod]
        public void DeleteSaveGame_DeletesListedSaveGamesMatchingOriginal()
        {
            var saveGameHandler = new SaveGameHandler(database, config);

            var sourcePort = new SourcePortData()
            {
                Executable = "zdoom.exe",
                AltSaveDirectory = new LauncherPath("SourcePortSaveGames")
            };
            database.InsertSourcePort(sourcePort);

            // Put the original file in the sourceport directory
            File.Copy(@"Resources\chocosave1.dsg", @"SourcePortSaveGames\chocosave1.dsg");
            var originalFileInfo = new FileInfo(@"SourcePortSaveGames\chocosave1.dsg");
            Assert.IsTrue(originalFileInfo.Exists);

            var gameFile = new GameFile() { FileName = "plasma.zip" };
            database.InsertGameFile(gameFile);
            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"SourcePortSaveGames\chocosave1.dsg");

            saveGameHandler.DeleteSaveGame(@"SourcePortSaveGames\chocosave1.dsg", new IFileData[] { saveGame });

            var saveGamesFromDB = database.GetFiles(gameFile, FileType.SaveGame).ToList();
            Assert.IsFalse(saveGamesFromDB.Any(file => file.FileID == saveGame.FileID));
            Assert.IsFalse(File.Exists(saveGame.FullFileName));
        }

        [TestMethod]
        public void CopySaveGameToSourcePort_CopiesGameToSourcePort()
        {
            var saveGameHandler = new SaveGameHandler(database, config);
            var sourcePort = new SourcePortData()
            {
                Executable = "zdoom.exe",
                AltSaveDirectory = new LauncherPath("SourcePortSaveGames")
            };
            database.InsertSourcePort(sourcePort);

            var gameFile = new GameFile() { FileName = "plasma.zip" };
            database.InsertGameFile(gameFile);
            var saveGame = saveGameHandler.InsertSaveGame(sourcePort, gameFile, @"Resources\chocosave1.dsg");

            saveGameHandler.CopySaveGameToSourcePort(sourcePort, saveGame);

            Assert.IsTrue(File.Exists(@"SourcePortSaveGames\chocosave1.dsg"));
        }
    }
}
