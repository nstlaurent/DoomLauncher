using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CTabControl : TabControl
    {
        private int Xwid = 8;
        private const int tab_margin = 3;

        public CTabControl()
        {
            if (!DesignMode)
                Multiline = true;

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Fixed;

            DrawItem += tabControl_DrawItem;

            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle fillRect = new Rectangle(0, 0, Width, Height);
            using (Brush controlBrush = new SolidBrush(ColorTheme.Current.Window))
                e.Graphics.FillRectangle(controlBrush, fillRect);

            int index = 0;
            foreach (TabPage tabPage in TabPages)
                DrawTab(tabPage, index++, e);

            base.OnPaint(e);
        }

        private void DrawTab(TabPage tabPage, int index, PaintEventArgs e)
        {
            Rectangle tabRect = GetTabRect(index);
            if (tabRect.Width == 0)
                return;

            tabRect.X += 2;
            tabRect.Height += 8;
            tabRect.Width -= 4;

            bool selected = SelectedIndex == index;
            if (selected)
                tabRect.Y -= 2;

            Brush controlBrush = new SolidBrush(ColorTheme.Current.WindowLight);
            Brush textBrush = new SolidBrush(ColorTheme.Current.Text);
            Pen boxPen = new Pen(ColorTheme.Current.WindowDark, 10);

            e.Graphics.FillRectangle(controlBrush, tabRect);
            RectangleF layout_rect = new RectangleF(tabRect.Left, tabRect.Y + tab_margin,
                tabRect.Width, tabRect.Height - 2 * tab_margin);

            using (StringFormat string_format = new StringFormat())
            {
                using (Font tabFont = new Font(Font, selected ? FontStyle.Bold : FontStyle.Regular))
                {
                    string_format.Alignment = StringAlignment.Center;
                    string_format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(TabPages[index].Text, tabFont, textBrush, layout_rect, string_format);
                }
            }

            //e.Graphics.DrawRectangle(boxPen, tabRect);

            if (SelectedIndex == index)
            {
                Rectangle selectRect = tabRect;
                selectRect.Height = 4;
                using (Brush selectBrush = new SolidBrush(ColorTheme.Current.Highlight))
                    e.Graphics.FillRectangle(selectBrush, selectRect);
            }
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {            
            Brush controlBrush = new SolidBrush(ColorTheme.Current.WindowLight);
            Brush textBrush = new SolidBrush(ColorTheme.Current.Text);
            Pen boxPen = new Pen(ColorTheme.Current.WindowDark, 10);

            Rectangle tabRect = GetTabRect(e.Index);
            //tabRect.Inflate(4,4);

            e.Graphics.FillRectangle(controlBrush, tabRect);
            RectangleF layout_rect = new RectangleF(tabRect.Left + tab_margin, tabRect.Y + tab_margin,
                tabRect.Width - 2 * tab_margin, tabRect.Height - 2 * tab_margin);

            using (StringFormat string_format = new StringFormat())
            {
                using (Font big_font = new Font(this.Font, FontStyle.Bold))
                {
                    string_format.Alignment = StringAlignment.Center;
                    string_format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(TabPages[e.Index].Text, Font, textBrush, layout_rect, string_format);
                }
            }

            //e.Graphics.DrawRectangle(boxPen, tabRect);
            e.Graphics.DrawRectangle(new Pen(Color.Red), tabRect);
            
            if (e.Index == TabPages.Count - 1)
            {
                Rectangle fillRect = new Rectangle(tabRect.Right, tabRect.Top - 5, Width - tabRect.Right, Height);
                e.Graphics.FillRectangle(controlBrush, fillRect);
            }

            Rectangle tabPageRect = TabPages[e.Index].Bounds;
            e.Graphics.DrawRectangle(new Pen(ColorTheme.Current.Window, 10), tabPageRect);
            e.DrawFocusRectangle();
        }
    }
}
