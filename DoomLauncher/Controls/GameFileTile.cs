using DoomLauncher.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GameFileTile : UserControl
    {
        private const int ImageWidth = 300;
        private const int LabelHeight = 32;
        private const int LabelPosition = (int)(LabelHeight * 0.75);

        public event MouseEventHandler TileClick;
        public event EventHandler TileDoubleClick;

        public IGameFile GameFile { get; set; }
        public bool Selected { get; private set; }

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

            MouseClick += CtrlMouseClick;
            pb.MouseClick += CtrlMouseClick;
            lblTitle.MouseClick += CtrlMouseClick;

            DoubleClick += CtrlDoubleClick;
            pb.DoubleClick += CtrlDoubleClick;
            lblTitle.DoubleClick += CtrlDoubleClick;
        }

        public void SetSelected(bool set)
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
                lblTitle.ForeColor = Color.Black;
            }
        }

        public void SetPicture(string file)
        {
            pb.CancelAsync();
            if (!string.IsNullOrEmpty(file))
                pb.LoadAsync(file);
        }

        public void SetImage(Image image)
        {
            pb.CancelAsync();
            pb.Image = image;
        }

        public void SetTitle(string title)
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
