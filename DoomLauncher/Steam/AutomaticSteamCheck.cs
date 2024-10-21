using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoomLauncher.Steam
{
    public class AutomaticSteamCheck
    {
        private IDataSourceAdapter m_dataSourceAdapter;
        private Func<SteamInstallation> m_loadSteam;
        private List<string> m_installedGameFiles;

        public AutomaticSteamCheck(Func<SteamInstallation> loadSteam, IDataSourceAdapter dataSourceAdapter)
        {
            m_dataSourceAdapter = dataSourceAdapter;
            m_loadSteam = loadSteam;
        }

        public delegate Task AsyncWadInstaller(List<string> wads);

        // We have to use Func<T, Task> instead of Action<T>, because we want to make sure the first
        // action fully completes before we run the second action.
        public async Task LoadGamesFromSteam(AsyncWadInstaller addIwadToGame, AsyncWadInstaller addPwadToGame)
        {
            try
            {
                var steam = m_loadSteam();

                m_installedGameFiles = m_dataSourceAdapter.GetGameFileNames().Select(Path.GetFileName).ToList();

                var iwadsFromSteam = steam.GetInstalledIWads().Where(NotInCollection).ToList();
                if (iwadsFromSteam.Count > 0)
                {
                    await addIwadToGame(iwadsFromSteam);
                }

                var pwadsFromSteam = steam.GetInstalledPWads().Where(NotInCollection).ToList();
                if (pwadsFromSteam.Count > 0)
                {
                    await addPwadToGame(pwadsFromSteam);
                }
            }
            catch (SteamLoaderException e)
            {
                // No big deal if we can't load Steam
                Console.WriteLine(e.Message);
            }
        }

        private bool NotInCollection(string file)
        {
            var justTheFileName = Path.GetFileName(file);
            var justTheFileNameZip = Path.GetFileNameWithoutExtension(file) + ".zip";

            return !m_installedGameFiles.Exists(x => x.Equals(justTheFileName) || x.Equals(justTheFileNameZip));
        }
    }
}
