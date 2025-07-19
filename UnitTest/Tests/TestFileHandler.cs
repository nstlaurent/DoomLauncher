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
            TileImageDirectory = new LauncherPath("TileImagesTest"),
            DemoDirectory = new LauncherPath("Demos"),
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("Screenshots");
            Directory.CreateDirectory("Thumbnails");
            Directory.CreateDirectory("TitlePics");
            Directory.CreateDirectory("TileImagesTest");
            Directory.CreateDirectory("Demos");
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

            if (Directory.Exists("Demos"))
                Directory.Delete("Demos", true);

            if (Directory.Exists("TileImagesTest"))
                Directory.Delete("TileImagesTest", true);

            if (File.Exists(@"Resources\happy_DELETE_ME.png"))
                File.Delete(@"Resources\happy_DELETE_ME.png");

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void InsertFileFromMemory_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(null, FileType.Thumbnail, imageStream, "png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertFileFromMemory_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertFileFromMemory_NullGameFileIdFails.zip",
            };

            var imageStream = GetImageStream(@"Resources\happy.png");
            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertFileFromMemory_CreatesFile()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertFileFromMemory_CreatesFile.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png");

            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void InsertFileFromMemory_CreatesDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertFileFromMemory_CreatesDatabaseEntry.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
        }

        [TestMethod]
        public void InsertFileFromMemory_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertFileFromMemory_AppliesEdits.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png", x =>
            {
                x.Description = "Hello";
                x.SourcePortID = 444;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();


            Assert.AreEqual("Hello", fileData.Description);
            Assert.AreEqual(444, fileData.SourcePortID);
            Assert.AreEqual("Hello", fileDataFromDB.Description);
            Assert.AreEqual(444, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void DeleteFile_DeletesFileOnDisk()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DeletesFileOnDisk.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
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
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DeletesDatabaseEntry.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely inserted it in DB and on disk
            Assert.IsNotNull(fileDataFromDB);
            Assert.IsTrue(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));

            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            // We definitely deleted it in DB and on disk
            Assert.IsNull(deletedFileDataFromDB);
            Assert.IsFalse(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));
        }

        [TestMethod]
        public void DeleteFile_DoesntDeleteFixedContentOnDisk()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DoesntDeleteFixedContent.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.TileImage, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();

            // This test doesnt make sense unless TileImages are fixed content
            Assert.IsTrue(FileType.TileImage.IsFixedContent());

            // We definitely inserted it
            Assert.IsNotNull(fileDataFromDB);
            Assert.IsTrue(File.Exists($@"TileImagesTest\{fileDataFromDB.FileName}"));

            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();

            // We deleted the DB record...
            Assert.IsNull(deletedFileDataFromDB);

            //... but not the file on disk.
            Assert.IsTrue(File.Exists($@"TileImagesTest\{fileDataFromDB.FileName}"));
        }

        [TestMethod]
        public void DeleteFile_DeletesDerivedFilesToo()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFile_DeletesDerivedFilesToo.zip");
            var imageStream = GetImageStream(@"Resources\happy.png");

            var fileData = fileHandler.InsertFromMemory(gameFile, FileType.Screenshot, imageStream, "png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();

            var derivedFileData = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png", file => 
                {
                    file.DerivedFromFileID = fileDataFromDB.FileID;
                });
            var derivedFileDataFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            // We definitely inserted both in DB and on disk
            Assert.IsNotNull(fileDataFromDB);
            Assert.IsTrue(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));
            Assert.IsNotNull(derivedFileData);
            Assert.IsTrue(File.Exists($@"Thumbnails\{derivedFileDataFromDB.FileName}"));

            // Delete only the first one
            fileHandler.DeleteFile(fileDataFromDB);
            var deletedFileDataFromDB = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();
            var derivedDeletedFileDataFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            // We definitely deleted both in DB and on disk
            Assert.IsNull(deletedFileDataFromDB);
            Assert.IsFalse(File.Exists($@"Screenshots\{fileDataFromDB.FileName}"));
            Assert.IsNull(derivedDeletedFileDataFromDB);
            Assert.IsFalse(File.Exists($@"Thumbnails\{derivedFileDataFromDB.FileName}"));
        }

        [TestMethod]
        public void InsertAndCopy_InsertsDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_InsertsDatabaseEntry.zip");

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

            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_InsertsFileOnDisk.zip");

            // Insert a file as a TitlePic
            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            // Tada!
            existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.AreEqual(1, existingTitlePics.Count());
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));

            // Source image is still around
            Assert.IsTrue(File.Exists(@"Resources\happy.png"));
        }

        [TestMethod]
        public void InsertAndCopy_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            var fileData = fileHandler.InsertAndCopy(null, FileType.TitlePic, @"Resources\happy.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndCopy_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertAndCopy_NullGameFileIdFails.zip",
            };

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndCopy_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_AppliesEdits.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png", x =>
            {
                x.Description = "Hi";
                x.SourcePortID = 433;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();


            Assert.AreEqual("Hi", fileData.Description);
            Assert.AreEqual(433, fileData.SourcePortID);
            Assert.AreEqual("Hi", fileDataFromDB.Description);
            Assert.AreEqual(433, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void InsertAndCopy_DoesntCopyFileThatIsntThere()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndCopy_DoesntCopyFileThatIsntThere.zip");

            var fileData = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\made-up-file.png");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();

            Assert.IsNull(fileData);
            Assert.IsNull(fileDataFromDB);
            Assert.IsFalse(fileHandler.GetFileInfo(FileType.TitlePic, @"Resources\made-up-file.png").Exists);
        }

        [TestMethod]
        public void InsertAndMove_InsertsDatabaseEntry()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_InsertsDatabaseEntry.zip");

            // We don't want to lose our normal copy!
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            Assert.IsTrue(File.Exists(@"Resources\happy_DELETE_ME.png"));

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");
            var filesInDB = database.GetFiles(gameFile).ToList();

            Assert.AreEqual(1, filesInDB.Count());
            Assert.AreEqual(fileData.FileName, filesInDB[0].FileName);
        }

        [TestMethod]
        public void InsertAndMove_InsertsFileOnDiskAndDeletesOldFile()
        {
            var fileHandler = new FileHandler(database, config);

            // Nothing up my sleeve
            var existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.IsFalse(existingTitlePics.Any());

            // We don't want to lose our normal copy!
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            Assert.IsTrue(File.Exists(@"Resources\happy_DELETE_ME.png"));

            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_InsertsFileOnDiskAndDeletesOldFile.zip");

            // Insert a file as a TitlePic
            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            // Tada!
            existingTitlePics = Directory.EnumerateFiles(config.TitlePicDirectory.GetFullPath());
            Assert.AreEqual(1, existingTitlePics.Count());
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));

            // Source image is gone now
            Assert.IsFalse(File.Exists(@"Resources\happy_DELETE_ME.png"));
        }

        [TestMethod]
        public void InsertAndMove_NullGameFileFails()
        {
            var fileHandler = new FileHandler(database, config);
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            var fileData = fileHandler.InsertAndMove(null, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndMove_NullGameFileIdFails()
        {
            var fileHandler = new FileHandler(database, config);

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "InsertAndMove_NullGameFileIdFails.zip",
            };

            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");
            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png");

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertAndMove_AppliesEdits()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_AppliesEdits.zip");
            File.Copy(@"Resources\happy.png", @"Resources\happy_DELETE_ME.png");

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.TitlePic, @"Resources\happy_DELETE_ME.png", x =>
            {
                x.Description = "good";
                x.SourcePortID = 665;
            });
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();


            Assert.AreEqual("good", fileData.Description);
            Assert.AreEqual(665, fileData.SourcePortID);
            Assert.AreEqual("good", fileDataFromDB.Description);
            Assert.AreEqual(665, fileDataFromDB.SourcePortID);
        }

        [TestMethod]
        public void InsertAndMove_DoesntMoveFileThatIsntThere()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("InsertAndMove_DoesntMoveFileThatIsntThere.zip");

            var fileData = fileHandler.InsertAndMove(gameFile, FileType.Demo, @"Resources\made-up-file.demo");
            var fileDataFromDB = database.GetFiles(gameFile, FileType.Demo).FirstOrDefault();

            Assert.IsNull(fileData);
            Assert.IsNull(fileDataFromDB);
            Assert.IsFalse(fileHandler.GetFileInfo(FileType.Demo, @"Resources\made-up-file.demo").Exists);
        }

        [TestMethod]
        public void DeleteFiles_DeletesAttachedFilesOfTheGivenType()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("DeleteFiles_DeletesAttachedFilesOfTheGivenType.zip");

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

        [TestMethod]
        public void GetFiles_ReturnsFilesFromTheDatabase()
        {
            var fileHandler = new FileHandler(database, config);
            IGameFile gameFile = CreateSavedGameFile("GetFiles_ReturnsFilesFromTheDatabase.zip");

            // 1 thumbnail, 2 screenshots
            var wrong = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var right1 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var right2 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            var fileDataList = fileHandler.GetFiles(gameFile, FileType.Screenshot);
            Assert.AreEqual(2, fileDataList.Count());
        }

        private IGameFile CreateSavedGameFile(string fileName)
        {
            IGameFile gameFile = new GameFile() { FileName = fileName };
            database.InsertGameFile(gameFile);
            return database.GetGameFile(fileName);
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
