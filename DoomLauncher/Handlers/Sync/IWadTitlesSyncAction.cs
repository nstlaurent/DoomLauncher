using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Handlers.Sync
{
    public class IWadTitlesSyncAction : ISyncAction
    {
        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            IWadInfo info = IWadInfo.GetIWadInfo(gameFile.FileName);
            if (info != null)
                gameFile.Title = info.Title;

            return SyncResult.EMPTY;
        }
    }
}
