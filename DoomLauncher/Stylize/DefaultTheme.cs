using System.Drawing;

namespace DoomLauncher
{
    public class DefaultTheme : IThemeColors
    {
        public Color Control => SystemColors.Control;

        public Color ControlLight => SystemColors.ControlLight;

        public Color ControlLightLight => SystemColors.ControlLightLight;

        public Color ControlDark => SystemColors.ControlDark;

        public Color ControlText => SystemColors.ControlText;

        public Color Highlight => SystemColors.Highlight;

        public Color HighlightText => SystemColors.HighlightText;

        public Color LinkText => Color.Blue;

        public Color Window => SystemColors.Window;

        public Color WindowText => SystemColors.WindowText;

        public Color ActiveBorder => SystemColors.ActiveBorder;

        public Color ActiveCaptionText => SystemColors.ActiveCaptionText;

        public Color ButtonBackground => SystemColors.Control;

        public Color CheckBoxBackground => SystemColors.ControlLightLight;
    }
}
