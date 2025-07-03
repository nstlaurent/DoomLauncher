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
        // Invoked when screenshots taken in-game are found in the source port
        public static IEnumerable<IFileData> HandleNewScreenshots(ISourcePortData sourcePort, IGameFile gameFile, string[] files)
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
                    fi.CopyTo(Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), fileName));

                    FileData fileData = new FileData
                    {
                        FileName = fileName,
                        GameFileID = gameFile.GameFileID.Value,
                        SourcePortID = sourcePort.SourcePortID,
                        FileTypeID = FileType.Screenshot,
                        FileOrder = short.MaxValue,
                    };

                    DataCache.Instance.DataSourceAdapter.InsertFile(fileData);
                    ret.Add(fileData);

                    if (DataCache.Instance.AppConfiguration.DeleteScreenshotsAfterImport)
                        File.Delete(file);
                }
                catch
                {
                    //failed, nothing to do
                }
            }

            return ret;
        }

        public static bool FindScreenshotThatIsReallyATitlePic(IGameFile gameFile, Image image, out IFileData titlePicScreenshot)
        {
            titlePicScreenshot = null;

            var screenshots = DataCache.Instance.DataSourceAdapter.GetFiles(gameFile, FileType.Screenshot);

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
                    FileInfo fi = new FileInfo(Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), screenshot.FileName));
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
