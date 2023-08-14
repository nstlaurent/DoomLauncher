using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DoomLauncher.Handlers;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameProfileUtil
    {
        [TestMethod]
        public void GetAllProfiles()
        {
            var adapter = TestUtil.CreateAdapter();
            TestUtil.CleanDatabase(adapter);
            foreach (var profile in CreateTestProfiles(1, new [] { "Test 1", "A Profile", "Mods" }))
                adapter.InsertGameProfile(profile);
            foreach (var profile in CreateTestProfiles(2, new[] { "New Profile", "Doom", "Mega Profile", "Another one" }))
                adapter.InsertGameProfile(profile);

            var globalProfile1 = GameProfile.CreateGlobalProfile("Z Global Profile 1");
            var globalProfile2 = GameProfile.CreateGlobalProfile("A Global Profile 2");
            SetFields(globalProfile1);
            SetFields(globalProfile2);

            adapter.InsertGameProfile(globalProfile1);
            adapter.InsertGameProfile(globalProfile2);

            var profiles = GameProfileUtil.GetAllProfiles(adapter, new GameFile { GameFileID = 1 }, out var globalProfiles);
            Assert.AreEqual(6, profiles.Count);
            Assert.AreEqual(2, globalProfiles.Count);

            // Global profiles first sorted by name, then default with game file profiles
            Assert.AreEqual(profiles[0].Name, "A Global Profile 2");
            Assert.AreEqual(profiles[1].Name, "Z Global Profile 1");
            Assert.AreEqual(profiles[2].Name, "Default Profile");
            Assert.AreEqual(profiles[3].Name, "A Profile");
            Assert.AreEqual(profiles[4].Name, "Mods");
            Assert.AreEqual(profiles[5].Name, "Test 1");

            profiles = GameProfileUtil.GetAllProfiles(adapter, new GameFile { GameFileID = 2 }, out globalProfiles);
            Assert.AreEqual(7, profiles.Count);
            Assert.AreEqual(2, globalProfiles.Count);

            Assert.AreEqual(profiles[0].Name, "A Global Profile 2");
            Assert.AreEqual(profiles[1].Name, "Z Global Profile 1");
            Assert.AreEqual(profiles[2].Name, "Default Profile");
            Assert.AreEqual(profiles[3].Name, "Another one");
            Assert.AreEqual(profiles[4].Name, "Doom");
            Assert.AreEqual(profiles[5].Name, "Mega Profile");
            Assert.AreEqual(profiles[6].Name, "New Profile");

            profiles = GameProfileUtil.GetAllProfiles(adapter, new GameFile { GameFileID = 3 }, out globalProfiles);
            Assert.AreEqual(3, profiles.Count);
            Assert.AreEqual(2, globalProfiles.Count);
            Assert.AreEqual(profiles[0].Name, "A Global Profile 2");
            Assert.AreEqual(profiles[1].Name, "Z Global Profile 1");
            Assert.AreEqual(profiles[2].Name, "Default Profile");

            var gameFile= new GameFile { GameFileID = 1 };
            Assert.IsTrue(GameProfileUtil.NameExists(adapter, gameFile, globalProfiles,
                profiles[0].GameProfileID, "z Global Profile 1"));
            Assert.IsFalse(GameProfileUtil.NameExists(adapter, gameFile, globalProfiles,
                profiles[0].GameProfileID, "z Global Profile 3**"));
            Assert.IsTrue(GameProfileUtil.NameExists(adapter, gameFile, globalProfiles,
                profiles[0].GameProfileID, GameFile.DefaultProfileName));
            Assert.IsTrue(GameProfileUtil.NameExists(adapter, gameFile, globalProfiles,
                profiles[0].GameProfileID, "mods"));
            Assert.IsFalse(GameProfileUtil.NameExists(adapter, gameFile, globalProfiles,
                profiles[0].GameProfileID, "new profile test"));
        }

        private List<IGameProfile> CreateTestProfiles(int gameFileId, string[] names)
        {
            List<IGameProfile> profiles = new List<IGameProfile>();
            int i = 1000;
            foreach (var name in names)
            {
                profiles.Add(new GameProfile()
                {
                    GameProfileID = i + 1,
                    GameFileID = gameFileId,
                    Name = name,
                    SourcePortID = i,
                    IWadID = i,
                    SettingsExtraParams = i.ToString(),
                    SettingsFiles = i.ToString(),
                    SettingsFilesIWAD = i.ToString(),
                    SettingsFilesSourcePort = i.ToString(),
                    SettingsMap = i.ToString(),
                    SettingsSkill = i.ToString(),
                    SettingsSpecificFiles = i.ToString(),
                    SettingsStat = false,
                    SettingsSaved = false
                });
                i++;
            }
            return profiles;
        }

        private void SetFields(IGameProfile gameProfile)
        {
            gameProfile.SourcePortID = 1;
            gameProfile.IWadID = 1;
        }
    }
}
