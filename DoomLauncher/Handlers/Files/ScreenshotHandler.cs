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
        public IEnumerable<IFileData> HandleNewScreenshots(ISourcePortData sourcePort, IGameFile gameFile, string[] screenshotFiles)
        {
            List<IFileData> ret = new List<IFileData>();
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return ret;

            foreach (string screenshotFile in screenshotFiles)
            {
                IFileData fileData;
                if (m_deleteScreenshotsAfterImport)
                {
                    fileData = m_fileHandler.InsertAndMove(gameFile, FileType.Screenshot, screenshotFile, file => 
                    {
                        file.SourcePortID = sourcePort.SourcePortID; 
                    });
                }
                else
                {
                    fileData = m_fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, screenshotFile, file => 
                    {
                        file.SourcePortID = sourcePort.SourcePortID; 
                    });
                }
                ret.Add(fileData);
            }

            return ret;
        }
    }
}
