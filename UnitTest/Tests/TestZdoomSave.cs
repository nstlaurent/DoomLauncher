using System;
using System.IO;
using DoomLauncher.SaveGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Tests;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestZdoomSave
    {
        [TestMethod]
        public void TestZdoomSaveBinary()
        {
            string save1 = "zdoomsave_v1.zds";
            DeleteSaveFile(save1);
            File.Copy(Path.Combine("Resources", save1), save1);

            ZDoomSaveGameReader reader = new ZDoomSaveGameReader(save1);

            Assert.AreEqual("Autosave Aug 21 13:30", reader.GetName());
        }

        [TestMethod]
        public void TestZdoomSaveJson()
        {
            string save1 = "zdoomsave_v2.zds";
            DeleteSaveFile(save1);
            File.Copy(Path.Combine("Resources", save1), save1);

            ZDoomSaveGameReader reader = new ZDoomSaveGameReader(save1);

            Assert.AreEqual("Autosave Aug 21 13:12", reader.GetName());
        }

        [TestMethod]
        public void TestZdoomSaveJson_3_5()
        {
            string save1 = "zdoomsave_v3.zds";
            DeleteSaveFile(save1);
            File.Copy(Path.Combine("Resources", save1), save1);

            ZDoomSaveGameReader reader = new ZDoomSaveGameReader(save1);

            Assert.AreEqual("Autosave Aug 28 06:25", reader.GetName());
        }

        private static void DeleteSaveFile(string save1)
        {
            if (File.Exists(save1))
                File.Delete(save1);
        }
    }
}
