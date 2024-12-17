using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.Steam;
using DoomLauncher;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestAutomaticSteamCheck
    {
        private IDataSourceAdapter database;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
        }

        [TestCleanup]
        public void CleanDatabase()
        {
            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
        }

        [TestMethod]
        public async Task AddGamesFromSteam_IgnoresWadsThatAreAlreadyThere()
        {
            // Already existing files
            database.InsertGameFile(new GameFile() { FileName = @"C:\gamefiles\doom.zip" });
            database.InsertGameFile(new GameFile() { FileName = "sigil.wad" });

            // Steam library containing "Ultimate Doom"
            var ultimateDoom = new SteamInstalledGame(SteamGame.ULTIMATE_DOOM,
                @"C:\SteamLib1\Ultimate Doom",
                new List<string>() { @"C:\SteamLib1\Ultimate Doom\doom.wad", @"C:\SteamLib1\Ultimate Doom\doom2.wad" },
                new List<string>() { @"C:\SteamLib1\Ultimate Doom\sigil.wad", @"C:\SteamLib1\Ultimate Doom\id1.wad" },
                null);

            var library = new SteamLibrary(@"C:\SteamLib1", new List<SteamInstalledGame>() { ultimateDoom });
            var steam = new SteamInstallation(@"C:\Steam", new List<SteamLibrary>() { library });

            var automaticSteamCheck = new AutomaticSteamCheck(() => steam, database);
            List<string> loadedIwads = new List<string>();
            List<string> loadedPwads = new List<string>();
            string loadedDoom64Exe = null;

            await automaticSteamCheck.LoadGamesFromSteam(
                async list => loadedIwads = list, 
                async list => loadedPwads = list,
                async exe => loadedDoom64Exe = exe);

            Assert.IsTrue(loadedIwads.Exists(file => file.Equals(@"C:\SteamLib1\Ultimate Doom\doom2.wad")));
            Assert.IsFalse(loadedIwads.Exists(file => file.Equals(@"C:\SteamLib1\Ultimate Doom\doom.wad"))); // Already exists
            Assert.IsTrue(loadedPwads.Exists(file => file.Equals(@"C:\SteamLib1\Ultimate Doom\id1.wad")));
            Assert.IsFalse(loadedPwads.Exists(file => file.Equals(@"C:\SteamLib1\Ultimate Doom\sigil.wad"))); // Already exists
        }

        [TestMethod]
        public async Task AddGamesFromSteam_DoesNothingIfNoIwads()
        {
            // Steam library containing "Ultimate Doom"
            var ultimateDoom = new SteamInstalledGame(SteamGame.ULTIMATE_DOOM,
                @"C:\SteamLib1\Ultimate Doom",
                new List<string>(), // No IWads
                new List<string>() { @"C:\SteamLib1\Ultimate Doom\sigil.wad" },
                null);

            var library = new SteamLibrary(@"C:\SteamLib1", new List<SteamInstalledGame>() { ultimateDoom });
            var steam = new SteamInstallation(@"C:\Steam", new List<SteamLibrary>() { library });

            var automaticSteamCheck = new AutomaticSteamCheck(() => steam, database);
            bool didTheIwadThing = false;
            bool didThePwadThing = false;
            bool didTheDoom64Thing = false;

            await automaticSteamCheck.LoadGamesFromSteam(
                async list => didTheIwadThing = true,
                async list => didThePwadThing = true,
                async exe => didTheDoom64Thing = true);

            Assert.IsTrue(didThePwadThing);
            Assert.IsFalse(didTheIwadThing);
            Assert.IsFalse(didTheDoom64Thing);
        }

        [TestMethod]
        public async Task AddGamesFromSteam_DoesNothingIfNoPwads()
        {
            // Steam library containing "Ultimate Doom"
            var ultimateDoom = new SteamInstalledGame(SteamGame.ULTIMATE_DOOM,
                @"C:\SteamLib1\Ultimate Doom",
                new List<string>() { @"C:\SteamLib1\Ultimate Doom\doom2.wad" }, 
                new List<string>(),
                null); // No PWads

            var library = new SteamLibrary(@"C:\SteamLib1", new List<SteamInstalledGame>() { ultimateDoom });
            var steam = new SteamInstallation(@"C:\Steam", new List<SteamLibrary>() { library });

            var automaticSteamCheck = new AutomaticSteamCheck(() => steam, database);
            bool didTheIwadThing = false;
            bool didThePwadThing = false;
            bool didTheDoom64Thing = false;

            await automaticSteamCheck.LoadGamesFromSteam(
                async list => didTheIwadThing = true,
                async list => didThePwadThing = true,
                async exe => didTheDoom64Thing = true);

            Assert.IsFalse(didThePwadThing);
            Assert.IsTrue(didTheIwadThing);
            Assert.IsFalse(didTheDoom64Thing);
        }

        [TestMethod]
        public async Task AddGamesFromSteam_SwallowsSteamLoaderExceptions()
        {
            var automaticSteamCheck = new AutomaticSteamCheck(() => throw new SteamLoaderException("Load Steam failed"), database);

            await automaticSteamCheck.LoadGamesFromSteam(
                async list => { },
                async list => { },
                async exe => { }); // No problem
        }
    }
}
