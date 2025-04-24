using DoomLauncher;
using DoomLauncher.Config;

namespace UnitTest.Tests
{
    class DirectoriesConfiguration : IDirectoriesConfiguration
    {
        public LauncherPath GameFileDirectory { get; set; }

        public LauncherPath ScreenshotDirectory { get; set; }

        public LauncherPath SaveGameDirectory { get; set; }

        public LauncherPath TempDirectory { get; set; }

        public LauncherPath DemoDirectory { get; set; }

        public LauncherPath ThumbnailDirectory { get; set; }
    }
}
