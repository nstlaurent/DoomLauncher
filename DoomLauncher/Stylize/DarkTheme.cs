using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class DarkTheme : IThemeColors
    {
        public Color TitlebarBackground => WindowDark;
        public Color WindowDark => Color.FromArgb(255, 32, 32, 32);
        public Color Window => Color.FromArgb(255, 48, 48, 54);
        public Color WindowLight => Color.FromArgb(255, 58, 58, 64);
        public Color Text => Color.White;
        public Color DisabledText => Color.FromArgb(255, 102, 102, 102);
        public Color HighlightText => Color.White;
        public Color Highlight => Color.FromArgb(255, 90, 101, 234);
        public FlatStyle ComboFlatStyle => FlatStyle.Flat;
        public FlatStyle ButtonFlatStyle => FlatStyle.Flat;
        public Color ButtonTextColor => Text;
        public Color ButtonBackground => Color.FromArgb(255, 90, 101, 234);
        public Color CheckBoxBackground => WindowLight;
        public Color TextBoxBackground => WindowDark;
        public Color DropDownBackground => WindowDark;
        public Color Border => Color.FromArgb(255, 32, 32, 32);
        public Color LinkText => Highlight;
        public Color GridBorder => WindowDark;
        public Color GridRow => WindowDark;
        public Color GridRowAlt => WindowDark;
        public Color Separator => Color.Gray;
        public Color StatBackgroundGradientFrom => WindowDark;
        public Color StatBackgroundGradientTo => Window;
        public Color StatText => Color.White;
        public Color CloseBackgroundHighlight => Color.FromArgb(255, 213, 50, 38);
        public Color CloseForeColorHighlight => Color.White;
        public Color ImageBackground => Color.Black;
        public Color GridBackground => WindowDark;
        public bool GridRowBorder => false;
        public bool IsDark => true;
    }
}
