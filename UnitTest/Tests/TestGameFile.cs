using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameFile
    {
        private static string s_garbage = "'s like = % ";

        private IDataSourceAdapter database;

        private static readonly GameFileFieldType[] s_fields = new GameFileFieldType[]
        {
            GameFileFieldType.GameFileID,
            GameFileFieldType.Filename,
            GameFileFieldType.Author,
            GameFileFieldType.Title,
            GameFileFieldType.Description,
            GameFileFieldType.Downloaded,
            GameFileFieldType.LastPlayed,
            GameFileFieldType.ReleaseDate,
            GameFileFieldType.Comments,
            GameFileFieldType.Rating,
            GameFileFieldType.MapCount,
            GameFileFieldType.MinutesPlayed,
            GameFileFieldType.IWadID
        };

        private GameFile CreateGameFile(string filename, int count)
        {
            DateTime dt = DateTime.Parse("1/1/17");

            GameFile gameFile = new GameFile
            {
                FileName = filename,
                Author = string.Format("Author_{0}", filename),
                Description = string.Format("Description_{0}", filename),
                Title = string.Format("Title_{0}", filename),
                Comments = string.Format("Comments_{0}", filename),
                ReleaseDate = dt,
                Downloaded = dt,
                LastPlayed = dt,
                IWadID = count,
                Rating = count,
                MinutesPlayed = count,
                MapCount = count,
                SourcePortID = count,
                SettingsExtraParams = filename,
                SettingsFiles = filename,
                SettingsFilesIWAD = filename,
                SettingsFilesSourcePort = filename,
                SettingsMap = filename,
                SettingsSkill = filename,
                SettingsSpecificFiles = filename,
                SettingsGameProfileID = count
            };

            return gameFile;
        }

        private List<IGameFile> CreateTestFileList()
        {
            List<IGameFile> files = new List<IGameFile>();
            int count = 0;

            for (int i = 0; i < 3; i++)
            {
                GameFile gameFile = CreateGameFile(string.Format("Test{0}.Zip", ++count), count);
                gameFile.GameFileID = count;
                files.Add(gameFile);
            }

            for (int i = 0; i < 3; i++)
            {
                GameFile gameFile = CreateGameFile(string.Format("TEST{0}.Zip", ++count), count);
                gameFile.GameFileID = count;
                files.Add(gameFile);
            }

            for (int i = 0; i < 3; i++)
            {
                GameFile gameFile = CreateGameFile(string.Format("TEST{0}{1}.Zip", s_garbage, ++count), count);
                gameFile.GameFileID = count;
                files.Add(gameFile);
            }

            return files;
        }

        private List<ITagData> CreateTestTagList()
        {
            List<ITagData> tags = new List<ITagData>();
            int count = 0;

            for (int i = 0; i < 6; i++)
                tags.Add(new TagData() { TagID = ++count, Name = Guid.NewGuid().ToString() });

            return tags;
        }

        private List<ITagMapping> CreateTestTagMapping()
        {
            List<ITagMapping> map = new List<ITagMapping>();
            var gameFiles = CreateTestFileList();
            var tags = CreateTestTagList();
            int count = 0;

            foreach(var gameFile in gameFiles)
            {
                map.Add(new TagMapping() { FileID = gameFile.GameFileID.Value, TagID = tags[count % tags.Count].TagID });
                count++;
            }

            return map;
        }

        public void TestInsertGameFile()
        {
            var gameFiles = CreateTestFileList();
            foreach (var gameFile in gameFiles)
                database.InsertGameFile(gameFile);
        }

        public void TestInsertTag()
        {
            var tags = CreateTestTagList();
            foreach (var tag in tags)
                database.InsertTag(tag);
        }

        public void TestInsertTagMap()
        {
            var mapTags = CreateTestTagMapping();
            foreach (var map in mapTags)
                database.InsertTagMapping(map);
        }


        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
            TestInsertGameFile();
            TestInsertTag();
            TestInsertTagMap();
        }

        [TestCleanup]
        public void CleanDatabase()
        {
            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from GameFiles");
            dataAccess.ExecuteNonQuery("delete from Tags");
            dataAccess.ExecuteNonQuery("delete from TagMapping");
        }

        [TestMethod]
        public void GetGameFiles_ReturnsTheExpectedGameFilesCount()
        {
            var gameFiles = CreateTestFileList();

            Assert.IsTrue(database.GetGameFiles().Count() == gameFiles.Count);
            Assert.IsTrue(database.GetGameFilesCount() == gameFiles.Count);
            Assert.IsTrue(database.GetGameFileNames().Count() == gameFiles.Count);
        }

        [TestMethod]
        public void GetTags_ReturnsTheExpectedTagCount()
        {
            var tags = CreateTestTagList();
            Assert.IsTrue(database.GetTags().Count() == tags.Count);
        }

        [TestMethod]
        public void GetTagMappings_MatchesTheGameFileCount()
        {
            var tagMap = CreateTestTagMapping();
            Assert.IsTrue(database.GetTagMappings().Count() == tagMap.Count);
            Assert.IsTrue(database.GetGameFiles().Count() == tagMap.Count);
        }

        [TestMethod]
        public void GetGameFiles_Tag_ReturnsTheExpectedGameFilesCount()
        {
            var tags = database.GetTags();
            var tagMap = database.GetTagMappings();

            foreach (var tag in tags)
            {
                var gameFileGet = database.GetGameFiles(tag);
                Assert.IsTrue(gameFileGet.Count() == tagMap.Count(x => x.TagID == tag.TagID));
            }
        }

        [TestMethod]
        public void GetGameFiles_Options_Tag_ReturnsTheExpectedGameFileCount()
        {
            var tags = database.GetTags();
            var tagMap = database.GetTagMappings();

            foreach (var tag in tags)
            {
                var gameFileGet = database.GetGameFiles(new GameFileGetOptions(s_fields), tag);
                Assert.IsTrue(gameFileGet.Count() == tagMap.Count(x => x.TagID == tag.TagID));
            }

            foreach (var tag in tags)
            {
                var options = new GameFileGetOptions(s_fields, new GameFileSearchField(GameFileFieldType.GameFileID, "1"));
                var gameFileGet = database.GetGameFiles(options, tag);
                Assert.IsTrue(gameFileGet.Count() == tagMap.Count(x => x.TagID == tag.TagID && x.FileID == 1));
            }
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsExpectedGameFiles()
        {
            var gameFiles = CreateTestFileList();

            var selectGameFiles = database.GetGameFiles(new GameFileGetOptions(s_fields));

            foreach(var gameFile in gameFiles)
            {
                var selectGameFile = selectGameFiles.FirstOrDefault(x => x.GameFileID == gameFile.GameFileID);
                Assert.IsNotNull(selectGameFile);
                Assert.IsTrue(selectGameFile.SettingsExtraParams.Length == 0);
                Assert.IsTrue(selectGameFile.SettingsFiles.Length == 0);
                Assert.IsTrue(selectGameFile.SettingsMap.Length == 0);
                Assert.IsTrue(selectGameFile.SettingsSkill.Length == 0);
                Assert.IsTrue(selectGameFile.SettingsSpecificFiles.Length == 0);

                Assert.IsTrue(selectGameFile.FileName.Equals(gameFile.FileName));
                Assert.IsTrue(selectGameFile.Author.Equals(gameFile.Author));
                Assert.IsTrue(selectGameFile.Title.Equals(gameFile.Title));
                Assert.IsTrue(selectGameFile.Description.Equals(gameFile.Description));
                Assert.IsTrue(selectGameFile.Downloaded.Equals(gameFile.Downloaded));
                Assert.IsTrue(selectGameFile.LastPlayed.Equals(gameFile.LastPlayed));
                Assert.IsTrue(selectGameFile.ReleaseDate.Equals(gameFile.ReleaseDate));
                Assert.IsTrue(selectGameFile.Comments.Equals(gameFile.Comments));
                Assert.IsTrue(selectGameFile.Rating.Equals(gameFile.Rating));
                Assert.IsTrue(selectGameFile.MapCount.Equals(gameFile.MapCount));
                Assert.IsTrue(selectGameFile.MinutesPlayed.Equals(gameFile.MinutesPlayed));
                Assert.IsTrue(selectGameFile.IWadID.Equals(gameFile.IWadID));
            }
        }

        [TestMethod]
        public void GetFileName_String_ReturnsGameFileOfThatName()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var gameFiles = CreateTestFileList();

            foreach (var gameFile in gameFiles)
            {
                var gameFileFind = adapter.GetGameFile(gameFile.FileName.ToLower()); //check that search is not case sensitive

                Assert.AreEqual(gameFile, gameFileFind); //Default operator, only checks against FileName
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFileFind));
            }
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFileById()
        {
            var gameFiles = CreateTestFileList();

            foreach (var gameFile in gameFiles)
            {
                IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.GameFileID, gameFile.GameFileID.ToString()));
                var gameFilesFind = database.GetGameFiles(options);

                Assert.AreEqual(1, gameFilesFind.Count());
                Assert.AreEqual(gameFile, gameFilesFind.First()); //Default operator, only checks against FileName
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFilesFind.First()));
            }
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFileLikeFileName()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var gameFiles = CreateTestFileList();

            foreach (var gameFile in gameFiles)
            {
                IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Filename, GameFileSearchOp.Like, gameFile.FileName));
                var gameFilesFind = database.GetGameFiles(options);

                Assert.AreEqual(1, gameFilesFind.Count());
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFilesFind.First()));
            }
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFilePartialLikeFileName()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Filename, GameFileSearchOp.Like, "test"));
            var gameFilesFind = database.GetGameFiles(options);

            Assert.AreEqual(gameFilesFind.Count(), CreateTestFileList().Count);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFilePartialLikeFileName_SqlSyntax()
        {
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Filename, GameFileSearchOp.Like, "test" + s_garbage));
            var gameFilesFind = database.GetGameFiles(options);

            Assert.AreEqual(3, gameFilesFind.Count());
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFilePartialLikeTitle()
        {
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Title, GameFileSearchOp.Like, "test"));
            var gameFilesFind = database.GetGameFiles(options);

            Assert.AreEqual(gameFilesFind.Count(), CreateTestFileList().Count);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFilePartialLikeDescription()
        {
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Description, GameFileSearchOp.Like, "test"));
            var gameFilesFind = database.GetGameFiles(options);

            Assert.AreEqual(gameFilesFind.Count(), CreateTestFileList().Count);
        }

        [TestMethod]
        public void GetGameFiles_Options_ReturnsGameFilePartialLikeAuthor()
        {
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Author, GameFileSearchOp.Like, "test"));
            var gameFilesFind = database.GetGameFiles(options);

            Assert.AreEqual(gameFilesFind.Count(), CreateTestFileList().Count);
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesFileName()
        {
            var gameFiles = CreateTestFileList();

            foreach(var gameFile in gameFiles)
            {
                string oldFileName = gameFile.FileName;
                gameFile.FileName = gameFile.FileName.Replace(gameFile.GameFileID.ToString(), string.Format("Update_{0}", gameFile.GameFileID));
                database.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.Filename });

                IGameFile gameFileFind = database.GetGameFile(oldFileName);
                Assert.IsTrue(gameFileFind == null);

                gameFileFind = database.GetGameFile(gameFile.FileName);
                Assert.AreEqual(gameFile, gameFileFind);
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFileFind));
            }
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesSpecifiedFields()
        {
            var gameFiles = CreateTestFileList();

            List<GameFileFieldType> fields = new List<GameFileFieldType>();

            foreach (var enumValue in Enum.GetValues(typeof(GameFileFieldType)))
                fields.Add((GameFileFieldType)enumValue);

            var names = fields.Select(x => x.ToString());
            PropertyInfo[] properties = typeof(TestGameFile).GetProperties().Where(x => names.Contains(x.Name)).ToArray();
            SetRandomFileValues(gameFiles, properties);

            foreach (var gameFile in gameFiles)
            {
                database.UpdateGameFile(gameFile, fields.ToArray());
                IGameFile gameFileFind = database.GetGameFile(gameFile.FileName);
                Assert.IsTrue(gameFileFind != null);
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFileFind));
            }
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesAllFields()
        {
            var gameFiles = CreateTestFileList();
            SetRandomFileValues(gameFiles, typeof(GameFile).GetProperties());

            foreach(var gameFile in gameFiles)
            {
                database.UpdateGameFile(gameFile);
                IGameFile gameFileFind = database.GetGameFile(gameFile.FileName);
                Assert.IsTrue(gameFileFind != null);
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFileFind));
            }
        }

        [TestMethod]
        public void UpdateGameFile_UpdatesConditionally()
        {
            //Note: This function is only used in SourcePortViewForm.cs
            var gameFiles = CreateTestFileList();
            int test = 1000;

            foreach(var gameFile in gameFiles)
            {
                database.UpdateGameFile(gameFile); //Make sure we're in sync with our test list first...

                //Test setting a real value
                database.UpdateGameFiles(GameFileFieldType.SourcePortID, GameFileFieldType.SourcePortID, gameFile.SourcePortID, test);
                gameFile.SourcePortID = test;

                IGameFile gameFileFind = database.GetGameFile(gameFile.FileName);
                Assert.IsTrue(gameFileFind != null);
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFileFind));

                //Test setting value to null
                database.UpdateGameFiles(GameFileFieldType.SourcePortID, GameFileFieldType.SourcePortID, gameFile.SourcePortID, null);
                gameFile.SourcePortID = null;

                gameFileFind = database.GetGameFile(gameFile.FileName);
                Assert.IsTrue(gameFileFind != null);
                Assert.IsTrue(TestUtil.AllFieldsEqual<IGameFile>(gameFile, gameFileFind));

                test++;
            }
        }

        private void SetRandomFileValues(List<IGameFile> gameFiles, PropertyInfo[] properties)
        {
            int value = 0;
            //Properties we do not write to the database
            string[] exclude = new string[] { "FileSizeBytes", "GameFileID", "FileNameNoPath", "LastDirectory" };

            foreach (var gameFile in gameFiles)
            {
                foreach (PropertyInfo pi in properties)
                {
                    if (!exclude.Contains(pi.Name))
                    {
                        Type pType = pi.PropertyType;

                        if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            pType = pType.GetGenericArguments()[0];

                        if (pType == typeof(int))
                            pi.SetValue(gameFile, ++value);
                        else if (pType == typeof(double))
                            pi.SetValue(gameFile, (double)++value);
                        else if (pType == typeof(string))
                            pi.SetValue(gameFile, Guid.NewGuid().ToString());
                        else if (pType == typeof(DateTime))
                            pi.SetValue(gameFile, DateTime.Now);
                    }
                }
            }
        }

        [TestMethod]
        public void DeleteGameFile_DeletesFile()
        {
            var gameFiles = CreateTestFileList();
            int count = gameFiles.Count;

            foreach (var gameFile in gameFiles)
            {
                database.DeleteGameFile(gameFile);

                IGameFile gameFileFind = database.GetGameFile(gameFile.FileName);
                Assert.IsTrue(gameFileFind == null);
                Assert.AreEqual(database.GetGameFiles().Count(), --count);
            }
        }
    }
}
