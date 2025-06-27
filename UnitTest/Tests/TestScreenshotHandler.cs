using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace UnitTest.Tests
{
    [TestClass]
    public class TestScreenshotHandler
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
        public void InsertScreenshot_NullGameFileFails()
        {
            var screenshotHandler = new ScreenshotHandler(database, config);

            List<IFileData> existingScreenshots = new List<IFileData>();

            var imageStream = GetImageStream(@"Resources\happy.png");

            var succeeded = screenshotHandler.InsertScreenshot(null, imageStream, existingScreenshots, out var fileData);

            Assert.IsFalse(succeeded);
            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertScreenshot_NullGameFileIdFails()
        {
            var screenshotHandler = new ScreenshotHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "Foo.zip",
            };

            List<IFileData> existingScreenshots = new List<IFileData>();

            var imageStream = GetImageStream(@"Resources\happy.png");

            var succeeded = screenshotHandler.InsertScreenshot(null, imageStream, existingScreenshots, out var fileData);

            Assert.IsFalse(succeeded);
            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertScreenshot_CreatesFileAndDatabaseEntry()
        {
            var screenshotHandler = new ScreenshotHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var imageStream = GetImageStream(@"Resources\happy.png");

            var succeeded = screenshotHandler.InsertScreenshot(gameFile, imageStream, null, out var fileData);
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            Assert.IsTrue(succeeded);
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
