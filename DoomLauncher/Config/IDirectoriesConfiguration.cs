
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
    }
}
