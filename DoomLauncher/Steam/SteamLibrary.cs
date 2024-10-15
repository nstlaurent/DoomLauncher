using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.Steam
{
    public class SteamLibrary
    {
        public string LibraryPath { get; }

        public List<SteamInstalledGame> InstalledGames { get; }

        public SteamLibrary(string libraryPath, List<SteamInstalledGame> installedGames) 
        {
            LibraryPath = libraryPath;
            InstalledGames = installedGames;
        }
    }
}
