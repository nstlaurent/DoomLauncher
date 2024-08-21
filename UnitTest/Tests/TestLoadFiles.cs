using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestLoadFiles
    {
        IDataSourceAdapter database;

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
            dataAccess.ExecuteNonQuery("delete from IWads");
            dataAccess.ExecuteNonQuery("delete from SourcePorts");
        }

        [TestMethod]
        public void GetIWadFiles_ReturnsFilesDirectlyAssociatedWithTheIWad()
        {
            var gameProfile = new GameFile
            {
                FileName = "foo.wad",
                SettingsFilesIWAD = "green.wad;blue.wad"
            };
            database.InsertGameFile(gameProfile);

            var gameFile1 = new GameFile // Won't get used
            { 
                FileName = "whocares.wad",
                SettingsFilesIWAD = "nope.wad"
            }; 
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "green.wad" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "blue.wad" };
            database.InsertGameFile(gameFile3);

            var wrongGameFile = new GameFile { FileName = "nope.wad" };
            database.InsertGameFile(wrongGameFile);

            var fileLoadHandler = new FileLoadHandler(database, gameFile1, gameProfile);
            var iwadFiles = fileLoadHandler.GetIWadFiles();
            Assert.AreEqual(2, iwadFiles.Count);
            Assert.IsTrue(iwadFiles.Exists(x => x.FileName.Equals("green.wad")));
            Assert.IsTrue(iwadFiles.Exists(x => x.FileName.Equals("blue.wad")));

            Assert.IsFalse(fileLoadHandler.IsIWadFile(gameProfile));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(gameFile1));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(wrongGameFile));
            Assert.IsTrue(fileLoadHandler.IsIWadFile(gameFile2));
            Assert.IsTrue(fileLoadHandler.IsIWadFile(gameFile3));
        }


        [TestMethod]
        public void GetIWadFiles_ReturnsFilesIndirectlyAssociatedWithTheIWad()
        {
            // if the iwad has a file attached to it because of a source port, that file should not be included when the iwad is selected
            // e.g. GZDoom has noendgame.wad User launched DOOM2.WAD with GZDoom. This adds noendgame to the additional files of DOOM2.WAD.
            // if the user selects DOOM2.WAD with another file (and not GZDoom as the port), noendgame.wad should not be added


            var iwadGameFile = InsertIWadAndGameFile("DOOM2.zip", 
                settingsFiles: "rightfile.zip;ignoreme1.zip", 
                settingsFilesSourcePort: "ignoreme1.zip"); // Should cancel out the same file in SettingsFiles

            var gameProfile = new GameFile
            {
                FileName = "profile.wad",
                SettingsFilesIWAD = "ignoreme2.wad", // Should get ignored because there is an IWadID
                IWadID = iwadGameFile.IWadID

            };
            database.InsertGameFile(gameProfile);

            var gameFile1 = new GameFile // Not used to find the IWAD additional files
            {
                FileName = "whoCares.wad",
                SettingsFilesIWAD = "ignoreme3.wad",
                IWadID = iwadGameFile.IWadID

            };
            database.InsertGameFile(gameFile1);

            var rightFile = new GameFile { FileName = "rightfile.zip" };
            database.InsertGameFile(rightFile);

            var ignoreFile1 = new GameFile { FileName = "ignoreme1.zip" };
            database.InsertGameFile(ignoreFile1);

            var ignoreFile2 = new GameFile { FileName = "ignoreme2.zip" };
            database.InsertGameFile(ignoreFile2);

            var ignoreFile3 = new GameFile { FileName = "ignoreme3.zip" };
            database.InsertGameFile(ignoreFile3);


            var fileLoadHandler = new FileLoadHandler(database, gameFile1, gameProfile);
            var iwadFiles = fileLoadHandler.GetIWadFiles();
            Assert.AreEqual(1, iwadFiles.Count);
            Assert.AreEqual("rightfile.zip", iwadFiles.First().FileName);
            Assert.IsTrue(fileLoadHandler.IsIWadFile(rightFile));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(gameFile1));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(ignoreFile1));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(ignoreFile2));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(ignoreFile3));
            Assert.IsFalse(fileLoadHandler.IsIWadFile(iwadGameFile));
        }

        [TestMethod]
        public void GetSourcePortFiles_ReturnsFilesAssociatedWithTheSourcePort()
        {
            var gameProfile = new GameFile
            {
                FileName = "blah.wad",
                SettingsFilesSourcePort = "a.wad;b.wad"
            };
            database.InsertGameFile(gameProfile);

            var gameFile1 = new GameFile { FileName = "ignore me.wad" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "a.wad" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "b.wad" };
            database.InsertGameFile(gameFile3);

            var fileLoadHandler = new FileLoadHandler(database, gameFile1, gameProfile);
            var sourcePortFiles = fileLoadHandler.GetSourcePortFiles();
            Assert.AreEqual(2, sourcePortFiles.Count);
            Assert.IsTrue(sourcePortFiles.Exists(x => x.FileName.Equals("a.wad")));
            Assert.IsTrue(sourcePortFiles.Exists(x => x.FileName.Equals("b.wad")));
            Assert.IsFalse(fileLoadHandler.IsSourcePortFile(gameFile1));
            Assert.IsFalse(fileLoadHandler.IsSourcePortFile(gameProfile));
            Assert.IsTrue(fileLoadHandler.IsSourcePortFile(gameFile2));
            Assert.IsTrue(fileLoadHandler.IsSourcePortFile(gameFile3));
        }

        [TestMethod]
        public void GetCurrentAdditionalFiles_ReturnsAssociatedFiles()
        {
            var gameProfile = new GameFile
            {
                FileName = "PROFILE.wad",
                SettingsFiles = "first.wad;second.wad"
            };
            database.InsertGameFile(gameProfile);

            var gameFile1 = new GameFile 
            { 
                FileName = "myhouse.wad",
                SettingsFiles = "nope.wad;nah.wad" // These should be ignored
            };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "first.wad" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "second.wad" };
            database.InsertGameFile(gameFile3);

            var wrongGameFile = new GameFile { FileName = "random.wad" };
            database.InsertGameFile(wrongGameFile);

            var fileLoadHandler = new FileLoadHandler(database, gameFile1, gameProfile);
            var currentAdditionalFiles = fileLoadHandler.GetCurrentAdditionalFiles();

            Assert.AreEqual(3, currentAdditionalFiles.Count());
            Assert.IsTrue(currentAdditionalFiles.Exists(x => x.FileName.Equals("myhouse.wad")));
            Assert.IsTrue(currentAdditionalFiles.Exists(x => x.FileName.Equals("first.wad")));
            Assert.IsTrue(currentAdditionalFiles.Exists(x => x.FileName.Equals("second.wad")));
        }

        [TestMethod]
        public void CalculateAdditionalFiles_IfNoIWadThenClearIWadAndSourcePortFiles()
        {
            var gameProfile = new GameFile
            {
                FileName = "the_profile.wad",
                SettingsFilesSourcePort = "sauce_port.zip",
                SettingsFilesIWAD = "eye_wad.zip"
            };
            database.InsertGameFile(gameProfile);

            var gameFile1 = new GameFile { FileName = "file IGNORE.wad" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "sauce_port IGNORE.wad" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "eye_wad IGNORE.wad" };
            database.InsertGameFile(gameFile3);

            var fileLoadHandler = new FileLoadHandler(database, gameFile1, gameProfile);
            fileLoadHandler.CalculateAdditionalFiles(null, null);

            Assert.AreEqual(0, fileLoadHandler.GetIWadFiles().Count);
            Assert.AreEqual(0, fileLoadHandler.GetSourcePortFiles().Count);
        }

        [TestMethod]
        public void CalculateAdditionalFiles_IfIWadIsGameFileThenIgnoreIWadAdditionalFiles()
        {
            // This was a bug, for an iwad like DOOM2 Doom Launcher sets the SettingsFile to DOOM2.WAD if the user loaded it.
            // We need to exclude this when using DOOM2 as the iwad for another file

            var gameProfile = new GameFile { FileName = "game-profiley.wad" };
            database.InsertGameFile(gameProfile);

            var iwadGameFile = InsertIWadAndGameFile("iwad.zip",
                settingsFiles: "chooseme.zip;ignoreme.zip",
                settingsFilesSourcePort: "ignoreme.zip"); // Should cancel out the same file in SettingsFiles

            var gameFile1 = new GameFile { FileName = "chooseme.zip" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "ignoreme.zip" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "secondone.zip" };
            database.InsertGameFile(gameFile3);

            var sourcePort1 = new SourcePortData { Name = "Floom", SettingsFiles = "secondone.zip" };
            database.InsertSourcePort(sourcePort1);

            var fileLoadHandler = new FileLoadHandler(database, iwadGameFile, gameProfile);

            fileLoadHandler.CalculateAdditionalFiles(iwadGameFile, sourcePort1);

            Assert.AreEqual(0, fileLoadHandler.GetIWadFiles().Count);

            Assert.AreEqual(1, fileLoadHandler.GetSourcePortFiles().Count);
            Assert.AreEqual("secondone.zip", fileLoadHandler.GetSourcePortFiles().First().FileName);
        }

        [TestMethod]
        public void CalculateAdditionalFiles_IfIWadIsNotGameFileThenTakeIWadAdditionalFiles()
        {
            var gameProfile = new GameFile { FileName = "game-profilex.wad" };
            database.InsertGameFile(gameProfile);

            var iwadGameFile = InsertIWadAndGameFile("iwady.zip",
                settingsFiles: "choosemee.zip;ignoremee.zip",
                settingsFilesSourcePort: "ignoremee.zip"); // Should cancel out the same file in SettingsFiles

            var theGameFile = new GameFile { FileName = "the-game-file.zip" };
            database.InsertGameFile(theGameFile);

            var gameFile1 = new GameFile { FileName = "choosemee.zip" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "ignoremee.zip" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "2nd-one.zip" };
            database.InsertGameFile(gameFile3);

            var sourcePort1 = new SourcePortData { Name = "Broom", SettingsFiles = "2nd-one.zip" };
            database.InsertSourcePort(sourcePort1);

            var fileLoadHandler = new FileLoadHandler(database, theGameFile, gameProfile);

            fileLoadHandler.CalculateAdditionalFiles(iwadGameFile, sourcePort1);

            Assert.AreEqual(1, fileLoadHandler.GetIWadFiles().Count);
            Assert.AreEqual("choosemee.zip", fileLoadHandler.GetIWadFiles().First().FileName);

            Assert.AreEqual(1, fileLoadHandler.GetSourcePortFiles().Count);
            Assert.AreEqual("2nd-one.zip", fileLoadHandler.GetSourcePortFiles().First().FileName);
        }

        [TestMethod]
        public void CalculateAdditionalFiles_CorrectlyExcludesThePreviousResults()
        {
            // Set up first calculation
            var gameProfile = new GameFile { FileName = "the-game-profile.wad" };
            database.InsertGameFile(gameProfile);

            var theGameFile = new GameFile { FileName = "GAME_FILE.zip" };
            database.InsertGameFile(theGameFile);

            var iwadGameFile1 = InsertIWadAndGameFile("IWAD1.zip",
                settingsFiles: "right1.zip;right2.zip;wrong1.zip",
                settingsFilesSourcePort: "wrong1.zip"); // Should cancel out the same file in SettingsFiles

            var sourcePort1 = new SourcePortData { Name = "Broom", SettingsFiles = "sp_right3.zip" };
            database.InsertSourcePort(sourcePort1);

            var gameFile1 = new GameFile { FileName = "right1.zip" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "right2.zip" };
            database.InsertGameFile(gameFile2);

            var gameFile3 = new GameFile { FileName = "sp_right3.zip" };
            database.InsertGameFile(gameFile3);

            var gameFile4 = new GameFile { FileName = "wrong1.zip" };
            database.InsertGameFile(gameFile4);

            var fileLoadHandler = new FileLoadHandler(database, theGameFile, gameProfile);

            // First calculation
            fileLoadHandler.CalculateAdditionalFiles(iwadGameFile1, sourcePort1);

            // Additional files includes the game file, plus the added ones
            var additionalFiles1 = fileLoadHandler.GetCurrentAdditionalFiles();
            Assert.AreEqual(4, additionalFiles1.Count);
            Assert.IsTrue(additionalFiles1.Exists(x => x.FileName.Equals("GAME_FILE.zip"))); 
            Assert.IsTrue(additionalFiles1.Exists(x => x.FileName.Equals("right1.zip")));
            Assert.IsTrue(additionalFiles1.Exists(x => x.FileName.Equals("right2.zip")));
            Assert.IsTrue(additionalFiles1.Exists(x => x.FileName.Equals("sp_right3.zip")));

            // They are all new except the game file, which was already there since creation
            var newAdditionalFiles1 = fileLoadHandler.GetCurrentAdditionalNewFiles();
            Assert.AreEqual(3, newAdditionalFiles1.Count);
            Assert.IsTrue(newAdditionalFiles1.Exists(x => x.FileName.Equals("right1.zip")));
            Assert.IsTrue(newAdditionalFiles1.Exists(x => x.FileName.Equals("right2.zip")));
            Assert.IsTrue(newAdditionalFiles1.Exists(x => x.FileName.Equals("sp_right3.zip")));

            // Set up second calculation
            var iwadGameFile2 = InsertIWadAndGameFile("IWAD2.zip",
                settingsFiles: "right1.zip;NEWRIGHT.ZIP;wrong1.zip", // NEWRIGHT.ZIP instead of right2.zip
                settingsFilesSourcePort: "wrong1.zip"); // Should cancel out the same file in SettingsFiles

            var sourcePort2 = new SourcePortData { Name = "Vroom", SettingsFiles = "sp_right3.zip" }; // Same as the other one
            database.InsertSourcePort(sourcePort2);

            var gameFile5 = new GameFile { FileName = "NEWRIGHT.ZIP" };
            database.InsertGameFile(gameFile5);

            // Second calculation
            fileLoadHandler.CalculateAdditionalFiles(iwadGameFile2, sourcePort2);

            // Now has NEWRIGHT.ZIP instead of right2.zip
            var additionalFiles2 = fileLoadHandler.GetCurrentAdditionalFiles();
            Assert.AreEqual(4, additionalFiles2.Count);
            Assert.IsTrue(additionalFiles2.Exists(x => x.FileName.Equals("GAME_FILE.zip")));
            Assert.IsTrue(additionalFiles2.Exists(x => x.FileName.Equals("right1.zip")));
            Assert.IsTrue(additionalFiles2.Exists(x => x.FileName.Equals("NEWRIGHT.ZIP")));
            Assert.IsTrue(additionalFiles2.Exists(x => x.FileName.Equals("sp_right3.zip")));

            // Only NEWRIGHT.ZIP is new now
            var newAdditionalFiles2 = fileLoadHandler.GetCurrentAdditionalNewFiles();
            Assert.AreEqual(1, newAdditionalFiles2.Count);
            Assert.AreEqual("NEWRIGHT.ZIP", newAdditionalFiles2.First().FileName); // Only this one is new since the first calculation
        }

        [TestMethod]
        public void CalculateAdditionalFiles_AdditionalFilesAreOrderedByIWadFilesThenByEarliest()
        {
            // Set up first calculation
            var gameProfile = new GameFile
            {
                FileName = "GAME_PROFILE.wad",
                SettingsFiles = "og_yes1.zip;og_yes2.zip"
            };
            database.InsertGameFile(gameProfile);

            var theGameFile = new GameFile { FileName = "THE_GAME_FILE.zip" };
            database.InsertGameFile(theGameFile);

            var gameFile1 = new GameFile { FileName = "og_yes1.zip" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "og_yes2.zip" };
            database.InsertGameFile(gameFile2);

            var fileLoadHandler = new FileLoadHandler(database, theGameFile, gameProfile);

            // First calculation
            fileLoadHandler.CalculateAdditionalFiles(null, null);
            Assert.AreEqual(3, fileLoadHandler.GetCurrentAdditionalFiles().Count());

            // Set up second calculation
            var iwadGameFile1 = InsertIWadAndGameFile("IWAD1.zip",
                settingsFiles: "iw_yes3.zip;iw_yes4.zip",
                settingsFilesSourcePort: "");

            var sourcePort1 = new SourcePortData { Name = "GLoom", SettingsFiles = "sp_yes5.zip;sp_yes6.zip" };
            database.InsertSourcePort(sourcePort1);

            var gameFile3 = new GameFile { FileName = "iw_yes3.zip" };
            database.InsertGameFile(gameFile3);

            var gameFile4 = new GameFile { FileName = "iw_yes4.zip" };
            database.InsertGameFile(gameFile4);

            var gameFile5 = new GameFile { FileName = "sp_yes5.zip" };
            database.InsertGameFile(gameFile5);

            var gameFile6 = new GameFile { FileName = "sp_yes6.zip" };
            database.InsertGameFile(gameFile6);

            // Second calculation
            fileLoadHandler.CalculateAdditionalFiles(iwadGameFile1, sourcePort1);

            var additionalFiles1 = fileLoadHandler.GetCurrentAdditionalFiles();
            Assert.AreEqual(7, additionalFiles1.Count());

            // IWad files are first
            Assert.AreEqual("iw_yes3.zip", additionalFiles1[0].FileName);
            Assert.AreEqual("iw_yes4.zip", additionalFiles1[1].FileName);

            // The GameFile was the first original file
            Assert.AreEqual("THE_GAME_FILE.zip", additionalFiles1[2].FileName);

            // Files from the first calculation are next
            Assert.AreEqual("og_yes1.zip", additionalFiles1[3].FileName);
            Assert.AreEqual("og_yes2.zip", additionalFiles1[4].FileName);

            // Then the rest
            Assert.AreEqual("sp_yes5.zip", additionalFiles1[5].FileName);
            Assert.AreEqual("sp_yes6.zip", additionalFiles1[6].FileName);
        }

        [TestMethod]
        public void Reset_RestoresAdditionalFilesToCreationState()
        {
            // Set up creation state
            var gameProfile = new GameFile
            {
                FileName = "the-game-profile.wad",
                SettingsFiles = "file1.zip;file2.zip"
            };
            database.InsertGameFile(gameProfile);

            var theGameFile = new GameFile { FileName = "the-game-file.zip" };
            database.InsertGameFile(theGameFile);

            var gameFile1 = new GameFile { FileName = "file1.zip" };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile { FileName = "file2.zip" };
            database.InsertGameFile(gameFile2);

            var fileLoadHandler = new FileLoadHandler(database, theGameFile, gameProfile);
            Assert.AreEqual(3, fileLoadHandler.GetCurrentAdditionalFiles().Count);

            // Set up new state
            var iwadGameFile1 = InsertIWadAndGameFile("the-iwad.zip",
                settingsFiles: "new1.zip;new2.zip",
                settingsFilesSourcePort: "");

            var gameFile3 = new GameFile { FileName = "new1.zip" };
            database.InsertGameFile(gameFile3);

            var gameFile4 = new GameFile { FileName = "new2.zip" };
            database.InsertGameFile(gameFile4);

            fileLoadHandler.CalculateAdditionalFiles(iwadGameFile1, null);
            Assert.AreEqual(5, fileLoadHandler.GetCurrentAdditionalFiles().Count);

            // Reset original state
            fileLoadHandler.Reset();
            var additionalFiles = fileLoadHandler.GetCurrentAdditionalFiles();
            Assert.AreEqual(3, additionalFiles.Count());
            Assert.IsTrue(additionalFiles.Exists(x => x.FileName.Equals("the-game-file.zip")));
            Assert.IsTrue(additionalFiles.Exists(x => x.FileName.Equals("file1.zip")));
            Assert.IsTrue(additionalFiles.Exists(x => x.FileName.Equals("file2.zip")));
        }

        private IGameFile InsertIWadAndGameFile(String fileName, String settingsFiles, String settingsFilesSourcePort)
        {
            IGameFile iwadGameFile = new GameFile
            {
                FileName = fileName,
                SettingsFiles = settingsFiles,
                SettingsFilesSourcePort = settingsFilesSourcePort
            };
            database.InsertGameFile(iwadGameFile);
            iwadGameFile = database.GetGameFile(fileName);

            IIWadData iwad1 = new IWadData
            {
                Name = fileName,
                GameFileID = iwadGameFile.GameFileID
            };
            database.InsertIWad(iwad1);
            iwad1 = database.GetIWad((int)iwadGameFile.GameFileID);
            iwadGameFile.IWadID = iwad1.IWadID;
            database.UpdateGameFile(iwadGameFile);

            return iwadGameFile;
        }
    }
}
