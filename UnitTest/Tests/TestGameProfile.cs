using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestGameProfile
    {
        [TestMethod]
        public void TestGameProfileData()
        {
            TestInsert();
            TestGameProfileSelect();
            TestGameProfileUpdate();
            TestGameProfileDelete();
        }

        [TestMethod]
        public void TestGlobalGameProfileData()
        {
            var adapter = TestUtil.CreateAdapter();
            TestUtil.CleanDatabase(adapter);
            var globalProfile1 = GameProfile.CreateGlobalProfile("Global Profile 1");
            var globalProfile2 = GameProfile.CreateGlobalProfile("Global Profile 2");
            SetFields(globalProfile1);
            SetFields(globalProfile2);

            adapter.InsertGameProfile(globalProfile1);
            adapter.InsertGameProfile(globalProfile2);

            TestInsert();

            var globalProfiles = adapter.GetGlobalGameProfiles().ToList();
            Assert.AreEqual(2, globalProfiles.Count);
            Assert.AreEqual("Global Profile 1", globalProfiles[0].Name);
            Assert.AreEqual("Global Profile 2", globalProfiles[1].Name);
            globalProfile1 = (GameProfile)globalProfiles[0];
            globalProfile2 = (GameProfile)globalProfiles[1];

            globalProfile1.Name = "Updated Global Profile 1";
            globalProfile2.Name = "Updated Global Profile 2";
            adapter.UpdateGameProfile(globalProfile1);
            adapter.UpdateGameProfile(globalProfile2);
            globalProfiles = adapter.GetGlobalGameProfiles().ToList();
            Assert.AreEqual(2, globalProfiles.Count);
            Assert.AreEqual("Updated Global Profile 1", globalProfiles[0].Name);
            Assert.AreEqual("Updated Global Profile 2", globalProfiles[1].Name);

            adapter.DeleteGameProfile(globalProfile1.GameProfileID);
            globalProfiles = adapter.GetGlobalGameProfiles().ToList();
            Assert.AreEqual(1, globalProfiles.Count);
            Assert.AreEqual("Updated Global Profile 2", globalProfiles[0].Name);
        }

        public void TestInsert()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            foreach (var profile in CreateTestProfiles())
                adapter.InsertGameProfile(profile);
        }

        public void TestGameProfileSelect()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var testProfiles = CreateTestProfiles();

            foreach (var profile in testProfiles)
            {
                var profiles = adapter.GetGameProfiles(profile.GameFileID.Value).ToList();
                Assert.AreEqual(1, profiles.Count);
                Assert.IsTrue(TestUtil.AllFieldsEqual(profile, profiles.First()));
            }
        }

        public void TestGameProfileUpdate()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var profiles = CreateTestProfiles();

            foreach (var profile in profiles)
            {
                profile.Name += profile.Name;
                profile.SourcePortID += profile.SourcePortID;
                profile.IWadID += profile.IWadID;
                profile.SettingsExtraParams += profile.SettingsExtraParams;
                profile.SettingsFiles += profile.SettingsFiles;
                profile.SettingsFilesIWAD += profile.SettingsFilesIWAD;
                profile.SettingsFilesSourcePort += profile.SettingsFilesSourcePort;
                profile.SettingsMap += profile.SettingsMap;
                profile.SettingsSkill += profile.SettingsSkill;
                profile.SettingsSpecificFiles += profile.SettingsSpecificFiles;
                profile.SettingsStat = !profile.SettingsStat;
                profile.SettingsSaved = !profile.SettingsSaved;

                adapter.UpdateGameProfile(profile);

                var testProfiles = adapter.GetGameProfiles(profile.GameFileID.Value).ToList();
                Assert.AreEqual(1, testProfiles.Count);
                Assert.IsTrue(TestUtil.AllFieldsEqual(profile, testProfiles.First()));
            }
        }

        public void TestGameProfileDelete()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var testProfiles = CreateTestProfiles();

            foreach (var profile in testProfiles)
            {
                var profiles = adapter.GetGameProfiles(profile.GameFileID.Value).ToList();
                Assert.AreEqual(1, profiles.Count);

                adapter.DeleteGameProfile(profile.GameProfileID);
                profiles = adapter.GetGameProfiles(profile.GameFileID.Value).ToList();
                Assert.AreEqual(0, profiles.Count);
            }
        }

        private List<IGameProfile> CreateTestProfiles()
        {
            List<IGameProfile> profiles = new List<IGameProfile>();
            for (int i = 0; i < 10; i++)
            {
                profiles.Add(new GameProfile()
                {
                    GameProfileID = i + 1,
                    GameFileID = i,
                    Name = i.ToString(),
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
