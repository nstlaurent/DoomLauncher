using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var screenshotHandler = new ScreenshotHandler(new FileHandler(database, config));
            var imageStream = GetImageStream(@"Resources\happy.png");

            var succeeded = screenshotHandler.InsertScreenshot(null, imageStream, out var fileData);

            Assert.IsFalse(succeeded);
            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertScreenshot_NullGameFileIdFails()
        {
            var screenshotHandler = new ScreenshotHandler(new FileHandler(database, config));

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "Foo.zip",
            };

            var imageStream = GetImageStream(@"Resources\happy.png");
            var succeeded = screenshotHandler.InsertScreenshot(null, imageStream, out var fileData);

            Assert.IsFalse(succeeded);
            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertScreenshot_CreatesFileAndDatabaseEntry()
        {
            var screenshotHandler = new ScreenshotHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var imageStream = GetImageStream(@"Resources\happy.png");

            var succeeded = screenshotHandler.InsertScreenshot(gameFile, imageStream, out var fileData);
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            Assert.IsTrue(succeeded);
            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void InsertScreenshot_BumpsUpTheOrderOfTheOtherFiles()
        {
            var screenshotHandler = new ScreenshotHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            // Add one screenshot record
            FileData file1 = new FileData() { 
                FileName = "1.png",
                FileTypeID = FileType.Screenshot,
                GameFileID = gameFile.GameFileID.Value, 
                FileOrder = 0 
            };
            database.InsertFile(file1);

            // Add another screenshot record
            FileData file2 = new FileData()
            {
                FileName = "2.png",
                FileTypeID = FileType.Screenshot,
                GameFileID = gameFile.GameFileID.Value,
                FileOrder = 1
            };
            database.InsertFile(file2);

            var imageStream = GetImageStream(@"Resources\happy.png");

            screenshotHandler.InsertScreenshot(gameFile, imageStream, out var fileData);
            var filesFromDB = database.GetFiles(gameFile, FileType.Screenshot);

            Assert.AreEqual(0, fileData.FileOrder);
            Assert.AreEqual(3, filesFromDB.Count());
            Assert.IsNotNull(filesFromDB.Where(f => f.FileName == "1.png" && f.FileOrder == 1).FirstOrDefault());
            Assert.IsNotNull(filesFromDB.Where(f => f.FileName == "2.png" && f.FileOrder == 2).FirstOrDefault());
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
