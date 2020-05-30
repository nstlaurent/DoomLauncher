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

        private Color m_titleColor = Color.Black;
        private bool m_new;

        public GameFileTile()
        {
            InitializeComponent();

            Width = ImageWidth;
            Height = ImageHeight + LabelHeight;

            pb.Width = Width;
            pb.Height = Height - LabelHeight;
            pb.BackColor = Color.Black;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.WaitOnLoad = false;

            MouseClick += CtrlMouseClick;
            pb.MouseClick += CtrlMouseClick;

            DoubleClick += CtrlDoubleClick;
            pb.DoubleClick += CtrlDoubleClick;

            pb.Paint += Screenshot_Paint;
            Paint += GameFileTile_Paint;
        }

        private void GameFileTile_Paint(object sender, PaintEventArgs e)
        {
            if (GameFile == null)
                return;

            string text = string.IsNullOrEmpty(GameFile.Title) ? GameFile.FileNameNoPath : GameFile.Title;
            SizeF size = e.Graphics.MeasureString(text, DisplayFont);
            float x = Width - size.Width - (Width - size.Width) / 2;
            float y = Height - size.Height - (LabelHeight - size.Height) / 2;
            if (Selected)
                e.Graphics.DrawString(text, DisplayFont, Brushes.White, x, y);
            else
                e.Graphics.DrawString(text, DisplayFont, new SolidBrush(m_titleColor), x, y);

            if (DrawBorder && !Selected)
                e.Graphics.DrawRectangle(SeparatorPen, 0, 0, Width - 1, Height - 1);
        }

        private void Screenshot_Paint(object sender, PaintEventArgs e)
        {
            if (m_new)
            {
                SizeF size = e.Graphics.MeasureString(NewString, DisplayFont);
                RectangleF rect = new RectangleF(pb.ClientRectangle.Right - size.Width - NewPadX - 1, pb.ClientRectangle.Height - size.Height - NewPadY -1, 
                    size.Width + NewPadX, size.Height + NewPadY);
                e.Graphics.FillRectangle(Brushes.Red, rect);
                e.Graphics.DrawRectangle(Pens.Gray, rect.Left, rect.Top, rect.Width, rect.Height);
                e.Graphics.DrawString(NewString, DisplayFont, Brushes.White, new PointF(rect.Left + NewPadX, rect.Top + NewPadY / 2));
            }
        }

        public override void SetSelected(bool set)
        {
            Selected = set;

            if (set)
            {
                BorderStyle = BorderStyle.FixedSingle;
                BackColor = SystemColors.Highlight;
                pb.BackColor = SystemColors.Highlight;
            }
            else
            {
                BorderStyle = BorderStyle.None;
                BackColor = SystemColors.Control;
                pb.BackColor = Color.Black;
            }
        }

        public override void SetData(IGameFile gameFile, IEnumerable<ITagData> tags)
        {
            if (gameFile == null || gameFile.Equals(GameFile))
                return;

            m_new = gameFile.Downloaded.HasValue && (DateTime.Now - gameFile.Downloaded.Value).TotalHours < 24;

            var colorTag = tags.FirstOrDefault(x => x.HasColor);
            if (colorTag != null)
                m_titleColor = Color.FromArgb(colorTag.Color.Value);
            else
                m_titleColor = Color.Black;

            GameFile = gameFile;
            Invalidate();
        }

        public override void ClearData()
        {
            GameFile = null;

            pb.CancelAsync();

            if (pb.Image != null)
                pb.Image = null;

            if (!string.IsNullOrEmpty(pb.ImageLocation))
                pb.ImageLocation = string.Empty;
        }

        public override void SetImageLocation(string file)
        {
            pb.CancelAsync();
            if (!string.IsNullOrEmpty(file))
                pb.LoadAsync(file);
        }

        public override void SetImage(Image image)
        {
            pb.CancelAsync();
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
