using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;

namespace DoomLauncher.Steam
{
    public class SteamInstallation
    {
        public string InstallPath { get; }
        private readonly List<SteamLibrary> m_libraries;

        public SteamInstallation(string installPath, List<SteamLibrary> libraries)
        {
            InstallPath = installPath;
            m_libraries = libraries;
        }

        public List<string> GetInstalledIWads()
        {
            return getInstalledWads(game => game.InstalledIWads);
        }

        public List<string> GetInstalledPWads()
        {
            return getInstalledWads(game => game.InstalledPWads);
        }

        private delegate List<string> WadSelector(SteamInstalledGame game);

        private List<string> getInstalledWads(WadSelector selector)
        {
            // The same wad could appear in multiple Steam games, but
            // we just want the first one.
            //
            // So this maps the bare filename to the full path.
            var filenamesToFullPaths = new Dictionary<string, string>();

            foreach (SteamLibrary library in m_libraries)
            {
                foreach (SteamInstalledGame game in library.InstalledGames)
                {
                    foreach (string wadFile in selector(game))
                    {
                        var justTheFile = Path.GetFileName(wadFile);
                        if (!filenamesToFullPaths.ContainsKey(justTheFile))
                        {
                            filenamesToFullPaths[justTheFile] = wadFile;
                        }
                    }
                }
            }

            return filenamesToFullPaths.Values.ToList();
        }
    }
}
