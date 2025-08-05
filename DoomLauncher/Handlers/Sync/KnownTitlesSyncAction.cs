using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Handlers.Sync
{
    public class KnownTitlesSyncAction : ISyncAction
    {
        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            var baseName = Path.GetFileNameWithoutExtension(file.FileName).ToLower();
            
            switch (baseName)
            {
                case "hexdd":
                    file.Title = "Hexen: Deathkings of the Dark Citadel";
                    break;
            };
            return SyncResult.EMPTY;
        }
    }
}
