using System.Drawing;

namespace DoomLauncher
{
    public class DefaultTheme : IThemeColors
    {
        public Color WindowDark => SystemColors.Control;
        public Color Window => SystemColors.Control;
        public Color WindowLight => SystemColors.Window;
        public Color Text => SystemColors.WindowText;
        public Color DisabledText => Color.FromArgb(255, 142, 142, 142);
        public Color HighlightText => SystemColors.HighlightText;
        public Color Highlight => SystemColors.Highlight;
        public Color ButtonBackground => SystemColors.Window;
        public Color CheckBoxBackground => SystemColors.Window;
        public Color TextBoxBackground => Color.White;
        public Color DropDownBackground => SystemColors.Window;
        public Color Border => SystemColors.Control;
        public Color LinkText => Highlight;
        public Color GridBorder => SystemColors.ControlDark;
        public Color GridRow => SystemColors.Window;
        public Color GridRowAlt => SystemColors.ControlLight;
        public Color Separator => Color.Gray;
        public Color StatBackgroundGradientFrom => Color.Gray;
        public Color StatBackgroundGradientTo => Color.FromArgb(255, 191, 191, 191);
        public Color StatText => Color.White;
        public Color CloseBackgroundHighlight => Color.FromArgb(255, 213, 50, 38);
        public Color CloseForeColorHighlight => Color.White;
        public bool GridRowBorder => true;
        public bool IsDark => true;
    }
}
