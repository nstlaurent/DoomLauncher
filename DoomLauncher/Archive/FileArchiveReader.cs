using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher
{
    class FileArchiveReader : IArchiveReader, IArchiveEntry
    {
        private readonly string m_file;
        private readonly List<IArchiveEntry> m_entries = new List<IArchiveEntry>();

        public bool EntriesHaveExtensions => true;

        public FileArchiveReader(string file)
        {
            m_file = file;
            m_entries.Add(this);
        }

        public long Length => new FileInfo(m_file).Length;

        public void Read(byte[] buffer, int offset, int length)
        {
            using (var stream = File.OpenRead(m_file))
                stream.Read(buffer, offset, length);
        }

        public IEnumerable<IArchiveEntry> Entries => m_entries;

        public string Name => Path.GetFileName(m_file);

        public string FullName => Path.GetFullPath(m_file);

        public bool ExtractRequired => false;

        public void ExtractToFile(string file, bool overwrite = false)
        {
            File.Copy(m_file, file, overwrite);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        public override bool Equals(object obj)
        {
            IArchiveEntry entry = obj as IArchiveEntry;
            if (entry == null)
                return false;

            return entry.FullName == FullName;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}
