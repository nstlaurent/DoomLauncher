using SharpCompress.Archives;
using System.IO;

namespace DoomLauncher.Archive.Rar
{
    public class RarArchiveEntry : IArchiveEntry
    {
        private readonly SharpCompress.Archives.Rar.RarArchiveEntry m_entry;

        public RarArchiveEntry(SharpCompress.Archives.Rar.RarArchiveEntry entry)
        {
            m_entry = entry;
        }

        public long Length => m_entry.Size;

        public string Name => Path.GetFileName(m_entry.Key);

        public string FullName => m_entry.Key;

        public bool ExtractRequired => true;

        public bool IsDirectory => m_entry.IsDirectory;

        public void ExtractToFile(string file, bool overwrite = false)
        {
            if (!overwrite && File.Exists(file))
                return;

            m_entry.WriteToFile(file);
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            using (MemoryStream ms = new MemoryStream(buffer, offset, length))
                m_entry.WriteTo(ms);
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

        public string GetNameWithoutExtension() => Path.GetFileNameWithoutExtension(Name);
    }
}
