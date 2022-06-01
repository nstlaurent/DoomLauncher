using System.IO.Compression;

namespace DoomLauncher
{
    class ZipArchiveReaderEntry : IArchiveEntry
    {
        private readonly ZipArchiveEntry m_entry;

        public ZipArchiveReaderEntry(ZipArchiveEntry zipArchiveEntry)
        {
            m_entry = zipArchiveEntry;
        }

        public long Length => m_entry.Length;

        public void Read(byte[] buffer, int offset, int length)
        {
            m_entry.Open().Read(buffer, offset, length);
        }

        public string Name => m_entry.Name;
        public string FullName => m_entry.FullName;

        public bool ExtractRequired => true;

        public bool IsDirectory => m_entry.FullName.EndsWith("/");

        public void ExtractToFile(string file, bool overwrite = false)
        {
            m_entry.ExtractToFile(file, overwrite);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IArchiveEntry entry))
                return false;

            return entry.FullName == FullName;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}
