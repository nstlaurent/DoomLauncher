
using System;

namespace DoomLauncher.Config
{
    public interface IDirectoriesConfiguration
    {
        LauncherPath GameFileDirectory { get; }
        LauncherPath ScreenshotDirectory { get; }
        LauncherPath SaveGameDirectory { get; }
        LauncherPath TempDirectory { get; }
        LauncherPath DemoDirectory { get; }
        LauncherPath ThumbnailDirectory { get; }
        LauncherPath TitlePicDirectory { get; }
        LauncherPath TileImageDirectory { get; }
    }

    public static class IDirectoriesConfigurationExtensions
    {
        public static LauncherPath GetFileDirectory(this IDirectoriesConfiguration config, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Screenshot:
                    return config.ScreenshotDirectory;
                case FileType.Demo:
                    return config.DemoDirectory;
                case FileType.SaveGame:
                    return config.SaveGameDirectory;
                case FileType.Thumbnail:
                    return config.ThumbnailDirectory;
                case FileType.TitlePic:
                    return config.TitlePicDirectory;
                case FileType.TileImage:
                    return config.TileImageDirectory;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, "Unknown file type");
            }
        }
    }

}
