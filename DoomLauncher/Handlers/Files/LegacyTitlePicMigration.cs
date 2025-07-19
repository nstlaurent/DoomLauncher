using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Handlers
{
    public static class LegacyTitlePicMigration
    {
        public static bool FindScreenshotThatIsReallyATitlePic(IFileHandler fileHandler, IGameFile gameFile, Image image, out IFileData titlePicScreenshot)
        {
            titlePicScreenshot = null;

            var screenshots = fileHandler.GetFiles(gameFile, FileType.Screenshot);

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
                    FileInfo fi = fileHandler.GetFileInfo(FileType.Screenshot, screenshot.FileName);
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
