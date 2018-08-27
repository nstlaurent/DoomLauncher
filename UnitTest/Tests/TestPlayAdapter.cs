using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
using System.IO;
using System.IO.Compression;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestPlayAdapter
    {
        [TestInitialize]
        public void Init()
        {
            CreateDirectoriesAndFiles();
        }

        [TestMethod]
        public void TestWarp()
        {
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    string map = string.Format("E{0}M{1}", i + 1, j + 1);
                    Assert.AreEqual(GameFilePlayAdapter.BuildWarpParamter(map), string.Format(" -warp {0} {1}", i + 1, j + 1));
                }
            }

            for(int i = 0; i < 32; i++)
            {
                string map = null;
                if (i + 1 > 9)
                    map = string.Format("MAP{0}", i + 1);
                else
                    map = string.Format("MAP0{0}", i + 1);

                Assert.AreEqual(GameFilePlayAdapter.BuildWarpParamter(map), string.Format(" -warp {0}", i + 1));
            }

            Assert.AreEqual(" -warp 0", GameFilePlayAdapter.BuildWarpParamter("MAP00"));
        }

        [TestMethod]
        public void TestMap()
        {
            Assert.AreEqual(" +map START", GameFilePlayAdapter.BuildWarpParamter("START"));
            Assert.AreEqual(" +map MAPSTART", GameFilePlayAdapter.BuildWarpParamter("MAPSTART"));
            Assert.AreEqual(" +map MAP01START", GameFilePlayAdapter.BuildWarpParamter("MAP01START"));
            Assert.AreEqual(" +map MAP1", GameFilePlayAdapter.BuildWarpParamter("MAP1"));
            Assert.AreEqual(" +map MAP001", GameFilePlayAdapter.BuildWarpParamter("MAP001"));
        }

        [TestMethod]
        public void TestParameters()
        {
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();

            CreateDirectoriesAndFiles();

            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            //test .wad and deh
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            string check = string.Format("-file \"{0}\"  -deh \"{1}\"",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"),
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.deh"));
            Assert.AreEqual(check.Trim(), launch.Trim());

            //.wad only
            launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad"), false);
            check = string.Format("-file \"{0}\"",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"));
            Assert.AreEqual(check.Trim(), launch.Trim());
        }

        [TestMethod]
        public void TestAdditionalFiles()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.AdditionalFiles = GetTestFiles().Skip(1).ToArray();

            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);

            //-file parameters should be together, then -deh files should be together
            string check = string.Format("-file \"{0}\\Temp\\test2.wad\" \"{0}\\Temp\\test3.wad\" \"{0}\\Temp\\test4.wad\" \"{0}\\Temp\\test1.wad\"  -deh \"{0}\\Temp\\test2.deh\" \"{0}\\Temp\\test3.deh\" \"{0}\\Temp\\test4.deh\" \"{0}\\Temp\\test1.deh\" ",
                Directory.GetCurrentDirectory());
            Assert.AreEqual(check.Trim(), launch.Trim());
        }

        [TestMethod]
        public void TestBadFile()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, new DoomLauncher.DataSources.GameFile() { FileName = "bad.zip" }, GetTestPort(".wad,.deh"), false);
            Assert.IsNull(launch);
            Assert.IsNotNull(adapter.LastError);
            Assert.IsTrue(adapter.LastError.Contains("bad.zip"));
        }

        [TestMethod]
        public void TestBadAdditionalFile()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.AdditionalFiles = new IGameFile[] { new DoomLauncher.DataSources.GameFile() { FileName = "badadd.zip" } };

            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            Assert.IsNull(launch);
            Assert.IsNotNull(adapter.LastError);
            Assert.IsTrue(adapter.LastError.Contains("badadd.zip"));
        }

        [TestMethod]
        public void TestIndividualFiles()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            string[] wads;

            using (ZipArchive za = ZipFile.OpenRead(Path.Combine("GameFiles", GetTestFiles().First().FileName)))
                wads = za.Entries.Where(x => Path.GetExtension(x.Name) == ".wad").Select(x => x.FullName).ToArray();

            adapter.SpecificFiles = wads.ToArray();

            string parameters = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFiles().First(), GetTestPort(".wad,.deh"), false);
            Assert.IsNull(adapter.LastError);
            Assert.IsTrue(parameters.Contains(".wad"));
            Assert.IsFalse(parameters.Contains(".deh"));
        }

        [TestMethod]
        public void TestIndividualFilesMultiple()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            List<string> wads = new List<string>();

            using (ZipArchive za = ZipFile.OpenRead(Path.Combine("GameFiles", GetTestFiles().First().FileName)))
                wads.AddRange(za.Entries.Where(x => Path.GetExtension(x.Name) == ".wad").Select(x => x.FullName));

            using (ZipArchive za = ZipFile.OpenRead(Path.Combine("GameFiles", GetTestFiles().Skip(1).First().FileName)))
                wads.AddRange(za.Entries.Where(x => Path.GetExtension(x.Name) == ".wad").Select(x => x.FullName));

            using (ZipArchive za = ZipFile.OpenRead(Path.Combine("GameFiles", GetTestFiles().Skip(2).First().FileName)))
                wads.AddRange(za.Entries.Where(x => Path.GetExtension(x.Name) == ".wad").Select(x => x.FullName));

            //this is test4.zip and it will NOT be added to the additional files list
            using (ZipArchive za = ZipFile.OpenRead(Path.Combine("GameFiles", GetTestFiles().Skip(3).First().FileName)))
                wads.AddRange(za.Entries.Where(x => Path.GetExtension(x.Name) == ".wad").Select(x => x.FullName));

            adapter.AdditionalFiles = GetTestFiles().Skip(1).Take(2).ToArray(); //test2.wad, test3.wad only
            adapter.SpecificFiles = wads.ToArray();

            string parameters = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFiles().First(), GetTestPort(".wad,.deh"), false);
            Assert.IsNull(adapter.LastError);
            Assert.IsTrue(parameters.Contains("test1.wad"));
            Assert.IsTrue(parameters.Contains("test2.wad"));
            Assert.IsTrue(parameters.Contains("test3.wad"));
            Assert.IsFalse(parameters.Contains("test4.wad"));
            Assert.IsFalse(parameters.Contains(".deh"));
        }

        private void CreateDirectoriesAndFiles()
        {
            if (Directory.Exists("GameFiles"))
                Directory.Delete("GameFiles", true);
            if (Directory.Exists("Temp"))
                Directory.Delete("Temp", true);
            if (Directory.Exists("SourcePorts"))
                Directory.Delete("SourcePorts", true);

            Directory.CreateDirectory("GameFiles");
            CreateTestFiles();

            Directory.CreateDirectory("Temp");

            Directory.CreateDirectory("SourcePorts");
            System.IO.File.WriteAllBytes(@"SourcePorts\zdoom.exe", new byte[] { });
        }

        private static void CreateTestFiles()
        {
            for (int i = 1; i < 5; i++)
            {
                string[] files = new string[] { string.Format("test{0}.wad", i), string.Format("test{0}.deh", i), string.Format("test{0}.txt", i) };

                string filename = string.Format(@"GameFiles\test{0}.zip", i);
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);

                using (ZipArchive za = ZipFile.Open(filename, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        string writeFile = Path.Combine(@"GameFiles\", file);
                        System.IO.File.WriteAllBytes(writeFile, new byte[] { });

                        za.CreateEntryFromFile(writeFile, file);
                    }
                }
            }
        }

        private static List<IGameFile> GetTestFiles()
        {
            List<IGameFile> files = new List<IGameFile>();
            for (int i = 1; i < 5; i++)
                files.Add(new GameFile { FileName = string.Format("test{0}.zip", i) });
            return files;
        }

        private static IGameFile GetTestFile()
        {
            return new GameFile { FileName = "test1.zip" };
        }

        private static ISourcePort GetTestPort(string extensions)
        {
            return new SourcePort { Executable = "zdoom.exe", Directory = new LauncherPath("SourcePorts"), SupportedExtensions = extensions, FileOption = "-file" };
        }
    }
}
