using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace DoomLauncher
{
    class ZipArchiveReader : IArchiveReader
    {
        private readonly ZipArchive m_archive;

        public ZipArchiveReader(string file)
        {
            m_archive = ZipFile.OpenRead(file);
        }

        public IEnumerable<IArchiveEntry> Entries
        {
            get
            {
                foreach (var entry in m_archive.Entries)
                    yield return new ZipArchiveReaderEntry(entry);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            m_archive.Dispose();
        }
    }
}
