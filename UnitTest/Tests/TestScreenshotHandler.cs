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
    public class TestScreenshotHandler
    {
        private IDataSourceAdapter database;
        private IFileHandler fileHandler;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            ScreenshotDirectory = new LauncherPath("Screenshots")
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            fileHandler = new FileHandler(database, config);
            Directory.CreateDirectory("Screenshots");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);

            if (File.Exists(@"Resources\happy_DELETE_ME.png"))
                File.Delete(@"Resources\happy_DELETE_ME.png");

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void HandleNewScreenshots_CopiesFiletoDiskPreservingSource()
        {
            var screenshotHandler = new ScreenshotHandler(fileHandler, false);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 731
            };

            // Nothing up my sleeve
            var existingScreenshots = Directory.EnumerateFiles(config.ScreenshotDirectory.GetFullPath());
            Assert.IsFalse(existingScreenshots.Any());

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "glaah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("glaah.zip");

            // Insert a file as a TitlePic
            var fileDataList = screenshotHandler.HandleNewScreenshots(sourcePort, gameFile, new string[] { @"Resources\happy.png" }).ToList();

            // Tada!
            existingScreenshots = Directory.EnumerateFiles(config.ScreenshotDirectory.GetFullPath());
            Assert.AreEqual(1, existingScreenshots.Count());
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileDataList[0].FileName)));

            // Source image is still around
            Assert.IsTrue(File.Exists(@"Resources\happy.png"));
        }

        [TestMethod]
        public void HandleNewScreenshots_InsertsDatabaseEntry()
        {
            var screenshotHandler = new ScreenshotHandler(fileHandler, false);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 536
            };

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "blaah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("blaah.zip");

            var fileDataList = screenshotHandler.HandleNewScreenshots(sourcePort, gameFile, new string[] { @"Resources\happy.png" }).ToList();
            var filesInDB = database.GetFiles(gameFile, FileType.Screenshot).ToList();

            Assert.AreEqual(1, fileDataList.Count());
            Assert.AreEqual(1, filesInDB.Count());
            Assert.AreEqual(536, fileDataList[0].SourcePortID);
            Assert.AreEqual(fileDataList[0].FileName, filesInDB[0].FileName);
        }

        [TestMethod]
        public void HandleNewScreenshots_DeletesOldScreenshotIfConfigTellsItTo()
        {
            var screenshotHandler = new ScreenshotHandler(fileHandler, true);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 335
            };

            // We don't want to lose our normal copy!
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            Assert.IsTrue(File.Exists(@"Resources\happy_DELETE_ME.png"));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "hoo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("hoo.zip");

            var fileDataList = screenshotHandler.HandleNewScreenshots(sourcePort, gameFile, new string[] { @"Resources\happy_DELETE_ME.png" }).ToList();
            
            Assert.AreEqual(1, fileDataList.Count());
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileDataList[0].FileName)));
            Assert.IsFalse(File.Exists(@"Resources\happy_DELETE_ME.png"));
        }
    }
}
