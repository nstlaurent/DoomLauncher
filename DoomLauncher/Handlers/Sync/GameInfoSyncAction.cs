using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoomLauncher.Handlers.Sync
{
    /// <summary>
    /// https://zdoom.org/wiki/GAMEINFO
    /// </summary>
    public class GameInfoSyncAction : ISyncAction
    {
        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            // Normally it's GAMEINFO, but I've seen GAMEINFO.txt in the wild. 
            var entry = reader.Entries.FirstOrDefault(x => x.Name.ToLower().StartsWith("gameinfo"));
            if (entry != null)
            {
                var text = entry.ReadString(Encoding.UTF7);
                var mapping = ParseGameInfo(text);

                if (mapping.TryGetValue("STARTUPTITLE", out var title) && !string.IsNullOrWhiteSpace(title))
                {
                    gameFile.Title = title;
                }

                if (mapping.TryGetValue("IWAD", out var iwad) && !string.IsNullOrEmpty(iwad))
                {
                    IWadInfo iwadInfo = IWadInfo.FromFileName(iwad);
                    if (iwadInfo != null)
                    {
                        gameFile.IntendedGame = iwadInfo;
                    }
                }
            }

            return SyncResult.EMPTY;
        }

        private Dictionary<string, string> ParseGameInfo(string gameInfo)
        {
            string[] lines = gameInfo.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> mapping = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                string[] keyValues = line.Split('=');
                if (keyValues.Length >= 2)
                {
                    string key = keyValues[0].Trim();
                    string value = keyValues.Skip(1).Aggregate("", (a, b) => a + b).Trim();

                    // This won't work for comma-separated values, as expected for LOAD and STARTUPCOLORS,
                    // but we don't care about those for the time being.
                    if (value[0] == '\"' && value[value.Length-1] == '\"')
                        value = value.Substring(1, value.Length - 2);
                    mapping[key] = value;
                }
            }
            return mapping;
        }
    }
}
