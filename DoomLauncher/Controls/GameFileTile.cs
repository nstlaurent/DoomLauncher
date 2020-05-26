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
        private const int ImageWidth = 300;
        private const int LabelHeight = 32;
        private const int LabelPosition = (int)(LabelHeight * 0.75);

        public override event MouseEventHandler TileClick;
        public override event EventHandler TileDoubleClick;

        public override IGameFile GameFile { get; protected set; }
        public override bool Selected { get; protected set; }

        private Color m_titleColor = Color.Black;

        public GameFileTile()
        {
            InitializeComponent();

            Width = ImageWidth;
            Height = (int)(Width / (16.0 / 9.0) + LabelHeight);

            lblTitle.Location = new Point(0, Height - LabelPosition);

            pb.Width = Width;
            pb.Height = Height - LabelHeight;
            pb.BackColor = Color.Black;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.WaitOnLoad = false;

            MouseClick += CtrlMouseClick;
            pb.MouseClick += CtrlMouseClick;
            lblTitle.MouseClick += CtrlMouseClick;

            DoubleClick += CtrlDoubleClick;
            pb.DoubleClick += CtrlDoubleClick;
            lblTitle.DoubleClick += CtrlDoubleClick;
        }

        public override void SetSelected(bool set)
        {
            Selected = set;

            if (set)
            {
                BorderStyle = BorderStyle.FixedSingle;
                BackColor = SystemColors.Highlight;
                pb.BackColor = SystemColors.Highlight;
                lblTitle.ForeColor = Color.White;
            }
            else
            {
                BorderStyle = BorderStyle.None;
                BackColor = SystemColors.Control;
                pb.BackColor = Color.Black;
                lblTitle.ForeColor = m_titleColor;
            }
        }

        public override void SetData(IGameFile gameFile, IEnumerable<ITagData> tags)
        {
            var colorTag = tags.FirstOrDefault(x => x.HasColor);
            if (colorTag != null)
                m_titleColor = Color.FromArgb(colorTag.Color.Value);
            else
               m_titleColor = Color.Black;

            lblTitle.ForeColor = m_titleColor;

            if (string.IsNullOrEmpty(gameFile.Title))
                SetTitle(gameFile.FileNameNoPath);
            else
                SetTitle(gameFile.Title);

            GameFile = gameFile;
        }

        public override void ClearData()
        {
            GameFile = null;
            lblTitle.Text = string.Empty;

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

        private void SetTitle(string title)
        {
            lblTitle.Text = title;
            Size size = TextRenderer.MeasureText(lblTitle.Text, lblTitle.Font);

            lblTitle.Location = new Point((Width - size.Width) / 2, Height - LabelPosition);
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
