using DoomLauncher;
using DoomLauncher.Adapters.Launch;
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace UnitTest.Tests
{

    [TestClass]
    public class TestGameLauncher
    {
        private static readonly string LocalFile1 = @"Local\file1.wad";
        private static readonly string LocalIwad1 = @"Local\iwad1.wad";

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
                    Assert.AreEqual(GenericSourcePortFlavor.BuildWarpParameter(map), string.Format(" -warp {0} {1}", i + 1, j + 1));
                }
            }

            for(int i = 0; i < 32; i++)
            {
                string map = null;
                if (i + 1 > 9)
                    map = string.Format("MAP{0}", i + 1);
                else
                    map = string.Format("MAP0{0}", i + 1);

                Assert.AreEqual(GenericSourcePortFlavor.BuildWarpParameter(map), string.Format(" -warp {0}", i + 1));
            }

            Assert.AreEqual(" -warp 0", GenericSourcePortFlavor.BuildWarpParameter("MAP00"));
        }

        [TestMethod]
        public void TestMap()
        {
            Assert.AreEqual(" +map START", GenericSourcePortFlavor.BuildWarpParameter("START"));
            Assert.AreEqual(" +map MAPSTART", GenericSourcePortFlavor.BuildWarpParameter("MAPSTART"));
            Assert.AreEqual(" +map MAP01START", GenericSourcePortFlavor.BuildWarpParameter("MAP01START"));
            Assert.AreEqual(" +map MAP1", GenericSourcePortFlavor.BuildWarpParameter("MAP1"));
            Assert.AreEqual(" +map MAP001", GenericSourcePortFlavor.BuildWarpParameter("MAP001"));
        }

        [TestMethod]
        public void TestParameters()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            GameLauncher launcher = new GameLauncher(directories);

            //test .wad and deh
            string launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.deh"), false).LaunchString;
            string check = string.Format("-file \"{0}\"  -deh \"{1}\"",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"),
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.deh"));
            Assert.AreEqual(check.Trim(), launch.Trim());

            Assert.IsTrue(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test1.deh")));

            //.wad only
            launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad"), false).LaunchString;
            check = string.Format("-file \"{0}\"",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"));
            Assert.AreEqual(check.Trim(), launch.Trim());
        }

        

        [TestMethod]
        public void TestExtractFalse()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var features = new List<ILaunchFeature>()
            {
                new GameFilesLaunchFeature(new List<IGameFile>() { GetTestFile() }, null, false)
            };

            GameLauncher launcher = new GameLauncher(directories, features);

            launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.deh"), false);

            Assert.IsFalse(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test1.wad")));
            Assert.IsFalse(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test1.deh")));
        }

        [TestMethod]
        public void TestParametersSourcePortExtraParams()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var features = new List<ILaunchFeature>()
            {
                new ExtraParametersLaunchFeature("-extra", false)
            };

            GameLauncher launcher = new GameLauncher(directories, features);

            var port = GetTestPort(".wad,.deh");
            port.ExtraParameters = "-extra";

            string launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), port, false).LaunchString;
            string check = string.Format("-file \"{0}\"  -deh \"{1}\" -extra",
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.wad"),
                Path.Combine(Directory.GetCurrentDirectory(), "Temp", "test1.deh"));
            Assert.AreEqual(check.Trim(), launch.Trim());

            Assert.IsTrue(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test1.deh")));
        }

        [TestMethod]
        public void TestSkillAndMap()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var features = new List<ILaunchFeature>()
            {
                new MapSkillLaunchFeature("MAP01", "3")
            };

            GameLauncher launcher = new GameLauncher(directories, features);
            var port = GetPrBoomTestPort(".wad,.deh");

            var launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), port, false).LaunchString;
            Assert.IsTrue(launch.Contains("-skill 3"));
            Assert.IsTrue(launch.Contains("-warp 1"));
        }

        [TestMethod]
        public void TestSkillAndMapZdoom()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var features = new List<ILaunchFeature>()
            {
                new MapSkillLaunchFeature("MAP01", "3")
            };

            GameLauncher launcher = new GameLauncher(directories, features);
            var port = GetTestPort(".wad,.deh");

            var launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), port, false).LaunchString;
            Assert.IsTrue(launch.Contains("-skill 3"));
            Assert.IsTrue(launch.Contains("+map MAP01"));
        }

        [TestMethod]
        public void TestRecord()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var features = new List<ILaunchFeature>()
            {
                new RecordLaunchFeature()
            };

            GameLauncher launcher = new GameLauncher(directories, features);
            var parameters = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.deh"), false);

            Assert.IsNotNull(parameters.RecordedFileName);
            Assert.IsTrue(parameters.LaunchString.Contains(string.Concat("-record \"", parameters.RecordedFileName, "\"")));
        }

        [TestMethod]
        public void TestPlayDemo()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            string demofile = Path.Combine(directories.TempDirectory.GetFullPath(), "testplay.lmp");
            var features = new List<ILaunchFeature>()
            {
                new PlayDemoLaunchFeature(demofile)
            };

            GameLauncher launcher = new GameLauncher(directories, features);
            var parameters = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.deh"), false);
            //the file doesn't exist
            Assert.IsTrue(parameters.Failed);
            Assert.IsNotNull(parameters.ErrorMessage);

            File.WriteAllText(demofile, "test");
            var launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.deh"), false).LaunchString;
            Assert.IsTrue(launch.Contains(string.Concat("-playdemo \"", demofile, "\"")));
        }

        [TestMethod]
        public void TestAdditionalFiles()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var additionalFiles = GetTestFiles().Skip(1).ToList();
            var features = new List<ILaunchFeature>()
            {
                new GameFilesLaunchFeature(additionalFiles, null)
            };

            GameLauncher launcher = new GameLauncher(directories, features);
            string launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.pk3,.deh"), false).LaunchString;

            //-file parameters should be together, then -deh files should be together
            string check = string.Format(" -file \"{0}\\Temp\\test2.wad\" \"{0}\\Temp\\test2.pk3\" \"{0}\\Temp\\test3.wad\" \"{0}\\Temp\\test3.pk3\" \"{0}\\Temp\\test4.wad\" \"{0}\\Temp\\test4.pk3\" \"{0}\\Temp\\test1.wad\" \"{0}\\Temp\\test1.pk3\"  -deh \"{0}\\Temp\\test2.deh\" \"{0}\\Temp\\test3.deh\" \"{0}\\Temp\\test4.deh\" \"{0}\\Temp\\test1.deh\" ",
                Directory.GetCurrentDirectory());
            Assert.AreEqual(check.Trim(), launch.Trim());

            var tempPath = directories.TempDirectory.GetFullPath();
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test2.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test3.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test4.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test2.deh")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test3.deh")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test4.deh")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test1.deh")));
        }

        [TestMethod]
        public void TestBadFile()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            GameLauncher launcher = new GameLauncher(directories);
            var parameters = launcher.GetLaunchParameters(new GameFile() { FileName = "bad.zip" }, GetAddFiles(), GetTestPort(".wad,.deh"), false);

            Assert.IsTrue(parameters.Failed);
            Assert.IsNotNull(parameters.ErrorMessage);
            Assert.IsTrue(parameters.ErrorMessage.Contains("bad.zip"));
        }

        [TestMethod]
        public void TestBadAdditionalFile()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var additionalFiles = new List<IGameFile> { new GameFile() { FileName = "badadd.zip" } };
            var features = new List<ILaunchFeature>()
            {
                new GameFilesLaunchFeature(additionalFiles, null)
            };

            GameLauncher adapter = new GameLauncher(directories, features);

            var parameters = adapter.GetLaunchParameters(GetTestFile(), GetAddFiles(), GetTestPort(".wad,.deh"), false);
            Assert.IsTrue(parameters.Failed);
            Assert.IsNotNull(parameters.ErrorMessage);
            Assert.IsTrue(parameters.ErrorMessage.Contains("badadd.zip"));
        }

        [TestMethod]
        public void TestIndividualFiles()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            string[] wads;
            using (ZipArchive za = ZipFile.OpenRead(Path.Combine("GameFiles", GetTestFiles().First().FileName)))
                wads = za.Entries.Where(x => Path.GetExtension(x.Name) == ".wad").Select(x => x.FullName).ToArray();

            var features = new List<ILaunchFeature>()
            {
                new GameFilesLaunchFeature(null, wads.ToList<string>())
            };

            GameLauncher launcher = new GameLauncher(directories, features);

            var parameters = launcher.GetLaunchParameters(GetTestFiles().First(), GetAddFiles(), GetTestPort(".wad,.deh"), false);
            Assert.IsNull(parameters.ErrorMessage);
            Assert.IsTrue(parameters.LaunchString.Contains(".wad"));
            Assert.IsFalse(parameters.LaunchString.Contains(".deh"));
        }

        [TestMethod]
        public void TestIndividualFilesMultiple()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };


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

            var additionalFiles = GetTestFiles().Skip(1).Take(2).ToList(); //test2.wad, test3.wad only

            var features = new List<ILaunchFeature>()
            {
                new GameFilesLaunchFeature(additionalFiles, wads)
            };
            GameLauncher launcher = new GameLauncher(directories, features);

            var parameters = launcher.GetLaunchParameters(GetTestFiles().First(), GetAddFiles(), GetTestPort(".wad,.deh"), false);

            Assert.IsNull(parameters.ErrorMessage);
            Assert.IsTrue(parameters.LaunchString.Contains("test1.wad"));
            Assert.IsTrue(parameters.LaunchString.Contains("test2.wad"));
            Assert.IsTrue(parameters.LaunchString.Contains("test3.wad"));
            Assert.IsFalse(parameters.LaunchString.Contains("test4.wad"));
            Assert.IsFalse(parameters.LaunchString.Contains(".deh"));

            var tempPath = directories.TempDirectory.GetFullPath();
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test1.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test2.wad")));
            Assert.IsTrue(File.Exists(Path.Combine(tempPath, "test3.wad")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath, "test4.wad")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath, "test1.deh")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath, "test2.deh")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath, "test3.deh")));
            Assert.IsFalse(File.Exists(Path.Combine(tempPath, "test4.deh")));
        }

        [TestMethod]
        public void TestIndividualPathedFiles()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var specialFiles = new List<string> { Path.Combine("data", "test.wad") };
            var features = new List<ILaunchFeature>()
            {
                new GameFilesLaunchFeature(null, specialFiles)
            };

            GameLauncher adapter = new GameLauncher(directories, features);

            IGameFile gameFile = new GameFile() { FileName = "testpathed.zip" };

            var parameters = adapter.GetLaunchParameters(gameFile, GetAddFiles(), GetTestPort(".wad,.deh"), false);

            Assert.IsNull(parameters.ErrorMessage);
            Assert.IsTrue(parameters.LaunchString.Contains("test.wad"));
            Assert.IsTrue(File.Exists(Path.Combine(directories.TempDirectory.GetFullPath(), "test.wad")));
        }

        [TestMethod]
        public void TestExtraWithStats()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var features = new List<ILaunchFeature>()
            {
                new ExtraParametersLaunchFeature("-extratest", false),
                new SourcePortExtraParametersLaunchFeature(),
                new StatisticsReaderLaunchFeature()
            };

            GameLauncher launcher = new GameLauncher(directories, features);
            var boomPort = GetPrBoomTestPort(".wad,.deh");
            boomPort.ExtraParameters = "-boomextra";

            string launch = launcher.GetLaunchParameters(GetTestFile(), GetAddFiles(), boomPort, false).LaunchString;

            Assert.IsTrue(launch.Contains(" -extratest "));
            Assert.IsTrue(launch.Contains(" -boomextra "));
            Assert.IsTrue(launch.Contains(" -levelstat"));
        }

        [TestMethod]
        public void RelativeUnmanagedFiles()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var iwad = new GameFile() { FileName = LocalIwad1 };
            var features = new List<ILaunchFeature>()
            {
                new IWadLaunchFeature(iwad)
            };

            GameLauncher launcher = new GameLauncher(directories, features);

            var port = GetPrBoomTestPort(".wad,.deh");

            var gameFile = new GameFile() { FileName = LocalFile1 };
            string launch = launcher.GetLaunchParameters(gameFile, GetAddFiles(), port, false).LaunchString;
            string check = string.Format("-iwad \"{0}\" -file \"{1}\"",
                Path.Combine(Directory.GetCurrentDirectory(), LocalIwad1),
                Path.Combine(Directory.GetCurrentDirectory(), LocalFile1));
            Assert.AreEqual(check, launch);
        }

        [TestMethod]
        public void VariableReplacements()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var iwad = new GameFile() { FileName = LocalIwad1 };
            var features = new List<ILaunchFeature>()
            {
                new ExtraParametersLaunchFeature("-savedir $iwad/$filename", false),
                new IWadLaunchFeature(iwad)
            };

            var launcher = new GameLauncher(directories,features);
            var port = GetPrBoomTestPort(".wad,.deh");

            var gameFile = new GameFile() { FileName = LocalFile1 };
            var launch = launcher.GetLaunchParameters(gameFile, GetAddFiles(), port, false).LaunchString;
            Assert.IsTrue(launch.Contains("-savedir iwad1/file1"));
        }

        [TestMethod]
        public void ExtraParametersOnly()
        {
            IDirectoriesConfiguration directories = new DirectoriesConfiguration()
            {
                GameFileDirectory = new LauncherPath("GameFiles"),
                TempDirectory = new LauncherPath("Temp")
            };

            var iwad = new GameFile() { FileName = LocalIwad1 };
            var features = new List<ILaunchFeature>()
            {
                new ExtraParametersLaunchFeature("-blah -blob", true),
                new IWadLaunchFeature(iwad),
                new MapSkillLaunchFeature("E1M2", null)
            };

            var launcher = new GameLauncher(directories, features);
            var port = GetPrBoomTestPort(".wad,.deh");
            var gameFile = new GameFile() { FileName = LocalFile1 };
            var launch = launcher.GetLaunchParameters(gameFile, GetAddFiles(), port, false).LaunchString;
            Assert.IsTrue(launch.Contains("-blah -blob"));
            Assert.IsFalse(launch.Contains("E1M2")); 
            Assert.IsFalse(launch.Contains("iwad"));
        }

        private void CreateDirectoriesAndFiles()
        {
            if (Directory.Exists("GameFiles"))
                Directory.Delete("GameFiles", true);
            if (Directory.Exists("Temp"))
                Directory.Delete("Temp", true);
            if (Directory.Exists("SourcePorts"))
                Directory.Delete("SourcePorts", true);
            if (Directory.Exists("Local"))
                Directory.Delete("Local", true);

            Directory.CreateDirectory("GameFiles");
            CreateTestFiles();
            CreateTestPathedFile();

            Directory.CreateDirectory("Local");
            CreateRelativeFiles();

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

        private void CreateRelativeFiles()
        {
            File.WriteAllText(LocalFile1, "");
            File.WriteAllText(LocalIwad1, "");
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

        private static IGameFile[] GetAddFiles()
        {
            return Array.Empty<IGameFile>();
        }

        private static ISourcePortData GetTestPort(string extensions)
        {
            return new SourcePortData { Executable = "zdoom.exe", Directory = new LauncherPath("SourcePorts"), SupportedExtensions = extensions, FileOption = "-file" };
        }

        private static ISourcePortData GetPrBoomTestPort(string extensions)
        {
            return new SourcePortData { Executable = "prboom-plus.exe", Directory = new LauncherPath("SourcePorts"), SupportedExtensions = extensions, FileOption = "-file" };
        }
    }
}
