using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WadReader;

namespace DoomLauncher
{
    internal class WadArchiveReader : IArchiveReader
    {
        private readonly FileStream m_fs;
        private readonly List<WadEntry> m_entries = new List<WadEntry>();

        public IEnumerable<IArchiveEntry> Entries => m_entries;
        public bool EntriesHaveExtensions => false;

        public WadArchiveReader(string file)
        {
            m_fs = File.OpenRead(file);
            WadFileReader wadReader = new WadFileReader(m_fs);
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
