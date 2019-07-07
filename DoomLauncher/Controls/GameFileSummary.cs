using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public partial class GameFileSummary : UserControl
    {
        private float m_labelHeight, m_imageHeight;

        public GameFileSummary()
        {
            InitializeComponent();
            m_labelHeight = GetRowStyle(lblTitle).Height;
            m_imageHeight = GetRowStyle(pbImage).Height;
            ShowCommentsSection(false);

            txtComments.WarnLinkClick = false;
        }

        public void SetTitle(string text)
        {
            lblTitle.Text = text;
            GetRowStyle(lblTitle).Height = lblTitle.Height + 6;
            if (GetRowStyle(lblTitle).Height < m_labelHeight)
                GetRowStyle(lblTitle).Height = m_labelHeight;
        }

        public void SetDescription(string text)
        {
            txtDescription.Clear();
            txtDescription.Visible = false;

            txtDescription.Text = Util.CleanDescription(text);

            txtDescription.Visible = true;
        }

        public void SetPreviewImage(string source, bool isUrl)
        {
            pbImage.CancelAsync();

            if (isUrl)
                pbImage.LoadAsync(source);
            else
                SetImageFromFile(source);

            ShowImageSection(true);
        }

        public void SetComments(string comments)
        {
            ShowCommentsSection(true);
            txtComments.Text = comments;
        }

        public void ClearPreviewImage()
        {
            pbImage.Image = null;
            ShowImageSection(false);
        }

        public void ClearComments()
        {
            txtComments.Text = string.Empty;
            ShowCommentsSection(false);
        }

        private void SetImageFromFile(string source)
        {
            try
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(source, FileMode.Open, FileAccess.Read);
                    pbImage.Image = Image.FromStream(fs);
                }
                catch { } //failed, nothin to do
                finally
                {
                    if (fs != null) fs.Close();
                }
            }
            catch
            {
                pbImage.Image = null;
            }
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
                GetRowStyle(txtComments).Height = 20;
            else
                GetRowStyle(txtComments).Height = 0;
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
                ctrlStats.Visible = true;
                GetRowStyle(ctrlStats).Height = 120;

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

            if (pbImage.Image != null)
                ShowImageSection(true);
        }
    }
}