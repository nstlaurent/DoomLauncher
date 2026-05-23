using SharpCompress.Archives.Rar;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.Archive.Rar
{
    public class RarArchiveReader : IArchiveReader
    {
        private readonly IRarArchive m_archive;
        private readonly RarArchiveEntry[] m_entries;

        public RarArchiveReader(string file)
        {
            m_archive = RarArchive.OpenArchive(file);
            m_entries = m_archive.Entries.Where(x =>
            {
                if (x is SharpCompress.Archives.Rar.RarArchiveEntry)
                {
                    return RarArchiveEntry.IsValid(x as SharpCompress.Archives.Rar.RarArchiveEntry);
                }
                return false;
            }).Select(x => new RarArchiveEntry(x as SharpCompress.Archives.Rar.RarArchiveEntry)).ToArray();
        }

        public IEnumerable<IArchiveEntry> Entries => m_entries;

        public bool EntriesHaveExtensions => true;

        public void Dispose()
        {
            m_archive.Dispose();
        }
    }
}
