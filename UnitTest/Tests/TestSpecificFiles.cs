using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestSpecificFiles
    {
        private static readonly string ResourceFolder = "Resources";
        private static readonly string[] SupportedExtensions = new string[] { ".wad", };

        [TestMethod]
        public void ZipFileManaged()
        {
            IGameFile gameFile = CreateGameFile(Path.Combine(ResourceFolder, "pyrrhic.zip"), managed: true);
            string[] files = SpecificFilesForm.GetSupportedFiles(ResourceFolder, gameFile, SupportedExtensions);

            Assert.IsFalse(gameFile.IsDirectory());
            Assert.IsFalse(gameFile.IsUnmanaged());
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual("pyrrhic_.wad", files[0]);
        }

        [TestMethod]
        public void ZipFileUnmanaged()
        {
            // Unmanaged zip files should still extract contents like normal
            string path = Path.GetFullPath(Path.Combine(ResourceFolder, "pyrrhic.zip"));
            IGameFile gameFile = CreateGameFile(path, managed: false);
            string[] files = SpecificFilesForm.GetSupportedFiles(ResourceFolder, gameFile, SupportedExtensions);

            Assert.IsFalse(gameFile.IsDirectory());
            Assert.IsTrue(gameFile.IsUnmanaged());
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual("pyrrhic_.wad", files[0]);
        }

        [TestMethod]
        public void Pk3FileUnmanaged()
        {
            // A pk3 is a zip, but should not have contents extracted
            string path = Path.GetFullPath(Path.Combine(ResourceFolder, "testpk3.pk3"));
            IGameFile gameFile = CreateGameFile(path, managed: false);
            string[] files = SpecificFilesForm.GetSupportedFiles(ResourceFolder, gameFile, SupportedExtensions);

            Assert.IsFalse(gameFile.IsDirectory());
            Assert.IsTrue(gameFile.IsUnmanaged());
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(path, files[0]);
        }

        [TestMethod]
        public void UnmanagedFile()
        {
            // The extension doesn't matter, it's unamanged so it should be added
            TestUtil.ExtractResourceToDirectory("pyrrhic.zip", "Doom");
            string path = Path.GetFullPath(Path.Combine("Doom", "pyrrhic_.wad"));
            IGameFile gameFile = CreateGameFile(path, managed: false);
            string[] files = SpecificFilesForm.GetSupportedFiles("Doom", gameFile, SupportedExtensions);

            Assert.IsFalse(gameFile.IsDirectory());
            Assert.IsTrue(gameFile.IsUnmanaged());
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(path, files[0]);
        }

        [TestMethod]
        public void Folder()
        {
            // Extension doesn't matter, it's an unmanaged folder so it should be added
            TestUtil.ExtractResourceToDirectory("pyrrhic.zip", "Doom");
            string path = Path.GetFullPath("Doom");
            IGameFile gameFile = CreateGameFile(path, managed: false);
            string[] files = SpecificFilesForm.GetSupportedFiles("Doom", gameFile, SupportedExtensions);

            Assert.IsTrue(gameFile.IsDirectory());
            Assert.IsTrue(gameFile.IsUnmanaged());
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(path, files[0]);
        }

        private static IGameFile CreateGameFile(string path, bool managed)
        {
            string filename = managed ? Path.GetFileName(path) : path;

            return new GameFile()
            {
                FileName = filename,                
            };
        }
    }
}
