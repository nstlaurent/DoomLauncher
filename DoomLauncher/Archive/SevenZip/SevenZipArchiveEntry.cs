using SevenZip;
using System.IO;

namespace DoomLauncher.Archive.SevenZip
{
    public class SevenZipArchiveEntry : IArchiveEntry
    {
        private readonly SevenZipExtractor m_extractor;
        private readonly ArchiveFileInfo m_archiveFileInfo;
        private readonly MemoryStreamManager m_streamManager;
        private MemoryStream m_ms;

        public SevenZipArchiveEntry(SevenZipExtractor extractor, ArchiveFileInfo archiveFileInfo, MemoryStreamManager streamManager)
        {
            m_extractor = extractor;
            m_archiveFileInfo = archiveFileInfo;
            m_streamManager = streamManager;
        }

        public long Length => (long)m_archiveFileInfo.Size;

        public string Name => Path.GetFileName(m_archiveFileInfo.FileName);

        public string FullName => m_archiveFileInfo.FileName;

        public bool ExtractRequired => true;

        public bool IsDirectory => m_archiveFileInfo.IsDirectory;

        public void ExtractToFile(string file, bool overwrite = false)
        {
            if (!overwrite && File.Exists(file))
                return;

            using (FileStream fs = File.Create(file))
                m_extractor.ExtractFile(m_archiveFileInfo.Index, fs);
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            if (m_ms == null)
            {
                m_ms = new MemoryStream();
                m_streamManager.Streams.Add(m_ms);
                m_extractor.ExtractFile(m_archiveFileInfo.Index, m_ms);
            }

            m_ms.Position = 0;
            m_ms.Read(buffer, offset, length);
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
