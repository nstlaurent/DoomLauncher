using System.IO;
using System.IO.Compression;

namespace DoomLauncher
{
    class ZipArchiveReaderEntry : AbstractArchiveEntry
    {
        private readonly ZipArchiveEntry m_entry;

        public ZipArchiveReaderEntry(ZipArchiveEntry zipArchiveEntry)
        {
            m_entry = zipArchiveEntry;
        }

        public override long Length => m_entry.Length;

        public override void Read(byte[] buffer, int offset, int length)
        {
            m_entry.Open().Read(buffer, offset, length);
        }

        public override string Name => m_entry.Name;
        public override string FullName => m_entry.FullName;

        public override bool ExtractRequired => true;

        public override bool IsDirectory => m_entry.FullName.EndsWith("/");

        public override void ExtractToFile(string file, bool overwrite = false)
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

        public override string GetNameWithoutExtension() => Path.GetFileNameWithoutExtension(Name);
    }
}
