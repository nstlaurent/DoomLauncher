using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;

namespace DoomLauncher.Steam
{
    public class SteamInstallation
    {
        private readonly string m_installPath;
        private readonly List<SteamLibrary> m_libraries;

        public SteamInstallation(string installPath, List<SteamLibrary> libraries)
        {
            m_installPath = installPath;
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

        private List<string> getInstalledWads(System.Func<SteamInstalledGame, List<string>> selector)
        {
            // The same wad could appear in multiple Steam games, but
            // we just want the first one.
            //
            // So this maps the bare filename to the full path.
            var files = new Dictionary<string, string>();

            foreach (SteamLibrary library in m_libraries)
            {
                foreach (SteamInstalledGame game in library.InstalledGames)
                {
                    foreach (string wadFile in selector(game))
                    {
                        var justTheFile = Path.GetFileName(wadFile);
                        if (!files.ContainsKey(justTheFile))
                        {
                            files[justTheFile] = wadFile;
                        }
                    }
                }
            }

            return files.Values.ToList();
        }
    }
}
