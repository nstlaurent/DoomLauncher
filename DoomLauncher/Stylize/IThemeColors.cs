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
        Color DropDownBackground { get; }
        Color Border { get; }
        Color LinkText { get; }
        Color GridBorder { get; }
        Color GridRow { get; }
        Color GridRowAlt { get; }
        Color Separator { get; }
        Color StatBackgroundGradientFrom { get; }
        Color StatBackgroundGradientTo { get; }
        Color StatText { get; }
        Color CloseBackgroundHighlight { get; }
        Color CloseForeColorHighlight { get; }
        bool GridRowBorder { get; }
        bool IsDark { get; }
    }
}
