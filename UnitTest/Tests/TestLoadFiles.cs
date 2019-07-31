using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
using DoomLauncher;
using System.Linq;
using DoomLauncher.Handlers;
using System.Collections.Generic;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestLoadFiles
    {
        private static string[] s_iwads = new[] { "DOOM.WAD", "DOOM2.WAD", "TNT.WAD" };
        private static string[] s_files = new[] { "GAMEFILE1.WAD", "GAMEFILE2.WAD", "GAMEFILE3.WAD" };
        private static string[] s_mods = new[] { "SUPERCOOLMOD.WAD", "MOD2.WAD", "MOD3.WAD", "MOD4.WAD", "PORTMOD1.WAD", "PORTMOD2.WAD", "IWADMOD1.WAD" };

        [TestMethod]
        public void TestFiles()
        {
            CreateDatabase();

            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var gameFiles = Util.GetAdditionalFiles(adapter, adapter.GetGameFile("COOLGAMEFILE.WAD"));

            Assert.AreEqual(2, gameFiles.Count);
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "SUPERCOOLMOD.WAD"));
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "MOD2.WAD"));

            gameFiles = Util.GetAdditionalFiles(adapter, adapter.GetGameFile("OTHERGAMEFILE.WAD"));
            Assert.AreEqual(2, gameFiles.Count);
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "MOD3.WAD"));
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "MOD4.WAD"));

            gameFiles = Util.GetAdditionalFiles(adapter, adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe"));
            Assert.AreEqual(1, gameFiles.Count);
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "PORTMOD1.WAD"));
        }

        [TestMethod]
        public void TestSimpleFileWithIwad()
        {
            //This was a bug, for an iwad like DOOM2 Doom Launcher sets the SettingsFile to DOOM2.WAD if the user loaded it. We need to exclude this when using DOOM2 as the iwad for another file
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "DOOM2.WAD");
            iwad.SettingsFiles = "DOOM2.WAD";
            adapter.UpdateGameFile(iwad);

            FileLoadHandler handler = new FileLoadHandler(adapter, adapter.GetGameFile("COOLGAMEFILE.WAD"));

            iwad = adapter.GetGameFileIWads().First(x => x.FileName == "DOOM2.WAD"); 
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");
            handler.CalculateAdditionalFiles(iwad, sourceport);

            Assert.IsNull(handler.GetCurrentAdditionalFiles().FirstOrDefault(x => x.FileName == "DOOM2.WAD"));
        }

        [TestMethod]
        public void TestHandlerSourcePort()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            FileLoadHandler handler = new FileLoadHandler(adapter, adapter.GetGameFile("COOLGAMEFILE.WAD"));
            Assert.AreEqual(3, handler.GetCurrentAdditionalFiles().Count);

            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "DOOM.WAD"); //no additional files for iwad
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            var gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(4, gameFiles.Count);
            Assert.IsNotNull(gameFiles.FirstOrDefault(x => x.FileName == "PORTMOD1.WAD"));

            //no change
            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.IsNotNull(gameFiles.First(x => x.FileName == "PORTMOD1.WAD"));

            Assert.IsFalse(handler.IsIWadFile(gameFiles.First(x => x.FileName == "PORTMOD1.WAD")));
            Assert.IsTrue(handler.IsSourcePortFile(gameFiles.First(x => x.FileName == "PORTMOD1.WAD")));
        }

        [TestMethod]
        public void TestHandlerIWad()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            FileLoadHandler handler = new FileLoadHandler(adapter, adapter.GetGameFile("COOLGAMEFILE.WAD"));
            Assert.AreEqual(3, handler.GetCurrentAdditionalFiles().Count);

            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "PLUTONIA.WAD");
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "odamex.exe"); //no additional files for port

            handler.CalculateAdditionalFiles(iwad, sourceport);
            var gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(4, gameFiles.Count);
            Assert.IsNotNull(gameFiles.FirstOrDefault(x => x.FileName == "IWADMOD1.WAD"));

            //no change
            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.IsNotNull(gameFiles.FirstOrDefault(x => x.FileName == "IWADMOD1.WAD"));

            Assert.IsTrue(handler.IsIWadFile(gameFiles.First(x => x.FileName == "IWADMOD1.WAD")));
            Assert.IsFalse(handler.IsSourcePortFile(gameFiles.First(x => x.FileName == "IWADMOD1.WAD")));
        }

        [TestMethod]
        public void TestHandlerMixed()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            FileLoadHandler handler = new FileLoadHandler(adapter, adapter.GetGameFile("COOLGAMEFILE.WAD"));
            Assert.AreEqual(3, handler.GetCurrentAdditionalFiles().Count);

            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "PLUTONIA.WAD");
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            var gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(5, gameFiles.Count);
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "IWADMOD1.WAD"));
            Assert.IsNotNull(gameFiles.First(x => x.FileName == "PORTMOD1.WAD"));

            Assert.IsTrue(handler.IsIWadFile(gameFiles.First(x => x.FileName == "IWADMOD1.WAD")));
            Assert.IsTrue(handler.IsSourcePortFile(gameFiles.First(x => x.FileName == "PORTMOD1.WAD")));
        }

        [TestMethod]
        public void TestHandlerOrder()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var mainFile = adapter.GetGameFile("COOLGAMEFILE.WAD");
            FileLoadHandler handler = new FileLoadHandler(adapter, mainFile);
            Assert.AreEqual(3, handler.GetCurrentAdditionalFiles().Count);

            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "PLUTONIA.WAD");
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            var gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(5, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[1].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[2].FileName);
            Assert.AreEqual("IWADMOD1.WAD", gameFiles[3].FileName);
            Assert.AreEqual("PORTMOD1.WAD", gameFiles[4].FileName);

            //user changed the order - the files should be returned in the order the user set
            string newOrder = "IWADMOD1.WAD;SUPERCOOLMOD.WAD;PORTMOD1.WAD;MOD2.WAD";
            mainFile.SettingsFiles = newOrder;
            handler = new FileLoadHandler(adapter, mainFile);

            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(5, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("IWADMOD1.WAD", gameFiles[1].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[2].FileName);
            Assert.AreEqual("PORTMOD1.WAD", gameFiles[3].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[4].FileName);

            //change source port - PORTMOD1 will be removed, PORTMOD2 should be added last
            sourceport = adapter.GetSourcePorts().First(x => x.Name == "prboom.exe");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(5, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("IWADMOD1.WAD", gameFiles[1].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[2].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[3].FileName);
            Assert.AreEqual("PORTMOD2.WAD", gameFiles[4].FileName);

            //change iwad - IWADMOD1 should be removed
            iwad = adapter.GetGameFileIWads().First(x => x.FileName == "DOOM2.WAD");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(4, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[1].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[2].FileName);
            Assert.AreEqual("PORTMOD2.WAD", gameFiles[3].FileName);

            //set source port back - order has chaged, PORTMOD1 will be last
            sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(4, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[1].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[2].FileName);
            Assert.AreEqual("PORTMOD1.WAD", gameFiles[3].FileName);

            //RESET - should load original files, but with new order specified in the gamefile
            handler.Reset();

            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(5, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("IWADMOD1.WAD", gameFiles[1].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[2].FileName);
            Assert.AreEqual("PORTMOD1.WAD", gameFiles[3].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[4].FileName);
        }

        [TestMethod]
        public void TestIWadExclude()
        {
            //if the iwad has a file attached to it because of a source port, that file should not be included when the iwad is selected
            //e.g. GZDoom has noendgame.wad User launched DOOM2.WAD with GZDoom. This adds noendgame to the additional files of DOOM2.WAD.
            //if the user selects DOOM2.WAD with another file (and not GZDoom as the port), noendgame.wad should not be added

            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var mainFile = adapter.GetGameFile("COOLGAMEFILE.WAD");
            FileLoadHandler handler = new FileLoadHandler(adapter, mainFile);
            Assert.AreEqual(3, handler.GetCurrentAdditionalFiles().Count);

            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "DOOM2.WAD");
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");

            iwad.SettingsFiles = "MOD4.WAD";
            adapter.UpdateGameFile(iwad);

            handler.CalculateAdditionalFiles(iwad, sourceport);
            var gameFiles = handler.GetCurrentAdditionalFiles();
            Assert.AreEqual(5, gameFiles.Count);
            Assert.IsNotNull(gameFiles.FirstOrDefault(x => x.FileName == "MOD4.WAD"));

            //now set up the file so it should be excluded
            iwad.SettingsFilesSourcePort = "MOD4.WAD";
            adapter.UpdateGameFile(iwad);

            handler = new FileLoadHandler(adapter, mainFile);
            handler.CalculateAdditionalFiles(iwad, sourceport);
            gameFiles = handler.GetCurrentAdditionalFiles();
            Assert.AreEqual(4, gameFiles.Count);
            Assert.IsNull(gameFiles.FirstOrDefault(x => x.FileName == "MOD4.WAD"));
        }

        [TestMethod]
        public void TestUserSet()
        {
            //base test
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var mainFile = adapter.GetGameFile("COOLGAMEFILE.WAD");
            FileLoadHandler handler = new FileLoadHandler(adapter, mainFile);
            Assert.AreEqual(3, handler.GetCurrentAdditionalFiles().Count);

            var iwad = adapter.GetGameFileIWads().First(x => x.FileName == "PLUTONIA.WAD");
            var sourceport = adapter.GetSourcePorts().First(x => x.Name == "zdoom.exe");

            handler.CalculateAdditionalFiles(iwad, sourceport);
            var gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(5, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[1].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[2].FileName);
            Assert.AreEqual("IWADMOD1.WAD", gameFiles[3].FileName);
            Assert.AreEqual("PORTMOD1.WAD", gameFiles[4].FileName);

            //**user removes iwad add file
            var addFiles = handler.GetCurrentAdditionalFiles();
            addFiles.RemoveAll(x => x.FileName == "IWADMOD1.WAD");
            SetAddFiles(mainFile, handler, addFiles);

            handler = new FileLoadHandler(adapter, mainFile);
            //do not call calculate, this will reset user set files
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(4, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[1].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[2].FileName);
            Assert.AreEqual("PORTMOD1.WAD", gameFiles[3].FileName);

            //**user removes source port add file
            addFiles = handler.GetCurrentAdditionalFiles();
            addFiles.RemoveAll(x => x.FileName == "PORTMOD1.WAD");
            SetAddFiles(mainFile, handler, addFiles);

            handler = new FileLoadHandler(adapter, mainFile);
            //do not call calculate, this will reset user set files
            gameFiles = handler.GetCurrentAdditionalFiles();

            Assert.AreEqual(3, gameFiles.Count);
            Assert.AreEqual("COOLGAMEFILE.WAD", gameFiles[0].FileName);
            Assert.AreEqual("SUPERCOOLMOD.WAD", gameFiles[1].FileName);
            Assert.AreEqual("MOD2.WAD", gameFiles[2].FileName);
        }

        private static void SetAddFiles(IGameFile mainFile, FileLoadHandler handler, List<IGameFile> addFiles)
        {
            mainFile.SettingsFiles = string.Join(";", addFiles.Select(x => x.FileName));
            mainFile.SettingsFilesIWAD = string.Join(";", addFiles.Intersect(handler.GetIWadFiles()).Select(x => x.FileName));
            mainFile.SettingsFilesSourcePort = string.Join(";", addFiles.Intersect(handler.GetSourcePortFiles()).Select(x => x.FileName));
        }

        private void CreateDatabase()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();

            Array.ForEach(s_iwads, x => CreateGameFile(adapter, x, true, string.Empty));
            Array.ForEach(s_files, x => CreateGameFile(adapter, x, false, string.Empty));
            Array.ForEach(s_mods, x => CreateGameFile(adapter, x, false, string.Empty));
            CreateGameFile(adapter, "PLUTONIA.WAD", true, "IWADMOD1.WAD");
            CreateGameFile(adapter, "COOLGAMEFILE.WAD", false, "SUPERCOOLMOD.WAD;MOD2.WAD;");
            CreateGameFile(adapter, "OTHERGAMEFILE.WAD", false, "MOD3.WAD;MOD4.WAD;");
            CreateSourcePort(adapter, "zdoom.exe", "PORTMOD1.WAD");
            CreateSourcePort(adapter, "prboom.exe", "PORTMOD2.WAD");
            CreateSourcePort(adapter, "odamex.exe", string.Empty);
        }

        private static void CreateGameFile(IDataSourceAdapter adapter, string name, bool isiwad, string addfiles)
        {
            GameFile gameFile = new GameFile()
            {
                FileName = name,
                Title = name,
                SettingsFiles = addfiles
            };

            adapter.InsertGameFile(gameFile);

            if (isiwad)
            {
                IWadData iwad = new IWadData()
                {
                    FileName = name,
                    Name = name,
                    GameFileID = adapter.GetGameFile(name).GameFileID
                };

                adapter.InsertIWad(iwad);
            }
        }

        private static void CreateSourcePort(IDataSourceAdapter adapter, string name, string addfiles)
        {
            SourcePortData sourcePort = new SourcePortData
            {
                Executable = name,
                Name = name,
                Directory = new LauncherPath("SourcePorts"),
                SupportedExtensions = ".wad",
                FileOption = "-file",
                SettingsFiles = addfiles
            };

            adapter.InsertSourcePort(sourcePort);
        }
    }
}
