using System.Drawing;

namespace DoomLauncher
{
    public interface IThemeColors
    {
        Color Window { get; }
        Color WindowLight { get; }
        Color WindowDark { get; }
        Color Text { get; }
        Color DisabledText { get; }
        Color HighlightText { get; }
        Color Highlight { get; }
        Color ButtonBackground { get; }
        Color CheckBoxBackground { get; }
        Color TextBoxBackground { get; }
        Color Border { get; }
        Color LinkText { get; }
        Color GridRow { get; }
        Color GridRowAlt { get; }
        Color Separator { get; }
        bool GridRowBorder { get; }
    }
}
