using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WadReader;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestWadReader
    {
        [TestMethod]
        public void ReadWadEntries()
        {
            string wadfile = "uroburos.wad";
            var zip = ZipFile.OpenRead(Path.Combine("Resources", "uroburos.zip"));
            zip.Entries.First(x => x.FullName == wadfile).ExtractToFile(wadfile, true);

            using (FileStream fs = File.OpenRead(wadfile))
            {
                WadFileReader reader = new WadFileReader(fs);
                var lumps = reader.ReadLumps();

                Assert.IsTrue(reader.IsValid);
                Assert.AreEqual(WadType.PWAD, reader.WadType);
                Assert.AreEqual(15, lumps.Count);
                Assert.AreEqual("D_RUNNIN", lumps[0].Name);
                Assert.AreEqual("WOLF1", lumps[1].Name);
                Assert.AreEqual("RSKY1", lumps[2].Name);
                Assert.AreEqual("TITLEPIC", lumps[3].Name);
                Assert.AreEqual("MAP01", lumps[4].Name);
                Assert.AreEqual("THINGS", lumps[5].Name);
                Assert.AreEqual("LINEDEFS", lumps[6].Name);
                Assert.AreEqual("SIDEDEFS", lumps[7].Name);
                Assert.AreEqual("VERTEXES", lumps[8].Name);
                Assert.AreEqual("SEGS", lumps[9].Name);
                Assert.AreEqual("SSECTORS", lumps[10].Name);
                Assert.AreEqual("NODES", lumps[11].Name);
                Assert.AreEqual("SECTORS", lumps[12].Name);
                Assert.AreEqual("REJECT", lumps[13].Name);
                Assert.AreEqual("BLOCKMAP", lumps[14].Name);

                Assert.AreEqual(25257, lumps[0].ReadData(fs).Length); //D_RUNNIN
                Assert.AreEqual(66888, lumps[3].ReadData(fs).Length); //TITLEPIC
                Assert.AreEqual(0, lumps[4].ReadData(fs).Length); //MAP01
                Assert.AreEqual(6082, lumps[14].ReadData(fs).Length); //BLOCKMAP
            }
        }

        [TestMethod]
        public void ReadInvalidWadHeaderType()
        {
            using (FileStream fs = File.OpenRead(Path.Combine("Resources", "invalid1.wad")))
            {
                WadFileReader reader = new WadFileReader(fs);

                Assert.IsFalse(reader.IsValid);
                Assert.AreEqual(WadType.Unknown, reader.WadType);
                Assert.AreEqual(0, reader.ReadLumps().Count);
            }
        }

        [TestMethod]
        public void ReadInvalidWadHeaderLumps()
        {
            using (FileStream fs = File.OpenRead(Path.Combine("Resources", "invalid2.wad")))
            {
                WadFileReader reader = new WadFileReader(fs);

                Assert.IsFalse(reader.IsValid);
                Assert.AreEqual(WadType.PWAD, reader.WadType);
                Assert.AreEqual(0, reader.ReadLumps().Count);
            }
        }
    }
}
