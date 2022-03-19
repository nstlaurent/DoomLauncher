using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestDirectoryArchive
    {
        [TestMethod]
        public void TestData()
        {
            string file = "Directory.zip";
            TestUtil.CopyResourceFile(file);
            ZipArchive za = ZipFile.OpenRead(file);
            string dir = "TestDirectory";
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            za.ExtractToDirectory(dir);

            DirectoryArchiveReader directoryArchive = new DirectoryArchiveReader(dir);
            var entries = directoryArchive.Entries;

            Assert.AreEqual(13, entries.Count());
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("map01.wad")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("map02.wad")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("map03.wad")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("map01.mid")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("map02.mid")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("map03.mid")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("tex1.png")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("tex2.png")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("tex3.png")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("sound1.wav")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("mapinfo.txt")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("sndinfo.txt")));
            Assert.IsNotNull(entries.FirstOrDefault(x => x.Name.Equals("textures.txt")));
        }
    }
}
