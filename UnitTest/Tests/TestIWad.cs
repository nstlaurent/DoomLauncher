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
    public class TestIWad
    {
        private IIWadDataSourceAdapter database;

        [TestInitialize]
        public void Initialize()
        {
            database = TestUtil.CreateAdapter();
        }

        [TestCleanup]
        public void CleanDatabase()
        {
            var dataAccess = ((DbDataSourceAdapter)database).DataAccess;
            dataAccess.ExecuteNonQuery("delete from IWads");
        }

        [TestMethod]
        public void GetIWads_ReturnsAllIWads()
        {
            var iWad1 = new IWadData()
            {
                FileName = "Doom.wad",
                Name = "Doom",
                GameFileID = 1
            };

            var iWad2 = new IWadData()
            {
                FileName = "Doom2.wad",
                Name = "Doom II",
                GameFileID = 2
            };

            database.InsertIWad(iWad1);
            database.InsertIWad(iWad2);

            var iWads = database.GetIWads();
            Assert.AreEqual(2, iWads.Count());

            var retrieved1 = iWads.Where(x => x.FileName.Equals(iWad1.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(iWad1, retrieved1, "IWadID"));

            var retrieved2 = iWads.Where(x => x.FileName.Equals(iWad2.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(iWad2, retrieved2, "IWadID"));
        }

        [TestMethod]
        public void GetIWad_GetsIWadForGameFileID()
        {
            var iWad1 = new IWadData()
            {
                FileName = "Heretic.wad",
                Name = "Heretic",
                GameFileID = 5
            };

            database.InsertIWad(iWad1);

            var retrievedIWad = database.GetIWad(5);

            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(iWad1, retrievedIWad, "IWadID"));
        }

        [TestMethod]
        public void DeleteIWad_DeletesTheIWad()
        {
            var iWad1 = new IWadData()
            {
                FileName = "plutonia.wad",
                Name = "The Plutonia Experiment",
                GameFileID = 3
            };

            var wrongIWad = new IWadData()
            {
                FileName = "tnt.wad",
                Name = "TNT Evilution",
                GameFileID = 4
            };

            database.InsertIWad(iWad1);
            database.InsertIWad(wrongIWad);

            // Need to fetch so that the primary key is populated.
            var savedIWad1 = database.GetIWad(3);
            database.DeleteIWad(savedIWad1);

            var iWads = database.GetIWads();
            Assert.AreEqual(1, iWads.Count());

            var retrieved2 = iWads.First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongIWad, retrieved2, "IWadID"));
        }

        [TestMethod]
        public void UpdateIWad_UpdatesTheRightFieldsInTheRightIWad()
        {
            var iWad1 = new IWadData()
            {
                FileName = "heretic.wad",
                Name = "Heretic",
                GameFileID = 5
            };

            var wrongIWad = new IWadData()
            {
                FileName = "chex.wad",
                Name = "Chex Quest",
                GameFileID = 6
            };

            database.InsertIWad(iWad1);
            database.InsertIWad(wrongIWad);

            // Need to fetch so that the primary key is populated.
            var savedIWad1 = database.GetIWad(5);
            savedIWad1.FileName = "daikatana.wad";
            savedIWad1.Name = "Daikatana";
            savedIWad1.GameFileID = 7;
            database.UpdateIWad(savedIWad1);

            var updatedIWads = database.GetIWads();
            Assert.AreEqual(2, updatedIWads.Count());

            var retrieved1 = updatedIWads.Where(x => x.IWadID.Equals(savedIWad1.IWadID)).First();
            Assert.AreEqual("daikatana.wad", retrieved1.FileName);
            Assert.AreEqual("Daikatana", retrieved1.Name);
            Assert.AreEqual(7, retrieved1.GameFileID);

            var retrieved2 = updatedIWads.Where(x => x.FileName.Equals(wrongIWad.FileName)).First();
            Assert.IsTrue(TestUtil.AllFieldsEqualIgnore(wrongIWad, retrieved2, "IWadID"));
        }
    }
}
