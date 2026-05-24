using DoomLauncher.Controls;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace DoomLauncher
{
    public partial class GameFileTile : GameFileTileBase
    {
        private static readonly int LabelHeight = 32;
        private static readonly string NewString = "New!";
        private static readonly int NewPadX = 6;
        private static readonly int NewPadY = 4;
        private static readonly Font DisplayFont = new Font("Arial", 10, FontStyle.Bold);
        private static readonly Pen SeparatorPen = new Pen(Color.LightGray, 1.0f);

        public override event MouseEventHandler TileClick;
        public override event EventHandler TileDoubleClick;

        public override int ImageWidth { get; protected set; }
        public readonly int ImageHeight;
        public bool DrawBorder { get; set; } = true;
        public override IGameFile GameFile { get; protected set; }
        public override bool Selected { get; protected set; }

        private Color m_titleColor = ColorTheme.Current.Text;
        private bool m_new;
        private CancellationTokenSource m_cancelToken;
        private bool m_isTitlepic;

        public GameFileTile()
        {
            InitializeComponent();

            DpiScale dpiScale = new DpiScale(CreateGraphics());

            ImageWidth = dpiScale.ScaleIntX(Math.Max(DataCache.Instance.AppConfiguration.TileImageSize, 100));
            ImageHeight = GetImageHeight(ImageWidth);
            int labelHeight = dpiScale.ScaleIntY(LabelHeight);

            Width = ImageWidth;
            Height = GetStandardHeight(dpiScale);

            pb.Width = Width;
            pb.Height = ImageHeight;
            pb.BackColor = Color.Black;

            MouseClick += CtrlMouseClick;
            pb.MouseClick += CtrlMouseClick;

            DoubleClick += CtrlDoubleClick;
            pb.DoubleClick += CtrlDoubleClick;

            pb.Paint += Screenshot_Paint;
            Paint += GameFileTile_Paint;

            pb.LoadCompleted += Pb_LoadCompleted;
        }

        private void Pb_LoadCompleted(object sender, EventArgs e)
        {
            var img = pb.Image;
            if (img == null)
                return;

            pb.ScaleMode = CalcImageScaleMode(img);
        }

        private ImageScaleMode CalcImageScaleMode(Image img)
        {
            if (m_isTitlepic)
                return ImageScaleMode.FitHeight;

            var imageAspect = img.Width / (double)img.Height;
            var tileAspect = pb.Width / (double)pb.Height;

            var match = Math.Abs(imageAspect - tileAspect) < 0.01;

            if (match)
            {
                return  ImageScaleMode.Stretch;
            }
            else
            {
                var testSquare = Math.Abs(imageAspect - 1);
                if (testSquare < 0.1)
                    return ImageScaleMode.Zoom;
                else
                    return  ImageScaleMode.CropToFill;
            }
        }

        public static int GetImageHeight(int imageWidth) => (int)(imageWidth / DataCache.Instance.AppConfiguration.TileImageAspectRatio);

        public int GetStandardHeight(DpiScale dpiScale)
        {
            return ImageHeight + dpiScale.ScaleIntY(LabelHeight);
        }

        private void GameFileTile_Paint(object sender, PaintEventArgs e)
        {
            if (GameFile == null)
                return;

            DpiScale dpiScale = new DpiScale(e.Graphics);
            int labelHeight = dpiScale.ScaleIntY(LabelHeight);
            int pad = dpiScale.ScaleIntX(1);

            SizeF layout = new SizeF(Width, 16);
            string text;
            if (!string.IsNullOrEmpty(GameFile.Title))
                text = Util.GetClippedEllipsesText(e.Graphics, DisplayFont, GameFile.Title, layout);
            else
                text = GameFile.FileNameNoPath;

            SizeF size = e.Graphics.MeasureDisplayString(text, DisplayFont);
            float x = Width - size.Width - (Width - size.Width) / 2;
            float y = Height - size.Height - (labelHeight - size.Height) / 2;
            if (Selected)
                e.Graphics.DrawString(text, DisplayFont, new SolidBrush(ColorTheme.Current.HighlightText), x, y);
            else
                e.Graphics.DrawString(text, DisplayFont, new SolidBrush(m_titleColor), x, y);

            SeparatorPen.Color = ColorTheme.Current.Border;
            if (DrawBorder && !Selected)
                e.Graphics.DrawRectangle(SeparatorPen, 0, 0, Width - pad, Height - pad);
        }

        private void Screenshot_Paint(object sender, PaintEventArgs e)
        {
            if (m_new)
            {
                DpiScale dpiScale = new DpiScale(e.Graphics);
                int newPadX = dpiScale.ScaleIntX(NewPadX);
                int newPadY = dpiScale.ScaleIntY(NewPadY);
                int pad1 = dpiScale.ScaleIntX(1);
                SizeF size = e.Graphics.MeasureString(NewString, DisplayFont);
                RectangleF rect = new RectangleF(pb.ClientRectangle.Right - size.Width - newPadX - pad1, pb.ClientRectangle.Height - size.Height - newPadY - pad1, 
                    size.Width + newPadX, size.Height + newPadY);
                e.Graphics.FillRectangle(Brushes.Red, rect);
                e.Graphics.DrawRectangle(Pens.Gray, rect.Left, rect.Top, rect.Width, rect.Height);
                e.Graphics.DrawString(NewString, DisplayFont, Brushes.White, new PointF(rect.Left + newPadX / 2 + pad1 + pad1, rect.Top + newPadY / 2 + pad1));
            }
        }

        public override void SetSelected(bool set)
        {
            Selected = set;

            if (set)
            {
                BorderStyle = BorderStyle.FixedSingle;
                BackColor = ColorTheme.Current.Highlight;
            }
            else
            {
                BorderStyle = BorderStyle.None;
                BackColor = ColorTheme.Current.WindowDark;
            }
        }

        public override void SetData(IGameFile gameFile, IEnumerable<ITagData> tags)
        {
            m_new = gameFile.Downloaded.HasValue && (DateTime.Now - gameFile.Downloaded.Value).TotalHours < 24;

            var colorTag = tags.FirstOrDefault(x => x.HasColor);
            if (colorTag != null && colorTag.Color != null)
                m_titleColor = Color.FromArgb(colorTag.Color.Value);
            else
                m_titleColor = ColorTheme.Current.Text;

            GameFile = gameFile;
            Invalidate();
        }

        public override void ClearData()
        {
            GameFile = null;
            ClearImage();
        }

        private void ClearImage()
        {
            m_isTitlepic = false;
            m_cancelToken?.Cancel();

            if (pb.Image != null)
                pb.Image = null;

            if (!string.IsNullOrEmpty(pb.FileLocation))
                pb.FileLocation = string.Empty;
        }

        // TODO: setting the mode is an override for using FitHeight on titlepics. Maybe not the greatest way but it works for now.
        public override void SetImageLocation(string file, bool titlepic = false)
        {
            if (file.Equals(pb.FileLocation))
                return;

            ClearImage();
            m_isTitlepic = titlepic;

            if (!string.IsNullOrEmpty(file))
            {
                m_cancelToken = new CancellationTokenSource();
                _ = pb.LoadAsync(file, m_cancelToken.Token);
            }
        }

        public override void SetImage(Image image)
        {
            ClearImage();
            pb.Image = image;
        }

        private void CtrlDoubleClick(object sender, EventArgs e)
        {
            TileDoubleClick?.Invoke(this, e);
        }

        private void CtrlMouseClick(object sender, MouseEventArgs e)
        {
            TileClick?.Invoke(this, e);
        }
    }
}
