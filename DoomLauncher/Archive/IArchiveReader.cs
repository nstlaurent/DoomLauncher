using System;
using System.Collections.Generic;

namespace DoomLauncher
{
    public interface IArchiveReader : IDisposable
    {
        IEnumerable<IArchiveEntry> Entries { get; }
        bool EntriesHaveExtensions { get; }
    }
}
