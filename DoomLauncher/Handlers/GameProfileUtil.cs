using System;
using System.Collections.Generic;
using System.Linq;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;

namespace DoomLauncher.Handlers
{
    public static class GameProfileUtil
    {
        public static IList<IGameProfile> GetAllProfiles(IDataSourceAdapter adapter, GameFile gameFile, out IList<IGameProfile> globalProfiles)
        {
            globalProfiles = adapter.GetGlobalGameProfiles().OrderBy(x => x.Name).ToList();

            if (!gameFile.GameFileID.HasValue)
                return Array.Empty<IGameProfile>();

            List<IGameProfile> profiles = globalProfiles.ToList();
            profiles.Add(gameFile);
            profiles.AddRange(adapter.GetGameProfiles(gameFile.GameFileID.Value).OrderBy(x => x.Name));
            return profiles;
        }

        public static bool IsValidProfileName(IDataSourceAdapter adapter, IGameFile gameFile, IList<IGameProfile> globalProfiles, 
            int gameProfileId, string name, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(name))
            {
                error = "A name must be entered.";
                return false;
            }

            if (NameExists(adapter, gameFile, globalProfiles, gameProfileId, name))
            {
                error = "This profile name already exists.";
                return false;
            }

            return true;
        }

        public static bool NameExists(IDataSourceAdapter adapter, IGameFile gameFile, IList<IGameProfile> globalProfiles,
            int excludeGameProfileId, string name)
        {
            if (gameFile.GameFileID.HasValue)
            {
                var gameProfiles = adapter.GetGameProfiles(gameFile.GameFileID.Value)
                    .Where(x => x.GameProfileID != excludeGameProfileId);
                if (FindProfileName(gameProfiles, name))
                    return true;
            }

            return FindProfileName(globalProfiles, name);
        }

        private static bool FindProfileName(IEnumerable<IGameProfile> profiles, string name) =>
            name.Equals(GameFile.DefaultProfileName, StringComparison.OrdinalIgnoreCase) || 
                   profiles.Any(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }
}
