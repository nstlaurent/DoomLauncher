using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GameFileTile : GameFileTileBase
    {
        public static readonly int ImageWidth = 300;
        public static readonly int ImageHeight = (int)(ImageWidth / (16.0 / 9.0));
        private static readonly int LabelHeight = 32;
        private static readonly string NewString = "New!";
        private static readonly int NewPadX = 6;
        private static readonly int NewPadY = 4;
        private static readonly Font DisplayFont = new Font("Arial", 10, FontStyle.Bold);
        private static readonly Pen SeparatorPen = new Pen(Color.LightGray, 1.0f);

        public override event MouseEventHandler TileClick;
        public override event EventHandler TileDoubleClick;

        public bool DrawBorder { get; set; } = true;
        public override IGameFile GameFile { get; protected set; }
        public override bool Selected { get; protected set; }

        private Color m_titleColor = ColorTheme.Current.Text;
        private bool m_new;
        private bool m_loadingImage;
        private Image m_setImage;

        public GameFileTile()
        {
            InitializeComponent();

            DpiScale dpiScale = new DpiScale(CreateGraphics());

            int imageWidth = dpiScale.ScaleIntX(ImageWidth);
            int imageHeight = dpiScale.ScaleIntY(ImageHeight);
            int labelHeight = dpiScale.ScaleIntY(LabelHeight);

            Width = imageWidth;
            Height = GetStandardHeight(dpiScale);

            pb.Width = Width;
            pb.Height = Height - labelHeight;
            pb.BackColor = Color.Black;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.WaitOnLoad = false;
            pb.LoadCompleted += Pb_LoadCompleted;

            MouseClick += CtrlMouseClick;
            pb.MouseClick += CtrlMouseClick;

            DoubleClick += CtrlDoubleClick;
            pb.DoubleClick += CtrlDoubleClick;

            pb.Paint += Screenshot_Paint;
            Paint += GameFileTile_Paint;
        }

        public int GetStandardHeight(DpiScale dpiScale)
        {
            return dpiScale.ScaleIntY(ImageHeight) + dpiScale.ScaleIntY(LabelHeight);
        }

        private void Pb_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            m_loadingImage = false;
            if (m_setImage != null)
            {
                pb.Image = m_setImage;
                m_setImage = null;
            }
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
                pb.BackColor = ColorTheme.Current.Highlight;
            }
            else
            {
                BorderStyle = BorderStyle.None;
                BackColor = ColorTheme.Current.WindowDark;
                pb.BackColor = Color.Black;
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
            m_setImage = null;

            pb.CancelAsync();

            if (pb.Image != null)
                pb.Image = null;

            if (!string.IsNullOrEmpty(pb.ImageLocation))
                pb.ImageLocation = string.Empty;
        }

        public override void SetImageLocation(string file)
        {
            if (file.Equals(pb.ImageLocation))
                return;

            ClearImage();

            if (!string.IsNullOrEmpty(file))
            {
                m_loadingImage = true;
                pb.LoadAsync(file);
            }
        }

        public override void SetImage(Image image)
        {
            ClearImage();

            // CancelAsync doesn't really work, to get around this set m_setImage set to pb.Image when Pb_LoadCompleted fires 
            if (m_loadingImage)
                m_setImage = image;

            m_loadingImage = true;
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
