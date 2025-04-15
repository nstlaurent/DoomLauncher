using System;
using System.Collections.Generic;

namespace DoomLauncher.Archive
{
    public class RecursiveArchiveReader : IArchiveReader
    {
        private readonly IArchiveReader m_root;
        private readonly Func<IArchiveEntry, IArchiveReader> m_getChildReader;

        private delegate void DisposeMethod();
        private event DisposeMethod Disposing;

        public RecursiveArchiveReader(IArchiveReader root, Func<IArchiveEntry, IArchiveReader> getChildReader) 
        {
            m_root = root;
            m_getChildReader = getChildReader;
        }

        public IEnumerable<IArchiveEntry> Entries => 
            RecursiveSelect();

        private IEnumerable<IArchiveEntry> RecursiveSelect()
        {
            // Recursive enumerable, adapted from https://stackoverflow.com/a/30441479
            var stack = new Stack<IEnumerator<IArchiveEntry>>();
            IEnumerator<IArchiveEntry> enumerator = m_root.Entries.GetEnumerator();

            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        IArchiveEntry entry = enumerator.Current;

                        yield return entry;

                        var possibleReader = m_getChildReader(entry);
                        if (possibleReader != null)
                        {
                            var recursiveReader = new RecursiveArchiveReader(possibleReader, m_getChildReader);
                            Disposing += recursiveReader.Dispose;

                            stack.Push(enumerator);
                            enumerator = recursiveReader.Entries.GetEnumerator();
                        }
                    }
                    else if (stack.Count > 0)
                    {
                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();

                while (stack.Count > 0) // Clean up in case of an exception.
                {
                    enumerator = stack.Pop();
                    enumerator.Dispose();
                }
            }
        }

        public bool EntriesHaveExtensions => m_root.EntriesHaveExtensions;

        public void Dispose()
        {
            m_root.Dispose();
            Disposing?.Invoke();
        }
    }
}
