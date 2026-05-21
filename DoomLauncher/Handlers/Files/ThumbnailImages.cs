using DoomLauncher.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DoomLauncher.Handlers.Files
{
    public static class ThumbnailImages
    {
        // Adapted from ImageExtensions.FixedSize. Needs to be separate, because we are slowing down this method
        // with fancier image processing, with the intention of it being called one time and subsequently cached on disk. 
        // However, FixedSize gets cosntantly called all over the place at runtime for all kinds of workflows, and
        // making it take longer has a devastating impact on application performance.
        public static Image CreateStandardizedThumbnail(this Image imgPhoto, int width, int height, IGameFile gameFile)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            int spaceFillingSourceWidth = sourceWidth;
            int spaceFillingSourceHeight = sourceHeight;
            int spaceFillingSourceX = sourceX;
            int spaceFillingSourceY = sourceY;

            float aspectRatio = width / (float)height;

            float scalingPercent;
            float scalingPercentW = width / (float)sourceWidth;
            float scalingPercentH = height / (float)sourceHeight;

            if (scalingPercentH < scalingPercentW) // Too tall, pillarbox
            {
                scalingPercent = scalingPercentH;
                destX = Convert.ToInt16((width - (sourceWidth * scalingPercent)) / 2);
                spaceFillingSourceHeight = Convert.ToInt16(sourceHeight / aspectRatio);
                spaceFillingSourceY = sourceHeight / 2 - spaceFillingSourceHeight / 2;
            }
            else // Too wide, letterbox
            {
                scalingPercent = scalingPercentW;
                destY = Convert.ToInt16((height - (sourceHeight * scalingPercent)) / 2);
                spaceFillingSourceWidth = Convert.ToInt16(sourceWidth / aspectRatio);
                spaceFillingSourceX = sourceWidth / 2 - spaceFillingSourceWidth / 2;

            }

            int destWidth = (int)(sourceWidth * scalingPercent);
            int destHeight = (int)(sourceHeight * scalingPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.Clear(Color.Black);

            // Already the right ratio OR Doom64, just draw it.
            // (Doom64 images tend to look better like this)
            if (gameFile.IsDoom64 || scalingPercentW == scalingPercentH)
            {
                
                grPhoto.DrawImage(imgPhoto,
                destRect: new Rectangle(destX, destY, destWidth, destHeight),
                srcRect: new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);
            }
            // It's the wrong ratio, fill the space and truncate
            else
            {
                grPhoto.DrawImage(imgPhoto,
                    destRect: new Rectangle(0, 0, width, height),
                    srcRect: new Rectangle(spaceFillingSourceX, spaceFillingSourceY, spaceFillingSourceWidth, spaceFillingSourceHeight),
                    GraphicsUnit.Pixel);
            }

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
