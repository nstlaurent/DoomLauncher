using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DoomLauncher;
using System.Linq;
using DoomLauncher.DataSources;
using System.IO.Compression;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestDirectoryAdapter
    {
        private static string s_testdir = "TestAdapterDirectory";

        private static string[] s_testfilenames =
            new[]
            {
                "test1.zip",
                "file.zip",
                "another_file.zip",
                "doom_file.zip",
                "doom_mod.zip"
            };

        [TestInitialize]
        public void Initialize()
        {
            if (Directory.Exists(s_testdir))
                Directory.Delete(s_testdir, true);
            Directory.CreateDirectory(s_testdir);
            Array.ForEach(s_testfilenames, x => CreateZip(x));
        }

        private static void CreateZip(string x)
        {
            using (var zip = ZipFile.Open(Path.Combine(s_testdir, x), ZipArchiveMode.Create))
            {
                zip.CreateEntry("file1.txt");
            }
        }

        [TestMethod]
        public void TestFileCount()
        {
            DirectoryDataSourceAdapter adapter = CreateAdapter();
            Assert.AreEqual(s_testfilenames.Length, adapter.GetGameFilesCount());
        }

        [TestMethod]
        public void TestFileNames()
        {
            DirectoryDataSourceAdapter adapter = CreateAdapter();
            var filenames = adapter.GetGameFileNames();

            Assert.AreEqual(s_testfilenames.Length, filenames.Count());

            foreach (var file in s_testfilenames)
                Assert.IsTrue(filenames.Contains(file));
        }

        [TestMethod]
        public void TestGetFiles()
        {
            DirectoryDataSourceAdapter adapter = CreateAdapter();
            var gameFiles = adapter.GetGameFiles();

            Assert.AreEqual(s_testfilenames.Length, gameFiles.Count());

            foreach (var file in s_testfilenames)
                Assert.IsNotNull(gameFiles.FirstOrDefault(x => x.FileName == file));
        }

        [TestMethod]
        public void TestGetFile()
        {
            DirectoryDataSourceAdapter adapter = CreateAdapter();
            var gameFile = adapter.GetGameFile(s_testfilenames[0]);

            Assert.IsNotNull(gameFile);
            Assert.AreEqual(s_testfilenames[0], gameFile.FileName);

            gameFile = adapter.GetGameFile("garbagefile.zip");
            Assert.IsNull(gameFile);
        }

        [TestMethod]
        public void TestGetFilesOptions()
        {
            DirectoryDataSourceAdapter adapter = CreateAdapter();
            GameFileGetOptions options = new GameFileGetOptions(2); //limit is the only support option
            var gameFiles = adapter.GetGameFiles(options);

            Assert.AreEqual(2, gameFiles.Count());
        }

        [TestMethod]
        public void TestDeleteFile()
        {
            DirectoryDataSourceAdapter adapter = CreateAdapter();

            Assert.AreEqual(s_testfilenames.Length, adapter.GetGameFilesCount());

            adapter.DeleteGameFile(new GameFile() { FileName = s_testfilenames[0] });
            Assert.AreEqual(s_testfilenames.Length - 1, adapter.GetGameFilesCount());
        }

        private static DirectoryDataSourceAdapter CreateAdapter()
        {
            return new DirectoryDataSourceAdapter(new LauncherPath(s_testdir)); ;
        }
    }
}
