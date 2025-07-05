using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
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
            ScreenshotDirectory = new LauncherPath("Screenshots"),
            ThumbnailDirectory = new LauncherPath("Thumbnails"),
            TitlePicDirectory = new LauncherPath("TitlePics"),
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("Screenshots");
            Directory.CreateDirectory("Thumbnails");
            Directory.CreateDirectory("TitlePics");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);

            if (Directory.Exists("Thumbnails"))
                Directory.Delete("Thumbnails", true);

            if (Directory.Exists("TitlePics"))
                Directory.Delete("TitlePics", true);

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
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
        public void InsertFileFromMemory_CreatesFile()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Zoo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Zoo.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFileFromMemory(gameFile, FileType.Screenshot, imageStream, "png");

            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void InsertFileFromMemory_CreatesDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Groo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Groo.zip");

            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFileFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
        }

        [TestMethod]
        public void DeleteFile_DeletesFileOnDisk()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFileFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely inserted it
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));

            fileHandler.DeleteFile(fileDataFromDB);

            // We definitely deleted it
            Assert.IsFalse(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void DeleteFile_DeletesDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFileFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely inserted it
            Assert.IsNotNull(fileDataFromDB);

            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely deleted it
            Assert.IsNull(deletedFileDataFromDB);
        }

        [TestMethod]
        public void InsertAndCopy_InsertsDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "blaah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("blaah.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");
            var filesInDB = database.GetFiles(gameFile).ToList();

            Assert.AreEqual(1, filesInDB.Count());
            Assert.AreEqual(fileData.FileName, filesInDB[0].FileName);
        }

        [TestMethod]
        public void InsertAndCopy_InsertsFileOnDisk()
        {
            var fileHandler = new FileHandler(database, config);

            // Nothing up my sleeve
            var existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.IsFalse(existingTitlePics.Any());

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "blaah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("blaah.zip");

            // Insert a file as a TitlePic
            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            // Tada!
            existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.AreEqual(1, existingTitlePics.Count());

            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void DeleteFiles_DeletesAttachedFilesOfTheGivenType()
        {
            var fileHandler = new FileHandler(database, config);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Gah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Gah.zip");

            // 1 thumbnail, 2 screenshots
            var wrong = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var right1 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var right2 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            // Yep they're here
            var thumbnails = database.GetFiles(gameFile, FileType.Thumbnail);
            var screenshots = database.GetFiles(gameFile, FileType.Screenshot);

            Assert.AreEqual(1, thumbnails.Count());
            Assert.AreEqual(2, screenshots.Count());

            // Delete screenshots
            fileHandler.DeleteFiles(gameFile, FileType.Screenshot);

            // Thumbnail is still here, screenshots are gone now
            thumbnails = database.GetFiles(gameFile, FileType.Thumbnail);
            screenshots = database.GetFiles(gameFile, FileType.Screenshot);

            Assert.AreEqual(1, thumbnails.Count());
            Assert.AreEqual(0, screenshots.Count());
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
