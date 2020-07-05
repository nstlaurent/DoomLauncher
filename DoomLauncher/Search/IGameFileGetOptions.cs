namespace DoomLauncher
{
    public interface IGameFileGetOptions
    {
        GameFileFieldType[] SelectFields { get; set; }
        GameFileSearchField SearchField { get; set; }
        GameFileFieldType? OrderField { get; set; }
        OrderType? OrderBy { get; set; }
        int? Limit { get; set; }
    }
}
