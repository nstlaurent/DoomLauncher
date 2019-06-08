namespace DoomLauncher
{
    public enum ScreenFilterType
    {
        Ellipse,
        Scanline
    }

    public class ScreenFilter
    {
        public ScreenFilterType Type { get; set; }
        public float Opacity { get; set; }
        public int LineThickness { get; set; }
        public int BlockSize { get; set; }
        public float SpacingX { get; set; }
        public float SpacingY { get; set; }
        public bool Stagger { get; set; }
        public int ScanlineSpacing { get; set; }
        public bool VerticalScanlines { get; set; }
        public bool HorizontalScanlines { get; set; }
        public bool Enabled { get; set; }
    }
}
