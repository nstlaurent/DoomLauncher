using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GameFileSummary : UserControl
    {
        private float m_labelHeight, m_imageHeight;

        private SlideShowPictureBox pbImage = new SlideShowPictureBox();

        public GameFileSummary()
        {
            InitializeComponent();

            tblMain.Controls.Add(pbImage, 0, 1);
            pbImage.Dock = DockStyle.Fill;

            m_labelHeight = GetRowStyle(lblTitle).Height;
            m_imageHeight = GetRowStyle(pbImage).Height;
            ShowCommentsSection(false);

            txtComments.WarnLinkClick = false;
        }

        public void SetTitle(string text)
        {
            if (lblTitle.Text == text)
                return;

            DpiScale dpiScale = new DpiScale(CreateGraphics());
            lblTitle.Text = text;

            float height = lblTitle.Height + dpiScale.ScaleFloatY(6);
            if (height < m_labelHeight)
                height = m_labelHeight;

            GetRowStyle(lblTitle).Height = height;
        }

        public void SetDescription(string text)
        {
            txtDescription.Clear();
            txtDescription.Visible = false;

            txtDescription.Text = Util.CleanDescription(text);

            txtDescription.Visible = true;
        }

        public void SetPreviewImages(List<string> imagePaths)
        {
            pbImage.SetImages(imagePaths);
            ShowImageSection(true);
        }

        public void SetPreviewImage(Image image)
        {
            pbImage.SetImage(image);
            ShowImageSection(true);
        }

        public void SetComments(string comments)
        {
            ShowCommentsSection(true);
            txtComments.Text = comments;
        }

        public void ClearPreviewImage()
        {
            pbImage.ClearImage();
            ShowImageSection(false);
        }

        public void ClearComments()
        {
            txtComments.Text = string.Empty;
            ShowCommentsSection(false);
        }

        private RowStyle GetRowStyle(Control ctrl)
        {
            return tblMain.RowStyles[tblMain.GetRow(ctrl)];
        }

        private void ShowImageSection(bool bShow)
        {
            if (bShow)
                GetRowStyle(pbImage).Height = m_imageHeight;
            else
                GetRowStyle(pbImage).Height = 0;
        }

        private void ShowCommentsSection(bool bShow)
        {
            if (bShow)
            {
                DpiScale dpiScale = new DpiScale(CreateGraphics());
                GetRowStyle(txtComments).Height = dpiScale.ScaleFloatY(20);
            }
            else
            {
                GetRowStyle(txtComments).Height = 0;
            }
        }

        public string TagText
        {
            get { return lblTags.Text; }
            set
            {
                lblTags.Text = value;
            }
        }

        public void SetTimePlayed(int minutes)
        {
            lblTimePlayed.Text = Util.GetTimePlayedString(minutes);
        }

        public void SetStatistics(IGameFile gameFile, IEnumerable<IStatsData> stats)
        {
            if (stats.Any())
            {
                DpiScale dpiScale = new DpiScale(CreateGraphics());
                ctrlStats.Visible = true;
                GetRowStyle(ctrlStats).Height = dpiScale.ScaleFloatY(120);

                ctrlStats.SetStatistics(gameFile, stats);

                lblLastMap.Text = stats.OrderByDescending(x => x.RecordTime).First().MapName;
            }
            else
            {
                ctrlStats.Visible = false;
                GetRowStyle(ctrlStats).Height = 0;
                lblLastMap.Text = "N/A";
            }
        }

        private double m_aspectWidth = 16, m_aspectHeight = 9;

        private void GameFileSummary_ClientSizeChanged(object sender, EventArgs e)
        {
            int width = Width;
            double height = width / (m_aspectWidth / m_aspectHeight);
            m_imageHeight = Convert.ToSingle(height);

            if (pbImage.ImageCount > 0)
                ShowImageSection(true);
        }
    }
}