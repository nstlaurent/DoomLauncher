using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.Tests
{
    public class Tree
    {
        public string Name { get; }

        public byte[] Content { get; }

        public string ContentString =>
            Encoding.UTF7.GetString(Content);

        public List<Tree> Children { get; }

        public bool HasChildren => Children.Count > 0;

        public Tree(string name, params Tree[] children) : this(name, name, children)
        {

        }

        public Tree(string name, byte[] content, params Tree[] children)
        {
            Name = name;
            Content = content;
            Children = children.ToList();
        }

        public Tree(string name, string content, params Tree[] children)
        {
            Name = name;
            Content = Encoding.UTF7.GetBytes(content);
            Children = children.ToList();
        }
    }
}