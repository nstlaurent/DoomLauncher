using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.GameStores.Steam
{
    public static class SteamLoader
    {
        public static string GetGameFolder(string steamPath, StoreGame game)
        {
            if (Directory.Exists(steamPath))
            {
                var libraryPaths = GetLibraryPaths(steamPath);
                foreach (var libraryPath in libraryPaths)
                {
                    var gameDirectory = GetGameDirectory(libraryPath, game.SteamId);
                    if (gameDirectory != null)
                    {
                        return Path.Combine(libraryPath, @"steamapps\common", gameDirectory);
                    }
                }
            }

            return null;
        }

        private static List<string> GetLibraryPaths(string steamPath)
        {
            var vdfPath = Path.Combine(steamPath, @"config\libraryfolders.vdf");
            if (SteamFileUtils.TryGetLibraryPaths(vdfPath, out var libraryPaths))
            {
                return libraryPaths;
            }
            return new List<string>();
        }

        private static string GetGameDirectory(string libraryPath, int gameSteamId)
        {
            var acfPath = Path.Combine(libraryPath, $@"steamapps\appmanifest_{gameSteamId}.acf");
            if (SteamFileUtils.TryGetInstallDir(acfPath, out var installDir))
            {
                return installDir;
            }

            return null;
        }
    }
}
