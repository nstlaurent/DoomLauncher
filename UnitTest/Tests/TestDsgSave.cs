using System;
using System.IO;
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
    }
}
