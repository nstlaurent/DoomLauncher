using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Controls
{
    public enum ImageScaleMode
    {
        Zoom,
        Stretch,
        CropToFill,
        Center
    }

    public enum ImageAlignment
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class CPictureBox : Control
    {
        public event EventHandler LoadCompleted;
        public string FileLocation = "";

        private Image m_image;

        public Image Image
        {
            get => m_image;
            set { m_image = value; Invalidate(); }
        }

        public ImageScaleMode ScaleMode { get; set; } = ImageScaleMode.Zoom;
        public ImageAlignment Alignment { get; set; } = ImageAlignment.Center;
        public InterpolationMode Interpolation { get; set; } = InterpolationMode.HighQualityBicubic;

        public CPictureBox()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        public async Task LoadAsync(string path, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            FileLocation = path;
            Image img = null;

            await Task.Run(() =>
            {
                img = Image.FromFile(path);
            }, token);

            if (token.IsCancellationRequested)
                return;

            var imageAspect = img.Width / (double)img.Height;
            var tileAspect = Width / (double)Height;

            var match = Math.Abs(imageAspect - tileAspect) < 0.01;

            if (match)
            {
                ScaleMode = ImageScaleMode.Stretch;
            }
            else
            {
                var testSquare = Math.Abs(imageAspect - 1);
                if (testSquare < 0.1)
                    ScaleMode = ImageScaleMode.Zoom;
                else
                    ScaleMode = ImageScaleMode.CropToFill;
            }
            
            Image = img;
            LoadCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_image == null)
                return;

            var g = e.Graphics;
            g.InterpolationMode = Interpolation;

            float boxW = Width;
            float boxH = Height;
            float imgW = m_image.Width;
            float imgH = m_image.Height;

            RectangleF rect = RectangleF.Empty;

            switch (ScaleMode)
            {
                case ImageScaleMode.Stretch:
                    rect = new RectangleF(0, 0, boxW, boxH);
                    break;

                case ImageScaleMode.Center:
                    rect = GetCenteredRect(imgW, imgH, boxW, boxH);
                    break;

                case ImageScaleMode.Zoom:
                    rect = GetZoomRect(imgW, imgH, boxW, boxH);
                    break;

                case ImageScaleMode.CropToFill:
                    rect = GetCropRect(imgW, imgH, boxW, boxH);
                    break;
            }

            g.DrawImage(m_image, rect);
        }

        private RectangleF GetCenteredRect(float imgW, float imgH, float boxW, float boxH)
        {
            float x = (boxW - imgW) / 2f;
            float y = (boxH - imgH) / 2f;
            return new RectangleF(x, y, imgW, imgH);
        }

        private RectangleF GetZoomRect(float imgW, float imgH, float boxW, float boxH)
        {
            float imgAspect = imgW / imgH;
            float boxAspect = boxW / boxH;

            if (imgAspect > boxAspect)
            {
                float scale = boxW / imgW;
                float scaledHeight = imgH * scale;
                float offsetY = (boxH - scaledHeight) / 2f;
                return new RectangleF(0, offsetY, boxW, scaledHeight);
            }
            else
            {
                float scale = boxH / imgH;
                float scaledWidth = imgW * scale;
                float offsetX = (boxW - scaledWidth) / 2f;
                return new RectangleF(offsetX, 0, scaledWidth, boxH);
            }
        }
        private RectangleF GetCropRect(float imgW, float imgH, float boxW, float boxH)
        {
            float imgAspect = imgW / imgH;
            float boxAspect = boxW / boxH;

            if (imgAspect > boxAspect)
            {
                float scale = boxH / imgH;
                float scaledWidth = imgW * scale;
                float offsetX = (boxW - scaledWidth) / 2f;
                return new RectangleF(offsetX, 0, scaledWidth, boxH);
            }
            else
            {
                float scale = boxW / imgW;
                float scaledHeight = imgH * scale;
                float offsetY = (boxH - scaledHeight) / 2f;
                return new RectangleF(0, offsetY, boxW, scaledHeight);
            }
        }
    }
}
