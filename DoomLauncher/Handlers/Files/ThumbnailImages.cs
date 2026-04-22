using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Documents;
using System.Windows.Navigation;

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
            int blurredSourceWidth = sourceWidth;
            int blurredSourceHeight = sourceHeight;
            int blurredSourceX = sourceX;
            int blurredSourceY = sourceY;

            float aspectRatio = width / (float)height;

            float scalingPercent;
            float scalingPercentW = width / (float)sourceWidth;
            float scalingPercentH = height / (float)sourceHeight;

            if (scalingPercentH < scalingPercentW) // Too tall, pillarbox
            {
                scalingPercent = scalingPercentH;
                destX = Convert.ToInt16((width - (sourceWidth * scalingPercent)) / 2);
                blurredSourceHeight = Convert.ToInt16(sourceHeight / aspectRatio);
                blurredSourceY = sourceHeight / 2 - blurredSourceHeight / 2;
            }
            else // Too wide, letterbox
            {
                scalingPercent = scalingPercentW;
                destY = Convert.ToInt16((height - (sourceHeight * scalingPercent)) / 2);
                blurredSourceWidth = Convert.ToInt16(sourceWidth / aspectRatio);
                blurredSourceX = sourceWidth / 2 - blurredSourceWidth / 2;

            }

            int destWidth = (int)(sourceWidth * scalingPercent);
            int destHeight = (int)(sourceHeight * scalingPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Doom64 images seem to be sometimes transparent, and not 
            // designed to look good in 4:3 mode.
            if (gameFile.IsDoom64)
            {
                grPhoto.Clear(Color.Black);
                grPhoto.DrawImage(imgPhoto,
                    destRect: new Rectangle(destX, destY, destWidth, destHeight),
                    srcRect: new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            }
            else
            {
                // The blurred one
                grPhoto.DrawImage(imgPhoto,
                    destRect: new Rectangle(0, 0, width, height),
                    srcRect: new Rectangle(blurredSourceX, blurredSourceY, blurredSourceWidth, blurredSourceHeight),
                    GraphicsUnit.Pixel);

                // If it is a wide image, don't blur the enlarged space-filling image;
                // we'll take that one as-is, and ignore the scaled-to-fit image. Wide
                // titlepic images tend to be designed to fit in 4:3.
                bool isWide = scalingPercentW < scalingPercentH;
                if (!isWide)
                {
                    bmPhoto = Blur(bmPhoto, new Rectangle(0, 0, width, height), 8);

                    grPhoto = Graphics.FromImage(bmPhoto);

                    // The real one
                    grPhoto.DrawImage(imgPhoto,
                        destRect: new Rectangle(destX, destY, destWidth, destHeight),
                        srcRect: new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);
                }
            }

            grPhoto.Dispose();
            return bmPhoto;
        }

        private static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }
    }
}
