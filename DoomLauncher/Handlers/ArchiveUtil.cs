using SevenZip;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DoomLauncher
{
    internal class ArchiveUtil
    {
        public static readonly string[] Exenstions = new string[] { ".7z", ".rar" };
        private static readonly string[] CoreExtensions = new string[] { ".zip", ".7z", ".rar" };

        public static bool IsTransformableToZip(string extension) => Exenstions.Contains(extension, StringComparer.OrdinalIgnoreCase);

        // If this is an archive that packages files together that Doom Launcher should read the contents of (".zip", ".7z", ".rar")
        public static bool ShouldReadPackagedArchive(string file) => CoreExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase);

        public static FileInfo CreateZipFrom(FileInfo fi, string tempDirectory)
        {
            if (fi.Extension.Equals(".7z", StringComparison.OrdinalIgnoreCase))
                return CreateZipFromSevenZip(fi, tempDirectory);
            else if (fi.Extension.Equals(".rar", StringComparison.OrdinalIgnoreCase))
                return CreateZipFromRar(fi, tempDirectory);

            throw new NotSupportedException();
        }

        private static FileInfo CreateZipFromSevenZip(FileInfo fi, string tempDirectory)
        {
            SevenZipBase.SetLibraryPath(Path.Combine(Util.GetInteropDirectory(), "7z.dll"));
            using (SevenZipExtractor extractor = new SevenZipExtractor(fi.FullName))
            {
                DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(tempDirectory, Guid.NewGuid().ToString()));
                extractor.ExtractArchive(dir.FullName);

                string zipFile = Path.Combine(tempDirectory, fi.Name.Replace(fi.Extension, ".zip"));
                ZipFile.CreateFromDirectory(dir.FullName, zipFile);

                Directory.Delete(dir.FullName, true);
                return new FileInfo(zipFile);
            }
        }

        private static FileInfo CreateZipFromRar(FileInfo fi, string tempDirectory)
        {
            using (RarArchive archive = RarArchive.Open(fi.FullName))
            {
                DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(tempDirectory, Guid.NewGuid().ToString()));

                var directoryEntires = archive.Entries.Where(entry => entry.IsDirectory);
                foreach (var dirEntry in directoryEntires)
                    Directory.CreateDirectory(Path.Combine(dir.FullName, dirEntry.Key));

                foreach (var entry in archive.Entries)
                {
                    string path = dir.FullName;
                    if (entry.Key.Contains(Path.DirectorySeparatorChar) || entry.Key.Contains(Path.AltDirectorySeparatorChar))
                        path = Path.Combine(dir.FullName, GetSubDirectoryPath(entry));

                    entry.WriteToDirectory(path);
                }

                string zipFile = Path.Combine(tempDirectory, fi.Name.Replace(fi.Extension, ".zip"));
                ZipFile.CreateFromDirectory(dir.FullName, zipFile);

                Directory.Delete(dir.FullName, true);
                return new FileInfo(zipFile);
            }
        }

        private static string GetSubDirectoryPath(RarArchiveEntry entry)
        {
            int index;
            if (entry.Key.Contains(Path.DirectorySeparatorChar))
                index = entry.Key.LastIndexOf(Path.DirectorySeparatorChar);
            else
                index = entry.Key.LastIndexOf(Path.AltDirectorySeparatorChar);

            if (index == -1)
                return string.Empty;
            
            return entry.Key.Substring(0, index);
        }
    }
}
