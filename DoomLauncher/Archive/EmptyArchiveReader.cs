using System;
using System.Collections.Generic;

namespace DoomLauncher
{
    public class EmptyArchiveReader : IArchiveReader
    {
        public IEnumerable<IArchiveEntry> Entries => Array.Empty<IArchiveEntry>();

        public void Dispose()
        {
        }
    }
}
