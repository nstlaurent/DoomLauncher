using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher.Demo;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestCldDemoParser
    {
        [TestMethod]
        public void TestCldDemoParserFile()
        {
            string file = "zandemo.cld";
            File.Copy(Path.Combine("Resources", file), file, true);

            IDemoParser parser = DemoUtil.GetDemoParser(file);

            Assert.IsNotNull(parser);

            string[] files = parser.GetRequiredFiles();
            Assert.AreEqual("doom2.wad", files[0]);
            Assert.AreEqual("pyrrhic_.wad", files[1]);
        }
    }
}
