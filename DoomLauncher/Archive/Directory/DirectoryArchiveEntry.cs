using System.IO;

namespace DoomLauncher
{
    internal class DirectoryArchiveEntry : AbstractArchiveEntry
    {
        private readonly FileInfo m_file;

        public DirectoryArchiveEntry(string file)
        {
            m_file = new FileInfo(file);
        }

        public override long Length => m_file.Length;

        public override string Name => m_file.Name;

        public override string FullName => m_file.FullName;

        public override bool ExtractRequired => false;

        public override bool IsDirectory => false;

        public override void ExtractToFile(string file, bool overwrite = false)
        {
            m_file.CopyTo(file, overwrite);
        }

        public override void Read(byte[] buffer, int offset, int length)
        {
            using (var fs = m_file.OpenRead())
                fs.Read(buffer, offset, length);
        }

        public override string GetNameWithoutExtension() => Name;
    }
}
