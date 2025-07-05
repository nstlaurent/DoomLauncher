using DoomLauncher.Config;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class ScreenshotHandler
    {
        private readonly IFileHandler m_fileHandler;
        private readonly bool m_deleteScreenshotsAfterImport;

        public ScreenshotHandler(IFileHandler fileHandler, bool deleteScreenshotsAfterImport)
        {
            m_fileHandler = fileHandler;
            m_deleteScreenshotsAfterImport = deleteScreenshotsAfterImport;
        }

        // Invoked when screenshots taken in-game are found in the source port
        public IEnumerable<IFileData> HandleNewScreenshots(ISourcePortData sourcePort, IGameFile gameFile, string[] files)
        {
            List<IFileData> ret = new List<IFileData>();
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return ret;

            foreach (string file in files)
            {
                IFileData fileData;
                if (m_deleteScreenshotsAfterImport)
                {
                    fileData = m_fileHandler.InsertAndMove(gameFile, FileType.Screenshot, file, sourcePort);
                }
                else
                {
                    fileData = m_fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, file, sourcePort);
                }
                ret.Add(fileData);
            }

            return ret;
        }

        public bool FindScreenshotThatIsReallyATitlePic(IGameFile gameFile, Image image, out IFileData titlePicScreenshot)
        {
            titlePicScreenshot = null;

            var screenshots = m_fileHandler.GetFiles(gameFile, FileType.Screenshot);

            if (!screenshots.Any())
                return false;

            // Create png image in memory and use the size to compare to existing screenshot file sizes.
            // This method should be accurate enough to determine if an existing screenshot is the titlepic.
            // This method only works with titlepics pull by Doom Laucher, existing user generated screenshots from source ports will not match.
            long fileSize = 0;
            try
            {
                using (var imageStream = new MemoryStream())
                {
                    image.Save(imageStream, ImageFormat.Png);
                    fileSize = imageStream.Length;
                }
            }
            catch { }

            foreach (IFileData screenshot in screenshots)
            {
                try
                {
                    FileInfo fi = m_fileHandler.GetFileInfo(FileType.Screenshot, screenshot.FileName);
                    if (fi.Length == fileSize)
                    {
                        titlePicScreenshot = screenshot;
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }
    }
}
