using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestLegacyTitlePicMigration
    {
        private static readonly string s_gameFileDir = "TestGameFileDir";
        private static readonly string s_tempDir = "TestDirTemp";
        private static readonly string s_screenshotDir = "TestScreenshotDir";
        private static readonly string s_titlePicDir = "TestTitlePicDir";
        private static readonly string s_tileImagesDir = "TestTileImagesDir";
        private static readonly string s_thumbnailsDir = "TestThumbnailsDir";

        private IDataSourceAdapter database;
        private IGameFile m_gameFile;

        private IDirectoriesConfiguration m_config = new DirectoriesConfiguration
        {
            GameFileDirectory = new LauncherPath(s_gameFileDir),
            TempDirectory = new LauncherPath(s_tempDir),
            ScreenshotDirectory = new LauncherPath(s_screenshotDir),
            TitlePicDirectory = new LauncherPath(s_titlePicDir),
            TileImageDirectory = new LauncherPath(s_tileImagesDir),
            ThumbnailDirectory = new LauncherPath(s_thumbnailsDir)
        };

        private IFileHandler m_fileHandler;



        [TestInitialize]
        public void Init()
        {
            database = TestUtil.CreateAdapter();
            m_fileHandler = new FileHandler(database, m_config);

            Cleanup();

            Directory.CreateDirectory(s_gameFileDir);
            Directory.CreateDirectory(s_tempDir);
            Directory.CreateDirectory(s_screenshotDir);
            Directory.CreateDirectory(s_titlePicDir);
            Directory.CreateDirectory(s_tileImagesDir);
            Directory.CreateDirectory(s_thumbnailsDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from Files");
            dataAccess.ExecuteNonQuery("delete from GameFiles");

            if (Directory.Exists(s_gameFileDir))
                Directory.Delete(s_gameFileDir, true);
            if (Directory.Exists(s_tempDir))
                Directory.Delete(s_tempDir, true);
            if (Directory.Exists(s_screenshotDir))
                Directory.Delete(s_screenshotDir, true);
            if (Directory.Exists(s_titlePicDir))
                Directory.Delete(s_titlePicDir, true);
            if (Directory.Exists(s_tileImagesDir))
                Directory.Delete(s_tileImagesDir, true);
            if (Directory.Exists(s_thumbnailsDir))
                Directory.Delete(s_thumbnailsDir, true);
        }

        [TestMethod]
        public void DeleteScreenshotThatIsReallyATitlePic_DeletesTitlePicScreenshot()
        {
            string gameFilePath = "uroburos.zip";
            File.Copy(Path.Combine("Resources", gameFilePath), Path.Combine(s_gameFileDir, gameFilePath));

            CreateLegacyTitlePicScreenshot(gameFilePath, 
                out var nonScaledTitlePicImage, 
                out var screenshotFile);

            bool anyDeleted = LegacyTitlePicMigration.DeleteScreenshotThatIsReallyATitlePic(m_fileHandler, m_gameFile, nonScaledTitlePicImage);

            Assert.IsTrue(anyDeleted);
            Assert.IsFalse(database.GetFiles(FileType.Screenshot).Any());
            Assert.IsFalse(File.Exists(screenshotFile.FullFileName));
        }

        [TestMethod]
        public void DeleteScreenshotThatIsReallyATitlePic_DoesntDeleteRegularScreenshots()
        {
            string gameFilePath = "uroburos.zip";
            File.Copy(Path.Combine("Resources", gameFilePath), Path.Combine(s_gameFileDir, gameFilePath));

            m_gameFile = new GameFile() { FileName = "uroburos.zip" };
            database.InsertGameFile(m_gameFile);

            GameFileImageHandler gfih = new GameFileImageHandler(m_fileHandler, database.GetIWadByIWadID);
            var screenshotFile = gfih.InsertScreenshot(new SourcePortData { SourcePortID = 444 },m_gameFile, Path.Combine("Resources", "happy.png"));
            Assert.IsTrue(database.GetFiles(FileType.Screenshot).Any());
            Assert.IsTrue(File.Exists(screenshotFile.FullFileName));

            bool anyDeleted = false;
            using (var screenshotImage = Image.FromFile(screenshotFile.FullFileName))
            {
                anyDeleted = LegacyTitlePicMigration.DeleteScreenshotThatIsReallyATitlePic(m_fileHandler, m_gameFile, screenshotImage);
            }

            Assert.IsFalse(anyDeleted);
            Assert.IsTrue(database.GetFiles(FileType.Screenshot).Any());
            Assert.IsTrue(File.Exists(screenshotFile.FullFileName));
        }

        private void CreateLegacyTitlePicScreenshot(string gameFilePath, out Image nonScaledTitlePicImage, out IFileData screenshotFile)
        {
            // Run a cut-down sync process, so we can get the real titlepic out of the wad 
            SyncLibraryHandler syncHandler = CreateSyncLibraryHandler();
            var syncResult = syncHandler.SyncManyFiles(new string[] { gameFilePath });
            m_gameFile = syncResult.AddedGameFiles[0];

            // Create a TitlePic just so that we can copy it into a legacy screenshot
            bool hasTitlePic = syncResult.GetTitlePic(m_gameFile, out nonScaledTitlePicImage);
            var scaledTitlePicImage = LegacyTitlePicMigration.LegacyScaleImage(nonScaledTitlePicImage);

            Assert.IsTrue(hasTitlePic);
            GameFileImageHandler gfih = new GameFileImageHandler(m_fileHandler, database.GetIWadByIWadID);
            IFileData titlePicFile = gfih.InsertTitlePic(m_gameFile, scaledTitlePicImage);
            screenshotFile = gfih.InsertScreenshot(new SourcePortData { SourcePortID = 333 }, m_gameFile, titlePicFile.FullFileName);
        }

        private SyncLibraryHandler CreateSyncLibraryHandler()
        {
            var syncActions = new List<ISyncAction>()
            {
                new TitlePicSyncAction(database, DataCache.Instance.DefaultPalette, DataCache.Instance.HexenPalette, DataCache.Instance.HereticPalette)
            };

            DirectoryDataSourceAdapter dirAdapter = new DirectoryDataSourceAdapter(m_config.GameFileDirectory);
            return new SyncLibraryHandler(database, dirAdapter, m_config, FileManagement.Managed, syncActions);
        }
    }
}
