using System;
using System.IO;

namespace DoomLauncher
{
    public static class ArchiveReader
    {
        public static IArchiveReader Create(string file)
        {
            // TODO probably should peek file header and not rely on extension...
            if (IsPk(Path.GetExtension(file)))
                return new ZipArchiveReader(file);
            else
                return new FileArchiveReader(file);
        }

        private static bool IsPk(string fileExtension)
        {
            var extensions = new string[] { ".pk3", ".zip" };
            foreach (string ext in extensions)
            {
                if (fileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
