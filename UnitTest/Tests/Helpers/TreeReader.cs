using DoomLauncher;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    public class TreeReader : IArchiveReader
    {
        public static int TreeReadersCreated { get; set; }
        public static int TreeReadersDisposed { get; set; }

        public Tree Tree { get; }

        public TreeReader(Tree tree)
        {
            Tree = tree;
            TreeReadersCreated++;
        }

        public IEnumerable<IArchiveEntry> Entries =>
            Tree.Children.Select(tree => new TreeEntry(tree));

        public bool EntriesHaveExtensions => false;

        public void Dispose()
        {
            TreeReadersDisposed++;
        }
    }
}