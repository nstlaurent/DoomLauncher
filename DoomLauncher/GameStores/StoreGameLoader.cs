using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.GameStores
{
    public static class StoreGameLoader
    {
        public delegate string GameFinder(StoreGame game); // null if not found

        public static GameStoreFiles LoadAllStoreGamesFromRegistry()
        {
            var gameFinders = new List<GameFinder> { GetSteamGameFolder, GetGogGameFolder };
            return LoadAllStoreGames(gameFinders);
        }

        private static string GetSteamGameFolder(StoreGame game)
        {
            var steamKey = $@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {game.SteamId}";
            return Registry.GetValue(steamKey, "InstallLocation", null)?.ToString();
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
