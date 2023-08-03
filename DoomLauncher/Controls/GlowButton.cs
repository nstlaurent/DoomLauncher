using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class GlowButton : Button
    {
        private readonly Timer m_timer;
        private readonly Color m_baseColor;
        private readonly Color m_baseTextColor;
        private int m_alpha = 0;
        private int m_increment = 25;

        public Color BackGroundGlowColor => ColorTheme.Current.Highlight;
        public Color TextGlowColor => ColorTheme.Current.WindowLight;

        public GlowButton()
        {
            DoubleBuffered = true;
            m_timer = new Timer() { Interval = 50 };
            m_timer.Tick += timer_Tick;

            FlatStyle = FlatStyle.Flat;

            m_baseColor = ColorTheme.Current.Window;
            m_baseTextColor = ColorTheme.Current.Text;
        }

        public void GlowOnce()
        {
            m_timer.Start();
            BackColor = CreateBlendColor(m_alpha, m_baseColor, BackGroundGlowColor);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int testValue = m_alpha + m_increment;

            if (testValue > 255 || testValue < 0)
            {
                if (testValue < 0)
                {
                    m_timer.Stop();
                    BackColor = m_baseColor;
                    ForeColor = m_baseTextColor;
                    return;
                }

                m_increment = -m_increment;
            }

            m_alpha += m_increment;
            BackColor = CreateBlendColor(m_alpha, m_baseColor, BackGroundGlowColor);
            ForeColor = CreateBlendColor(m_alpha, m_baseTextColor, TextGlowColor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                m_timer.Dispose();
            base.Dispose(disposing);
        }

        private Color CreateBlendColor(int alpha, Color baseColor, Color glowColor)
        {
            return AlphaBlend(Color.FromArgb(alpha, glowColor), baseColor);
        }

        public Color AlphaBlend(Color A, Color B)
        {
            var r = (A.R * A.A / 255) + (B.R * B.A * (255 - A.A) / (255 * 255));
            var g = (A.G * A.A / 255) + (B.G * B.A * (255 - A.A) / (255 * 255));
            var b = (A.B * A.A / 255) + (B.B * B.A * (255 - A.A) / (255 * 255));
            var a = A.A + (B.A * (255 - A.A) / 255);
            return Color.FromArgb(a, r, g, b);
        }
    }
}