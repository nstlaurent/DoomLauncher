using System.IO;
using WadReader;

namespace DoomLauncher
{
    public class WadEntry : IArchiveEntry
    {
        private readonly FileStream m_fs;
        private readonly FileLump m_lump;

        public long Length => m_lump.Length;

        public string Name { get; private set; }

        public string FullName => Name;

        public bool ExtractRequired => false;

        public bool IsDirectory => false;

        public WadEntry(FileStream fs, FileLump lump)
        {
            m_fs = fs;
            m_lump = lump;
            Name = lump.Name;
        }

        public void ExtractToFile(string file, bool overwrite = false)
        {
            File.WriteAllBytes(file, m_lump.ReadData(m_fs));
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            m_lump.ReadData(m_fs, buffer, offset, length);
        }
    }
}
