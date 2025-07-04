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
    public class TestTitlePicHandler
    {
        private IDataSourceAdapter database;

        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            TitlePicDirectory = new LauncherPath("TitlePics")
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Directory.CreateDirectory("TitlePics");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("TitlePics"))
                Directory.Delete("TitlePics", true);

            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
        }

        [TestMethod]
        public void InsertTitlePic_NullGameFileFails()
        {
            var titlePicHandler = new TitlePicHandler(new FileHandler(database, config));
            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = titlePicHandler.InsertTitlePic(null, image);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertTitlePic_NullGameFileIdFails()
        {
            var titlePicHandler = new TitlePicHandler(new FileHandler(database, config));

            var gameFile = new GameFile()
            {
                GameFileID = null,
                FileName = "Foo.zip",
            };

            var image = Image.FromFile(@"Resources\happy.png");
            var fileData = titlePicHandler.InsertTitlePic(gameFile, image);

            Assert.IsNull(fileData);
        }

        [TestMethod]
        public void InsertTitlePic_CreatesFileAndDatabaseEntry()
        {
            var titlePicHandler = new TitlePicHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Boo.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Boo.zip");

            var image = Image.FromFile(@"Resources\happy.png");

            var fileData = titlePicHandler.InsertTitlePic(gameFile, image);
            var fileDataFromDB = database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();

            Assert.IsNotNull(fileData);
            Assert.IsNotNull(fileDataFromDB);
            Assert.AreEqual(fileData.FileName, fileDataFromDB.FileName);
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData.FileName)));
        }

        [TestMethod]
        public void InsertTitlePic_AlwaysDeletesThePreviousOne()
        {
            var titlePicHandler = new TitlePicHandler(new FileHandler(database, config));

            // Save a game file
            IGameFile gameFile = new GameFile() { FileName = "Blah.zip" };
            database.InsertGameFile(gameFile);
            gameFile = database.GetGameFile("Blah.zip");

            var image = Image.FromFile(@"Resources\happy.png");

            var fileData1 = titlePicHandler.InsertTitlePic(gameFile, image);
            var fileData2 = titlePicHandler.InsertTitlePic(gameFile, image);
            var filesFromDB = database.GetFiles(gameFile, FileType.TitlePic).ToList();
            
            Assert.IsNotNull(fileData1);
            Assert.IsNotNull(fileData2);
            Assert.AreNotEqual(fileData1.FileName, fileData2.FileName);
            Assert.AreEqual(1, filesFromDB.Count);
            Assert.AreEqual(fileData2.FileName, filesFromDB.FirstOrDefault()?.FileName);
            Assert.IsTrue(File.Exists(config.TitlePicDirectory.GetFullPath(fileData2.FileName)));
            Assert.IsFalse(File.Exists(config.TitlePicDirectory.GetFullPath(fileData1.FileName)));
        }
    }
}
