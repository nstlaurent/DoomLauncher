using System.IO;

namespace DoomLauncher
{
    internal class DirectoryArchiveEntry : IArchiveEntry
    {
        private readonly FileInfo m_file;

        public DirectoryArchiveEntry(string file)
        {
            m_file = new FileInfo(file);
        }

        public long Length => m_file.Length;

        public string Name => m_file.Name;

        public string FullName => m_file.FullName;

        public bool ExtractRequired => false;

        public bool IsDirectory => false;

        public void ExtractToFile(string file, bool overwrite = false)
        {
            m_file.CopyTo(file, overwrite);
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            using (var fs = m_file.OpenRead())
                fs.Read(buffer, offset, length);
        }

        public string GetNameWithoutExtension() => Name;
    }
}
