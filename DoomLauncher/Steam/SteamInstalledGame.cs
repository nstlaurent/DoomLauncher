
using System.Collections.Generic;

namespace DoomLauncher.Steam
{
    public class SteamInstalledGame
    {
        private readonly string m_gamePath;

        public SteamInstalledGame(SteamGame game, string gamePath, List<string> installedIwads, List<string> installedPwads) 
        {
            Game = game;
            m_gamePath = gamePath;
            InstalledIWads = installedIwads;
            InstalledPWads = installedPwads;
        }

        public SteamGame Game { get; }

        public List<string> InstalledIWads { get; }

        public List<string> InstalledPWads { get; }
    }
}
