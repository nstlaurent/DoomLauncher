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
            TileImageDirectory = new LauncherPath("TileImagesTest")
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("TitlePics");
            Directory.CreateDirectory("Thumbnails");
            Directory.CreateDirectory("TileImagesTest");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("TitlePics"))
                Directory.Delete("TitlePics", true);

            if (Directory.Exists("Thumbnails"))
                Directory.Delete("Thumbnails", true);

            if (Directory.Exists("TileImagesTest"))
                Directory.Delete("TileImagesTest", true);

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
            gameFile = database.GetGameFile("Flahg.zip");

            // No Thumbnails exist yet
            var thumbnailFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();
            Assert.IsNull(thumbnailFromDB);

            var image = Image.FromFile(@"Resources\happy.png");

            var titlePic = gameFileImageHandler.InsertTitlePic(gameFile, image);
            var titlePicFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();
            Assert.IsNotNull(titlePicFromDB);

            thumbnailFromDB = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();
            Assert.IsNotNull(titlePicFromDB);
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
    }
}
