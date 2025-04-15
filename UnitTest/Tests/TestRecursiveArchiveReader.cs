using DoomLauncher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using DoomLauncher.Archive;

namespace UnitTest.Tests
{
    public class Tree
    {
        public string Name { get; }

        public List<Tree> Children { get; }

        public bool HasChildren => Children.Count > 0;

        public Tree(string name, params Tree[] children)
        {
            Name = name;
            Children = children.ToList();
        }
    }

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

    public class TreeEntry : AbstractArchiveEntry
    {
        public Tree Tree { get; }

        public TreeEntry(Tree tree)
        {
            Tree = tree;
        }

        public TreeReader GetTreeReader()
        {
            if (Tree.HasChildren)
                return new TreeReader(Tree);
            else
                return null;
        }

        public override long Length => 1;

        public override string Name => Tree.Name;

        public override string FullName => Name;

        public override bool ExtractRequired => false;

        public override bool IsDirectory => false;

        public override void ExtractToFile(string file, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public override string GetNameWithoutExtension()
        {
            throw new NotImplementedException();
        }

        public override void Read(byte[] buffer, int offset, int length)
        {
            throw new NotImplementedException();
        }
    }


    [TestClass]
    public class TestRecursiveArchiveReader
    {


        [TestInitialize]
        public void Initialize()
        {
            TreeReader.TreeReadersCreated = 0;
            TreeReader.TreeReadersDisposed = 0;
        }


        private IArchiveReader GetReaderFromEntry(IArchiveEntry entry) =>
            (entry as TreeEntry)?.GetTreeReader();

        [TestMethod]
        public void EntriesIncludesNextLevelDown()
        {
            var tree = new Tree("wads",
                new Tree("wads/a"),
                new Tree("wads/b"),
                new Tree("wads/id",
                    new Tree("wads/id/c"),
                    new Tree("wads/id/d")));

            IArchiveReader root = new TreeReader(tree);
            var reader = new RecursiveArchiveReader(root, GetReaderFromEntry);
            var entryNames = reader.Entries.Select(e => e.Name).ToList();

            Assert.AreEqual(5, entryNames.Count);
            Assert.AreEqual("wads/a", entryNames[0]);
            Assert.AreEqual("wads/b", entryNames[1]);
            Assert.AreEqual("wads/id", entryNames[2]);
            Assert.AreEqual("wads/id/c", entryNames[3]);
            Assert.AreEqual("wads/id/d", entryNames[4]);

        }

        [TestMethod]
        public void EntriesRespectUnderlyingLaziness()
        {
            var tree = new Tree("fruit",
                new Tree("apple"),
                new Tree("banana"),
                new Tree("berries", 
                    new Tree("strawberry"), 
                    new Tree("cherry"), 
                    new Tree("ILLEGAL")),
                new Tree("ILLEGAL"));

            IArchiveReader root = new TreeReader(tree);
            var reader = new RecursiveArchiveReader(root, (entry) =>
            {
                if (entry.Name == "ILLEGAL")
                    throw new Exception("YOU WEREN'T SUPPOSED TO LOOK AT THIS ONE!");
                else
                    return GetReaderFromEntry(entry);
            });
            var entryNames = reader.Entries.Take(5).Select(e => e.Name).ToList();
            Assert.AreEqual(5, entryNames.Count);
            Assert.AreEqual("apple", entryNames[0]);
            Assert.AreEqual("banana", entryNames[1]);
            Assert.AreEqual("berries", entryNames[2]);
            Assert.AreEqual("strawberry", entryNames[3]);
            Assert.AreEqual("cherry", entryNames[4]);
        }

        [TestMethod]
        public void DisposeRecursiveReaderDisposesAllTheChildReaders()
        {
            var tree = new Tree("wads", // 1st TreeReader
                new Tree("wads/a"),
                new Tree("wads/b"),
                new Tree("wads/id", // 2nd TreeReader
                    new Tree("wads/id/c"),
                    new Tree("wads/id/d")));

            Assert.AreEqual(0, TreeReader.TreeReadersCreated);
            Assert.AreEqual(0, TreeReader.TreeReadersDisposed);

            IArchiveReader root = new TreeReader(tree);

            Assert.AreEqual(1, TreeReader.TreeReadersCreated);
            Assert.AreEqual(0, TreeReader.TreeReadersDisposed);

            using (var reader = new RecursiveArchiveReader(root, GetReaderFromEntry))
            {
                var makeItExplicit = reader.Entries.ToList();
            }

            Assert.AreEqual(2, TreeReader.TreeReadersCreated);
            Assert.AreEqual(2, TreeReader.TreeReadersDisposed);
        }
    }
}
