using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DoomLauncher
{
    public static class Icons
    {
        public static DpiScale DpiScale { get; set; } = new DpiScale(2, 2);
        public static Image ArrowUp => GetIcon(Properties.Resources.UpArrow);
        public static Image ArrowDown => GetIcon(Properties.Resources.DownArrow);
        public static Image Bars => GetIcon(Properties.Resources.Bars);
        public static Image Delete => GetIcon(Properties.Resources.Delete);
        public static Image Download => GetIcon(Properties.Resources.th);
        public static Image Edit => GetIcon(Properties.Resources.Edit);
        public static Image Export => GetIcon(Properties.Resources.Export);
        public static Image ExportAll => GetIcon(Properties.Resources.ExportAll);
        public static Image File => GetIcon(Properties.Resources.File);
        public static Image FolderOpen => GetIcon(Properties.Resources.FolderOpen);
        public static Image Play => GetIcon(Properties.Resources.Play);
        public static Image Save => GetIcon(Properties.Resources.Save);
        public static Image Search => GetIcon(Properties.Resources.Search);
        public static Image StepBack => GetIcon(Properties.Resources.StepBack);
        public static Image Video => GetIcon(Properties.Resources.Video);
        public static Image Tags => GetIcon(Properties.Resources.Tags);
        public static Image Pin => GetIcon(Properties.Resources.Pin);

        private static Image GetIcon(Bitmap bitmap)
        {
            return ColorizeIcon(bitmap, ColorTheme.Current.Text);
        }

        private static Bitmap ColorizeIcon(Bitmap bitmap, Color color)
        {
            Bitmap copy;
            if (DpiScale.DpiScaleX > 1 || DpiScale.DpiScaleY > 1)
                copy = ResizeBitmap(bitmap, (int)DpiScale.ScaleFloatX(bitmap.Width * 0.8f), (int)DpiScale.ScaleFloatY(bitmap.Height * 0.8f));
            else
                copy = new Bitmap(bitmap, bitmap.Width, bitmap.Height);

            for (int x = 0; x < copy.Width; x++)
            {
                for (int y = 0; y < copy.Height; y++)
                {
                    var pixel = copy.GetPixel(x, y);
                    if (pixel.A != 0)
                        copy.SetPixel(x, y, Color.FromArgb(pixel.A, color));
                }
            }

            return copy;
        }

        public static Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
