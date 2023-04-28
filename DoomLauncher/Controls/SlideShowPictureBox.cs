using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace DoomLauncher
{
    public partial class SlideShowPictureBox : UserControl
    {
        private const int ImageMilliseconds = 4000;
        private const int FadeMilliseconds = 500;
        private const int FadeTimes = 10;
        private readonly Timer m_timer = new Timer();
        private readonly Brush m_bgBrush = new SolidBrush(Color.Black);
        private readonly Stopwatch m_stopwatch = new Stopwatch();
        private Stopwatch m_fadeOut = new Stopwatch();
        private int m_index = 0;
        private int m_fadeCount = 0;
        private int m_lastFadeDiff = 0;
        private List<string> m_images = new List<string>();
        private float m_alpha = 0.0F;

        private SlideshowState m_state = SlideshowState.SetImage;
        private Image m_currentImage;
        private Image m_drawImage;
        private Graphics m_currentGraphics;

        public float ImageAlpha => m_alpha;
        public int ImageIndex => m_index;

        private enum SlideshowState
        {
            SetImage,
            FadeIn,
            Wait,
            FadeOut
        }

        public int ImageCount => m_images.Count;

        public event EventHandler<int> ImageChanged;
        public event EventHandler<PaintEventArgs> ImagePaint;

        public SlideShowPictureBox()
        {
            InitializeComponent();
            pbImage.BackColor = Color.Black;
            pbImage.Paint += PbImage_Paint;

            m_timer.Interval = FadeMilliseconds / FadeTimes;
            m_timer.Tick += M_timer_Tick;

            Resize += SlideShowPictureBox_Resize;
        }

        private void PbImage_Paint(object sender, PaintEventArgs e)
        {
            ImagePaint?.Invoke(this, e);
        }

        public void Stop()
        {
            if (m_images.Count > 0)
            {
                SetImages(m_images);
                m_timer.Stop();
            }
        }

        public void Resume()
        {
            if (m_images.Count > 0)
                SetImages(m_images);
        }

        public void SetImage(Image image)
        {
            ClearImage();
            pbImage.Image = image;
            ImageChanged?.Invoke(this, m_index);
        }

        public Image GetImage() => pbImage.Image;

        public bool SetImages(List<string> imagePaths, int startIndex = 0)
        {
            imagePaths = imagePaths.Where(x => File.Exists(x)).ToList();
            if (imagePaths.Count == 0)
            {
                ClearImage();
                m_images.Clear();
                m_timer.Stop();
                return false;
            }

            if (startIndex < 0 || startIndex > imagePaths.Count)
                startIndex = imagePaths.Count - 1;

            m_alpha = 1.0F;
            m_index = startIndex;
            m_images = imagePaths;
            m_state = SlideshowState.Wait;
            m_timer.Stop();

            pbImage.CancelAsync();

            // Don't cycle with one image
            if (imagePaths.Count == 1)
            {
                pbImage.ImageLocation = m_images[m_index];
                return true;
            }

            m_timer.Start();
            m_fadeOut.Restart();

            SetImage();

            return true;
        }

        public void ClearImage()
        {
            pbImage.CancelAsync();
            m_images = new List<string>();
            m_timer.Stop();
            pbImage.ImageLocation = string.Empty;
        }

        private void SlideShowPictureBox_Resize(object sender, EventArgs e)
        {
            SetImage();
        }

        private void HandleState()
        {
            switch (m_state)
            {
                case SlideshowState.SetImage:
                    m_state = SlideshowState.FadeIn;
                    m_alpha = 0.0F;
                    m_fadeOut.Restart();
                    SetImage();

                    m_fadeCount = FadeTimes;
                    break;

                case SlideshowState.FadeIn:
                    Fade(1.0F / FadeTimes);
                    if (m_fadeCount == 0)
                        m_state = SlideshowState.Wait;
                    break;

                case SlideshowState.Wait:
                    if (m_fadeOut.ElapsedMilliseconds >= ImageMilliseconds - FadeMilliseconds)
                    {
                        m_fadeCount = FadeTimes;
                        m_state = SlideshowState.FadeOut;
                    }
                    break;

                case SlideshowState.FadeOut:
                    Fade(-1.0F / FadeTimes);
                    if (m_fadeCount == 0)
                    {
                        m_index++;
                        m_index %= m_images.Count;
                        m_state = SlideshowState.SetImage;
                    }
                    break;
            }
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            HandleState();
        }

        private void Fade(float step)
        {
            m_stopwatch.Start();
            m_fadeCount--;
            m_alpha += step;

            SetTransparency();

            // SetTransparency uses DrawImage and becomes slower as the image becomes larger
            // If the blend takes longer than expected then skip the next one to keep the overall fade time roughly the same
            if (m_stopwatch.ElapsedMilliseconds + m_lastFadeDiff >= FadeMilliseconds / FadeTimes)
            {
                m_lastFadeDiff = (int)m_stopwatch.ElapsedMilliseconds - (FadeMilliseconds / FadeTimes);
                m_fadeCount--;
                m_alpha += step;

                if (m_fadeCount <= 0)
                {
                    if (step < 0)
                        m_alpha = 0.0F;
                    else
                        m_alpha = 1.0F;
                    m_fadeCount = 0;
                }
            }
            else
            {
                m_lastFadeDiff = 0;
            }

            m_stopwatch.Reset();
        }

        private void SetImage()
        {
            if (m_images.Count == 0)
            {
                pbImage.ImageLocation = string.Empty;
                return;
            }

            try
            {
                pbImage.Image = null;
                InitBlendCache();
                pbImage.Image = m_drawImage;
                SetTransparency();
            }
            catch
            {
                // File data is not accessible or out of memory, ignore for now
            }
        }

        private void InitBlendCache()
        {
            m_currentImage?.Dispose();
            m_currentGraphics?.Dispose();
            m_drawImage?.Dispose();

            using (var image = Image.FromFile(m_images[m_index]))
                m_currentImage = image.FixedSize(pbImage.Width, pbImage.Height, Color.Black);
            m_drawImage = new Bitmap(m_currentImage.Width, m_currentImage.Height);
            m_currentGraphics = Graphics.FromImage(m_drawImage);
        }

        private void SetTransparency()
        {
            try
            {
                if (m_alpha >= 1.0F)
                {
                    pbImage.Image = m_currentImage;
                    return;
                }

                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
                    new [] { 1.0F, 0.0F, 0.0F, 0.0F, 0.0F },
                    new [] { 0.0F, 1.0F, 0.0F, 0.0F, 0.0F },
                    new [] { 0.0F, 0.0F, 1.0F, 0.0F, 0.0F },
                    new [] { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F },
                    new [] { 0.0F, 0.0F, 0.0F, m_alpha, 1.0F },
                });

                Rectangle rect = new Rectangle(0, 0, m_currentImage.Width, m_currentImage.Height);
                m_currentGraphics.FillRectangle(m_bgBrush, rect);

                ImageAttributes imageAttrs = new ImageAttributes();
                imageAttrs.SetColorMatrix(colorMatrix);

                m_currentGraphics.DrawImage(m_currentImage, rect, 0, 0,
                    m_drawImage.Width, m_drawImage.Height, GraphicsUnit.Pixel, imageAttrs);
                pbImage.Image = m_drawImage;
                pbImage.Refresh();
            }
            catch
            {
                // Sometimes this can randomly fail, can be timing or whatever... just ignore
            }
        }
    }
}
