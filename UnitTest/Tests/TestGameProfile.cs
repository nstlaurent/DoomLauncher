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
        public void TestInsert()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            foreach (var profile in CreateTestProfiles())
                adapter.InsertGameProfile(profile);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
    }
}
