using SevenZip;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.Archive.SevenZip
{
    public class SevenZipArchiveReader : IArchiveReader
    {
        private readonly SevenZipExtractor m_extractor;
        private readonly MemoryStreamManager m_streamManager = new MemoryStreamManager();

        public SevenZipArchiveReader(string file)
        {
            SevenZipBase.SetLibraryPath(Path.Combine(Util.GetInteropDirectory(), "7z.dll"));
            m_extractor = new SevenZipExtractor(file);
        }

        public IEnumerable<IArchiveEntry> Entries
        {
            get
            {
                foreach (var entry in m_extractor.ArchiveFileData)
                    yield return new SevenZipArchiveEntry(m_extractor, entry, m_streamManager);
            }
        }

        public bool EntriesHaveExtensions => true;

        public void Dispose()
        {
            m_extractor.Dispose();
            foreach (var stream in m_streamManager.Streams)
                stream.Dispose();
        }
    }

    public class MemoryStreamManager
    {
        public List<MemoryStream> Streams = new List<MemoryStream>();
    }
}
