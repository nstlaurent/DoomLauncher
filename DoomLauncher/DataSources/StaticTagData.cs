using DoomLauncher.DataSources;

namespace DoomLauncher
{
    class StaticTagData : TagData
    {
        public static string GetFavoriteName(string name) => string.Concat("● ", name);

        public override string FavoriteName => GetFavoriteName(Name);
    }
}
