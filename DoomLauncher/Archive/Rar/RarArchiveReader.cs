using SharpCompress.Archives.Rar;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.Archive.Rar
{
    public class RarArchiveReader : IArchiveReader
    {
        private readonly RarArchive m_archive;
        private readonly RarArchiveEntry[] m_entries;

        public RarArchiveReader(string file)
        {
            m_archive = RarArchive.Open(file);
            m_entries = m_archive.Entries.Where(x => RarArchiveEntry.IsValid(x)).Select(x => new RarArchiveEntry(x)).ToArray();
        }

        public IEnumerable<IArchiveEntry> Entries => m_entries;

        public bool EntriesHaveExtensions => true;

        public void Dispose()
        {
            m_archive.Dispose();
        }
    }
}
