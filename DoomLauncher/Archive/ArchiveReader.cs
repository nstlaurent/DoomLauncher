using System;
using System.IO;

namespace DoomLauncher
{
    public static class ArchiveReader
    {
        public static IArchiveReader EmptyArchiveReader = new EmptyArchiveReader();

        public static IArchiveReader Create(string path, bool ignorePk3 = false)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return EmptyArchiveReader;

            if (Util.IsDirectory(path))
                return new DirectoryArchiveReader(path);

            if (!ignorePk3 && IsPk(Path.GetExtension(path)))
                return new ZipArchiveReader(path);
            else
                return new FileArchiveReader(path);
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
