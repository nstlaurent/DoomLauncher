namespace DoomLauncher
{
    public enum FileType
    {
        Unknown = 0,
        Screenshot = 1,
        Demo = 2,
        SaveGame = 3,
        Thumbnail = 4,
        TileImage = 5,
        TitlePic = 6
    }

    public static class FileTypeExtensions
    {
        public static bool IsFixedContent(this FileType fileType) => 
            fileType == FileType.TileImage;
    }
}
