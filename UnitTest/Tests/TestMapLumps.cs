using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WadReader;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestMapLumps
    {
        [TestMethod]
        public void TestMap()
        {
            List<FileLump> lumps = CreateMapSetLumps("MAP01");
            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);
        
            Assert.AreEqual(1, mapLumps.Count);
            Assert.AreEqual("MAP01", mapLumps.First().Name);
        }

        [TestMethod]
        public void TestMapWithExtra()
        {
            List<FileLump> lumps = CreateMapSetLumps("MAP01");

            lumps.Insert(0, new FileLump("JUNK"));
            for (int i = 0; i < 10; i++)
                lumps.Add(new FileLump("LUMP" + i.ToString()));

            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            Assert.AreEqual(1, mapLumps.Count);
            Assert.AreEqual("MAP01", mapLumps.First().Name);
        }

        [TestMethod]
        public void TestMultipleMaps()
        {
            string[] maps = new string[] { "MAP01", "MAP02", "MAP03", "MAP04", "MAP05", "MAP06" };
            List<FileLump> lumps = new List<FileLump>();

            Array.ForEach(maps, x => lumps.AddRange(CreateMapSetLumps(x)));

            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);
            
            Assert.AreEqual(maps.Length, mapLumps.Count);
            Assert.AreEqual(maps.Length, mapLumps.Count(x => maps.Contains(x.Name)));
        }

        [TestMethod]
        public void TestNonStandardMapNames()
        {
            string[] maps = new string[] { "MYMAP", "ZMAP1", "XMAP2", "ANOTHERMAP", "WHOCARES", "LOOKATME" };
            List<FileLump> lumps = new List<FileLump>();

            Array.ForEach(maps, x => lumps.AddRange(CreateMapSetLumps(x)));

            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            Assert.AreEqual(maps.Length, mapLumps.Count);
            Assert.AreEqual(maps.Length, mapLumps.Count(x => maps.Contains(x.Name)));
        }

        [TestMethod]
        public void TestMultipleMapsWithExtra()
        {
            string[] maps = new string[] { "MAP01", "MAP02", "MAP03", "MAP04", "MAP05", "MAP06" };
            List<FileLump> lumps = new List<FileLump>();

            foreach(var map in maps)
            {
                lumps.AddRange(CreateMapSetLumps(map));
                for (int i = 0; i < 10; i++)
                    lumps.Add(new FileLump("LUMP" + i.ToString()));
            }

            for (int i = 0; i < 10; i++)
                lumps.Add(new FileLump("LUMP" + i.ToString()));

            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            Assert.AreEqual(maps.Length, mapLumps.Count);
            Assert.AreEqual(maps.Length, mapLumps.Count(x => maps.Contains(x.Name)));
        }

        [TestMethod]
        public void TestBadMapLump()
        {
            List<FileLump> lumps = CreateMapSetLumps("MAP01");
            lumps.Insert(2, new FileLump("BADLUMP"));
            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            //MAP01 should not pass, but BADLUMP will because it has at least 5 valid map lumps
            Assert.AreEqual(1, mapLumps.Count);
            Assert.AreEqual("BADLUMP", mapLumps.First().Name);
        }

        [TestMethod]
        public void TestBadMapLumpNoMap()
        {
            List<FileLump> lumps = CreateMapSetLumps("MAP01");
            lumps.Insert(4, new FileLump("BADLUMP"));
            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            //requires at least 5 valid map lumps to pass, inserting BADLUMP in the middle will fail both
            Assert.AreEqual(0, mapLumps.Count);
        }

        [TestMethod]
        public void TestUdmf()
        {
            List<FileLump> lumps = CreateLumps(new string[] { "MAP01", "TEXTMAP", "ZNODES", "ENDMAP"  });
            List<FileLump> mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            Assert.AreEqual(1, mapLumps.Count);
            Assert.AreEqual("MAP01", mapLumps.First().Name);

            lumps.AddRange(CreateLumps(new string[] { "WHOCARES", "MAP02", "TEXTMAP", "ZNODES", "REJECT", "DIALOGUE", "ENDMAP", "JUNK" }));
            mapLumps = WadFileReader.GetMapMarkerLumps(lumps);

            Assert.AreEqual(2, mapLumps.Count);
            Assert.AreEqual("MAP01", mapLumps[0].Name);
            Assert.AreEqual("MAP02", mapLumps[1].Name);
        }

        private List<FileLump> CreateMapSetLumps(string mapName)
        {
            return CreateLumps(new string[] { mapName, "THINGS", "LINEDEFS", "SIDEDEFS", "VERTEXES", "SEGS", "SECTORS", "GL_NODES", "GL_ANY" });
        }

        private List<FileLump> CreateLumps(string[] names)
        {
            List<FileLump> ret = new List<FileLump>();
            Array.ForEach(names, x => ret.Add(new FileLump(x)));
            return ret;
        }
    }
}
