using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestStartupImageSyncAction
    {
        [TestMethod]
        public void ApplyToGameFile_CanProcessPNGImages()
        {
            var gameFile = new GameFile
            {
                FileName = "blahblah.wad"
            };

            byte[] pngBytes = File.ReadAllBytes(@"resources\happy.png");

            Tree files = new Tree("root", new Tree("STARTUP", pngBytes));
            var reader = new TreeReader(files);

            var syncAction = new StartupImageSyncAction();
            var result = syncAction.ApplyToGameFile(gameFile, reader, null);

            Assert.IsTrue(result.GetTitlePic(gameFile, out _));
        }

        [TestMethod]
        public void ApplyToGameFile_CanProcessPlanarImages()
        {
            var gameFile = new GameFile
            {
                FileName = "wad_with_planar.wad"
            };

            byte[] planarBytes = File.ReadAllBytes(@"resources\planarimage.dat");

            Tree files = new Tree("root", new Tree("STARTUP.dat", planarBytes));
            var reader = new TreeReader(files);

            var syncAction = new StartupImageSyncAction();
            var result = syncAction.ApplyToGameFile(gameFile, reader, null);

            Assert.IsTrue(result.GetTitlePic(gameFile, out _));
        }

        [TestMethod]
        public void ApplyToGameFile_IgnoreStartupDotWad()
        {
            var gameFile = new GameFile
            {
                FileName = "wad_with_planar.wad"
            };

            byte[] planarBytes = File.ReadAllBytes(@"resources\planarimage.dat");

            Tree files = new Tree("root", new Tree("STARTUP.wad", planarBytes));
            var reader = new TreeReader(files);

            var syncAction = new StartupImageSyncAction();
            var result = syncAction.ApplyToGameFile(gameFile, reader, null);

            Assert.IsFalse(result.GetTitlePic(gameFile, out _));
        }

        [TestMethod]
        public void ApplyToGameFile_IgnoresStrifeStartupImage()
        {
            var gameFile = new GameFile
            {
                FileName = "wad_with_planar.wad"
            };

            byte[] planarBytes = File.ReadAllBytes(@"resources\planarimage.dat");

            Tree files = new Tree("root", new Tree("STARTUP0", planarBytes));
            var reader = new TreeReader(files);

            var syncAction = new StartupImageSyncAction();
            var result = syncAction.ApplyToGameFile(gameFile, reader, null);

            Assert.IsFalse(result.GetTitlePic(gameFile, out _));
        }


        [TestMethod]
        public void ApplyToGameFile_NonsenseDoesntProduceAnImage()
        {
            var gameFile = new GameFile
            {
                FileName = "wadname.wad"
            };

            byte[] planarBytes = File.ReadAllBytes(@"resources\planarimage.dat");

            Tree files = new Tree("root", new Tree("STARTUP.blah", new byte[] { 0x01, 0x02, 0x03, 0xab, 0xcd }));
            var reader = new TreeReader(files);

            var syncAction = new StartupImageSyncAction();
            var result = syncAction.ApplyToGameFile(gameFile, reader, null);

            Assert.IsFalse(result.GetTitlePic(gameFile, out _));
        }
    }
}
