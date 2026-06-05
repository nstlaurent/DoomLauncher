using DoomLauncher.Interfaces;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DoomLauncher.Handlers
{
    public static class LegacyTitlePicMigration
    {
        public static bool DeleteScreenshotThatIsReallyATitlePic(IFileHandler fileHandler, IGameFile gameFile, Image image)
        {
            var screenshots = fileHandler.GetFiles(gameFile, FileType.Screenshot);

            if (!screenshots.Any())
                return false;

            image = LegacyScaleImage(image);

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
                    FileInfo fi = new FileInfo(screenshot.FullFileName);
                    if (fi.Length == fileSize)
                    {
                        fileHandler.DeleteFile(screenshot);
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        // This used to be a standard part of title pic generation. We still need it here 
        // to emulate the old titlepic screenshot images so we can find them by size.
        public static Image LegacyScaleImage(Image image)
        {
            // Check for Doom's aspect ratio and force to 1.33 like the original so the image doesn't look distored.
            if (!(image.Width / (float)image.Height).ApproxEquals(1.6f))
                return image;

            int multiplier = image.Width / 320;
            return image.Resize(640 * multiplier, 480 * multiplier, InterpolationMode.NearestNeighbor);
        }
    }
}
