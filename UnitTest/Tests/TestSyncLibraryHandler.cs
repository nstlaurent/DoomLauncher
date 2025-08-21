using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using DoomLauncher.Handlers.Sync;
using System.Collections.Generic;
using DoomLauncher.Interfaces;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSyncLibraryHandler
    {
        private static readonly string s_filedir = "TestSyncDir";
        private static readonly string s_tempdir = "TestSyncDirTemp";

        private IDataSourceAdapter database;

        [TestInitialize]
        public void Init()
        {
            database = TestUtil.CreateAdapter();
            Cleanup();
            Directory.CreateDirectory(s_filedir);
            Directory.CreateDirectory(s_tempdir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var gameFiles = database.GetGameFiles();
            gameFiles.ToList().ForEach(x => database.DeleteGameFile(x));

            if (Directory.Exists(s_filedir))
                Directory.Delete(s_filedir, true);
            if (Directory.Exists(s_tempdir))
                Directory.Delete(s_tempdir, true);
        }

        [TestMethod]
        public void TestSyncSingleFile()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "uroburos.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            var syncResult = handler.SyncManyFiles(new string[] { "uroburos.zip" });

            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            Assert.AreEqual(file, syncResult.AddedGameFiles[0].FileName);
            Assert.AreEqual(1, database.GetGameFilesCount());
            var gameFile = database.GetGameFiles().First();

            Assert.AreEqual(file, gameFile.FileName);
            Assert.AreEqual("MAP01", gameFile.Map);
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Title));
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Author));
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Description));
            Assert.IsNotNull(gameFile.ReleaseDate);
            Assert.IsNotNull(gameFile.Downloaded);
        }

        [TestMethod]
        public void TestSyncMultiFile()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string[] files = new string[] { "uroburos.zip", "pyrrhic.zip" };
            Array.ForEach(files, x => File.Copy(Path.Combine("Resources", x), Path.Combine(s_filedir, x)));

            var syncResult = handler.SyncManyFiles(files);

            Assert.AreEqual(2, syncResult.AddedGameFiles.Count);
            Assert.AreEqual(2, database.GetGameFilesCount());

            var gameFiles = database.GetGameFiles();
            var gameFile = gameFiles.First(x => x.FileName == files[0]);

            Assert.AreEqual("uroburos.zip", gameFile.FileName);
            Assert.AreEqual("MAP01", gameFile.Map);
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Title));
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Author));
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Description));
            Assert.IsNotNull(gameFile.ReleaseDate);
            Assert.IsNotNull(gameFile.Downloaded);

            gameFile = gameFiles.First(x => x.FileName == files[1]);
            Assert.AreEqual("pyrrhic.zip", gameFile.FileName);
            Assert.AreEqual("MAP01, MAP02, MAP03, MAP04, MAP05, MAP06, MAP07, MAP08, MAP09, MAP10, MAP11, MAP12, MAP13, MAP14, MAP15", gameFile.Map);
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Title));
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Author));
            Assert.IsFalse(string.IsNullOrEmpty(gameFile.Description));
            Assert.IsNotNull(gameFile.ReleaseDate);
            Assert.IsNotNull(gameFile.Downloaded);
        }

        [TestMethod]
        public void TestMapInfo()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "joymaps1.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, database.GetGameFilesCount());
            var gameFile = database.GetGameFiles().First();

            Assert.AreEqual("MAP01, MAP02, MAP03, MAP04, MAP05, MAP06, MAP07, MAP08, MAP09, MAP10, MAP11, MAP12, MAP13, MAP14, MAP15", gameFile.Map);
        }

        [TestMethod]
        public void TestMapInfoInclude()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "mapinfo.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, database.GetGameFilesCount());
            var gameFile = database.GetGameFiles().First();

            string maps = "E1M1, E1M2, E1M3, E1M4, E1M5, E1M6, E1M7, E1M8, E1M9, E2M1, E2M2, E2M3, E2M4, E2M5, E2M6, E2M7, E2M8, E2M9, E3M1, E3M2, E3M3, E3M4, E3M5, E3M6, E3M7, E3M8, E3M9, E4M1, E4M2, E4M3, E4M4, E4M5, E4M6, E4M7, E4M8, E4M9, MAP01, MAP02, MAP03, MAP04, MAP05, MAP06, MAP07, MAP08, MAP09, MAP10, MAP11, MAP12, MAP13, MAP14, MAP15, MAP16, MAP17, MAP18, MAP19, MAP20, MAP21, MAP22, MAP23, MAP24, MAP25, MAP26, MAP27, MAP28, MAP29, MAP30, MAP31, MAP32, NV_MAP01, NV_MAP02, NV_MAP03, NV_MAP04, NV_MAP05, NV_MAP06, NV_MAP07, NV_MAP08, NV_MAP09, ML_MAP01, ML_MAP02, ML_MAP03, ML_MAP04, ML_MAP05, ML_MAP06, ML_MAP07, ML_MAP08, ML_MAP09, ML_MAP10, ML_MAP11, ML_MAP12, ML_MAP13, ML_MAP14, ML_MAP15, ML_MAP16, ML_MAP17, ML_MAP18, ML_MAP19, ML_MAP20, ML_MAP21, TEST00, TN_MAP01, TN_MAP02, TN_MAP03, TN_MAP04, TN_MAP05, TN_MAP06, TN_MAP07, TN_MAP08, TN_MAP09, TN_MAP10, TN_MAP11, TN_MAP12, TN_MAP13, TN_MAP14, TN_MAP15, TN_MAP16, TN_MAP17, TN_MAP18, TN_MAP19, TN_MAP20, TN_MAP21, TN_MAP22, TN_MAP23, TN_MAP24, TN_MAP25, TN_MAP26, TN_MAP27, TN_MAP28, TN_MAP29, TN_MAP30, TN_MAP31, TN_MAP32, PL_MAP01, PL_MAP02, PL_MAP03, PL_MAP04, PL_MAP05, PL_MAP06, PL_MAP07, PL_MAP08, PL_MAP09, PL_MAP10, PL_MAP11, PL_MAP12, PL_MAP13, PL_MAP14, PL_MAP15, PL_MAP16, PL_MAP17, PL_MAP18, PL_MAP19, PL_MAP20, PL_MAP21, PL_MAP22, PL_MAP23, PL_MAP24, PL_MAP25, PL_MAP26, PL_MAP27, PL_MAP28, PL_MAP29, PL_MAP30, PL_MAP31, PL_MAP32";
            Assert.AreEqual(maps, gameFile.Map);
        }

        [TestMethod]
        public void TestNonDoom64FileIsntDoom64()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();

            string file = "mapinfo.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.SyncManyFiles(new string[] { file });

            var gameFile = database.GetGameFiles().First();

            Assert.IsFalse(gameFile.IsDoom64);
        }

        [TestMethod]
        public void TestDoom64FileIsDoom64()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();

            string file = "mapinfo_doom64.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.SyncManyFiles(new string[] { file });

            var gameFile = database.GetGameFiles().First();

            Assert.IsTrue(gameFile.IsDoom64);
        }

        [TestMethod]
        public void TestMapsMultiFile()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "pyrrhicmaps.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, database.GetGameFilesCount());
            var gameFile = database.GetGameFiles().First();

            //each map (MAP01, MAP02, and MAP03) is it's own wad, make sure they are all found
            Assert.AreEqual("MAP01, MAP02, MAP03", gameFile.Map);
        }

        [TestMethod]
        public void UnmanagedWadGetsMapString()
        {
            string file = "simple.wad";
            string fullPathFile = Path.Combine(Directory.GetCurrentDirectory(), s_filedir, file);

            SyncLibraryHandler handler = CreateSyncLibraryHandler(false, FileManagement.Unmanaged);
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));

            var syncResult = handler.SyncManyFiles(new string[] { fullPathFile });
            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);

            Assert.AreEqual("MAP01", syncResult.AddedGameFiles[0].Map);
        }

        [TestMethod]
        public void TestSyncUpdate()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "joymaps1.zip";
            File.Copy(Path.Combine("Resources", "joymaps1.zip"), Path.Combine(s_filedir, "joymaps1.zip"));
            var syncResult = handler.SyncManyFiles(new string[] { "joymaps1.zip" });

            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            Assert.AreEqual(1, database.GetGameFilesCount());
            var gameFile = database.GetGameFiles().First();

            Assert.AreEqual("MAP01, MAP02, MAP03, MAP04, MAP05, MAP06, MAP07, MAP08, MAP09, MAP10, MAP11, MAP12, MAP13, MAP14, MAP15", gameFile.Map);
            Assert.AreEqual("The Joy of Mapping #1", gameFile.Title);
            Assert.AreEqual("Jimmy & Various", gameFile.Author);
            Assert.IsTrue(gameFile.Description.StartsWith("This was a livestreamed communal mapping session"));
            Assert.AreEqual(gameFile.ReleaseDate, DateTime.Parse("8/1/2016", CultureInfo.InvariantCulture));

            File.Copy(Path.Combine("Resources", "uroburos.zip"), Path.Combine(s_filedir, "joymaps1.zip"), true);
            var syncResult2 = handler.SyncManyFiles(new string[] { "joymaps1.zip" });

            Assert.AreEqual(0, syncResult2.AddedGameFiles.Count);
            Assert.AreEqual(1, syncResult2.UpdatedGameFiles.Count);
            Assert.AreEqual("joymaps1.zip", syncResult2.UpdatedGameFiles[0].FileName);
            Assert.AreEqual(1, database.GetGameFilesCount());
            gameFile = database.GetGameFiles().First();

            Assert.AreEqual(gameFile.FileName, file);
            Assert.AreEqual("MAP01", gameFile.Map);
            Assert.AreEqual("Uroburos", gameFile.Title);
            Assert.AreEqual("hobomaster22", gameFile.Author);
            Assert.IsTrue(gameFile.Description.StartsWith("A 1on1 map"));
            Assert.AreEqual(gameFile.ReleaseDate, DateTime.Parse("3/5/2005", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void TestInUse()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "uroburos.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));

            var syncResult = SyncResult.EMPTY;
            using (var reader = File.OpenWrite(Path.Combine(s_filedir, file))) //lock file
            {
                syncResult = handler.SyncManyFiles(new string[] { file });
            }

            Assert.AreEqual(1, database.GetGameFilesCount());
            Assert.AreEqual(1, syncResult.InvalidFiles.Count);
            Assert.AreEqual(file, syncResult.InvalidFiles[0].FileName);
        }

        [TestMethod]
        public void TestCorruptZip()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, database.GetGameFilesCount());

            string file = "bad.zip";
            File.WriteAllText(Path.Combine(s_filedir, file), "bad data");

            var syncResult = handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, database.GetGameFilesCount());
            Assert.AreEqual(1, syncResult.InvalidFiles.Count);
            Assert.AreEqual(file, syncResult.InvalidFiles[0].FileName);
        }

        [TestMethod]
        public void TestDoomImageTitlepicWad()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler(true);

            string file = "uroburos.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));

            var syncResult = handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            bool hasTitlepic = syncResult.GetTitlePic(syncResult.AddedGameFiles[0], out Image titlepic);
            Assert.IsTrue(hasTitlepic);
            Assert.AreEqual(320, titlepic.Width);
            Assert.AreEqual(200, titlepic.Height);
        }

        [TestMethod]
        public void TestLargeDoomImageTitlepicWad()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler(true);

            string file = "pyrrhic.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            var syncResult = handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            bool hasTitlepic = syncResult.GetTitlePic(syncResult.AddedGameFiles[0], out Image titlepic);
            Assert.IsTrue(hasTitlepic);
            Assert.AreEqual(640, titlepic.Width);
            Assert.AreEqual(400, titlepic.Height);
        }

        [TestMethod]
        public void TestPngTitlepicPk3()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler(true);

            string file = "joymaps1.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            var syncResult = handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            bool hasTitlepic = syncResult.GetTitlePic(syncResult.AddedGameFiles[0], out Image titlepic);
            Assert.IsTrue(hasTitlepic);
            Assert.AreEqual(320, titlepic.Width);
            Assert.AreEqual(200, titlepic.Height);
        }

        [TestMethod]
        public void TestTilepicZMapInfo()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler(true);

            string file = "zmapinfo.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            var syncResult = handler.SyncManyFiles(new string[] { file });

            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            bool hasTitlepic = syncResult.GetTitlePic(syncResult.AddedGameFiles[0], out Image titlepic);
            Assert.IsTrue(hasTitlepic);
            Assert.AreEqual(478, titlepic.Width);
            Assert.AreEqual(313, titlepic.Height);
        }

        [TestMethod]
        public void TestMultipleTitlepicFiles()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler(true);

            string file = "pyrrhic.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            string file2 = "joymaps1.zip";
            File.Copy(Path.Combine("Resources", file2), Path.Combine(s_filedir, file2));
            var syncResult = handler.SyncManyFiles(new string[] { file, file2 });

            Assert.AreEqual(2, syncResult.AddedGameFiles.Count);

            bool hasTitlepic = syncResult.GetTitlePic(syncResult.AddedGameFiles[0], out Image pyrrhicTitlepic);
            Assert.IsTrue(hasTitlepic);
   
            Assert.AreEqual(640, pyrrhicTitlepic.Width);
            Assert.AreEqual(400, pyrrhicTitlepic.Height);

            hasTitlepic = syncResult.GetTitlePic(syncResult.AddedGameFiles[1], out Image joymapsTitlepic);
            Assert.IsTrue(hasTitlepic);

            Assert.AreEqual(320, joymapsTitlepic.Width);
            Assert.AreEqual(200, joymapsTitlepic.Height);
        }

        [TestMethod]
        public void UnmanagedFiles()
        {
            string file = "simple.wad";
            string file1 = "file1.wad";
            string file2 = "file2.wad";
            string fullPathFile1 = Path.Combine(Directory.GetCurrentDirectory(), s_filedir, file1);
            string fullPathFile2 = Path.Combine(Directory.GetCurrentDirectory(), s_filedir, file2);

            SyncLibraryHandler handler = CreateSyncLibraryHandler(false, FileManagement.Unmanaged);
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file1));
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file2));

            // Change directory to force ignore relative paths
            string dir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(s_filedir);
            var syncResult = handler.SyncManyFiles(new string[] { fullPathFile1, fullPathFile2 });
            Assert.AreEqual(2, syncResult.AddedGameFiles.Count);
            Directory.SetCurrentDirectory(dir);

            Assert.AreEqual(fullPathFile1, syncResult.AddedGameFiles[0].FileName);
            Assert.AreEqual(fullPathFile2, syncResult.AddedGameFiles[1].FileName);
        }

        [TestMethod]
        public void UnmanagedRelativeFiles()
        {
            string file = "simple.wad";
            string file1 = "file1.wad";
            string file2 = "file2.wad";
            string fullPathFile1 = Path.Combine(Directory.GetCurrentDirectory(), s_filedir, file1);
            string fullPathFile2 = Path.Combine(Directory.GetCurrentDirectory(), s_filedir, file2);

            SyncLibraryHandler handler = CreateSyncLibraryHandler(false, FileManagement.Unmanaged);
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file1));
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file2));

            var syncResult = handler.SyncManyFiles(new string[] { fullPathFile1, fullPathFile2 });
            Assert.AreEqual(2, syncResult.AddedGameFiles.Count);

            Assert.AreEqual(Path.Combine(s_filedir, file1), syncResult.AddedGameFiles[0].FileName);
            Assert.AreEqual(Path.Combine(s_filedir, file2), syncResult.AddedGameFiles[1].FileName);
        }

        [TestMethod]
        public void WadLumpWithBadChars()
        {
            string file1 = "pathtest.zip";

            SyncLibraryHandler handler = CreateSyncLibraryHandler(false, FileManagement.Managed);
            File.Copy(Path.Combine("Resources", file1), Path.Combine(s_filedir, file1));

            var syncResult = handler.SyncManyFiles(new string[] { file1 });
            Assert.AreEqual(1, syncResult.AddedGameFiles.Count);
            Assert.AreEqual(0, syncResult.InvalidFiles.Count);

            var addedFile = syncResult.AddedGameFiles[0];
            Assert.AreEqual(addedFile.Map, "MAP01");
        }

        private SyncLibraryHandler CreateSyncLibraryHandler(bool pullTitlepic = false, FileManagement fileManagement = FileManagement.Managed)
        {
            var directories = new DirectoriesConfiguration 
            { 
                GameFileDirectory = new LauncherPath(s_filedir),
                TempDirectory = new LauncherPath(s_tempdir)
            };

            var dateParseFormats = new string[] { "dd/M/yy", "dd/MM/yyyy", "dd MMMM yyyy" };

            var syncActions = new List<ISyncAction>()
            {
                new TextFileSyncAction(new IdGamesTextFileParser(dateParseFormats).Parse),
                new Doom64SyncAction(database),
                new MapStringSyncAction(directories.TempDirectory)
            };

            if (pullTitlepic)
                syncActions.Add(new TitlePicSyncAction(database, DataCache.Instance.DefaultPalette, DataCache.Instance.HexenPalette, DataCache.Instance.HereticPalette));

            return new SyncLibraryHandler(database, CreateDirectoryAdapater(), directories, fileManagement, syncActions);
        }

        private static DirectoryDataSourceAdapter CreateDirectoryAdapater()
        {
            return new DirectoryDataSourceAdapter(new LauncherPath("TestSyncDir"));
        }
    }
}
