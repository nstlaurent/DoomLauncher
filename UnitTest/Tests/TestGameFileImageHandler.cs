using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameFileImageHandler
    {
        private IDataSourceAdapter database;
        private IFileHandler fileHandler;

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
            fileHandler = new FileHandler(database, config);
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
            dataAccess.ExecuteNonQuery("delete from IWads");
        }

        [TestMethod]
        public void GetMainImageLarge_PrefersTitlePics()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile() { FileName = "GetMainImageLarge_PrefersTitlePics.zip" };
            database.InsertGameFile(gameFile);

            var screenshot = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var titlePic = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");
            var tileImage = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, @"Resources\happy.png");

            string mainImage = gameFileImageHandler.GetMainImageLarge(gameFile).FullFileName;

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(titlePic.FileName));
        }

        [TestMethod]
        public void GetMainImageLarge_PrefersScreenshotsOverTileImages()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile() { FileName = "GetMainImageLarge_PrefersScreenshotsOverTileImages.zip" };
            database.InsertGameFile(gameFile);

            var tileImage = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, @"Resources\happy.png");
            var screenshot = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            string mainImage = gameFileImageHandler.GetMainImageLarge(gameFile).FullFileName;

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(screenshot.FileName));
        }

        [TestMethod]
        public void GetMainImageLarge_NewTileImageRespectsSelectedIWadOverIntendedGame()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            // Heretic IWAD
            IIWadData hereticIWad = new IWadData() { FileName = "heretic.zip" };
            database.InsertIWad(hereticIWad);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "GetMainImageLarge_NewTileImageRespectsSelectedIWadOverIntendedGame.zip",
                IWadID = hereticIWad.IWadID,
                IntendedGame = IWadInfo.Plutonia
            };
            database.InsertGameFile(gameFile);

            // Heretic IWAD tile image
            var hereticTileImagePath = config.TileImageDirectory.GetFullPath("heretic.png");
            File.Copy(@"Resources\happy.png", hereticTileImagePath, true);
            Assert.IsTrue(File.Exists(hereticTileImagePath));

            string mainImage = gameFileImageHandler.GetMainImageLarge(gameFile).FullFileName;

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains("heretic.png"));
        }

        [TestMethod]
        public void GetMainImageLarge_NewTileImageRespectsIntendedGame()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "GetMainImageLarge_NewTileImageRespectsIntendedGame.zip",
                IntendedGame = IWadInfo.Hexen
            };
            database.InsertGameFile(gameFile);

            var hexenTileImagePath = config.TileImageDirectory.GetFullPath("hexen.png");
            File.Copy(@"Resources\happy.png", hexenTileImagePath, true);
            Assert.IsTrue(File.Exists(hexenTileImagePath));

            string mainImage = gameFileImageHandler.GetMainImageLarge(gameFile).FullFileName;

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains("hexen.png"));
        }

        [TestMethod]
        public void GetMainImageLarge_NewTileImagePicksDefaultImageIfAllElseFails()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile()
            {
                FileName = "GetMainImageLarge_NewTileImagePicksDefaultImageIfAllElseFails.zip",
            };
            database.InsertGameFile(gameFile);

            var defaultImagePath = config.TileImageDirectory.GetFullPath(GameFileImageHandler.DEFAULT_TILE_IMAGE);
            File.Copy(@"Resources\happy.png", defaultImagePath, true);
            Assert.IsTrue(File.Exists(defaultImagePath));

            string mainImage = gameFileImageHandler.GetMainImageLarge(gameFile).FullFileName;

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(defaultImagePath));
        }

        [TestMethod]
        public void GetMainImageLarge_PicksDefaultImageIfGameFileIsNotInDB()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "not_in_db.zip",
            };

            var defaultImagePath = config.TileImageDirectory.GetFullPath(GameFileImageHandler.DEFAULT_TILE_IMAGE);
            File.Copy(@"Resources\happy.png", defaultImagePath, true);
            Assert.IsTrue(File.Exists(defaultImagePath));

            string mainImage = GetMainImageSmall(gameFileImageHandler, gameFile);

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(defaultImagePath));
        }

        [TestMethod]
        public void GetMainImageSmall_PrefersThumbnails()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile() { FileName = "GetMainImageSmall_PrefersThumbnails.zip" };
            database.InsertGameFile(gameFile);

            var titlePic = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");
            var screenshot = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var thumbnail = fileHandler.InsertAndCopy(gameFile, FileType.Thumbnail, @"Resources\happy.png");
            var tileImage = fileHandler.InsertAndRefer(gameFile, FileType.TileImage, @"Resources\happy.png");

            string mainImage = GetMainImageSmall(gameFileImageHandler, gameFile);

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(thumbnail.FileName));
        }

        [TestMethod]
        public void GetMainImageSmall_NewTileImageRespectsSelectedIWadOverIntendedGame()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            // TNT IWAD
            IIWadData tntIWad = new IWadData() { FileName = "tnt.zip" };
            database.InsertIWad(tntIWad);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "GetMainImageSmall_NewTileImageRespectsSelectedIWadOverIntendedGame.zip",
                IWadID = tntIWad.IWadID,
                IntendedGame = IWadInfo.Doom64
            };
            database.InsertGameFile(gameFile);

            // TNT IWAD tile image
            var tntTileImagePath = config.TileImageDirectory.GetFullPath("tnt.png");
            File.Copy(@"Resources\happy.png", tntTileImagePath, true);
            Assert.IsTrue(File.Exists(tntTileImagePath));

            string mainImage = GetMainImageSmall(gameFileImageHandler, gameFile);

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains("tnt.png"));
        }


        [TestMethod]
        public void GetMainImageSmall_NewTileImageRespectsIntendedGame()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "GetMainImageSmall_NewTileImageRespectsIntendedGame.zip",
                IntendedGame = IWadInfo.Strife1
            };
            database.InsertGameFile(gameFile);

            var strifeTileImagePath = config.TileImageDirectory.GetFullPath("strife.png");
            File.Copy(@"Resources\happy.png", strifeTileImagePath, true);
            Assert.IsTrue(File.Exists(strifeTileImagePath));

            string mainImage = GetMainImageSmall(gameFileImageHandler, gameFile);

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains("strife.png"));
        }

        private string GetMainImageSmall(GameFileImageHandler handler, IGameFile gameFile)
        {
            var files = handler.GetImageFiles(new IGameFile[] { gameFile });
            return files[gameFile.GameFileID.Value].FirstOrDefault()?.FullFileName;
        }

        [TestMethod]
        public void GetMainImageSmall_NewTileImagePicksDefaultImageIfAllElseFails()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "GetMainImageSmall_NewTileImagePicksDefaultImageIfAllElseFails.zip",
            };
            database.InsertGameFile(gameFile);

            var defaultImagePath = config.TileImageDirectory.GetFullPath(GameFileImageHandler.DEFAULT_TILE_IMAGE);
            File.Copy(@"Resources\happy.png", defaultImagePath, true);
            Assert.IsTrue(File.Exists(defaultImagePath));

            string mainImage = GetMainImageSmall(gameFileImageHandler, gameFile);

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(defaultImagePath));
        }

        [TestMethod]
        public void GetMainImageSmall_PicksDefaultImageIfGameFileIsNotInDB()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile()
            {
                GameFileID = 1,
                FileName = "not_in_db.zip",
            };

            var defaultImagePath = config.TileImageDirectory.GetFullPath(GameFileImageHandler.DEFAULT_TILE_IMAGE);
            File.Copy(@"Resources\happy.png", defaultImagePath, true);
            Assert.IsTrue(File.Exists(defaultImagePath));

            string mainImage = GetMainImageSmall(gameFileImageHandler, gameFile);

            Assert.IsNotNull(mainImage);
            Assert.IsTrue(mainImage.Contains(defaultImagePath));
        }

        [TestMethod]
        public void GetMainImageAndScreenshots_IncludesTitlePicAndScreenshotsInOrder()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile() { FileName = "GetMainImageAndScreenshots_IncludesTitlePicAndScreenshots.zip" };
            database.InsertGameFile(gameFile);

            var screenshot1 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var titlePic = fileHandler.InsertAndCopy(gameFile, FileType.TitlePic, @"Resources\happy.png");
            var screenshot2 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            var list = gameFileImageHandler.GetMainImageAndScreenshots(gameFile);
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list[0].FullFileName.Contains(titlePic.FileName));
            Assert.IsTrue(list[1].FullFileName.Contains(screenshot1.FileName));
            Assert.IsTrue(list[2].FullFileName.Contains(screenshot2.FileName));
        }

        [TestMethod]
        public void GetMainImageAndScreenshots_DoesntDoubleUpScreenshots()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile() { FileName = "GetMainImageAndScreenshots_DoesntDoubleUpScreenshots.zip" };
            database.InsertGameFile(gameFile);

            var screenshot1 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");
            var screenshot2 = fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, @"Resources\happy.png");

            // Confirm the main image is the first screenshot
            Assert.IsTrue(gameFileImageHandler.GetMainImageLarge(gameFile).FullFileName.Contains(screenshot1.FileName));

            var list = gameFileImageHandler.GetMainImageAndScreenshots(gameFile);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list[0].FullFileName.Contains(screenshot1.FileName));
            Assert.IsTrue(list[1].FullFileName.Contains(screenshot2.FileName));
        }

        [TestMethod]
        public void GetMainImageAndScreenshots_ReturnsTileImageIfNoTitlePicOrScreenshots()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            IGameFile gameFile = new GameFile() { FileName = "GetMainImageAndScreenshots_ReturnsTileImageIfNoTitlePicOrScreenshots.zip" };
            database.InsertGameFile(gameFile);

            var defaultImagePath = config.TileImageDirectory.GetFullPath(GameFileImageHandler.DEFAULT_TILE_IMAGE);
            File.Copy(@"Resources\happy.png", defaultImagePath, true);
            Assert.IsTrue(File.Exists(defaultImagePath));

            var list = gameFileImageHandler.GetMainImageAndScreenshots(gameFile);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0].FullFileName.Contains(GameFileImageHandler.DEFAULT_TILE_IMAGE));
        }

        [TestMethod]
        public void InsertTitlePic_NullGameFileFails()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = gameFileImageHandler.InsertTitlePic(null, image);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertTitlePic_NullGameFileIdFails()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID);

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
        public void InsertScreenshot_CopiesFiletoDiskPreservingSource()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, true);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

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
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

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
        public void GetScreenshots_ReturnsTheScreenshots()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);
            IGameFile gameFile = new GameFile() { FileName = "BBB.zip" };
            database.InsertGameFile(gameFile);
            var sourcePort = new SourcePortData() { SourcePortID = 516 };

            var screenshot1 = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            var screenshot2 = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            var screenshot3 = gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");

            List<string> screenshots = gameFileImageHandler.GetScreenshots(gameFile).Select(file => file.FullFileName).ToList();

            Assert.AreEqual(3, screenshots.Count);
            Assert.IsTrue(screenshots.Exists(s => s.Contains(screenshot1.FileName)));
            Assert.IsTrue(screenshots.Exists(s => s.Contains(screenshot2.FileName)));
            Assert.IsTrue(screenshots.Exists(s => s.Contains(screenshot3.FileName)));
        }

        [TestMethod]
        public void UpdateImages_RecreatesThumbnailForExistingTitlepic()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

            IGameFile gameFile = new GameFile() { FileName = "Yellow.zip" };
            database.InsertGameFile(gameFile);

            gameFileImageHandler.InsertTitlePic(gameFile, Image.FromFile(@"Resources\happy.png"));

            var existingThumbnail = fileHandler.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();
            gameFileImageHandler.UpdateImages(gameFile);

            var newThumbnails = fileHandler.GetFiles(gameFile, FileType.Thumbnail);
            Assert.AreEqual(1, newThumbnails.Count);
            Assert.AreNotEqual(existingThumbnail.FileID, newThumbnails[0].FileID);
        }

        [TestMethod]
        public void UpdateImages_DeletesExistingThumbnailWithNoMainImage()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

            IGameFile gameFile = new GameFile() { FileName = "Yellow.zip" };
            database.InsertGameFile(gameFile);

            var thumbnail = fileHandler.InsertAndSave(gameFile, FileType.Thumbnail, Image.FromFile(@"Resources\happy.png"), "png");
            var existingThumbnails = fileHandler.GetFiles(gameFile, FileType.Thumbnail);
            Assert.AreEqual(1, existingThumbnails.Count);

            gameFileImageHandler.UpdateImages(gameFile);

            var newThumbnails = fileHandler.GetFiles(gameFile, FileType.Thumbnail);
            Assert.AreEqual(0, newThumbnails.Count);
        }

        [TestMethod]
        public void UpdateImages_RecreatesThumbnailForExistingScreenshot()
        {
            var gameFileImageHandler = new GameFileImageHandler(fileHandler, database.GetIWadByIWadID, false);

            IGameFile gameFile = new GameFile() { FileName = "Yellow.zip" };
            database.InsertGameFile(gameFile);
            var sourcePort = new SourcePortData() { SourcePortID = 947 };
            gameFileImageHandler.InsertScreenshot(sourcePort, gameFile, @"Resources\happy.png");
            var existingThumbnail = fileHandler.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            gameFileImageHandler.UpdateImages(gameFile);

            var newThumbnails = fileHandler.GetFiles(gameFile, FileType.Thumbnail);
            Assert.AreEqual(1, newThumbnails.Count);
            Assert.AreNotEqual(existingThumbnail.FileID, newThumbnails[0].FileID);
        }
    }
}
