using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class FilterForm : Form
    {
        private List<RectangleF> m_rects = new List<RectangleF>();
        private List<Tuple<PointF, PointF>> m_lines = new List<Tuple<PointF, PointF>>();

        private readonly Pen m_pen;

        public FilterForm(Screen screen, ScreenFilter filter)
        {
            Init(screen);

            m_pen = new Pen(new SolidBrush(Color.Black), filter.LineThickness);
            Opacity = filter.Opacity;
            Rectangle rect = screen.Bounds;

            if (filter.Type == ScreenFilterType.Ellipse)
                InitEllipse(filter, rect);
            else
                InitScanline(filter, rect);
        }

        private void InitScanline(ScreenFilter filter, Rectangle rect)
        {
            if (filter.VerticalScanlines)
            {
                for (float x = rect.Left; x < rect.Right; x += filter.ScanlineSpacing)
                    m_lines.Add(new Tuple<PointF, PointF>(new PointF(x, rect.Top), new PointF(x, rect.Bottom)));
            }

            if (filter.HorizontalScanlines)
            {
                for (float y = rect.Top; y < rect.Height; y += filter.ScanlineSpacing)
                    m_lines.Add(new Tuple<PointF, PointF>(new PointF(rect.Left, y), new PointF(rect.Right, y)));
            }
        }

        private void InitEllipse(ScreenFilter filter, Rectangle rect)
        {
            int count = 0;
            float add = 0.0f;

            for (float x = rect.Left; x < rect.Right; x += filter.BlockSize + filter.SpacingX)
            {
                if (filter.Stagger)
                    add = count % 2 == 0 ? 0 : filter.BlockSize / 2;

                for (float y = rect.Top - filter.BlockSize; y < rect.Height + filter.BlockSize; y += filter.BlockSize + filter.SpacingY)
                    m_rects.Add(new RectangleF(new PointF(x, y + add), new SizeF(filter.BlockSize, filter.BlockSize)));

                count++;
            }
        }

        private void Init(Screen screen)
        {
            InitializeComponent();
            Text = string.Empty;
            NativeMethods.MoveWindow(Handle, screen.Bounds.Location.X, screen.Bounds.Location.Y, screen.Bounds.Width, screen.Bounds.Height, false);
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            TransparencyKey = BackColor;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            //For some reason the first call randomly doesn't take up the whole screen, so call it again
            NativeMethods.MoveWindow(Handle, screen.Bounds.Location.X, screen.Bounds.Location.Y, screen.Bounds.Width, screen.Bounds.Height, false);

            Load += CrtFilterForm_Load;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            int wl = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL.ExStyle);
            wl = wl | 0x80000 | 0x20;
            NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL.ExStyle, wl);
        }

        private void CrtFilterForm_Load(object sender, EventArgs e)
        {
            Task.Run(() => TopMostFunc());
        }

        private void TopMostFunc()
        {
            try
            {
                while (true)
                {
                    Invoke(new Action(SetTopMost));
                    System.Threading.Thread.Sleep(5);
                }
            }
            catch
            {
                //exception will happen when the form closes, just use this to stop the thread
            }
        }

        private void SetTopMost()
        {
            TopMost = true;
            TopLevel = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var item in m_rects)
                e.Graphics.DrawEllipse(m_pen, item);

            foreach (var item in m_lines)
                e.Graphics.DrawLine(m_pen, item.Item1, item.Item2);
        }
    }
}
