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

namespace DoomLauncher
{
    public partial class GameFileSummary : UserControl
    {
        private float m_labelHeight, m_imageHeight;

        public GameFileSummary()
        {
            InitializeComponent();
            m_labelHeight = tblMain.RowStyles[0].Height;
            m_imageHeight = tblMain.RowStyles[1].Height;
            ShowCommentsSection(false);

            txtComments.WarnLinkClick = false;
        }

        public void SetTitle(string text)
        {
            lblTitle.Text = text;
            tblMain.RowStyles[0].Height = lblTitle.Height + 6;
            if (tblMain.RowStyles[0].Height < m_labelHeight)
                tblMain.RowStyles[0].Height = m_labelHeight;
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

        private void ShowImageSection(bool bShow)
        {
            if (bShow)
                tblMain.RowStyles[1].Height = m_imageHeight;
            else
                tblMain.RowStyles[1].Height = 0;
        }

        private void ShowCommentsSection(bool bShow)
        {
            if (bShow)
                tblMain.RowStyles[6].Height = 20;
            else
                tblMain.RowStyles[6].Height = 0;
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

        public void SetStatistics(IEnumerable<IStatsData> stats)
        {
            if (stats.Count() > 0)
            {
                ctrlStats.Visible = true;
                tblMain.RowStyles[3].Height = 92;

                ctrlStats.SetStatistics(stats);
            }
            else
            {
                ctrlStats.Visible = false;
                tblMain.RowStyles[3].Height = 0;
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