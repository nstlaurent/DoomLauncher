using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSyncLibraryHandler
    {
        private static string s_filedir = "TestSyncDir";
        private static string s_tempdir = "TestSyncDirTemp";

        [TestInitialize]
        public void Init()
        {
            Cleanup();
            Directory.CreateDirectory(s_filedir);
            Directory.CreateDirectory(s_tempdir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var adapter = TestUtil.CreateAdapter();
            var gameFiles = adapter.GetGameFiles();
            gameFiles.ToList().ForEach(x => adapter.DeleteGameFile(x));

            if (Directory.Exists(s_filedir))
                Directory.Delete(s_filedir, true);
            if (Directory.Exists(s_tempdir))
                Directory.Delete(s_tempdir, true);
        }

        [TestMethod]
        public void TestSyncSingleFile()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string file = "uroburos.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.Execute(new string[] { "uroburos.zip" });

            Assert.AreEqual(1, handler.AddedGameFiles.Count);
            Assert.AreEqual(file, handler.AddedGameFiles[0].FileName);
            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            var gameFile = handler.DbDataSource.GetGameFiles().First();

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
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string[] files = new string[] { "uroburos.zip", "pyrrhic.zip" };
            Array.ForEach(files, x => File.Copy(Path.Combine("Resources", x), Path.Combine(s_filedir, x)));

            handler.Execute(files);

            Assert.AreEqual(2, handler.AddedGameFiles.Count);
            Assert.AreEqual(2, handler.DbDataSource.GetGameFilesCount());

            var gameFiles = handler.DbDataSource.GetGameFiles();
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
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string file = "joymaps1.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.Execute(new string[] { file });

            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            var gameFile = handler.DbDataSource.GetGameFiles().First();

            Assert.AreEqual("MAP01, MAP02, MAP03, MAP04, MAP05, MAP06, MAP07, MAP08, MAP09, MAP10, MAP11, MAP12, MAP13, MAP14, MAP15", gameFile.Map);
        }

        [TestMethod]
        public void TestMapsMultiFile()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string file = "pyrrhicmaps.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.Execute(new string[] { file });

            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            var gameFile = handler.DbDataSource.GetGameFiles().First();

            //each map (MAP01, MAP02, and MAP03) is it's own wad, make sure they are all found
            Assert.AreEqual("MAP01, MAP02, MAP03", gameFile.Map);
        }

        [TestMethod]
        public void TestSyncUpdate()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string file = "joymaps1.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));
            handler.Execute(new string[] { file });

            Assert.AreEqual(1, handler.AddedGameFiles.Count);
            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            var gameFile = handler.DbDataSource.GetGameFiles().First();

            Assert.AreEqual("MAP01, MAP02, MAP03, MAP04, MAP05, MAP06, MAP07, MAP08, MAP09, MAP10, MAP11, MAP12, MAP13, MAP14, MAP15", gameFile.Map);
            Assert.AreEqual("The Joy of Mapping #1", gameFile.Title);
            Assert.AreEqual("Jimmy & Various", gameFile.Author);
            Assert.IsTrue(gameFile.Description.StartsWith("This was a livestreamed communal mapping session"));
            Assert.AreEqual(gameFile.ReleaseDate, DateTime.Parse("8/1/2016"));

            File.Copy(Path.Combine("Resources", "uroburos.zip"), Path.Combine(s_filedir, file), true);
            handler.Execute(new string[] { file });

            Assert.AreEqual(0, handler.AddedGameFiles.Count);
            Assert.AreEqual(1, handler.UpdatedGameFiles.Count);
            Assert.AreEqual("joymaps1.zip", handler.UpdatedGameFiles[0].FileName);
            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            gameFile = handler.DbDataSource.GetGameFiles().First();

            Assert.AreEqual(gameFile.FileName, file);
            Assert.AreEqual("MAP01", gameFile.Map);
            Assert.AreEqual("Uroburos", gameFile.Title);
            Assert.AreEqual("hobomaster22", gameFile.Author);
            Assert.IsTrue(gameFile.Description.StartsWith("A 1on1 map"));
            Assert.AreEqual(gameFile.ReleaseDate, DateTime.Parse("3/5/2005"));
        }

        [TestMethod]
        public void TestInUse()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string file = "uroburos.zip";
            File.Copy(Path.Combine("Resources", file), Path.Combine(s_filedir, file));

            using (var reader = File.OpenWrite(Path.Combine(s_filedir, file))) //lock file
            {
                handler.Execute(new string[] { file });
            }

            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            Assert.AreEqual(1, handler.InvalidFiles.Length);
            Assert.AreEqual(file, handler.InvalidFiles[0].FileName);
        }

        [TestMethod]
        public void TestCorruptZip()
        {
            SyncLibraryHandler handler = CreateSyncLibraryHandler();
            Assert.AreEqual(0, handler.DbDataSource.GetGameFilesCount());

            string file = "bad.zip";
            File.WriteAllText(Path.Combine(s_filedir, file), "bad data");

            handler.Execute(new string[] { file });

            Assert.AreEqual(1, handler.DbDataSource.GetGameFilesCount());
            Assert.AreEqual(1, handler.InvalidFiles.Length);
            Assert.AreEqual(file, handler.InvalidFiles[0].FileName);
        }

        //todo test exception paths (Invalid Files)
        //todo test mapinfo

        private static SyncLibraryHandler CreateSyncLibraryHandler()
        {
            return new SyncLibraryHandler(TestUtil.CreateAdapter(), CreateDirectoryAdapater(), new LauncherPath(s_filedir), 
                new LauncherPath(s_tempdir), new string[] {"dd/M/yy", "dd/MM/yyyy", "dd MMMM yyyy" }, FileManagement.Managed);
        }

        private static DirectoryDataSourceAdapter CreateDirectoryAdapater()
        {
            return new DirectoryDataSourceAdapter(new LauncherPath("TestSyncDir"));
        }
    }
}
