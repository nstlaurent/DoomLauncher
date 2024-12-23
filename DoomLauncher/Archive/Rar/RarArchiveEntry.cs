using SharpCompress.Archives;
using System.IO;

namespace DoomLauncher.Archive.Rar
{
    public class RarArchiveEntry : AbstractArchiveEntry
    {
        private readonly SharpCompress.Archives.Rar.RarArchiveEntry m_entry;

        public RarArchiveEntry(SharpCompress.Archives.Rar.RarArchiveEntry entry)
        {
            m_entry = entry;
        }

        public override long Length => m_entry.Size;

        public override string Name => Path.GetFileName(m_entry.Key);

        public override string FullName => m_entry.Key;

        public override bool ExtractRequired => true;

        public override bool IsDirectory => m_entry.IsDirectory;

        public override void ExtractToFile(string file, bool overwrite = false)
        {
            if (!overwrite && File.Exists(file))
                return;

            m_entry.WriteToFile(file);
        }

        public override void Read(byte[] buffer, int offset, int length)
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

        public override string GetNameWithoutExtension() => Path.GetFileNameWithoutExtension(Name);
    }
}
