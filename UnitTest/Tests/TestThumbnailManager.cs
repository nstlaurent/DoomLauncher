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
    public class TestThumbnailManager
    {
        private DbDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            ThumbnailDirectory = new LauncherPath("Thumbnails"),
            TitlePicDirectory = new LauncherPath("TitlePics"),
            ScreenshotDirectory = new LauncherPath("Screenshots")
        };

        //private IFileHandler fileHandler;

        [TestInitialize]
        public void Initialize()
        {
            database = (DbDataSourceAdapter)TestUtil.CreateAdapter();
            Directory.CreateDirectory("Thumbnails");
            Directory.CreateDirectory("TitlePics");
            Directory.CreateDirectory("Screenshots");
            //fileHandler = new FileHandler(database, config);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Thumbnails"))
                Directory.Delete("Thumbnails", true);

            if (Directory.Exists("TitlePics"))
                Directory.Delete("TitlePics", true);

            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);

            var dataAccess = database.DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from IWads");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void GetOrCreateThumbnail_ReturnsThumbnailThatAlreadyExists()
        {
            var thumbnailManager = new ThumbnailManager(database, config);

            IGameFile gameFile = new GameFile()
            {
                FileName = "GetOrCreateThumbnail_ReturnsThumbnailThatAlreadyExists.zip"
            };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile(gameFile.FileName);

            IFileData thumbnail = new FileData()
            {
                GameFileID = gameFile.GameFileID ?? 0,
                FileTypeID = FileType.Thumbnail,
                FileName = "blah.png"
            };
            database.InsertFile(thumbnail);
            thumbnail = database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            var fileData = thumbnailManager.GetOrCreateThumbnail(gameFile);
            Assert.AreEqual(thumbnail.FileID, fileData.FileID);
            Assert.AreEqual("blah.png", fileData.FileName);

        }

        public void GetOrCreateThumbnail_UsesTitlePicIfThere()
        {
            var thumbnailManager = new ThumbnailManager(database, config);
            var gameFileImageHandler = new GameFileImageHandler(new FileHandler(database, config));

            IGameFile gameFile = new GameFile()
            {
                FileName = "GetOrCreateThumbnail_UsesTitlePicIfThere.zip"
            };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile(gameFile.FileName);

            var titlePic = gameFileImageHandler.InsertTitlePic(gameFile, Image.FromFile(@"Resources\happy.png")); 
            titlePic = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();

            var thumbnail = thumbnailManager.GetOrCreateThumbnail(gameFile);

            Assert.IsNotNull(thumbnail);
            Assert.IsFalse(string.IsNullOrEmpty(thumbnail.FileName));
            Assert.IsFalse(ThumbnailManager.IsTileImage(thumbnail));
            Assert.AreEqual(titlePic.FileID, thumbnail.DerivedFromFileID);
        }

        [TestMethod]
        public void GetOrCreateThumbnail_UsesScreenshotIfThere()
        {
            var thumbnailManager = new ThumbnailManager(database, config);
            var screenshotHandler = new ScreenshotHandler(new FileHandler(database, config), false);

            IGameFile gameFile = new GameFile()
            {
                FileName = "GetOrCreateThumbnail_UsesScreenshotIfThere.zip"
            };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile(gameFile.FileName);

            ISourcePortData sourcePort = new SourcePortData() { SourcePortID = 99 };

            var screenshot = screenshotHandler.HandleNewScreenshots(sourcePort, gameFile, new string[] { @"Resources\happy.png" }).FirstOrDefault();
            screenshot = database.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();
            
            var thumbnail = thumbnailManager.GetOrCreateThumbnail(gameFile);

            Assert.IsNotNull(thumbnail);
            Assert.IsFalse(string.IsNullOrEmpty(thumbnail.FileName));
            Assert.IsFalse(ThumbnailManager.IsTileImage(thumbnail));
            Assert.AreEqual(screenshot.FileID, thumbnail.DerivedFromFileID); 
        }

        [TestMethod]
        public void GetOrCreateThumbnail_UsesIWadTileImageIfThere()
        {
            var thumbnailManager = new ThumbnailManager(database, config);

            IGameFile gameFile = new GameFile() { FileName = "GetOrCreateThumbnail_UsesIWadTileImageIfThere.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile(gameFile.FileName);

            InsertIWadAndGameFile(IWadInfo.DOOM, out var iwad, out var iwadGameFile);
            gameFile.IWadID = iwad.IWadID;
            database.UpdateGameFile(gameFile);

            ISourcePortData sourcePort = new SourcePortData() { SourcePortID = 99 };

            var thumbnail = thumbnailManager.GetOrCreateThumbnail(gameFile);

            Assert.IsNotNull(thumbnail);
            Assert.AreEqual("doom.png", Path.GetFileName(thumbnail.FileName));
            Assert.IsTrue(ThumbnailManager.IsTileImage(thumbnail));
        }

        private void InsertIWadAndGameFile(IWadInfo info, out IIWadData iwad, out IGameFile iwadGameFile)
        {
            var iwadFileName = $"{info.GameName.ToLower()}.zip";
            iwadGameFile = new GameFile() { FileName = iwadFileName };
            database.InsertGameFile(iwadGameFile);
            iwadGameFile = database.GetGameFile(iwadGameFile.FileName);

            iwad = new IWadData() 
            { 
                GameFileID = iwadGameFile.GameFileID,
                FileName = iwadFileName
            };
            database.InsertIWad(iwad);
            iwad = database.GetIWad(iwadGameFile.GameFileID.Value);

            iwadGameFile.IWadID = iwad.IWadID;
            database.UpdateGameFile(iwadGameFile);
        }

        // Test IntendedIwad

        // Test DefaultImage

        // Test true IWad

        // Get rid of all the hardcoded IDs in the various tests
    }
}
