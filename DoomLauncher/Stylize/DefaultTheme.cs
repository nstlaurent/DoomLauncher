using System.Drawing;

namespace DoomLauncher
{
    public class DefaultTheme : IThemeColors
    {
        public Color WindowDark => Color.FromArgb(255, 32, 32, 32);
        public Color Window => Color.FromArgb(255, 48, 48, 54);
        public Color WindowLight => Color.FromArgb(255, 58, 58, 64);
        public Color Text => Color.White;
        public Color DisabledText => Color.FromArgb(255, 142, 142, 142);
        public Color HighlightText => Color.White;
        public Color Highlight => Color.FromArgb(255, 90, 101, 234);
        public Color ButtonBackground => Color.FromArgb(255, 90, 101, 234);
        public Color CheckBoxBackground => Color.FromArgb(255, 48, 48, 54);
        public Color TextBoxBackground => Color.White;
        public Color Border => Color.Black;
        public Color LinkText => Highlight;

        //public Color Control => SystemColors.Control;

        //public Color ControlLight => SystemColors.ControlLight;

        //public Color ControlLightLight => SystemColors.ControlLightLight;

        //public Color ControlDark => SystemColors.ControlDark;

        //public Color ControlText => SystemColors.ControlText;

        //public Color Highlight => SystemColors.Highlight;

        //public Color HighlightText => SystemColors.HighlightText;

        //public Color LinkText => Color.Blue;

        //public Color Window => SystemColors.Window;

        //public Color WindowText => SystemColors.WindowText;

        //public Color ActiveBorder => SystemColors.ActiveBorder;

        //public Color ActiveCaptionText => SystemColors.ActiveCaptionText;

        //public Color ButtonBackground => SystemColors.Control;

        //public Color CheckBoxBackground => SystemColors.ControlLightLight;
    }
}
