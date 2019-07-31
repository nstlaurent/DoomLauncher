using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestLauncherPath
    {
        [TestMethod]
        public void TestPaths()
        {
            string folder = "relativepath\\";
            LauncherPath path = new LauncherPath(folder);

            Assert.AreEqual(folder, path.GetPossiblyRelativePath());
            Assert.AreEqual(Path.Combine(Directory.GetCurrentDirectory(), folder), path.GetFullPath());

            folder = Path.Combine("\\subdir", "relativepath\\");
            string newfolder = Path.Combine(Directory.GetCurrentDirectory(), folder);

            path = new LauncherPath(newfolder);

            Assert.AreEqual(folder, path.GetPossiblyRelativePath());
            Assert.AreEqual(Path.Combine(Directory.GetCurrentDirectory(), folder), path.GetFullPath());
        }
    }
}
