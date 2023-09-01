using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DoomLauncher
{
    public enum IconImage
    {
        UpArrow,
        DownArrow,
        Bars,
        Delete,
        Download,
        Edit,
        Export,
        ExportAll,
        File,
        FolderOpen,
        Play,
        Save,
        Search,
        StepBack,
        Video,
        Tags,
        Pin
    }

    public static class Icons
    {
        private static readonly Dictionary<(IconImage, Color), Bitmap> IconLookup = new Dictionary<(IconImage, Color), Bitmap>();

        public static DpiScale DpiScale { get; set; } = new DpiScale(2, 2);
        public static Image ArrowUp => GetIcon(IconImage.UpArrow);
        public static Image ArrowDown => GetIcon(IconImage.DownArrow);
        public static Image Bars => GetIcon(IconImage.Bars);
        public static Image Delete => GetIcon(IconImage.Delete);
        public static Image Download => GetIcon(IconImage.Download);
        public static Image Edit => GetIcon(IconImage.Edit);
        public static Image Export => GetIcon(IconImage.Export);
        public static Image ExportAll => GetIcon(IconImage.ExportAll);
        public static Image File => GetIcon(IconImage.File);
        public static Image FolderOpen => GetIcon(IconImage.FolderOpen);
        public static Image Play => GetIcon(IconImage.Play);
        public static Image Save => GetIcon(IconImage.Save);
        public static Image Search => GetIcon(IconImage.Search);
        public static Image StepBack => GetIcon(IconImage.StepBack);
        public static Image Video => GetIcon(IconImage.Video);
        public static Image Tags => GetIcon(IconImage.Tags);
        public static Image Pin => GetIcon(IconImage.Pin);

        public static Image GetIcon(IconImage iconImage, Color color) => ColorizeIcon(iconImage, color);

        private static Bitmap GetBitmap(IconImage iconImage)
        {
            switch (iconImage)
            {
                case IconImage.UpArrow:
                    return Properties.Resources.UpArrow;
                case IconImage.DownArrow:
                    return Properties.Resources.DownArrow;
                case IconImage.Bars:
                    return Properties.Resources.Bars;
                case IconImage.Delete:
                    return Properties.Resources.Delete;
                case IconImage.Download:
                    return Properties.Resources.th;
                case IconImage.Edit:
                    return Properties.Resources.Edit;
                case IconImage.Export:
                    return Properties.Resources.Export;
                case IconImage.ExportAll:
                    return Properties.Resources.ExportAll;
                case IconImage.File:
                    return Properties.Resources.File;
                case IconImage.FolderOpen:
                    return Properties.Resources.FolderOpen;
                case IconImage.Play:
                    return Properties.Resources.Play;
                case IconImage.Save:
                    return Properties.Resources.Save;
                case IconImage.Search:
                    return Properties.Resources.Search;
                case IconImage.StepBack:
                    return Properties.Resources.StepBack;
                case IconImage.Video:
                    return Properties.Resources.Video;
                case IconImage.Tags:
                    return Properties.Resources.Tags;
                case IconImage.Pin:
                    return Properties.Resources.Pin;
                default:
                    return new Bitmap(0, 0);
            }
        }

        private static Image GetIcon(IconImage iconImage) =>
            ColorizeIcon(iconImage, ColorTheme.Current.Text);
        
        private static Bitmap ColorizeIcon(IconImage iconImage, Color color)
        {
            if (IconLookup.TryGetValue((iconImage, color), out var bitmap))
                return bitmap;

            bitmap = GetBitmap(iconImage);
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

            IconLookup[(iconImage, color)] = copy;
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
