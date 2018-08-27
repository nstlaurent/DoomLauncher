using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    public class TagData : ITagData
    {
        public int TagID { get; set; }
        public string Name { get; set; }
        public bool HasTab { get; set; }
        public bool HasColor { get; set; }
        public int? Color { get; set; }

        public override bool Equals(object obj)
        {
            ITagData tag = obj as ITagData;

            if (tag != null)
            {
                return tag.TagID == this.TagID;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TagID;
        }
    }
}
