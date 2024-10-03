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
        private const string STEAM_REGISTRY_KEY_32 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam";
        private const string STEAM_REGISTRY_KEY_64 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam";
        
        public static SteamInstallation LoadFromEnvironment()
        {
            return LoadFromPath(CalculateSteamInstallPath());
        }

        private static void assertFileExists(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException($"Expected file {filename} not found");
            Console.WriteLine($"Confirmed: {filename} exists");
        }

        public static SteamInstallation LoadFromPath(string steamInstallPath)
        {
            var configFile = Path.Combine(steamInstallPath, @"config\libraryfolders.vdf");
            assertFileExists(configFile);

            VToken libraryFolderConfig = VdfConvert.Deserialize(File.ReadAllText(configFile)).Value;
            List<string> libraryPaths = GetValveList(libraryFolderConfig).Select(x => x.Value<string>("path")).ToList();
            List<SteamLibrary> libraries = libraryPaths.Select(LoadLibrary).ToList();

            return new SteamInstallation(steamInstallPath, libraries);
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
            return Registry.GetValue(steamKey, "InstallPath", null).ToString();
        }
    }
}
