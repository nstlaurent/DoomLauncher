using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.SaveGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestDsgSave
    {
        [TestMethod]
        public void TestPrBoomSave()
        {
            string save1 = "prboomsave1.dsg";
            TestUtil.CopyResourceFile(save1);

            DsgSaveGameReader reader = new DsgSaveGameReader(save1);

            Assert.AreEqual("NEW2", reader.GetName());
        }

        [TestMethod]
        public void TestChocolateDoomSave()
        {
            string save1 = "chocosave1.dsg";
            TestUtil.CopyResourceFile(save1);

            DsgSaveGameReader reader = new DsgSaveGameReader(save1);

            Assert.AreEqual("CHOCOTESTTESTTESTTESTTE", reader.GetName());
        }

        [TestMethod]
        public void TestChocolateDoomLoad()
        {
            string save1 = "chocosave1.dsg";
            TestUtil.CopyResourceFile(save1);
            DsgSaveGameReader reader = new DsgSaveGameReader(save1);
            var iwadData = new IWadData();
            iwadData.FileName = "doom.wad";
            iwadData.GameFileID = 1;
            iwadData.IWadID = 1;
            iwadData.Name = "Doom";

            var sourcePort = new SourcePortData();
            sourcePort.Name = "Chocolate doom";
            sourcePort.SourcePortID = 1;


            var saveInfo = reader.GetInfoFromFile(iwadData, sourcePort);

            Assert.AreEqual(0, saveInfo.ArmorType);
            Assert.AreEqual(100, saveInfo.PlayerHealth);
            Assert.AreEqual(0, saveInfo.PlayerArmor);
            Assert.AreEqual(1, saveInfo.GameEpisode);
            Assert.AreEqual(1, saveInfo.GameMap);
            Assert.AreEqual(2, saveInfo.SkillLevel);
        }
    }
}
