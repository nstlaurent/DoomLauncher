using System.Drawing;

namespace DoomLauncher
{
    public class DarkTheme : IThemeColors
    {
        public Color WindowDark => Color.FromArgb(255, 32, 32, 32);
        public Color Window => Color.FromArgb(255, 48, 48, 54);
        public Color WindowLight => Color.FromArgb(255, 58, 58, 64);
        public Color Text => Color.White;
        public Color DisabledText => Color.FromArgb(255, 142, 142, 142);
        public Color HighlightText => Color.FromArgb(255, 90, 101, 234);
        public Color Highlight => Color.FromArgb(255, 90, 101, 234);
        public Color ButtonBackground => Color.FromArgb(255, 90, 101, 234);
        public Color CheckBoxBackground => Color.FromArgb(255, 48, 48, 54);
        public Color TextBoxBackground => WindowDark;
        public Color Border => Color.White;
        public Color LinkText => Highlight;
    }
}
