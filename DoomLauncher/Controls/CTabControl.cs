using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CTabControl : TabControl
    {
        private const int TabMargin = 3;

        private readonly Dictionary<int, List<int>> m_tabRowLookup = new Dictionary<int, List<int>>();
        private readonly List<int> m_tabRows = new List<int>();
        private bool m_setting;
        private bool m_showHeaders = true;
        private int m_lastTabCount;
        private float m_spaceWidth = -1;
        private Size m_originalItemSize = new Size(0, 0);   

        public CTabControl()
        {
            if (!DesignMode)
                Multiline = true;

            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Normal;
            SetStyle(ControlStyles.UserPaint, true);
        }

        public void SetShowHeaders(bool set)
        {
            m_showHeaders = set;
            if (set)
            {
                Appearance = TabAppearance.Normal;
                SizeMode = TabSizeMode.Normal;
                ItemSize = m_originalItemSize;
            }
            else
            {
                Appearance = TabAppearance.FlatButtons;
                SizeMode = TabSizeMode.Fixed;
                ItemSize = new Size(0, 1);
            }
        }

        protected override void OnCreateControl()
        {
            int width = 0;
            if (SizeMode == TabSizeMode.Fixed)
                width = ItemSize.Width;

            var dpiScale = new DpiScale(CreateGraphics());
            ItemSize = new Size(width, dpiScale.ScaleIntY(18));
            m_originalItemSize = ItemSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_setting)
                return;

            if (m_lastTabCount != TabCount && SizeMode != TabSizeMode.Fixed)
            {
                m_setting = true;
                var dpiScale = new DpiScale(e.Graphics);

                for (int i = 0; i < TabPages.Count; i++)
                {
                    string text = TabPages[i].Text;
                    var stringSize = e.Graphics.MeasureString(text, Font);
                    int originalWidth = GetTabRect(i).Width;
                    int width = (int)Math.Ceiling(stringSize.Width) + dpiScale.ScaleIntX(16);
                    // Can't guarantee the dpi is working. Have to measure by injecting a space in the tab text.
                    if (m_spaceWidth < 0)
                    {
                        TabPages[i].Text += " ";
                        m_spaceWidth = GetTabRect(i).Width - originalWidth;
                    }

                    int spaces = (int)Math.Ceiling((width - GetTabRect(i).Width) / m_spaceWidth) + 2;
                    for (int j = 0; j < spaces; j++)
                        text += " ";
                    TabPages[i].Text = text;
                }

                m_setting = false;
                m_lastTabCount = TabCount;
            }

            Rectangle fillRect = new Rectangle(0, 0, Width, Height);
            using (Brush controlBrush = new SolidBrush(ColorTheme.Current.Window))
                e.Graphics.FillRectangle(controlBrush, fillRect);

            if (!m_showHeaders)
                return;

            var renderOrder = GetTabPageRenderOrder();
            if (renderOrder.Count == 0)
                return;

            int selectedRow = SelectedIndex == -1 || SelectedIndex >= m_tabRows.Count ? 0 : m_tabRows[SelectedIndex];
            foreach (int tabIndex in renderOrder)
                DrawTab(tabIndex, e, m_tabRows[tabIndex] == selectedRow);

            base.OnPaint(e);
        }

        private List<int> GetTabPageRenderOrder()
        {
            m_tabRowLookup.Clear();
            m_tabRows.Clear();
            for (int i = 0; i < TabPages.Count; i++)
            {
                Rectangle tabRect = GetTabRect(i);
                if (!m_tabRowLookup.TryGetValue(tabRect.Y, out var list))
                {
                    list = new List<int>();
                    m_tabRowLookup[tabRect.Y] = list;
                }

                list.Add(i);
            }

            var keys = m_tabRowLookup.Keys.ToList();
            keys.Sort();

            m_tabRows.Clear();
            foreach (var key in keys)
                m_tabRows.AddRange(m_tabRowLookup[key]);
            return m_tabRows;
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
            else
                tabRect.Height += 2;

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
