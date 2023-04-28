using System.Drawing;

namespace DoomLauncher
{
    public class DefaultTheme : IThemeColors
    {
        public Color WindowDark => SystemColors.Control;
        public Color Window => SystemColors.Window;
        public Color WindowLight => SystemColors.Window;
        public Color Text => SystemColors.WindowText;
        public Color DisabledText => Color.FromArgb(255, 142, 142, 142);
        public Color HighlightText => SystemColors.HighlightText;
        public Color Highlight => SystemColors.Highlight;
        public Color ButtonBackground => SystemColors.Control;
        public Color CheckBoxBackground => SystemColors.Control;
        public Color TextBoxBackground => Color.White;
        public Color Border => SystemColors.Control;
        public Color LinkText => Highlight;
        public Color GridRow => SystemColors.Window;
        public Color GridRowAlt => SystemColors.ControlLight;
        public bool GridRowBorder => true;
    }
}
