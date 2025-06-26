using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


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
            File.Copy(@"Resources/happy.png", @"Screenshots/happy.png");

            Assert.IsTrue(File.Exists(@"Screenshots/happy.png"));
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

        private static MemoryStream GetImageStream(string fileName)
        {
            var imageStream = new MemoryStream();
            var image = Image.FromFile(fileName);
            image.Save(imageStream, ImageFormat.Png);
            return imageStream;
        }
    }
}
