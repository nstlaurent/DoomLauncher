using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestArchives
    {
        [DllImport("kernel32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);

        [TestMethod]
        public void TestCompressedArchives()
        {
            string[] files = new string[] { "archive.zip", "archive.rar", "archive.7z" };
            // Unit test architecture may not match so we have to check load failure
            ArchiveReader.SetSevenZipInteropLibrary(Path.Combine("x64", "7z.dll"));
            if (LoadLibrary(ArchiveReader.SevenZipInteropLibrary) == IntPtr.Zero)
                ArchiveReader.SetSevenZipInteropLibrary(Path.Combine("x86", "7z.dll"));

            foreach (string file in files)
            {
                using (IArchiveReader archive = ArchiveReader.Create(Path.Combine("Resources", file)))
                {
                    List<IArchiveEntry> entries = archive.Entries.OrderBy(x => x.FullName).ToList();
                    int index = 0;
                    // .NET zip implementation doesn't include the first folder as an entry...
                    if (archive is ZipArchiveReader)
                    {
                        Assert.AreEqual(5, entries.Count);
                    }
                    else
                    {
                        Assert.AreEqual(6, entries.Count);
                        Assert.AreEqual("Folder/", GetFullPath(entries[0]));
                        Assert.IsTrue(entries[0].IsDirectory);
                        index = 1;
                    }

                    Assert.AreEqual("Folder/SubFolder/", GetFullPath(entries[index]));
                    Assert.IsTrue(entries[index].IsDirectory);

                    Assert.AreEqual("Folder/SubFolder/othertextfile.txt", GetFullPath(entries[index + 1]));
                    Assert.IsFalse(entries[index + 1].IsDirectory);

                    Assert.AreEqual("Folder/switch.WAD", GetFullPath(entries[index + 2]));
                    Assert.IsFalse(entries[index + 2].IsDirectory);

                    Assert.AreEqual("Folder/textfile.txt", GetFullPath(entries[index + 3]));
                    Assert.IsFalse(entries[index + 3].IsDirectory);

                    Assert.AreEqual("mapinfo.txt", GetFullPath(entries[index + 4]));
                    Assert.IsFalse(entries[index + 4].IsDirectory);
                }
            }
        }

        private static string GetFullPath(IArchiveEntry entry)
        {
            string path = entry.FullName.Replace("\\", "/");
            if (entry.IsDirectory && !path.EndsWith("/"))
                path += "/";
            return path;
        }
    }
}
