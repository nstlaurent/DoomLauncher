using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class DpiScale
    {
        public DpiScale(Graphics g)
        {
            DpiScaleX = g.DpiX / 96.0f;
            DpiScaleY = g.DpiY / 96.0f;
        }

        public DpiScale(float scaleX, float scaleY)
        {
            DpiScaleX = scaleX;
            DpiScaleY = scaleY;
        }

        public int ScaleIntY(int height) => (int)(height * DpiScaleY);
        public int ScaleIntX(int width) => (int)(width * DpiScaleY);

        public float ScaleFloatY(float height) => height * DpiScaleY;
        public float ScaleFloatX(float width) => width * DpiScaleX;

        public readonly float DpiScaleX;
        public readonly float DpiScaleY;

        public void ScaleControl(Control control)
        {
            control.Width = ScaleIntX(control.Width);
            control.Height = ScaleIntY(control.Height);
        }
    }
}
