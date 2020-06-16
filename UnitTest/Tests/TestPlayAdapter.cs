using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists("Temp"))
                Directory.Delete("Temp", true);
        }

        [TestMethod]
        public void TestWarp()
        {
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    string map = string.Format("E{0}M{1}", i + 1, j + 1);
                    Assert.AreEqual(GenericSourcePort.BuildWarpParameter(map), string.Format(" -warp {0} {1}", i + 1, j + 1));
                }
            }

            for(int i = 0; i < 32; i++)
            {
                string map = null;
                if (i + 1 > 9)
                    map = string.Format("MAP{0}", i + 1);
                else
                    map = string.Format("MAP0{0}", i + 1);

                Assert.AreEqual(GenericSourcePort.BuildWarpParameter(map), string.Format(" -warp {0}", i + 1));
            }

            Assert.AreEqual(" -warp 0", GenericSourcePort.BuildWarpParameter("MAP00"));
        }

        [TestMethod]
        public void TestMap()
        {
            Assert.AreEqual(" +map START", GenericSourcePort.BuildWarpParameter("START"));
            Assert.AreEqual(" +map MAPSTART", GenericSourcePort.BuildWarpParameter("MAPSTART"));
            Assert.AreEqual(" +map MAP01START", GenericSourcePort.BuildWarpParameter("MAP01START"));
            Assert.AreEqual(" +map MAP1", GenericSourcePort.BuildWarpParameter("MAP1"));
            Assert.AreEqual(" +map MAP001", GenericSourcePort.BuildWarpParameter("MAP001"));
        }

        [TestMethod]
        public void TestParameters()
        {
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            //test .wad and deh
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            string check = string.Format("-file \"{0}\"  -deh \"{1}\"",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"),
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.deh"));
            Assert.AreEqual(check.Trim(), launch.Trim());

            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.deh")));

            //.wad only
            launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad"), false);
            check = string.Format("-file \"{0}\"",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"));
            Assert.AreEqual(check.Trim(), launch.Trim());
        }

        [TestMethod]
        public void TestExtractFalse()
        {
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.ExtractFiles = false;
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");
            adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);

            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.wad")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.deh")));
        }

        [TestMethod]
        public void TestParametersSourcePortExtraParams()
        {
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            var port = GetTestPort(".wad,.deh");
            port.ExtraParameters = "-extra";

            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), port, false);
            string check = string.Format("-file \"{0}\"  -deh \"{1}\"  -extra",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"),
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.deh"));
            Assert.AreEqual(check.Trim(), launch.Trim());

            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.deh")));
        }

        [TestMethod]
        public void TestSkill()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.ExtractFiles = false;
            adapter.Skill = "3";
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            //only add skill with map
            Assert.IsFalse(launch.Contains("-skill 3"));

            adapter.Map = "MAP01";
            launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            Assert.IsTrue(launch.Contains("-skill 3"));
            Assert.IsTrue(launch.Contains("-warp 1"));
        }

        [TestMethod]
        public void TestRecord()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.ExtractFiles = false;
            adapter.Record = true;
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);

            Assert.IsNotNull(adapter.RecordedFileName);
            Assert.IsTrue(launch.Contains(string.Concat("-record \"", adapter.RecordedFileName, "\"")));
        }

        [TestMethod]
        public void TestPlayDemo()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            string demofile = Path.Combine(tempPath.GetFullPath(), "testplay.lmp");
            adapter.ExtractFiles = false;
            adapter.PlayDemo = true;
            adapter.PlayDemoFile = demofile;
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            //the file doesn't exist
            Assert.IsNull(launch);
            Assert.IsNotNull(adapter.LastError);

            File.WriteAllText(demofile, "test");
            launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.deh"), false);
            Assert.IsTrue(launch.Contains(string.Concat("-playdemo \"", demofile, "\"")));
        }

        [TestMethod]
        public void TestAdditionalFiles()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            adapter.AdditionalFiles = GetTestFiles().Skip(1).ToArray();

            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), GetTestPort(".wad,.pk3,.deh"), false);

            //-file parameters should be together, then -deh files should be together
            string check = string.Format(" -file \"{0}\\Temp\\test2.wad\" \"{0}\\Temp\\test2.pk3\" \"{0}\\Temp\\test3.wad\" \"{0}\\Temp\\test3.pk3\" \"{0}\\Temp\\test4.wad\" \"{0}\\Temp\\test4.pk3\" \"{0}\\Temp\\test1.wad\" \"{0}\\Temp\\test1.pk3\"  -deh \"{0}\\Temp\\test2.deh\" \"{0}\\Temp\\test3.deh\" \"{0}\\Temp\\test4.deh\" \"{0}\\Temp\\test1.deh\" ",
                Directory.GetCurrentDirectory());
            Assert.AreEqual(check.Trim(), launch.Trim());

            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test2.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test3.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test4.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test2.deh")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test3.deh")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test4.deh")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.deh")));
        }

        [TestMethod]
        public void TestBadFile()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, new GameFile() { FileName = "bad.zip" }, GetTestPort(".wad,.deh"), false);
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
            adapter.AdditionalFiles = new IGameFile[] { new GameFile() { FileName = "badadd.zip" } };

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

            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test2.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test3.wad")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test4.wad")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test1.deh")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test2.deh")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test3.deh")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath.GetFullPath(), "test4.deh")));
        }

        [TestMethod]
        public void TestIndividualPathedFiles()
        {
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");

            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            //testpathed.zip
            string file = Path.Combine("data", "test.wad");

            IGameFile gameFile = new GameFile() { FileName = "testpathed.zip" };
            adapter.SpecificFiles = new string[] { file };

            string parameters = adapter.GetLaunchParameters(gameFilePath, tempPath, gameFile, GetTestPort(".wad,.deh"), false);
            Assert.IsNull(adapter.LastError);
            Assert.IsTrue(parameters.Contains("test.wad"));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath.GetFullPath(), "test.wad")));
        }

        [TestMethod]
        public void TestExtraWithStats()
        {
            GameFilePlayAdapter adapter = new GameFilePlayAdapter();
            LauncherPath gameFilePath = new LauncherPath("GameFiles");
            LauncherPath tempPath = new LauncherPath("Temp");
            var boomPort = GetPrBoomTestPort(".wad,.deh");
            boomPort.ExtraParameters = "-boomextra";

            adapter.ExtraParameters = "-extratest";
            adapter.SaveStatistics = true;

            string launch = adapter.GetLaunchParameters(gameFilePath, tempPath, GetTestFile(), boomPort, false);

            Assert.IsTrue(launch.Contains(" -extratest "));
            Assert.IsTrue(launch.Contains(" -boomextra "));
            Assert.IsTrue(launch.Contains(" -levelstat"));
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
            CreateTestPathedFile();

            Directory.CreateDirectory("Temp");

            Directory.CreateDirectory("SourcePorts");
            File.WriteAllBytes(@"SourcePorts\zdoom.exe", new byte[] { });
        }

        private static void CreateTestPathedFile()
        {
            string filename = @"GameFiles\testpathed.zip";
            if (File.Exists(filename))
                File.Delete(filename);

            using (ZipArchive za = ZipFile.Open(filename, ZipArchiveMode.Create))
            {
                string writeFile = Path.Combine(@"GameFiles\", "test.wad");
                File.WriteAllBytes(writeFile, new byte[] { });

                za.CreateEntryFromFile(writeFile, Path.Combine("data", "test.wad"));
            }
        }

        private static void CreateTestFiles()
        {
            for (int i = 1; i < 5; i++)
            {
                string[] files = new string[] { string.Format("test{0}.wad", i), string.Format("test{0}.deh", i), string.Format("test{0}.pk3", i), string.Format("test{0}.txt", i) };

                string filename = string.Format(@"GameFiles\test{0}.zip", i);
                if (File.Exists(filename))
                    File.Delete(filename);

                using (ZipArchive za = ZipFile.Open(filename, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        string writeFile = Path.Combine(@"GameFiles\", file);
                        File.WriteAllBytes(writeFile, new byte[] { });

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

        private static ISourcePortData GetTestPort(string extensions)
        {
            return new SourcePortData { Executable = "zdoom.exe", Directory = new LauncherPath("SourcePorts"), SupportedExtensions = extensions, FileOption = "-file" };
        }

        private static ISourcePortData GetPrBoomTestPort(string extensions)
        {
            return new SourcePortData { Executable = "prboom.exe", Directory = new LauncherPath("SourcePorts"), SupportedExtensions = extensions, FileOption = "-file" };
        }
    }
}
