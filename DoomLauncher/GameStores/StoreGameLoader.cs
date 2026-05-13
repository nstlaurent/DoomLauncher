using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;

namespace DoomLauncher.GameStores
{
    public static class StoreGameLoader
    {
        // As documented at https://help.steampowered.com/en/faqs/view/3C73-90F9-F600-0266
        private const string STEAM_REGISTRY_KEY_32 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";
        private const string STEAM_REGISTRY_KEY_64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";

        public delegate string GameFinder(StoreGame game); // null if not found

        public static GameStoreFiles LoadAllStoreGamesFromRegistry()
        {
            var gameFinders = new List<GameFinder> { GetSteamGameFolder, GetGogGameFolder };
            return LoadAllStoreGames(gameFinders);
        }

        private static string GetSteamGameFolder(StoreGame game)
        {
            var steamPath = GetSteamPath();
            if (Directory.Exists(steamPath))
            {
                var libraryPaths = GetSteamLibraryPaths(steamPath);
                foreach (var libraryPath in libraryPaths)
                {
                    var gameDirectory = GetSteamGameDirectory(libraryPath, game.SteamId);
                    if (gameDirectory != null)
                    {
                        return Path.Combine(libraryPath, @"steamapps\common", gameDirectory);
                    }
                }
            }

            return null;
        }

        private static string GetSteamPath()
        {
            var steamKey = Environment.Is64BitOperatingSystem ? STEAM_REGISTRY_KEY_64 : STEAM_REGISTRY_KEY_32;
            var installPath = Registry.GetValue(steamKey, "InstallPath", null);
            return installPath?.ToString();
        }

        private static List<string> GetSteamLibraryPaths(string steamPath)
        {
            var vdfPath = Path.Combine(steamPath, @"config\libraryfolders.vdf");
            if (TryLoadSteamFileToObject(vdfPath, out Dictionary<string, SteamLibraryFolder> libraryFolders))
            {
                return libraryFolders.Select(l => l.Value.Path).ToList();
            }
            return new List<string>();
        }

        private static string GetSteamGameDirectory(string libraryPath, int gameSteamId)
        {
            var acfPath = Path.Combine(libraryPath, $@"steamapps\appmanifest_{gameSteamId}.acf");
            if (TryLoadSteamFileToObject(acfPath, out SteamAppState steamAppState))
            {
                return steamAppState.InstallDir;
            }

            return null;
        }

        private static bool TryLoadSteamFileToObject<T>(string filePath, out T fileObject)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    fileObject = VdfConvert
                        .Deserialize(File.ReadAllText(filePath))
                        .ToJson()
                        .Value
                        .ToObject<T>();
                    return true;
                }
                catch (Exception ex)
                    when (ex is VdfException || ex is JsonSerializationException || ex is JsonReaderException)
                {
                    // Invalid .vdf/.acf, or not expected structure
                }
            }

            fileObject = default;
            return false;
        }

        private static string GetGogGameFolder(StoreGame game)
        {
            var gogKey = $@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\GOG.com\Games\{game.GogId}";
            return Registry.GetValue(gogKey, "path", null)?.ToString();
        }

        public static GameStoreFiles LoadAllStoreGames(List<GameFinder> gameFinders)
        {
            var gameStoreFiles = 
                from game in StoreGame.GAMES_IN_PRIORITY_ORDER
                from finder in gameFinders
                select LoadStoreGame(game, finder(game));

            return gameStoreFiles.Aggregate(GameStoreFiles.EMPTY, (a, b) => a.Combine(b));
        }


        public static GameStoreFiles LoadStoreGame(StoreGame game, string gamePath)
        {
            // Expected IWad/PWad files are allowed to not exist; Id Software has changed the directory structure 
            // a few times, and it's fine if we can't find something. We'll take what we can get.

            if (gamePath == null || !Directory.Exists(gamePath))
                return GameStoreFiles.EMPTY;

            List<string> installedIwads =
                (from iwad in game.ExpectedIWadFiles
                 let absolutePath = Path.Combine(gamePath, iwad)
                 where File.Exists(absolutePath)
                 select absolutePath).ToList();

            List<string> installedPwads =
                (from pwad in game.ExpectedPWadFiles
                 let absolutePath = Path.Combine(gamePath, pwad)
                 where File.Exists(absolutePath)
                 select absolutePath).ToList();

            string installedDoom64Exe = null;
            if (game.ExpectedDoom64Exe != null)
            {
                var absolutePath = Path.Combine(gamePath, game.ExpectedDoom64Exe);
                if (File.Exists(absolutePath))
                    installedDoom64Exe = absolutePath;
            }

            return new GameStoreFiles(installedIwads, installedPwads, installedDoom64Exe);
        }
    }
}
