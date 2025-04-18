using System.Collections.Generic;
using System.Linq;

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
}
