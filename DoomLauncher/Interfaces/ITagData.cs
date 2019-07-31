namespace DoomLauncher.Interfaces
{
    public interface ITagData
    {
        int TagID { get; set; }
        string Name { get; set; }
        bool HasTab { get; set; }
        bool HasColor { get; set; }
        int? Color { get; set; }
    }
}
