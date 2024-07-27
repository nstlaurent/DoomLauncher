using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameFile
    {
        private IDataSourceAdapter database;

        private GameFile CreateGameFile(string filename, int salt)
        {
            DateTime dt = DateTime.Parse("1/1/17");

            GameFile gameFile = new GameFile
            {
                FileName = filename,
                Author = "the author" + salt,
                Description = "the description" + salt,
                Title = "the title" + salt,
                Comments = "the comments" + salt,
                ReleaseDate = dt,
                Downloaded = dt,
                LastPlayed = dt,
                IWadID = 22 + salt,
                Rating = 33 + salt,
                MinutesPlayed = 44 + salt,
                MapCount = 55 + salt,
                SourcePortID = 66 + salt,
                SettingsExtraParams = "the extra params" + salt,
                SettingsFiles = "the settings files" + salt,
                SettingsFilesIWAD = "the settings files iwad" + salt,
                SettingsFilesSourcePort = "the settings files source port" + salt,
                SettingsMap = "the settings map" + salt,
                SettingsSkill = "the settings skill" + salt,
                SettingsSpecificFiles = "the settings specific files" + salt,
                SettingsGameProfileID = 567 + salt
            };

            return gameFile;
        }


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
        public void GetGameFiles_ReturnsAllGameFiles ()
        {
            var gameFile1 = new GameFile
            {
                FileName = "cacodemon.png",
                Author = "John Romero",
                Description = "Red ball of fury",
                Title = "Cacodemon image",
                Comments = "Hi res",
                ReleaseDate = DateTime.Parse("2/1/2018"),
                Downloaded = DateTime.Parse("3/5/2024"),
                LastPlayed = DateTime.Parse("4/5/2024"),
                IWadID = 9,
                Rating = 4,
                MinutesPlayed = 55,
                MapCount = 4,
                SourcePortID = 3,
                SettingsExtraParams = "extra",
                SettingsFiles = "settings file",
                SettingsFilesIWAD = "settings file iwad",
                SettingsFilesSourcePort = "settings file sp",
                SettingsMap = "settings map",
                SettingsSkill = "settings skill",
                SettingsSpecificFiles = "ssf",
                SettingsGameProfileID = 44
            };
            database.InsertGameFile(gameFile1);

            var gameFile2 = new GameFile
            {
                FileName = "baron.png",
                Author = "Bobby Prince",
                Description = "tough but fair",
                Title = "Baron of Hell",
                Comments = "Made from clay models",
                ReleaseDate = null,
                Downloaded = null,
                LastPlayed = null,
                IWadID = null,
                Rating = null,
                MinutesPlayed = 4,
                MapCount = null,
                SourcePortID = null,
                SettingsExtraParams = "aaa",
                SettingsFiles = "bbb",
                SettingsFilesIWAD = "ccc",
                SettingsFilesSourcePort = "ddd",
                SettingsMap = "eee",
                SettingsSkill = "fff",
                SettingsSpecificFiles = "ggg",
                SettingsGameProfileID = null
            };
            database.InsertGameFile(gameFile2);

            var gameFiles = database.GetGameFiles();

            Assert.AreEqual(gameFiles.Count(), 2);
            Assert.AreEqual(database.GetGameFiles().Count(), 2);
            Assert.AreEqual(database.GetGameFilesCount(), 2);
            Assert.AreEqual(database.GetGameFileNames().Count(), 2);

            var retrieved1 = gameFiles.Where(x => x.FileName.Equals(gameFile1.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile1, retrieved1, "GameFileID"));

            var retrieved2 = gameFiles.Where(x => x.FileName.Equals(gameFile2.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile2, retrieved2, "GameFileID"));
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsTheSpecifiedFields()
        {
            var gameFile1 = CreateGameFile("keen.zip", 0);
            gameFile1.Map = "Dangerous Dave";
            database.InsertGameFile(gameFile1);

            var fields = new GameFileFieldType[] 
            { 
                GameFileFieldType.Filename, 
                GameFileFieldType.Map 
            };

            var options = new GameFileGetOptions(fields);

            var gameFiles = database.GetGameFiles(options);
            Assert.AreEqual(1, gameFiles.Count());

            var retrievedGameFile1 = gameFiles.First();
            Assert.AreEqual("", retrievedGameFile1.Description);
            Assert.IsNull(retrievedGameFile1.Rating);
            Assert.AreEqual(gameFile1.FileName, retrievedGameFile1.FileName);
            Assert.AreEqual(gameFile1.Map, retrievedGameFile1.Map);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsMatching()
        {
            var gameFile1 = CreateGameFile("dukenukem.zip", 1);
            gameFile1.Rating = 5;
            gameFile1.Comments = "righto!!!";
            database.InsertGameFile(gameFile1);

            var wrongGameFile = CreateGameFile("jazzjackrabbit.zip", 2);
            database.InsertGameFile(wrongGameFile);

            var options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Filename, "dukenukem.zip"));

            var retrievedGameFiles = database.GetGameFiles(options);

            Assert.AreEqual(1, retrievedGameFiles.Count());
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile1, retrievedGameFiles.First(), "GameFileID"));
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsLike()
        {
            var gameFile1 = CreateGameFile("banana.zip", 3);
            database.InsertGameFile(gameFile1);

            var gameFile2 = CreateGameFile("banano.zip", 4);
            database.InsertGameFile(gameFile2);

            var gameFile3 = CreateGameFile("zanany.zip", 5);
            database.InsertGameFile(gameFile3);

            var wrongGameFile = CreateGameFile("cucumber.zip", 6);
            database.InsertGameFile(wrongGameFile);

            var options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Filename, GameFileSearchOp.Like, "%anan%"));

            var retrievedGameFiles = database.GetGameFiles(options);

            Assert.AreEqual(3, retrievedGameFiles.Count());

            var retrievedGameFile1 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile1.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile1);

            var retrievedGameFile2 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile2.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile2);

            var retrievedGameFile3 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile3.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile3);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsNotMatching()
        {
            var gameFile1 = CreateGameFile("batman.zip", 7);
            gameFile1.Comments = "b";
            database.InsertGameFile(gameFile1);

            var gameFile2 = CreateGameFile("robin.zip", 8);
            gameFile2.Comments = "r";
            database.InsertGameFile(gameFile2);

            var gameFile3 = CreateGameFile("joker.zip", 9);
            gameFile3.Comments = "j";
            database.InsertGameFile(gameFile3);

            var options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Comments, GameFileSearchOp.NotEqual, "r"));
            var retrievedGameFiles = database.GetGameFiles(options);

            Assert.AreEqual(2, retrievedGameFiles.Count());

            var retrievedGameFile1 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile1.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile1);

            var retrievedGameFile3 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile3.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile3);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsLessThan()
        {
            var gameFile1 = CreateGameFile("harpo.zip", 10);
            gameFile1.Rating = 1;
            database.InsertGameFile(gameFile1);

            var gameFile2 = CreateGameFile("groucho.zip", 11);
            gameFile2.Rating = 2;
            database.InsertGameFile(gameFile2);

            var gameFile3 = CreateGameFile("chico.zip", 12);
            gameFile3.Rating = 3;
            database.InsertGameFile(gameFile3);

            var options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Rating, GameFileSearchOp.LessThan, "3"));
            var retrievedGameFiles = database.GetGameFiles(options);

            Assert.AreEqual(2, retrievedGameFiles.Count());

            var retrievedGameFile1 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile1.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile1);

            var retrievedGameFile2 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile2.FileName)).FirstOrDefault();
            Assert.IsNotNull(retrievedGameFile2);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGreaterThan()
        {
            var gameFile1 = CreateGameFile("green.zip", 13);
            gameFile1.MapCount = 5;
            database.InsertGameFile(gameFile1);

            var gameFile2 = CreateGameFile("blue.zip", 14);
            gameFile2.MapCount = 6;
            database.InsertGameFile(gameFile2);

            var gameFile3 = CreateGameFile("red.zip", 15);
            gameFile3.MapCount = null;
            database.InsertGameFile(gameFile3);

            var options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.MapCount, GameFileSearchOp.GreaterThan, "5"));
            var retrievedGameFiles = database.GetGameFiles(options);

            Assert.AreEqual(1, retrievedGameFiles.Count());

            var retrievedGameFile2 = retrievedGameFiles.Where(x => x.FileName.Equals(gameFile2.FileName)).FirstOrDefault();
            Assert.AreEqual(gameFile2.FileName, "blue.zip");
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsMatchingWithSelectedFields()
        {
            var gameFile1 = CreateGameFile("doomslayer.zip", 16);
            gameFile1.Map = "E1M1";
            gameFile1.Author = "Adrian Carmack";
            database.InsertGameFile(gameFile1);

            var wrongGameFile = CreateGameFile("animalcrossing.zip", 16);
            database.InsertGameFile(wrongGameFile);

            var fields = new GameFileFieldType[]
            {
                GameFileFieldType.Map,
                GameFileFieldType.Author
            };

            var options = new GameFileGetOptions(fields, new GameFileSearchField(GameFileFieldType.Filename, "doomslayer.zip"));

            var retrievedGameFiles = database.GetGameFiles(options);

            Assert.AreEqual(1, retrievedGameFiles.Count());

            var retrievedGameFile1 = retrievedGameFiles.First();
            Assert.AreEqual(gameFile1.Map, retrievedGameFile1.Map);
            Assert.AreEqual(gameFile1.Author, retrievedGameFile1.Author);
        }

        [TestMethod]
        public void GetGameFile_ReturnsGameFileWithMatchingFilename()
        {
            var gameFile1 = CreateGameFile("blah.zip", 17);
            database.InsertGameFile(gameFile1);

            var wrongGameFile = CreateGameFile("wrongwrongwrong.zip", 18);
            database.InsertGameFile(wrongGameFile);

            var retrievedGameFile = database.GetGameFile("blah.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile1, retrievedGameFile, "GameFileID"));
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesFields()
        {
            var gameFile1 = CreateGameFile("explosion.zip", 20);
            database.InsertGameFile(gameFile1);

            var wrongGameFile = CreateGameFile("notme.zip", 21);
            database.InsertGameFile(wrongGameFile);

            var savedGameFile1 = database.GetGameFile("explosion.zip");
            savedGameFile1.Comments = "Hi";
            savedGameFile1.Description = "Good stuff";
            savedGameFile1.MinutesPlayed = 444;

            database.UpdateGameFile(savedGameFile1);

            var retrievedGameFile1 = database.GetGameFile("explosion.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqual(savedGameFile1, retrievedGameFile1));

            var retrievedWrongGameFile = database.GetGameFile("notme.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongGameFile, retrievedWrongGameFile, "GameFileID"));
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesOnlySpecifiedFields()
        {
            var gameFile1 = CreateGameFile("rabbit.zip", 22);
            gameFile1.Comments = "Don't update this";
            gameFile1.Description = "Update me";
            database.InsertGameFile(gameFile1);

            var wrongGameFile = CreateGameFile("incorrect.zip", 23);
            database.InsertGameFile(wrongGameFile);

            var savedGameFile1 = database.GetGameFile("rabbit.zip");
            savedGameFile1.Description = "The new value";
            savedGameFile1.Comments = "Should be ignored";

            database.UpdateGameFile(savedGameFile1, new GameFileFieldType[] { GameFileFieldType.Description });

            var retrievedGameFile1 = database.GetGameFile("rabbit.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile1, retrievedGameFile1, "Description", "GameFileID"));
            Assert.AreEqual("The new value", retrievedGameFile1.Description);

            var retrievedWrongGameFile = database.GetGameFile("incorrect.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongGameFile, retrievedWrongGameFile, "GameFileID"));
        }

        [TestMethod]
        public void DeleteGameFile_DeletesExistingGameFile()
        {
            var gameFile1 = CreateGameFile("deleteme.zip", 24);
            database.InsertGameFile(gameFile1);

            var wrongGameFile = CreateGameFile("keepme.zip", 25);
            database.InsertGameFile(wrongGameFile);

            var savedGameFile1 = database.GetGameFile("deleteme.zip");

            database.DeleteGameFile(savedGameFile1);

            var retrievedGameFiles = database.GetGameFiles();

            Assert.AreEqual(1, retrievedGameFiles.Count());
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongGameFile, retrievedGameFiles.First(), "GameFileID"));
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesConditionally()
        {
            var gameFile1 = CreateGameFile("right1.zip", 26);
            gameFile1.Author = "Michael Abrash";
            gameFile1.Comments = "replace me";
            database.InsertGameFile(gameFile1);

            var gameFile2 = CreateGameFile("right2.zip", 27);
            gameFile2.Author = "Michael Abrash";
            gameFile2.Comments = "replace me too";
            database.InsertGameFile(gameFile2);

            var wrongGameFile = CreateGameFile("wrongo.zip", 28);
            wrongGameFile.Author = "Scott Miller";
            wrongGameFile.Comments = "don't replace me";
            database.InsertGameFile(wrongGameFile);

            //Note: This function is only used in SourcePortViewForm.cs
            database.UpdateGameFiles(GameFileFieldType.Author, GameFileFieldType.Comments, "Michael Abrash", "The new comments");

            var retrievedGameFile1 = database.GetGameFile("right1.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile1, retrievedGameFile1, "GameFileID", "Comments"));
            Assert.AreEqual("The new comments", retrievedGameFile1.Comments);

            var retrievedGameFile2 = database.GetGameFile("right2.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(gameFile2, retrievedGameFile2, "GameFileID", "Comments"));
            Assert.AreEqual("The new comments", retrievedGameFile2.Comments);

            var retrievedWrongGameFile = database.GetGameFile("wrongo.zip");
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongGameFile, retrievedWrongGameFile, "GameFileID"));
        }
    }
}
