using Microsoft.Win32;
using System;
using System.IO;
using Gameloop.Vdf;
using System.Collections.Generic;
using System.Linq;
using System.Deployment.Application;
using Gameloop.Vdf.Linq;

namespace DoomLauncher.Steam
{
    public class SteamLoader
    {
        private const string STEAM_REGISTRY_KEY_32 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";
        private const string STEAM_REGISTRY_KEY_64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";
        
        public static SteamInstallation LoadFromEnvironment()
        {
            return LoadFromPath(CalculateSteamInstallPath());
        }

        // Loads a SteamInstallation from the installation path, throwing SteamLoaderException if anything goes wrong.
        public static SteamInstallation LoadFromPath(string steamInstallPath)
        {
            var configFile = Path.Combine(steamInstallPath, @"config\libraryfolders.vdf");
            if (!File.Exists(configFile))
                throw new SteamLoaderException($"Couldn't find Steam's {configFile} file, which would have told us where to find the Steam libraries");

            try
            {
                VToken libraryFolderConfig = VdfConvert.Deserialize(File.ReadAllText(configFile)).Value;
                List<string> libraryPaths = GetValveList(libraryFolderConfig).Select(x => x.Value<string>("path")).ToList();
                List<SteamLibrary> libraries = libraryPaths.Select(LoadLibrary).ToList();
                return new SteamInstallation(steamInstallPath, libraries);
            }
            catch (VdfException)
            {
                throw new SteamLoaderException($"Couldn't parse Valve KeyValues format in file {configFile}");
            }
                

            
        }

        // Retrieve a list encoded as a map with integer keys.
        // The VDF.GameLoop library doesn't seem to have a built-in way to do this.
        private static List<VToken> GetValveList(VToken parent)
        {
            List<VToken> list = new List<VToken>();
            bool keepLooking = true;
            int i = 0;
            while (keepLooking)
            {
                VToken child = parent[i.ToString()];
                if (child != null)
                {
                    list.Add(child);
                }
                else
                {
                    keepLooking = false;
                }
                i++;
            }
            return list;
        }

        private static SteamLibrary LoadLibrary(string libraryPath)
        {
            var installedGames = new List<SteamInstalledGame>();

            foreach (var game in SteamGame.GAMES_IN_PRIORITY_ORDER)
            {
                var gameConfigFile = Path.Combine(libraryPath, "steamapps", $"appmanifest_{game.GameId}.acf");

                // Game files are allowed to not exist - if they're not installed, then it is what it is
                if (File.Exists(gameConfigFile))
                {
                    dynamic gameConfig = VdfConvert.Deserialize(File.ReadAllText(gameConfigFile));
                    string installDir = gameConfig.Value.installdir.ToString();
                    var fullGamePath = Path.Combine(libraryPath, "steamapps", "common", installDir);
                    var installedGame = LoadSteamGame(game, fullGamePath);

                    installedGames.Add(installedGame);
                }
            }

            return new SteamLibrary(libraryPath, installedGames);

        }

        private static SteamInstalledGame LoadSteamGame(SteamGame game, string gamePath)
        {
            // Expected IWad/PWad files are allowed to not exist; Id Software has changed the directory structure 
            // a few times, and it's fine if we can't find something. We'll take what we can get.

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

            return new SteamInstalledGame(game, gamePath, installedIwads, installedPwads);
        }

        private static string CalculateSteamInstallPath()
        {
            var is64Bit = Environment.Is64BitProcess;
            string steamKey = is64Bit ? STEAM_REGISTRY_KEY_64 : STEAM_REGISTRY_KEY_32;
            var value = Registry.GetValue(steamKey, "InstallPath", null);
            if (value != null)
            {
                return value.ToString();
            }
            else
            {
                throw new SteamLoaderException("Steam is not installed");
            }
        }
    }

    public class SteamLoaderException : Exception
    {
        public SteamLoaderException(string message) : base(message)
        {

        }  
    }
}
