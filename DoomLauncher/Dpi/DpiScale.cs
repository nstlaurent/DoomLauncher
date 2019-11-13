using System.Drawing;

namespace DoomLauncher
{
    public class DpiScale
    {
        public DpiScale(Graphics g)
        {
            DpiScaleX = g.DpiX / 100.0f;
            DpiScaleY = g.DpiY / 100.0f;
        }

        public int ScaleIntY(int height) => (int)(height * DpiScaleY);
        public int ScaleIntX(int width) => (int)(width * DpiScaleY);

        public float ScaleFloatY(float height) => height * DpiScaleY;
        public float ScaleFloatX(float width) => width * DpiScaleX;

        public readonly float DpiScaleX;
        public readonly float DpiScaleY;
    }
}
