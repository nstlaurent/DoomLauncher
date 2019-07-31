namespace DoomLauncher
{
    public interface ICustomParam
    {
        int CustomParamID { get; set; }
        int GameFileID { get; set; }
        string FileName { get; set; }
        string Parameter { get; set; }
    }
}
