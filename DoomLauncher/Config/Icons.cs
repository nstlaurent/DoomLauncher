using System.Drawing;

namespace DoomLauncher
{
    public static class Icons
    {   
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
            Bitmap copy = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = copy.GetPixel(x, y);
                    if (pixel.A != 0)
                        copy.SetPixel(x, y, Color.FromArgb(pixel.A, color));
                }
            }

            return copy;
        }
    }
}
