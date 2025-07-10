using DoomLauncher;
using DoomLauncher.Config;
using DoomLauncher.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestTileImageHandler
    {
        private readonly IDirectoriesConfiguration config = new DirectoriesConfiguration()
        {
            TileImageDirectory = new LauncherPath(@"TileImages")
        };

        [TestMethod]
        public void GetTileImage_GetsAppropriateIWadImage()
        {
            var tileImageHandler = new TileImageHandler();

            var fileData = tileImageHandler.GetTileImage(IWadInfo.Doom);
            Assert.AreEqual("doom.png", fileData.FileName);
            Assert.AreEqual(FileType.TileImage, fileData.FileTypeID);

            fileData = tileImageHandler.GetTileImage(IWadInfo.Doom2);
            Assert.AreEqual("doom2.png", fileData.FileName);
            Assert.AreEqual(FileType.TileImage, fileData.FileTypeID);

            fileData = tileImageHandler.GetTileImage(IWadInfo.Heretic);
            Assert.AreEqual("heretic.png", fileData.FileName);
            Assert.AreEqual(FileType.TileImage, fileData.FileTypeID);

            fileData = tileImageHandler.GetTileImage(IWadInfo.Hexen);
            Assert.AreEqual("hexen.png", fileData.FileName);
            Assert.AreEqual(FileType.TileImage, fileData.FileTypeID);
        }
    }
}
