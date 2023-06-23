using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CProgressBar : ProgressBar
    {
        private readonly Timer m_timer;
        private int m_blend;
        private bool m_direction = true;

        public CProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
            VisibleChanged += CProgressBar_VisibleChanged;
            m_timer = new Timer();
            m_timer.Interval = 5;
            m_timer.Tick += Timer_Tick;
        }

        private void CProgressBar_VisibleChanged(object sender, System.EventArgs e)
        {
            if (Visible && !m_timer.Enabled)
                m_timer.Start();

            if (!Visible && !m_timer.Enabled)
                m_timer.Stop();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            if (!Visible)
            {
                m_timer.Stop();
                return;
            }

            const int Amount = 4;
            if (m_direction)
                m_blend += Amount;
            else
                m_blend -= Amount;

            if (m_blend <= 0 && !m_direction)
            {
                m_blend = 0;
                m_direction = true;
            }

            if (m_blend >= 255 && m_direction)
            {
                m_blend = 255;
                m_direction = false;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int value = Value;
            var brush = new SolidBrush(ColorTheme.Current.Highlight);
            if (Style == ProgressBarStyle.Marquee)
            {
                Value = Maximum;
                brush = new SolidBrush(Color.FromArgb(m_blend, ColorTheme.Current.Highlight));
            }

            Rectangle rect = e.ClipRectangle;
            DpiScale dpiScale = new DpiScale(e.Graphics);
            int padX = dpiScale.ScaleIntX(2);
            int padY = dpiScale.ScaleIntY(2);

            if (Maximum != 0)
                rect.Width = (int)(rect.Width * ((double)value / Maximum));
            rect.Width -= (padX * 2);

            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rect.Height -= padY * 2;
            e.Graphics.FillRectangle(brush, padX, padY, rect.Width, rect.Height);
        }
    }
}
