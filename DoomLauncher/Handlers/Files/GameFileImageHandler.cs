using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Handlers.Files
{
    public class GameFileImageHandler
    {
        private readonly ThumbnailManager m_thumbnailManager;
        private readonly ScreenshotHandler m_screenshotHandler;

        public GameFileImageHandler(ThumbnailManager thumbnailHandler, ScreenshotHandler screenshotHandler)
        {
            m_thumbnailManager = thumbnailHandler;
            m_screenshotHandler = screenshotHandler;
        }

        public string GetMainImageLarge(IGameFile gameFile)
        {
            return null;
        }

        public string GetMainImageSmall(IGameFile gameFile)
        {
            return null;
        }

        public List<string> GetMainImageAndScreenshots(IGameFile gameFile)
        {
            return null;
        }
    }
}
