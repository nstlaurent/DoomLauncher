using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.Steam
{
    public class SteamLibrary
    {
        private readonly string m_libraryPath;
        public List<SteamInstalledGame> InstalledGames { get; }

        public SteamLibrary(string libraryPath, List<SteamInstalledGame> installedGames) 
        {
            m_libraryPath = libraryPath;
            InstalledGames = installedGames;
        }

        public List<string> GetInstalledIWads() =>
            InstalledGames.SelectMany(x => x.InstalledIWads).ToList();

        public List<string> GetInstalledPWads() =>
            InstalledGames.SelectMany(x => x.InstalledPWads).ToList();
    }
}
