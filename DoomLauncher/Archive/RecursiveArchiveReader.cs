using System;
using System.Collections.Generic;

namespace DoomLauncher.Archive
{
    public class RecursiveArchiveReader : IArchiveReader
    {
        public IArchiveReader Root { get; }
        private readonly Func<IArchiveEntry, IArchiveReader> m_getChildReader; // null return value means no children

        private delegate void DisposeMethod();
        private event DisposeMethod Disposing;

        public RecursiveArchiveReader(IArchiveReader root, Func<IArchiveEntry, IArchiveReader> getChildReader) 
        {
            Root = root;
            m_getChildReader = getChildReader;
        }

        public IEnumerable<IArchiveEntry> Entries => 
            RecursiveSelect();

        private IEnumerable<IArchiveEntry> RecursiveSelect()
        {
            // Recursive enumerable, adapted from https://stackoverflow.com/a/30441479
            var stack = new Stack<IEnumerator<IArchiveEntry>>();
            IEnumerator<IArchiveEntry> enumerator = Root.Entries.GetEnumerator();

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

        public bool EntriesHaveExtensions => Root.EntriesHaveExtensions;

        public void Dispose()
        {
            Root.Dispose();
            Disposing?.Invoke();
        }
    }
}
