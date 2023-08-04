using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CTabControl : TabControl
    {
        private const int TabMargin = 3;

        private readonly List<int> m_tabRows = new List<int>();
        private readonly List<int> m_renderOrder = new List<int>();

        public CTabControl()
        {
            if (!DesignMode)
                Multiline = true;

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Fixed;
            SetStyle(ControlStyles.UserPaint, true);
            DrawItem += CTabControl_DrawItem;
        }

        private void CTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            int index = e.Index;
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
            Pen borderPen = new Pen(ColorTheme.Current.Border);

            e.Graphics.FillRectangle(controlBrush, tabRect);
            RectangleF layoutRect = new RectangleF(tabRect.Left, tabRect.Y + TabMargin,
                tabRect.Width, tabRect.Height - 2 * TabMargin);

            using (StringFormat format = new StringFormat())
            {
                using (Font tabFont = new Font(Font, selected ? FontStyle.Bold : FontStyle.Regular))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(TabPages[index].Text, tabFont, textBrush, layoutRect, format);
                }
            }

            if (selected)
            {
                Rectangle selectRect = tabRect;
                selectRect.Height = 4;
                using (Brush selectBrush = new SolidBrush(ColorTheme.Current.Highlight))
                    e.Graphics.FillRectangle(selectBrush, selectRect);
            }

            e.Graphics.DrawRectangle(borderPen, tabRect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle fillRect = new Rectangle(0, 0, Width, Height);
            using (Brush controlBrush = new SolidBrush(ColorTheme.Current.Window))
                e.Graphics.FillRectangle(controlBrush, fillRect);

            var renderOrder = GetTabPageRenderOrder();
            if (m_tabRows.Count == 0)
                return;

            int selectedRow = SelectedIndex == -1 ? 0 : m_tabRows[SelectedIndex];
            foreach (int tabIndex in renderOrder)
                DrawTab(tabIndex, e, m_tabRows[tabIndex] == selectedRow);

            base.OnPaint(e);
        }

        // TabPages order is always the same. When rows are stacked Windows changes the ordering of the rows.
        // The row containing the selected item needs to be rendered last.
        private IEnumerable<int> GetTabPageRenderOrder()
        {
            m_tabRows.Clear();
            m_renderOrder.Clear();
            int rowWidth = 0;
            int row = 0;

            for (int i = 0; i < TabPages.Count; i++)
            {
                int tabWidth = GetTabRect(i).Width;
                if (rowWidth + tabWidth > Width)
                {
                    rowWidth = 0;
                    row++;
                }

                m_tabRows.Add(row);
                rowWidth += tabWidth;
            }

            if (m_tabRows.Count == 0)
                return Array.Empty<int>();

            int selectedRow = SelectedIndex == - 1 ? 0 : m_tabRows[SelectedIndex];
            for (int i = 0; i < TabPages.Count; i++)
            {
                if (m_tabRows[i] != selectedRow)
                    m_renderOrder.Add(i);
            }

            for (int i = 0; i < TabPages.Count; i++)
            {
                if (m_tabRows[i] == selectedRow)
                    m_renderOrder.Add(i);
            }

            return m_renderOrder;
        }

        private void DrawTab(int index, PaintEventArgs e, bool selectedRow)
        {
            Rectangle tabRect = GetTabRect(index);
            if (tabRect.Width == 0)
                return;

            tabRect.X += 2;
            if (selectedRow)
                tabRect.Height += 8;
            tabRect.Width -= 4;

            bool selected = SelectedIndex == index;
            if (selected)
                tabRect.Y -= 2;

            Brush controlBrush = new SolidBrush(ColorTheme.Current.WindowLight);
            Brush textBrush = new SolidBrush(ColorTheme.Current.Text);
            Pen borderPen = new Pen(ColorTheme.Current.Border);

            e.Graphics.FillRectangle(controlBrush, tabRect);
            RectangleF layoutRect = new RectangleF(tabRect.Left, tabRect.Y + TabMargin,
                tabRect.Width, tabRect.Height - 2 * TabMargin);

            using (StringFormat format = new StringFormat())
            {
                using (Font tabFont = new Font(Font, selected ? FontStyle.Bold : FontStyle.Regular))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(TabPages[index].Text, tabFont, textBrush, layoutRect, format);
                }
            }

            if (selected)
            {
                Rectangle selectRect = tabRect;
                selectRect.Height = 4;
                using (Brush selectBrush = new SolidBrush(ColorTheme.Current.Highlight))
                    e.Graphics.FillRectangle(selectBrush, selectRect);
            }

            e.Graphics.DrawLine(borderPen, new Point(tabRect.Left, tabRect.Top), new Point(tabRect.Right, tabRect.Top));
            e.Graphics.DrawLine(borderPen, new Point(tabRect.Left, tabRect.Top), new Point(tabRect.Left, tabRect.Bottom));
            e.Graphics.DrawLine(borderPen, new Point(tabRect.Right, tabRect.Top), new Point(tabRect.Right, tabRect.Bottom));
        }
    }
}
