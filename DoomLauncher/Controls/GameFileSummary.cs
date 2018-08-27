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
        }

        public string Title
        {
            get { return lblTitle.Text; }
            set
            { 
                lblTitle.Text = value;
                tblMain.RowStyles[0].Height = lblTitle.Height + 6;
                if (tblMain.RowStyles[0].Height < m_labelHeight)
                    tblMain.RowStyles[0].Height = m_labelHeight;
            }
        }

        public string Description
        {
            get { return txtDescription.Text; }
            set
            {
                txtDescription.Clear();
                txtDescription.Visible = false;

                string[] items = value.Split(new char[] { '\n' });
                StringBuilder sb = new StringBuilder();

                foreach (string item in items)
                {
                    sb.Append(Regex.Replace(item, @"\s+", " "));
                    sb.Append(Environment.NewLine);
                }

                txtDescription.Text = sb.ToString();

                txtDescription.Visible = true;
            }
        }

        public void SetPreviewImage(string source, bool isUrl)
        {
            pbImage.CancelAsync();
            //if (pbImage.Image != null)
            //    pbImage.Image = null;

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

        private bool m_setting = false;

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            SetTextBoxScrollBars();
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            SetTextBoxScrollBars();
        }

        private void SetTextBoxScrollBars()
        {
            SetTextBoxScroll(txtDescription);
            SetTextBoxScroll(txtComments);
        }

        private void SetTextBoxScroll(TextBox tb)
        {
            if (m_setting) return;
            m_setting = true;
            Size tS = TextRenderer.MeasureText(tb.Text, tb.Font);
            bool vsb = tb.ClientSize.Height < tS.Height + Convert.ToInt32(tb.Font.Size);
            bool hsb = tb.ClientSize.Width < tS.Width;

            if (hsb)
                tb.ScrollBars = ScrollBars.Horizontal;
            if (vsb)
                tb.ScrollBars = ScrollBars.Vertical;

            if (hsb && vsb)
                tb.ScrollBars = ScrollBars.Both;
            else if (!hsb && !vsb)
                tb.ScrollBars = ScrollBars.None;

            m_setting = false;
        }

        private double m_aspectWidth = 16, m_aspectHeight = 9;

        private void GameFileSummary_ClientSizeChanged(object sender, EventArgs e)
        {
            int width = Width;
            double height = width / (m_aspectWidth / m_aspectHeight);
            m_imageHeight = Convert.ToSingle(height);

            if (pbImage.Image != null)
                ShowImageSection(true);

            SetTextBoxScrollBars();
        }
    }
}