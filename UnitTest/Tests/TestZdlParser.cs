using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestZdlParser
    {
        [TestMethod]
        public void TestZdlParserFile()
        {
            string file = @"[zdl.save]
port=gzdoom
iwad=C:/ZDL/DOOM2.WAD
extra=extra
file0=C:/ZDL/pinnacle.zip
file1=C:/ZDL/bd21testApr25.zip
skill=4
warp=MAP01
[zdl.general]
conflib=sunrise
";

            ZdlParser parser = new ZdlParser(CreateSourcePorts(), CreateIWads());
            IGameFile[] gameFiles = parser.Parse(file);

            Assert.AreEqual(2, gameFiles.Length);

            Assert.AreEqual("C:/ZDL/pinnacle.zip", gameFiles[0].FileName);
            Assert.AreEqual("MAP01", gameFiles[0].SettingsMap);
            Assert.AreEqual("4", gameFiles[0].SettingsSkill);
            Assert.AreEqual("extra", gameFiles[0].SettingsExtraParams);

            Assert.AreEqual("C:/ZDL/bd21testApr25.zip", gameFiles[1].FileName);
        }

        private IEnumerable<IIWadData> CreateIWads()
        {
            return new IIWadData[]
            {
                new IWadData { FileName = "DOOM2.WAD" },
                new IWadData { FileName = "DOOM.WAD" },
                new IWadData { FileName = "PLUTONIA.WAD" }
            };
        }

        private IEnumerable<ISourcePort> CreateSourcePorts()
        {
            return new ISourcePort[]
            {
                new SourcePort { Name = "GZDoom" },
                new SourcePort { Name = "PrBoom" },
                new SourcePort { Name = "Chocolate Doom" }
            };
        }
    }
}
