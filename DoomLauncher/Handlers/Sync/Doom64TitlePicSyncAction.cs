using DoomLauncher.Interfaces;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DoomLauncher.Handlers.Sync {
    public class Doom64TitlePicSyncAction : ISyncAction
    {
        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            var entry = reader.Entries.FirstOrDefault(e => e.FullName.EndsWith("Doom64_HiRes.png"));

            if (entry != null)
            {
                var image = Image.FromStream(new MemoryStream(entry.ReadEntry()));
                return SyncResult.TitlePic(gameFile, image);
            }

            return SyncResult.EMPTY;
        }
    }
}
