using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DoomLauncher
{
    public class ZipArchiveReader : IArchiveReader
    {
        private readonly ZipArchive m_archive;

        public bool EntriesHaveExtensions => true;

        public ZipArchiveReader(string file)
        {
            m_archive = new ZipArchive(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
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
