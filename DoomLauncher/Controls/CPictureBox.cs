using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
        Center,
        FitHeight
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
        public new event EventHandler<PaintEventArgs> Paint;
        public event EventHandler LoadCompleted;
        public string FileLocation = "";

        private Image m_image;

        public Image Image
        {
            get => m_image;
            set
            {
                if (m_image != null && m_image != value)
                    m_image.Dispose();

                m_image = value;
                Invalidate();
            }
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

            Image?.Dispose();
            Image = null;

            Image img = null;

            await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                img = LoadImage(path);
            }, token);

            if (token.IsCancellationRequested)
            {
                img?.Dispose();
                return;
            }

            Image = img;
            LoadCompleted?.Invoke(this, EventArgs.Empty);
        }

        private static Image LoadImage(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawImage(e);
            Paint?.Invoke(this, new PaintEventArgs(e.Graphics, e.ClipRectangle));
        }

        private void DrawImage(PaintEventArgs e)
        {
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

                case ImageScaleMode.FitHeight:
                    rect = GetFitHeightRect(imgW, imgH, boxW, boxH);
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

        private RectangleF GetFitHeightRect(float imgW, float imgH, float boxW, float boxH)
        {
            float scale = boxH / imgH;
            float scaledWidth = imgW * scale;
            float scaledHeight = boxH;
            float x = (boxW - scaledWidth) / 2f;
            return new RectangleF(x, 0f, scaledWidth, scaledHeight);
        }
    }
}
