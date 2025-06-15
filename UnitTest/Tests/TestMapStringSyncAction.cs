using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestMapStringSyncAction
    {
        [TestMethod]
        public void MapString()
        {
            var mapInfo = @"
                map map01 {
                    music = ""test1""
                }

                MAP MAP02 {
                    music = ""test2""
                }
            ";

            var frag = new MapStringSyncAction(new LauncherPath(Directory.GetCurrentDirectory()));
            var gameFile = new GameFile();
            frag.ApplyToGameFile(gameFile, null, new string[] { mapInfo });

            Assert.AreEqual("map01, MAP02", gameFile.Map);
            Assert.AreEqual(2, gameFile.MapCount);
        }
    }
}
