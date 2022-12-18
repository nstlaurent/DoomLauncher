using System.Collections.Generic;
using System.IO;
using WadReader;

namespace DoomLauncher
{
    internal class WadArchiveReader : IArchiveReader
    {
        private readonly FileStream m_fs;
        private readonly List<WadEntry> m_entries = new List<WadEntry>();

        public IEnumerable<IArchiveEntry> Entries => m_entries;
        public bool EntriesHaveExtensions => false;
        public readonly bool IsValid;
        public readonly string Filename;

        public WadArchiveReader(string file)
        {
            Filename = file;
            m_fs = File.OpenRead(file);
            WadFileReader wadReader = new WadFileReader(m_fs);
            IsValid = wadReader.IsValid;
            var lumps = wadReader.ReadLumps();
            foreach (var lump in lumps)
                m_entries.Add(new WadEntry(m_fs, lump));
        }

        public void Dispose()
        {
            m_fs.Dispose();
        }
    }
}
