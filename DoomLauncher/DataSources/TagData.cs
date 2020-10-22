using DoomLauncher.Interfaces;

namespace DoomLauncher.DataSources
{
    public class TagData : ITagData
    {
        public static readonly string FavoriteChar = "★";

        public int TagID { get; set; }
        public string Name { get; set; }
        public bool HasTab { get; set; }
        public bool HasColor { get; set; }
        public int? Color { get; set; }
        public bool ExcludeFromOtherTabs { get; set; }
        public bool Favorite { get; set; }

        public virtual string FavoriteName => Favorite ? string.Concat(FavoriteChar, " ", Name) : Name;

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
