using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.IO;
using System.Linq;


namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameFileImageHandler
    {
        private IDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            TitlePicDirectory = new LauncherPath("TitlePics"),
            ThumbnailDirectory = new LauncherPath("Thumbnails"),
            ScreenshotDirectory = new LauncherPath("Screenshots"),
            TileImageDirectory = new LauncherPath("TileImagesTest")
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("TitlePics");
            Directory.CreateDirectory("Thumbnails");
            Directory.CreateDirectory("Screenshots");
            Directory.CreateDirectory("TileImagesTest");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("TitlePics"))
                Directory.Delete("TitlePics", true);

            if (Directory.Exists("Thumbnails"))
                Directory.Delete("Thumbnails", true);

            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);

            if (Directory.Exists("TileImagesTest"))
                Directory.Delete("TileImagesTest", true);

            if (File.Exists(@"Resources\happy_DELETE_ME.png"))
                File.Delete(@"Resources\happy_DELETE_ME.png");

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void InsertTitlePic_NullGameFileFails()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config));
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = gameFileImageHandler.InsertTitlePic(null, image);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertTitlePic_NullGameFileIdFails()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config));

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "Foo.zip",
            };

            var image = Image.FromFile(@"Resources\happy.png");
            var fileData = gameFileImageHandler.InsertTitlePic(gameFile, image);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertTitlePic_CreatesFileAndDatabaseEntry()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = gameFileImageHandler.InsertTitlePic(gameFile, image);
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();

            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void InsertTitlePic_AlwaysDeletesThePreviousOne()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Blah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Blah.zip");

            var image = Image.FromFile(@"Resources\happy.png");

            var fileData1 = gameFileImageHandler.InsertTitlePic(gameFile, image);
            var fileData2 = gameFileImageHandler.InsertTitlePic(gameFile, image);
            var filesFromDB = database.GetFiles(gameFile, FileType.TitlePic).ToList();
            
            Assert.IsNotNull(fileData1);
            Assert.IsNotNull(fileData2);
            Assert.AreNotEqual(fileData1.FileName, fileData2.FileName);
            Assert.AreEqual(1, filesFromDB.Count);
            Assert.AreEqual(fileData2.FileName, filesFromDB.FirstOrDefault()?.FileName);
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData2.FileName)));
            Assert.IsFalse(File.Exists(config.TitlePicDirectory.GetFullPath(fileData1.FileName)));
        }

        [TestMethod]
        public void InsertTitlePic_InsertsThumbnailIfSuccessful()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Flahg.zip" };
            database.InsertGameFile(gameFile);

            // No Thumbnails exist yet
            var thumbnailFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();
            Assert.IsNull(thumbnailFromDB);

            var image = Image.FromFile(@"Resources\happy.png");

            var titlePic = gameFileImageHandler.InsertTitlePic(gameFile, image);
            Assert.IsNotNull(titlePic);

            thumbnailFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();
            Assert.IsNotNull(titlePic);
            Assert.AreEqual(titlePic.FileID, thumbnailFromDB.DerivedFromFileID);
        }

        
        [TestMethod]
        public void InsertTitlePic_DeletesTileImagesIfSuccessful()
        {
            var fileHandler = new FileHandler(database, config);
            var gameFileImageHandler = new GameFileImageHandler(fileHandler);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Grah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Grah.zip");

            // Successfully create a TileImage
            var theRightLocation = config.TileImageDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));
            fileHandler.InsertAndRefer(gameFile, FileType.TileImage, theRightLocation);
            var tileImage = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();
            Assert.IsNotNull(tileImage);

            var image = Image.FromFile(@"Resources\happy.png");
            var titlePic = gameFileImageHandler.InsertTitlePic(gameFile, image);
            var titlePicFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();
            Assert.IsNotNull(titlePicFromDB);

            // TileImage no longer exists
            tileImage = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();
            Assert.IsNull(tileImage);
        }

        [TestMethod]
        public void InsertScreenshot_CopiesFiletoDiskPreservingSource()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config), false);

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

            // Insert a file as a screenshot
            var fileData = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            Assert.IsNotNull(fileData);

            // Tada!
            existingScreenshots = Directory.EnumerateFiles(config.ScreenshotDirectory.GetFullPath());
            Assert.AreEqual(1, existingScreenshots.Count());
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));

            // Source image is still around
            Assert.IsTrue(File.Exists(@"Resources\happy.png"));
        }

        [TestMethod]
        public void InsertScreenshot_InsertsDatabaseEntry()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config), false);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 536
            };

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "blaah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("blaah.zip");

            var fileData = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            var filesInDB = database.GetFiles(gameFile, FileType.Screenshot).ToList();

            Assert.IsNotNull(fileData);
            Assert.AreEqual(1, filesInDB.Count());
            Assert.AreEqual(536, fileData.SourcePortID);
            Assert.AreEqual(fileData.FileName, filesInDB[0].FileName);
        }

        [TestMethod]
        public void InsertScreenshot_DeletesOldScreenshotIfConfigTellsItTo()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config), true);

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

            var fileData = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy_DELETE_ME.png");

            Assert.IsNotNull(fileData);
            Assert.IsTrue(File.Exists(config.ScreenshotDirectory.GetFullPath(fileData.FileName)));
            Assert.IsFalse(File.Exists(@"Resources\happy_DELETE_ME.png"));
        }

        [TestMethod]
        public void InsertScreenshot_InsertsThumbnailIfNoneExists()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config), false);

            var sourcePort = new SourcePortData()
            {
                SourcePortID = 536
            };

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "derp.zip" };
            database.InsertGameFile(gameFile);

            // Prove we are starting with no thumbnails
            var thumbnailsInDB = database.GetFiles(gameFile, FileType.Thumbnail).ToList();
            Assert.AreEqual(0, thumbnailsInDB.Count());

            var fileData = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            Assert.IsNotNull(fileData);

            thumbnailsInDB = database.GetFiles(gameFile, FileType.Thumbnail).ToList();
            Assert.AreEqual(1, thumbnailsInDB.Count());
            Assert.AreEqual(fileData.FileID, thumbnailsInDB[0].DerivedFromFileID);
        }

        [TestMethod]
        public void InsertScreenshot_DoesntInsertThumbnailIfOneAlreadyExists()
        {
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config), false);

            var sourcePort = new SourcePortData() { SourcePortID = 222 };

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "derp.zip" };
            database.InsertGameFile(gameFile);

            var existingThumbnail = new FileData()
            {
                FileTypeID = FileType.Thumbnail,
                GameFileID = gameFile.GameFileID.Value,
                FileName = "blah.txt"
            };
            database.InsertFile(existingThumbnail);

            // Prove we are starting with a thumbnail
            var thumbnailsInDB = database.GetFiles(gameFile, FileType.Thumbnail).ToList();
            Assert.AreEqual(1, thumbnailsInDB.Count());

            var fileData = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            Assert.IsNotNull(fileData);

            thumbnailsInDB = database.GetFiles(gameFile, FileType.Thumbnail).ToList();
            Assert.AreEqual(1, thumbnailsInDB.Count());

            // Check it's definitely the original one
            Assert.IsNull(thumbnailsInDB[0].DerivedFromFileID);
            Assert.AreEqual(existingThumbnail.FileID, thumbnailsInDB[0].FileID);
        }

        [TestMethod]
        public void InsertScreenshot_DeletesTileImagesIfSuccessful()
        {
            var fileHandler = new FileHandler(database, config);
            var gameFileImageHandler = new GameFileImageHandler(fileHandler);
            var sourcePort = new SourcePortData() { SourcePortID = 123 };


            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Grah.zip" };
            database.InsertGameFile(gameFile);

            // Successfully create a TileImage
            var theRightLocation = config.TileImageDirectory.GetFullPath("happy.png");
            File.Copy(@"Resources\happy.png", theRightLocation);
            Assert.IsTrue(File.Exists(theRightLocation));
            var tileImage = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, theRightLocation);
            Assert.IsNotNull(tileImage);

            var screenshot = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            Assert.IsNotNull(screenshot);

            // TileImage no longer exists
            tileImage = database.GetFiles(gameFile, FileType.TileImage).FirstOrDefault();
            Assert.IsNull(tileImage);
        }
    }
}
