using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher.Handlers
{
    public class TileImageHandler
    {
        private Dictionary<IWadInfo, IFileData> m_tileImageCache = null;
        private static readonly string DEFAULT_IMAGE_NAME = "DoomLauncherTile";


        public TileImageHandler()
        {
            m_tileImageCache = PopulateDictionary();
        }

        public IFileData GetTileImage(IWadInfo iWadInfo)
        {
            return m_tileImageCache[iWadInfo];
        }

        private Dictionary<IWadInfo, IFileData> PopulateDictionary()
        {
            var cache = new Dictionary<IWadInfo, IFileData>();
            foreach (var iWadInfo in IWadInfo.All)
            {
                cache[iWadInfo] = new FileData()
                {
                    FileTypeID = FileType.TileImage,
                    FileName = $"{iWadInfo.GameName.ToLower()}.png"
                };
            }
            return cache;
        }
    }
}
