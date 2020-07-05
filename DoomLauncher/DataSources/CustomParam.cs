namespace DoomLauncher
{
    public class CustomParamDataSource : ICustomParam
    {
        public int CustomParamID { get; set; }
        public string FileName { get; set; }
        public string Parameter { get; set; }
        public int GameFileID { get; set; }
    }
}
