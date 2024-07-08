using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestFile
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
            TestUtil.CleanDatabase(database);
        }

        [TestMethod]
        public void GetFiles_GameFile_ReturnsMatchingFiles()
        {

            var gameFileId = 37;

            var file1 = 
                new FileData 
                {
                    FileName = "Rabbits.txt",
                    FileTypeID = FileType.SaveGame,
                    FileOrder = 33,
                    Description = "Furry creatures",
                    DateCreated = DateTime.Parse("2/1/2018"),
                    GameFileID = gameFileId,
                    SourcePortID = 2,
                    OriginalFileName = "Bunnies.txt",
                    OriginalFilePath = "lagomorphs\\Bunnies.txt",
                    UserTitle = "All about rabbits",
                    UserDescription = "I didn't understand it",
                    Map = "zzz"
                };

            var file2 =
                new FileData
                {
                    FileName = "unreal.txt",
                    FileTypeID = FileType.TileImage,
                    FileOrder = 2,
                    Description = "Tim Sweeney's revenge",
                    DateCreated = DateTime.Parse("5/6/1996"),
                    GameFileID = gameFileId,
                    SourcePortID = 8,
                    OriginalFileName = "unrealio_dealio.txt",
                    OriginalFilePath = "whynot\\unrealio_dealio.txt",
                    UserTitle = "The truth about Unreal Tournament",
                    UserDescription = "A gripping tale",
                    Map = "aaa"
                };

            var wrongFile =
                new FileData
                {
                    FileName = "wrong.txt",
                    FileTypeID = FileType.Unknown,
                    FileOrder = 2,
                    Description = "Wrong file",
                    DateCreated = DateTime.Parse("1/7/1991"),
                    GameFileID = -1,
                    SourcePortID = 9,
                    OriginalFileName = "wrongity_wrong.txt",
                    OriginalFilePath = "bad\\wrongity_wrong.txt",
                    UserTitle = "It's wrong",
                    UserDescription = "Don't use this one",
                    Map = "666"
                };

            database.InsertFile(file1);
            database.InsertFile(file2);
            database.InsertFile(wrongFile);

            var files = database.GetFiles(new GameFile { GameFileID = gameFileId });
            Assert.AreEqual(2, files.Count());

            var retrieved1 = files.Where(x => x.FileName.Equals(file1.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file1, retrieved1, "FileID"));

            var retrieved2 = files.Where(x => x.FileName.Equals(file2.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file2, retrieved2, "FileID"));

            Assert.AreEqual(0, files.Where(x => x.FileName.Equals(wrongFile.FileName)).Count());
        }

        [TestMethod]
        public void GetFiles_GameFile_FileType_ReturnsMatchingFiles()
        {
            var gameFileId = 999;
            FileType fileType = FileType.Thumbnail;

            var file1 =
                new FileData
                {
                    FileName = "red.txt",
                    FileTypeID = fileType,
                    FileOrder = 24,
                    Description = "A redulent color",
                    DateCreated = DateTime.Parse("9/3/2033"),
                    GameFileID = gameFileId,
                    SourcePortID = 2,
                    OriginalFileName = "hongse.txt",
                    OriginalFilePath = "colors\\hongse.txt",
                    UserTitle = "It's another color",
                    UserDescription = "Primary color, a bit angry",
                    Map = "yyy"
                };

            var wrongGameIdFile =
                new FileData
                {
                    FileName = "wrong.txt",
                    FileTypeID = fileType,
                    FileOrder = 2,
                    Description = "Wrong file",
                    DateCreated = DateTime.Parse("1/7/1991"),
                    GameFileID = 444,
                    SourcePortID = 9,
                    OriginalFileName = "wrongity_wrong.txt",
                    OriginalFilePath = "bad\\wrongity_wrong.txt",
                    UserTitle = "Wrong game ID",
                    UserDescription = "Don't use this one",
                    Map = "666"
                };

            var wrongFileTypeFile =
                new FileData
                {
                    FileName = "false.txt",
                    FileTypeID = FileType.SaveGame,
                    FileOrder = 3,
                    Description = "False, tricksy file",
                    DateCreated = DateTime.Parse("3/8/2000"),
                    GameFileID = gameFileId,
                    SourcePortID = 8,
                    OriginalFileName = "this_aint_it.txt",
                    OriginalFilePath = "incorrect\\this_aint_it.txt",
                    UserTitle = "Wrong file type",
                    UserDescription = "Absolutely not",
                    Map = "667"
                };

            database.InsertFile(file1);
            database.InsertFile(wrongGameIdFile);
            database.InsertFile(wrongFileTypeFile);

            var files = database.GetFiles(new GameFile { GameFileID = gameFileId }, fileType);

            Assert.AreEqual(1, files.Count());

            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file1, files.First(), "FileID"));
        }

        [TestMethod]
        public void GetFiles_NoArgs_ReturnsAllFiles()
        {
            var file1 =
                new FileData
                {
                    FileName = "hippo.txt",
                    FileTypeID = FileType.Demo,
                    FileOrder = 3,
                    Description = "river horse",
                    DateCreated = DateTime.Parse("6/9/2033"),
                    GameFileID = 555,
                    SourcePortID = 2,
                    OriginalFileName = "hippopotamus.txt",
                    OriginalFilePath = "animals\\hippopotamus.txt",
                    UserTitle = "I like hippos",
                    UserDescription = "They are very hungry",
                    Map = "hjkl"
                };

            var file2 =
                new FileData
                {
                    FileName = "giraffe.txt",
                    FileTypeID = FileType.Screenshot,
                    FileOrder = 9,
                    Description = "long neck deer",
                    DateCreated = DateTime.Parse("1/1/1992"),
                    GameFileID = 553,
                    SourcePortID = 6,
                    OriginalFileName = "giraffe_pattern.txt",
                    OriginalFilePath = "animals\\giraffe_pattern.txt",
                    UserTitle = "Long neck giraffe",
                    UserDescription = "Why are their necks so long",
                    Map = "aaa"
                };

            database.InsertFile(file1);
            database.InsertFile(file2);

            var files = database.GetFiles();

            Assert.AreEqual(2, files.Count());

            var retrieved1 = files.Where(x => x.FileName.Equals(file1.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file1, retrieved1, "FileID"));

            var retrieved2 = files.Where(x => x.FileName.Equals(file2.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file2, retrieved2, "FileID"));
        }

        [TestMethod]
        public void GetFiles_FileType_ReturnsMatchingFiles()
        {
            var file1 =
                new FileData
                {
                    FileName = "cacodemon.txt",
                    FileTypeID = FileType.Demo,
                    FileOrder = 1,
                    Description = "red floaty boi",
                    DateCreated = DateTime.Parse("1/2/2011"),
                    GameFileID = 236,
                    SourcePortID = 2,
                    OriginalFileName = "caco.txt",
                    OriginalFilePath = "monsters\\caco.txt",
                    UserTitle = "The fanciest demon",
                    UserDescription = "Three double shotty hits on a good day",
                    Map = "yyy"
                };

            var file2 =
                new FileData
                {
                    FileName = "imp.txt",
                    FileTypeID = FileType.Demo,
                    FileOrder = 1,
                    Description = "brown fella",
                    DateCreated = DateTime.Parse("3/6/1998"),
                    GameFileID = 353,
                    SourcePortID = 8,
                    OriginalFileName = "impy.txt",
                    OriginalFilePath = "monsters\\impy.txt",
                    UserTitle = "One shotty blast",
                    UserDescription = "Two if you miss",
                    Map = "yyy"
                };

            var wrongFile =
                new FileData
                {
                    FileName = "oops.txt",
                    FileTypeID = FileType.Thumbnail,
                    FileOrder = 8,
                    Description = "wrong file type",
                    DateCreated = DateTime.Parse("5/3/1997"),
                    GameFileID = 222,
                    SourcePortID = 9,
                    OriginalFileName = "whoops.txt",
                    OriginalFilePath = "ohno\\whoops.txt",
                    UserTitle = "Very much the wrong one",
                    UserDescription = "no way",
                    Map = "www"
                };

            database.InsertFile(file1);
            database.InsertFile(file2);
            database.InsertFile(wrongFile);

            var files = database.GetFiles(FileType.Demo);

            Assert.AreEqual(2, files.Count());

            var retrieved1 = files.Where(x => x.FileName.Equals(file1.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file1, retrieved1, "FileID"));

            var retrieved2 = files.Where(x => x.FileName.Equals(file2.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(file2, retrieved2, "FileID"));
        }

        [TestMethod]
        public void UpdateFile_UpdatesTheRightFieldsInTheRightFile()
        {
            var file1 =
                new FileData
                {
                    FileName = "pain_elemental.txt",
                    FileTypeID = FileType.SaveGame,
                    FileOrder = 11,
                    Description = "brown caco",
                    DateCreated = DateTime.Parse("11/7/2004"),
                    GameFileID = 222,
                    SourcePortID = 2,
                    OriginalFileName = "paine.txt",
                    OriginalFilePath = "monsters\\paine.txt",
                    UserTitle = "What if a demon shot more demons out of its mouth",
                    UserDescription = "What if a pain elemental shot more pain elementals out of its mouth",
                    Map = "s34"
                };

            var wrongFile =
                new FileData
                {
                    FileName = "hellknight.txt",
                    FileTypeID = FileType.Screenshot,
                    FileOrder = 2,
                    Description = "the cheapest monster",
                    DateCreated = DateTime.Parse("1/3/2007"),
                    GameFileID = 222,
                    SourcePortID = 5,
                    OriginalFileName = "hk.txt",
                    OriginalFilePath = "monsters\\hk.txt",
                    UserTitle = "Change the color, ship it",
                    UserDescription = "Deadlines are deadlines",
                    Map = "ooo"
                };

            database.InsertFile(file1);
            database.InsertFile(wrongFile);

            // Need to fetch so that the primary key is populated.
            var savedFiles = database.GetFiles(new GameFile { GameFileID = 222 });
            var savedFile1 = savedFiles.Where(x => x.FileName.Equals(file1.FileName)).First();

            // Update every updatable field with new value
            savedFile1.SourcePortID = 33;
            savedFile1.Description = "New Description";
            savedFile1.FileOrder = 44;
            savedFile1.DateCreated = new DateTime(2011, 3, 2);
            savedFile1.UserTitle = "New UserTitle";
            savedFile1.UserDescription = "New UserDescription";
            savedFile1.Map = "New map";
            database.UpdateFile(savedFile1);

            var updatedFiles = database.GetFiles(new GameFile { GameFileID = 222 });

            // Values were updated in that record
            var retrieved1 = updatedFiles.Where(x => x.FileID.Equals(savedFile1.FileID)).First();
            Assert.AreEqual(file1.FileName, retrieved1.FileName);
            Assert.AreEqual(33, retrieved1.SourcePortID);
            Assert.AreEqual("New Description", retrieved1.Description);
            Assert.AreEqual(44, retrieved1.FileOrder);
            Assert.AreEqual(new DateTime(2011, 3, 2), retrieved1.DateCreated);
            Assert.AreEqual("New UserTitle", retrieved1.UserTitle);
            Assert.AreEqual("New UserDescription", retrieved1.UserDescription);
            Assert.AreEqual("New map", retrieved1.Map);

            // The other record was unaffected
            var retrieved2 = updatedFiles.Where(x => x.FileName.Equals(wrongFile.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongFile, retrieved2, "FileID"));
        }

        [TestMethod]
        public void DeleteFile_FileID_DeletesJustThatFile()
        {
            var file1 =
                new FileData
                {
                    FileName = "chaingunner.txt",
                    FileTypeID = FileType.Demo,
                    FileOrder = 5,
                    Description = "Bastard hitscanner",
                    DateCreated = DateTime.Parse("7/11/2006"),
                    GameFileID = 123,
                    SourcePortID = 9,
                    OriginalFileName = "chaingun_dude.txt",
                    OriginalFilePath = "monsters\\chaingun_dude.txt",
                    UserTitle = "I hate this guy",
                    UserDescription = "Kill them kill them",
                    Map = "E1M2"
                };

            var wrongFile =
                new FileData
                {
                    FileName = "wrongo.txt",
                    FileTypeID = FileType.TileImage,
                    FileOrder = 99,
                    Description = "Something else",
                    DateCreated = DateTime.Parse("11/7/1921"),
                    GameFileID = 123,
                    SourcePortID = 88,
                    OriginalFileName = "nope.txt",
                    OriginalFilePath = "bad\\nope.txt",
                    UserTitle = "Yeah nah",
                    UserDescription = "Womp womp womp",
                    Map = "555"
                };

            database.InsertFile(file1);
            database.InsertFile(wrongFile);

            // Need to fetch so that the primary key is populated.
            var savedFiles = database.GetFiles();
            var savedFile1 = savedFiles.Where(x => x.FileName.Equals(file1.FileName)).First();

            database.DeleteFile(savedFile1);

            var files = database.GetFiles();

            Assert.AreEqual(1, files.Count());
            Assert.AreEqual(wrongFile.FileName, files.First().FileName);
        }

        [TestMethod]
        public void DeleteFile_GameFileID_DeletesMatchingFiles()
        {
            var file1 =
                new FileData
                {
                    FileName = "pinky_demon.txt",
                    FileTypeID = FileType.Thumbnail,
                    FileOrder = 8,
                    Description = "oink",
                    DateCreated = DateTime.Parse("17/06/2008"),
                    GameFileID = 777,
                    SourcePortID = 9,
                    OriginalFileName = "pinky.txt",
                    OriginalFilePath = "monsters\\pinky.txt",
                    UserTitle = "Should be called piggy demon am I right",
                    UserDescription = "Use a rocket for the dumbest suicide imaginable",
                    Map = "E1M3"
                };

            var file2 =
                new FileData
                {
                    FileName = "revenant.txt",
                    FileTypeID = FileType.SaveGame,
                    FileOrder = 10,
                    Description = "evil bastard",
                    DateCreated = DateTime.Parse("1/1/2014"),
                    GameFileID = 777,
                    SourcePortID = 8,
                    OriginalFileName = "rev.txt",
                    OriginalFilePath = "monsters\\rev.txt",
                    UserTitle = "Awful horrible demon",
                    UserDescription = "Kill it before it kills you",
                    Map = "E2M4"
                };

            var wrongFile =
                new FileData
                {
                    FileName = "wrongy.txt",
                    FileTypeID = FileType.Demo,
                    FileOrder = 99,
                    Description = "Naaaah",
                    DateCreated = DateTime.Parse("7/2/1933"),
                    GameFileID = 11,
                    SourcePortID = 60,
                    OriginalFileName = "bah.txt",
                    OriginalFilePath = "bad\\bah.txt",
                    UserTitle = "bah humbug",
                    UserDescription = "Don't pick me",
                    Map = "MAP04"
                };

            database.InsertFile(file1);
            database.InsertFile(file2);
            database.InsertFile(wrongFile);

            database.DeleteFile(new GameFile { GameFileID = 777 });

            var files = database.GetFiles();

            Assert.AreEqual(1, files.Count());
            Assert.AreEqual(wrongFile.FileName, files.First().FileName);
        }

        //[TestMethod]
        /*
        public void TestDeleteFile()
        {
            var gameFileIDs = insertedFiles.Select(x => x.GameFileID).Distinct();

            int id = gameFileIDs.First();

            var dbFiles = database.GetFiles(new GameFile() { GameFileID = id });
            int count = dbFiles.Count();
            foreach (var dbFile in dbFiles)
            {
                database.DeleteFile(dbFile);
                count--;
                Assert.AreEqual(count, database.GetFiles(new GameFile() { GameFileID = id }).Count());
            }

            id = gameFileIDs.Skip(1).First();
            database.DeleteFile(new GameFile() { GameFileID = id });
            Assert.AreEqual(0, database.GetFiles(new GameFile() { GameFileID = id }).Count());
        }*/
    }
}
