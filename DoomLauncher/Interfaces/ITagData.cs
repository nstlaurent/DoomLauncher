namespace DoomLauncher.Interfaces
{
    public interface ITagData
    {
        int TagID { get; set; }
        string Name { get; set; }
        bool HasTab { get; set; }
        bool HasColor { get; set; }
        int? Color { get; set; }
        bool ExcludeFromOtherTabs { get; set; }
        bool Favorite { get; set; }
        string FavoriteName { get; }
    }
}
