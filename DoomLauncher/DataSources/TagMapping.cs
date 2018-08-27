using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    public class TagMapping : ITagMapping
    {
        public int TagID { get; set; }
        public int FileID { get; set; }

        public override bool Equals(object obj)
        {
            ITagMapping mapping = obj as ITagMapping;

            if (mapping != null)
            {
                return mapping.FileID == this.FileID && mapping.TagID == this.TagID;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return string.Format("{0},{1}", FileID, TagID).GetHashCode();
        }
    }
}
