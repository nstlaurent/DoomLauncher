using DoomLauncher;
using System;
using System.Linq;

namespace UnitTest.Tests
{
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

        public override long Length => Tree.Content.Length;

        public override string Name => Tree.Name;

        public override string FullName => Name;

        public override bool ExtractRequired => false;

        public override bool IsDirectory => false;

        public override void ExtractToFile(string file, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public override string GetNameWithoutExtension() => Name;

        public override void Read(byte[] buffer, int offset, int length)
        {
            Tree.Content.Take(buffer.Length).ToArray().CopyTo(buffer, offset);
        }
    }
}