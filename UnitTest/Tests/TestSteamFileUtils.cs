using DoomLauncher.GameStores.Steam;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSteamFileUtils
    {
        [TestMethod]
        public void TryGetInstallDir_ReturnsTrueAndInstallDirWhenAcfIsValid()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamLibrary1\steamapps\appmanifest_2280.acf");
            var success = SteamFileUtils.TryGetInstallDir(path, out var installDir);
            
            Assert.IsTrue(success);
            Assert.AreEqual("TestDoom", installDir);
        }

        [TestMethod]
        public void TryGetInstallDir_ReturnsFalseAndNullWhenAcfDoesNotExist()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamLibrary1\steamapps\appmanifest_317040.acf");
            var success = SteamFileUtils.TryGetInstallDir(path, out var installDir);

            Assert.IsFalse(success);
            Assert.IsNull(installDir);
        }

        [TestMethod]
        public void TryGetInstallDir_ReturnsFalseAndNullWhenAcfIsInvalid()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamLibrary3\steamapps\appmanifest_2280.acf");
            var success = SteamFileUtils.TryGetInstallDir(path, out var installDir);

            Assert.IsFalse(success);
            Assert.IsNull(installDir);
        }

        [TestMethod]
        public void TryGetInstallDir_ReturnsFalseAndNullWhenInstallDirIsNotPresent()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamLibrary3\steamapps\appmanifest_2360.acf");
            var success = SteamFileUtils.TryGetInstallDir(path, out var installDir);

            Assert.IsFalse(success);
            Assert.IsNull(installDir);
        }

        [TestMethod]
        public void TryGetInstallDir_ReturnsFalseAndNullWhenInstallDirIsInvalid()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamLibrary3\steamapps\appmanifest_2390.acf");
            var success = SteamFileUtils.TryGetInstallDir(path, out var installDir);

            Assert.IsFalse(success);
            Assert.IsNull(installDir);
        }

        [TestMethod]
        public void TryGetLibraryPaths_ReturnsTrueAndLibraryPathsWhenVdfIsValid()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall\config\libraryfolders.vdf");
            var success = SteamFileUtils.TryGetLibraryPaths(path, out var libraryPaths);

            Assert.IsTrue(success);
            Assert.AreEqual(2, libraryPaths.Count);
            Assert.AreEqual(@"Resources\TestSteamLibrary1", libraryPaths[0]);
            Assert.AreEqual(@"Resources\TestSteamLibrary2", libraryPaths[1]);
        }

        [TestMethod]
        public void TryGetLibraryPaths_ReturnsFalseAndNullWhenVdfDoesNotExist()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall2\config\libraryfolders.vdf");
            var success = SteamFileUtils.TryGetLibraryPaths(path, out var libraryPaths);

            Assert.IsFalse(success);
            Assert.IsNull(libraryPaths);
        }

        [TestMethod]
        public void TryGetLibraryPaths_ReturnsFalseAndNullWhenVdfIsInvalid()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall3\config\libraryfolders.vdf");
            var success = SteamFileUtils.TryGetLibraryPaths(path, out var libraryPaths);

            Assert.IsFalse(success);
            Assert.IsNull(libraryPaths);
        }

        [TestMethod]
        public void TryGetLibraryPaths_ReturnsFalseAndNullWhenPathIsNotPresent()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall4\config\libraryfolders.vdf");
            var success = SteamFileUtils.TryGetLibraryPaths(path, out var libraryPaths);

            Assert.IsFalse(success);
            Assert.IsNull(libraryPaths);
        }

        [TestMethod]
        public void TryGetLibraryPaths_ReturnsFalseAndNullWhenPathIsInvalid()
        {
            var path = Path.GetFullPath(@"Resources\TestSteamInstall5\config\libraryfolders.vdf");
            var success = SteamFileUtils.TryGetLibraryPaths(path, out var libraryPaths);

            Assert.IsFalse(success);
            Assert.IsNull(libraryPaths);
        }
    }
}
