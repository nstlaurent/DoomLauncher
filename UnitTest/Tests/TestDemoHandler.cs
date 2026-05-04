using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestDemoHandler
    {
        private IDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            DemoDirectory = new LauncherPath("Demos"),
        };

        private IFileHandler fileHandler;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("Demos");
            fileHandler = new FileHandler(database, config);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Demos"))
                Directory.Delete("Demos", true);

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void InsertNewDemo_NullGameFileIdFails()
        {
            var demoHandler = new DemoHandler(fileHandler);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 11
            };

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertNewDemo_NullGameFileIdFails.zip"
            };

            var fileData = demoHandler.InsertNewDemo(sourcePort, gameFile, @"Resources\zandemo.cld", "Great description");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertNewDemo_NonExistingFileFails()
        {
            var demoHandler = new DemoHandler(fileHandler);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 11
            };

            var gameFile = new GameFile()
            {
                GameFileID = 36,
                FileName = "InsertNewDemo_NonExistingFileFails.zip"
            };
            database.InsertGameFile(gameFile);

            var fileData = demoHandler.InsertNewDemo(sourcePort, gameFile, "MADE_UP_DEMO.cld", "Doesn't exist");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertNewDemo_InsertsFileOnDisk()
        {
            var demoHandler = new DemoHandler(fileHandler);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 11
            };

            var gameFile = new GameFile()
            {
                GameFileID = 36,
                FileName = "InsertNewDemo_InsertsFileOnDisk.zip"
            };
            database.InsertGameFile(gameFile);

            // Nothing up my sleeve
            var existingDemos = Directory.EnumerateFiles(config.DemoDirectory.GetFullPath());
            Assert.IsFalse(existingDemos.Any());

            var fileData = demoHandler.InsertNewDemo(sourcePort, gameFile, @"Resources\zandemo.cld", "Copy it");

            existingDemos = Directory.EnumerateFiles(config.DemoDirectory.GetFullPath());

            Assert.IsNotNull(fileData);
            Assert.AreEqual(1, existingDemos.Count());
            Assert.IsTrue(File.Exists(config.DemoDirectory.GetFullPath(fileData.FileName)));

            // Source file is still around
            Assert.IsTrue(File.Exists(@"Resources\zandemo.cld"));
        }

        [TestMethod]
        public void InsertNewDemo_InsertsFileToDatabase()
        {
            var demoHandler = new DemoHandler(fileHandler);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 11
            };

            IGameFile gameFile = new GameFile()
            {
                FileName = "InsertNewDemo_InsertsFileToDatabase.zip"
            };
            database.InsertGameFile(gameFile);

            // Nothing up my sleeve
            var existingDemos = database.GetFiles(gameFile, FileType.Demo).ToList();
            Assert.IsFalse(existingDemos.Any());

            var fileData = demoHandler.InsertNewDemo(sourcePort, gameFile, @"Resources\zandemo.cld", "Save to DB!!");

            Assert.IsNotNull(fileData);
            existingDemos = database.GetFiles(gameFile, FileType.Demo).ToList();
            Assert.AreEqual(1, existingDemos.Count());
            var fileData2 = existingDemos[0];

            Assert.AreEqual("Save to DB!!", fileData.Description);
            Assert.AreEqual("Save to DB!!", fileData2.Description);
            Assert.AreEqual(fileData.Description, fileData2.Description);
            Assert.AreEqual(gameFile.GameFileID, fileData.GameFileID);
            Assert.AreEqual(gameFile.GameFileID, fileData2.GameFileID);
            Assert.AreEqual(11, fileData.SourcePortID);
            Assert.AreEqual(11, fileData2.SourcePortID);
            Assert.AreEqual(FileType.Demo, fileData.FileTypeID);
            Assert.AreEqual(FileType.Demo, fileData2.FileTypeID);
        }
    }
}
