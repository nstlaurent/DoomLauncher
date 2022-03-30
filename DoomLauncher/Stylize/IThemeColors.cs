using System.Drawing;

namespace DoomLauncher
{
    public interface IThemeColors
    {
        Color Control { get; }
        Color ControlLight { get; }
        Color ControlLightLight { get; }
        Color ControlDark { get; }
        Color ControlText { get; }
        Color Highlight { get; }
        Color HighlightText { get; }
        Color LinkText { get; }
        Color Window { get; }
        Color WindowText { get; }
        Color ActiveBorder { get; }
        Color ActiveCaptionText { get; }
        Color ButtonBackground { get; }
        Color CheckBoxBackground { get; }
    }
}
