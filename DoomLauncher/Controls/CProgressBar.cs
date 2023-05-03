using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class CProgressBar : ProgressBar
    {
        public CProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = e.ClipRectangle;

            DpiScale dpiScale = new DpiScale(e.Graphics);
            int padX = dpiScale.ScaleIntX(2);
            int padY = dpiScale.ScaleIntY(2);

            rect.Width = (int)(rect.Width * ((double)Value / Maximum)) - (padX*2);
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rect.Height -= padY * 2;
            e.Graphics.FillRectangle(new SolidBrush(ColorTheme.Current.Highlight), padX, padY, rect.Width, rect.Height);
        }
    }
}
