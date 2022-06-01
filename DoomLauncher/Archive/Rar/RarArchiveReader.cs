using SharpCompress.Archives.Rar;
using System.Collections.Generic;

namespace DoomLauncher.Archive.Rar
{
    internal class RarArchiveReader : IArchiveReader
    {
        private readonly RarArchive m_archive;

        public RarArchiveReader(string file)
        {
            m_archive = RarArchive.Open(file);
        }

        public IEnumerable<IArchiveEntry> Entries
        {
            get
            {
                foreach (var entry in m_archive.Entries)
                    yield return new RarArchiveEntry(entry);
            }
        }

        public bool EntriesHaveExtensions => true;

        public void Dispose()
        {
            m_archive.Dispose();
        }
    }
}
