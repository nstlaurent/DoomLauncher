using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            if (SteamFileUtils.TryLoadToObject(vdfPath, out Dictionary<string, SteamLibraryFolder> libraryFolders))
            {
                return libraryFolders.Select(l => l.Value.Path).ToList();
            }
            return new List<string>();
        }

        private static string GetGameDirectory(string libraryPath, int gameSteamId)
        {
            var acfPath = Path.Combine(libraryPath, $@"steamapps\appmanifest_{gameSteamId}.acf");
            if (SteamFileUtils.TryLoadToObject(acfPath, out SteamAppState steamAppState))
            {
                return steamAppState.InstallDir;
            }

            return null;
        }
    }
}
