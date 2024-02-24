using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DoomLauncher
{
    internal static class ImageExtensions
    {
        public static bool TryFromFile(string path, out Image image)
        {
            image = null;
            try
            {
                if (!File.Exists(path))
                    return false;

                image = Image.FromFile(path);
                return true;
            }
            catch
            {
            }

            return false;
        }

        public static Image FromFileOrDefault(string path)
        {
            if (TryFromFile(path, out var image))
                return image;

            return new Bitmap(1, 1, PixelFormat.Format32bppRgb);
        }

        public static Image FixedSize(this Image imgPhoto, int width, int height, Color backColor)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent;
            float nPercentW = width / (float)sourceWidth;
            float nPercentH = height / (float)sourceHeight;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(backColor);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image Rotate(this Image image, float angle)
        {
            Bitmap bm = image as Bitmap;

            Matrix matrixOrigin = new Matrix();
            matrixOrigin.Rotate(angle);

            PointF[] points =
            {
                new PointF(0, 0),
                new PointF(bm.Width, 0),
                new PointF(bm.Width, bm.Height),
                new PointF(0, bm.Height),
            };
            matrixOrigin.TransformPoints(points);
            GetPointBounds(points, out float xMin, out float xMax,
                out float yMin, out float yMax);

            int width = (int)Math.Round(xMax - xMin);
            int height = (int)Math.Round(yMax - yMin);
            Bitmap result = new Bitmap(width, height);

            Matrix matrixCenter = new Matrix();
            matrixCenter.RotateAt(angle, new PointF(width / 2f, height / 2f));

            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.InterpolationMode = InterpolationMode.High;
                gr.Clear(bm.GetPixel(0, 0));
                gr.Transform = matrixCenter;

                int x = (width - bm.Width) / 2;
                int y = (height - bm.Height) / 2;
                gr.DrawImage(bm, x, y);
            }

            return result;
        }

        public static Image Resize(this Image image, int width, int height, InterpolationMode interpolationMode)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.InterpolationMode = interpolationMode;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image ScaleDoomImage(this Image image)
        {
            // Check for Doom's aspect ratio and force to 1.33 like the original so the image doesn't look distored.
            if (image.Width / (float)image.Height != 1.6f)
                return image;

            int multiplier = image.Width / 320;
            return image.Resize(640 * multiplier, 480 * multiplier, InterpolationMode.NearestNeighbor);
        }

        private static void GetPointBounds(PointF[] points,
            out float xmin, out float xmax,
            out float ymin, out float ymax)
        {
            xmin = points[0].X;
            xmax = xmin;
            ymin = points[0].Y;
            ymax = ymin;
            foreach (PointF point in points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }
        }
    }
}
