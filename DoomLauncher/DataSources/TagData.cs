using DoomLauncher.Interfaces;

namespace DoomLauncher.DataSources
{
    public class TagData : ITagData
    {
        public int TagID { get; set; }
        public string Name { get; set; }
        public bool HasTab { get; set; }
        public bool HasColor { get; set; }
        public int? Color { get; set; }
        public bool ExcludeFromOtherTabs { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ITagData tag)
                return tag.TagID == TagID;

            return false;
        }

        public override int GetHashCode()
        {
            return TagID;
        }
    }
}
