using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher.Controls
{
    public class ComboBoxItemStyle
    {
        public string ValueMember { get; set; }
        public string Text { get; set; }
        public Font Font { get; set; }
        public Color ForeColor { get; set; }
        public DrawItemEventArgs DrawItem { get; set; }
    }

    public class CComboBox : ComboBox
    {
        private static readonly IThemeColors CurrentTheme = ColorTheme.Current;

        public event EventHandler<ComboBoxItemStyle> StyleItem;

        public CComboBox()
        {
            BackColor = CurrentTheme.TextBoxBackground;
            ForeColor = CurrentTheme.Text;
            DrawItem += ComboBoxDrawItemInternal;
            DrawMode = DrawMode.OwnerDrawFixed;
            FlatStyle = FlatStyle.Flat;
        }

        public static void ComboBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBoxDrawItem(sender, e, null);
        }

        private void ComboBoxDrawItemInternal(object sender, DrawItemEventArgs e)
        {
            ComboBoxDrawItem(sender, e, StyleItem);
        }

        private static void ComboBoxDrawItem(object sender, DrawItemEventArgs e, EventHandler<ComboBoxItemStyle> styleItem = null)
        {
            if (!(sender is ComboBox comboBox))
                return;

            bool selected = e.State.HasFlag(DrawItemState.Selected);
            if (selected)
                e.Graphics.FillRectangle(new SolidBrush(CurrentTheme.Highlight), e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(CurrentTheme.DropDownBackground), e.Bounds);

            if (e.Index < 0 || e.Index >= comboBox.Items.Count)
                return;

            var item = comboBox.Items[e.Index];
            string text;
            if (!string.IsNullOrEmpty(comboBox.DisplayMember))
                text = GetPropertyString(item, comboBox.DisplayMember);
            else if (item is string str)
                text = str;
            else
                text = item.ToString();

            Color color = selected ? CurrentTheme.HighlightText : CurrentTheme.Text;

            if (styleItem != null)
            {
                string value = string.Empty;
                if (!string.IsNullOrEmpty(comboBox.ValueMember))
                    value = GetPropertyString(item, comboBox.ValueMember);

                var style = new ComboBoxItemStyle()
                {
                    ValueMember = value,
                    Text = text,
                    Font = e.Font,
                    ForeColor = color,
                    DrawItem = e
                };

                styleItem.Invoke(sender, style);
                e.Graphics.DrawString(style.Text, style.Font, new SolidBrush(style.ForeColor), new PointF(0, e.Bounds.Y));
                return;
            }

            e.Graphics.DrawString(text, e.Font, new SolidBrush(color), new PointF(0, e.Bounds.Y));
        }

        private static string GetPropertyString(object item, string propertyName)
        {
            var pi = item.GetType().GetProperty(propertyName);
            if (pi != null)
                return pi.GetValue(item).ToString();
            return string.Empty;
        }
    }
}
