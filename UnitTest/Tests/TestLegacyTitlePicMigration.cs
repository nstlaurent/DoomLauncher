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
    public class TestLegacyTitlePicMigration
    {
        private static readonly string s_screenshotDir = "TestScreenshotDir";
        private static readonly string s_titlePicDir = "TestTitlePicDir";
        private static readonly string s_imageFileName = "eviternity-titlepic.png";
        private IDataSourceAdapter database;
        private IFileData m_fileData;
        private IGameFile m_gameFile;
        private IFileHandler m_fileHandler;

        private readonly IDirectoriesConfiguration m_config = new DirectoriesConfiguration()
        {
            ScreenshotDirectory = new LauncherPath(s_screenshotDir),
            TitlePicDirectory = new LauncherPath(s_titlePicDir),
        };

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            Cleanup();
            
            Directory.CreateDirectory(s_screenshotDir);
            Directory.CreateDirectory(s_titlePicDir);

            
            var imagePath = Path.Combine(s_screenshotDir, s_imageFileName);
            File.Copy(Path.Combine("Resources", s_imageFileName), imagePath);
            Assert.IsTrue(File.Exists(imagePath));

            m_gameFile = new GameFile { FileName = "foo.wad" };
            database.InsertGameFile(m_gameFile);

            m_fileData = new FileData
            {
                FileName = s_imageFileName,
                FileTypeID = FileType.Screenshot,
                GameFileID = (int)m_gameFile.GameFileID,
            };
            database.InsertFile(m_fileData);

            m_fileHandler = new FileHandler(database, m_config);
        }


        [TestCleanup]
        public void Cleanup()
        {
            if (m_fileData != null)
                database.DeleteFile(m_fileData);
            if (m_gameFile != null)
                database.DeleteGameFile(m_gameFile);

            if (Directory.Exists(s_screenshotDir))
                Directory.Delete(s_screenshotDir, true);
        }

        [TestMethod]
        public void DeleteScreenshotThatIsReallyATitlePic_DeletesTitlePicScreenshot()
        {

            var palette = DataCache.Instance.DefaultPalette;

            var screenshotImage = Path.Combine(s_screenshotDir, s_imageFileName);

            bool anyDeleted = false;
            using (var titlePicImage = Image.FromFile(screenshotImage))
            {
                anyDeleted = LegacyTitlePicMigration.DeleteScreenshotThatIsReallyATitlePic(m_fileHandler, m_gameFile, titlePicImage);
            }

            Assert.IsTrue(anyDeleted);
            Assert.IsFalse(database.GetFiles().Any());
            Assert.IsFalse(File.Exists(screenshotImage));
        }
    }
}
