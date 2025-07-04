using DoomLauncher.Config;
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
        private readonly IDataSourceAdapter m_database;
        private readonly bool m_deleteScreenshotsAfterImport;
        private readonly LauncherPath m_screenshotDirectory;

        public ScreenshotHandler(IDataSourceAdapter database, AppConfiguration config)
        {
            this.m_database = database;
            this.m_deleteScreenshotsAfterImport = config.DeleteScreenshotsAfterImport;
            this.m_screenshotDirectory = config.ScreenshotDirectory;
        }

        // Invoked when screenshots taken in-game are found in the source port
        public IEnumerable<IFileData> HandleNewScreenshots(ISourcePortData sourcePort, IGameFile gameFile, string[] files)
        {
            List<IFileData> ret = new List<IFileData>();
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return ret;

            foreach (string file in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    string fileName = Guid.NewGuid().ToString() + fi.Extension;
                    fi.CopyTo(m_screenshotDirectory.GetFullPath(fileName));

                    FileData fileData = new FileData
                    {
                        FileName = fileName,
                        GameFileID = gameFile.GameFileID.Value,
                        SourcePortID = sourcePort.SourcePortID,
                        FileTypeID = FileType.Screenshot,
                        FileOrder = short.MaxValue,
                    };

                    m_database.InsertFile(fileData);
                    ret.Add(fileData);

                    if (m_deleteScreenshotsAfterImport)
                        File.Delete(file);
                }
                catch
                {
                    //failed, nothing to do
                }
            }

            return ret;
        }

        public bool FindScreenshotThatIsReallyATitlePic(IGameFile gameFile, Image image, out IFileData titlePicScreenshot)
        {
            titlePicScreenshot = null;

            var screenshots = m_database.GetFiles(gameFile, FileType.Screenshot);

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
                    FileInfo fi = new FileInfo(m_screenshotDirectory.GetFullPath(screenshot.FileName));
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
