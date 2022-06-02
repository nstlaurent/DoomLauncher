using DoomLauncher.Archive.Rar;
using DoomLauncher.Archive.SevenZip;
using System;
using System.IO;

namespace DoomLauncher
{
    public static class ArchiveReader
    {
        public static IArchiveReader EmptyArchiveReader = new EmptyArchiveReader();

        public static string SevenZipInteropLibrary { get; private set; } = Path.Combine(Util.GetInteropDirectory(), "7z.dll");

        public static void SetSevenZipInteropLibrary(string file) =>
            SevenZipInteropLibrary = file;

        public static IArchiveReader Create(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return EmptyArchiveReader;

            if (Util.IsDirectory(path))
                return new DirectoryArchiveReader(path);

            string ext = Path.GetExtension(path);
            if (IsPk(ext))
                return new ZipArchiveReader(path);
            if (IsWad(ext))
                return new WadArchiveReader(path);
            if (IsSevenZip(ext))
                return new SevenZipArchiveReader(path, SevenZipInteropLibrary);
            if (IsRar(ext))
                return new RarArchiveReader(path);
            
            return new FileArchiveReader(path);
        }

        private static bool IsRar(string ext) =>
            ext.Equals(".rar", StringComparison.OrdinalIgnoreCase);

        private static bool IsSevenZip(string ext) =>
            ext.Equals(".7z", StringComparison.OrdinalIgnoreCase);

        public static bool IsWad(string ext) =>
            ext.Equals(".wad", StringComparison.OrdinalIgnoreCase);

        public static bool IsPk(string fileExtension)
        {
            var extensions = Util.GetReadablePkExtensions();
            foreach (string ext in extensions)
            {
                if (fileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
