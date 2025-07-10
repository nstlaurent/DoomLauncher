using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestThumbnailManager
    {
        private IDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            ThumbnailDirectory = new LauncherPath("Thumbnails"),
        };

        //private IFileHandler fileHandler;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("Thumbnails");
            //fileHandler = new FileHandler(database, config);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Thumbnails"))
                Directory.Delete("Thumbnails", true);

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Files");
        }

        [TestMethod]
        public void GetOrCreateThumbnail_ReturnsThumbnailThatAlreadyExists()
        {
            var thumbnailManager = new ThumbnailManager(database, config);

            IGameFile gameFile = new GameFile() 
            { 
                GameFileID = 111,
                FileName = "GetOrCreateThumbnail_ReturnsThumbnailThatAlreadyExists.zip"
            };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("GetOrCreateThumbnail_ReturnsThumbnailThatAlreadyExists.zip");

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
    }
}
