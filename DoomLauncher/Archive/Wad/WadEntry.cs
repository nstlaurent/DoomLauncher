using System.IO;
using WadReader;

namespace DoomLauncher
{
    public class WadEntry : AbstractArchiveEntry
    {
        private readonly FileStream m_fs;
        private readonly FileLump m_lump;

        public override long Length => m_lump.Length;

        public override string Name { get; }

        public override string FullName => Name;

        public override bool ExtractRequired => false;

        public override bool IsDirectory => false;

        public WadEntry(FileStream fs, FileLump lump)
        {
            m_fs = fs;
            m_lump = lump;
            Name = lump.Name;
        }

        public override void ExtractToFile(string file, bool overwrite = false)
        {
            File.WriteAllBytes(file, m_lump.ReadData(m_fs));
        }

        public override void Read(byte[] buffer, int offset, int length)
        {
            m_lump.ReadData(m_fs, buffer, offset, length);
        }

        public override string GetNameWithoutExtension() => Name;
    }
}
