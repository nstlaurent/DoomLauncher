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
    public class TestGameFileTags
    {
        private IDataSourceAdapter database;

        private GameFile CreateGameFile(string filename)
        {
            DateTime dt = DateTime.Parse("1/1/17");

            GameFile gameFile = new GameFile
            {
                FileName = filename,
                Author = "the author",
                Description = "the description",
                Title = "the title",
                Comments = "the comments",
                ReleaseDate = dt,
                Downloaded = dt,
                LastPlayed = dt,
                IWadID = 22,
                Rating = 33,
                MinutesPlayed = 44,
                MapCount = 55,
                SourcePortID = 66,
                SettingsExtraParams = "the extra params",
                SettingsFiles = "the settings files",
                SettingsFilesIWAD = "the settings files iwad",
                SettingsFilesSourcePort = "the settings files source port",
                SettingsMap = "the settings map",
                SettingsSkill = "the settings skill",
                SettingsSpecificFiles = "the settings specific files",
                SettingsGameProfileID = 567
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
            dataAccess.ExecuteNonQuery("delete from Tags");
            dataAccess.ExecuteNonQuery("delete from TagMapping");
        }

        [TestMethod]
        public void GetTags_ReturnsAllTags()
        {
            var gameFile1 = CreateGameFile("foo.zip");

            var tag1 = new TagData
            {
                Name = "Cool maps",
                HasTab = true,
                HasColor = true,
                Color = 0xff00ff,
                ExcludeFromOtherTabs = true,
            };

            var tag2 = new TagData
            {
                Name = "Cool bananas",
                HasTab = false,
                HasColor = false,
                Color = null,
                ExcludeFromOtherTabs = false,
            };


            database.InsertTag(tag1);
            database.InsertTag(tag2);

            var tags = database.GetTags();

            var retrievedTag1 = tags.Where(x => x.Name.Equals(tag1.Name)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(tag1, retrievedTag1, "TagID"));

            var retrievedTag2 = tags.Where(x => x.Name.Equals(tag2.Name)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(tag2, retrievedTag2, "TagID"));
        }

        [TestMethod]
        public void GetTagMappings_ReturnsAllTagMappings()
        {
            var tagMapping1 = new TagMapping
            {
                TagID = 1,
                FileID = 2
            };

            var tagMapping2 = new TagMapping
            {
                TagID = 77,
                FileID = 44
            };

            database.InsertTagMapping(tagMapping1);
            database.InsertTagMapping(tagMapping2);

            var tagMappings = database.GetTagMappings();

            var retrievedTagMapping1 = tagMappings.Where(x => x.TagID.Equals(tagMapping1.TagID)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqual(tagMapping1, retrievedTagMapping1));

            var retrievedTagMapping2 = tagMappings.Where(x => x.TagID.Equals(tagMapping2.TagID)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqual(tagMapping2, retrievedTagMapping2));
        }

        [TestMethod]
        public void GetGameFiles_Tag_ReturnsMatchingGameFiles()
        {
            var taggedFile = CreateGameFile("taggy.zip");
            var wrongFile = CreateGameFile("wrong.zip");

            var tag = new TagData { Name = "Fave" };

            database.InsertGameFile(taggedFile);
            database.InsertGameFile(wrongFile);
            database.InsertTag(tag);

            var retrievedGameFile1 = database.GetGameFile("taggy.zip");
            var retrievedTag = database.GetTags().First();

            database.InsertTagMapping(new TagMapping()
            {
                TagID = retrievedTag.TagID,
                FileID = (int)retrievedGameFile1.GameFileID
            });

            var retrievedGameFiles = database.GetGameFiles(retrievedTag);
            Assert.AreEqual(1, retrievedGameFiles.Count());
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(taggedFile, retrievedGameFiles.First(), "GameFileID"));
        }

        [TestMethod]
        public void GetGameFiles_Options_Tag_ReturnsMatchingTaggedGameFiles()
        {
            var rightTaggedFile = CreateGameFile("right_tagged.zip");
            rightTaggedFile.Description = "Robots";

            var rightUntaggedFile = CreateGameFile("right_untagged.zip");
            rightUntaggedFile.Description = "Robots";

            var wrongTaggedFile = CreateGameFile("wrong_tagged.zip");
            var wrongUntaggedFile = CreateGameFile("wrong_untagged.zip");

            var tag = new TagData { Name = "Wow" };

            database.InsertGameFile(rightTaggedFile);
            database.InsertGameFile(rightUntaggedFile);
            database.InsertGameFile(wrongTaggedFile);
            database.InsertGameFile(wrongUntaggedFile);
            database.InsertTag(tag);

            var retrievedRightTaggedFile = database.GetGameFile("right_tagged.zip");
            var retrievedWrongTaggedFile = database.GetGameFile("wrong_tagged.zip");
            var retrievedTag = database.GetTags().First();

            database.InsertTagMapping(new TagMapping()
            {
                TagID = retrievedTag.TagID,
                FileID = (int)retrievedRightTaggedFile.GameFileID
            });

            database.InsertTagMapping(new TagMapping()
            {
                TagID = retrievedTag.TagID,
                FileID = (int)retrievedWrongTaggedFile.GameFileID
            });

            var options = new GameFileGetOptions(new GameFileSearchField(GameFileFieldType.Description, GameFileSearchOp.Equal, "Robots"));
            var retrievedGameFiles = database.GetGameFiles(options, retrievedTag);


            Assert.AreEqual(1, retrievedGameFiles.Count());
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(rightTaggedFile, retrievedGameFiles.First(), "GameFileID"));
        }

    }
}
