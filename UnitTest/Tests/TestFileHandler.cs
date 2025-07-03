using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace UnitTest.Tests
{
    [TestClass]
    public class TestFileHandler
    {
        private IDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            ScreenshotDirectory = new LauncherPath("Screenshots")
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("Screenshots");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
        }

        [TestMethod]
        public void InsertFileFromMemory_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFileFromMemory(null, FileType.Thumbnail, imageStream, "png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertFileFromMemory_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "Foo.zip",
            };

            var imageStream = GetImageStream(@"Resources\happy.png");
            var fileData = fileHandler.InsertFileFromMemory(null, FileType.Screenshot, imageStream, "png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertFileFromMemory_CreatesFileAndDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFileFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        private static MemoryStream GetImageStream(string fileName)
        {
            var imageStream = new MemoryStream();
            var image = Image.FromFile(fileName);
            image.Save(imageStream, ImageFormat.Png);
            return imageStream;
        }
    }
}
