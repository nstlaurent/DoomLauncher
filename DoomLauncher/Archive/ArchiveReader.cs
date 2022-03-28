using System;
using System.IO;

namespace DoomLauncher
{
    public static class ArchiveReader
    {
        public static IArchiveReader EmptyArchiveReader = new EmptyArchiveReader();

        public static IArchiveReader Create(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return EmptyArchiveReader;

            if (Util.IsDirectory(path))
                return new DirectoryArchiveReader(path);

            if (IsPk(Path.GetExtension(path)))
                return new ZipArchiveReader(path);
            if (IsWad(Path.GetExtension(path)))
                return new WadArchiveReader(path);
            else
                return new FileArchiveReader(path);
        }

        private static bool IsWad(string ext)
        {
            return ext.Equals(".wad", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPk(string fileExtension)
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
