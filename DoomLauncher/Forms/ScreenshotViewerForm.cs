using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class ScreenshotViewerForm : Form
    {
        private string[] m_images = Array.Empty<string>();
        private IList<IFileData> m_files = Array.Empty<IFileData>();
        private int m_index;
        private bool m_slideshow;
        private IGameFile m_gameFile;
        private IDataSourceAdapter m_adapter;

        private readonly SlideShowPictureBox pbMain = new SlideShowPictureBox();

        public ScreenshotViewerForm()
        {
            InitializeComponent();
            pbMain.Dock = DockStyle.Fill;
            tblContainer.Controls.Add(pbMain, 0, 0);

            statsControl.SetMapsVisible(false);
            statisticsView.SetMapsVisible(false);

            btnSave.Image = Icons.Save;

            KeyPreview = true;
            KeyUp += ScreenshotViewerForm_KeyUp;
            MouseWheel += ScreenshotViewerForm_MouseWheel;
            Stylizer.Stylize(this, DesignMode);
        }

        private void ScreenshotViewerForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                SetNextImage();
            else if (e.Delta > 0)
                SetPreviousImage();       
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //this is to set the focus of the left/right buttons
            if (!msg.HWnd.Equals(this.Handle) && (keyData == Keys.Left || keyData == Keys.Right))
            {
                if (keyData == Keys.Right)
                    btnNext.Focus();
                else if (keyData == Keys.Left)
                    btnPrev.Focus();

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ScreenshotViewerForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    SetNextImage();
                    break;
                case Keys.Left:
                    SetPreviousImage();
                    break;
                case Keys.Home:
                    SetFirst();
                    break;
                case Keys.End:
                    SetEnd();
                    break;
                default:
                    break;
            }
        }

        private void SetEnd()
        {
            if (m_images.Length > 0)
                m_index = m_images.Length - 1;
            SetImage();
        }

        private void SetFirst()
        {
            m_index = 0;
            SetImage();
        }

        public void SetImageFileData(IDataSourceAdapter adapter, IGameFile gameFile, 
            string[] imagePaths, IList<IFileData> screenshots)
        {
            m_adapter = adapter;
            m_gameFile = gameFile;
            m_images = imagePaths;
            m_files = screenshots;
            statisticsView.DataSourceAdapter = adapter;
        }

        public void SetImage(string filename)
        {
            for (int i = 0; i < m_images.Length; i++)
            {
                if (m_images[i] == filename)
                {
                    m_index = i;
                    SetImage();
                    break;
                }
            }
        }

        private void SetImage()
        {
            SetSlideshow(false);

            if (m_images.Length > 0)
            {
                Image image = pbMain.GetImage();
                pbMain.SetImage(Image.FromFile(GetImageFilename()));
                image?.Dispose();
                Text = string.Format("Screenshot Viewer - {0}/{1}", m_index + 1, m_images.Length);

                SetUserDescription(GetFileData());
            }
        }

        private void SetUserDescription(IFileData fileData)
        {
            if (string.IsNullOrEmpty(fileData.UserTitle) && string.IsNullOrEmpty(fileData.UserDescription) && string.IsNullOrEmpty(fileData.Map))
            {
                ShowImageUserData(false);
                return;
            }

            ShowImageUserData(true);
            lblTitle.Text = GetTitle(fileData);
            lblDescription.Text = string.IsNullOrEmpty(fileData.UserDescription) ? string.Empty : fileData.UserDescription;

            if (string.IsNullOrEmpty(fileData.Map))
            {
                statsControl.Visible = false;
                statisticsView.Visible = false;
                UpdateStatsHeights();
                return;
            }

            statsControl.Visible = true;
            statisticsView.Visible = true;
            var stats = statisticsView.SetDataByMap(m_gameFile, fileData.Map);
            statsControl.Visible = stats.Any();
            statsControl.SetStatistics(m_gameFile, stats);
            UpdateStatsHeights();
        }

        private string GetTitle(IFileData fileData)
        {
            string title = FileData.GetTitle(fileData);
            return string.IsNullOrEmpty(title) ? "N/A" : fileData.UserTitle;
        }

        private void UpdateStatsHeights()
        {
            if (statsControl.Visible)
                tblData.RowStyles[1].Height = 120;
            else
                tblData.RowStyles[1].Height = 0;

            if (statisticsView.Visible)
                tblData.RowStyles[2].Height = 200;
            else
                tblData.RowStyles[2].Height = 0;
        }

        private void ShowImageUserData(bool set)
        {
            if (set)
                tblContainer.ColumnStyles[1].Width = 50;
            else
                tblContainer.ColumnStyles[1].Width = 0;
        }

        private string GetImageFilename()
        {
            return m_images[m_index];
        }

        private IFileData GetFileData()
        {
            return m_files[m_index];
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            SetPreviousImage();
        }

        private void SetPreviousImage()
        {
            m_index = (m_images.Length + --m_index) % m_images.Length;
            SetImage();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SetNextImage();
        }

        private void SetNextImage()
        {
            m_index = ++m_index % m_images.Length;
            SetImage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            string ext = Path.GetExtension(GetImageFilename());
            if (!string.IsNullOrEmpty(ext))
                dialog.Filter = string.Format("{0}|*{0}", ext);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.Copy(GetImageFilename(), dialog.FileName, true);
                }
                catch
                {
                    MessageBox.Show(this, "Unable to save file.", "Unable to Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSlideshow_Click(object sender, EventArgs e)
        {
            SetSlideshow(!m_slideshow);

            if (m_slideshow)
            {
                Text = "Slideshow";
                pbMain.SetImages(m_images.ToList(), m_index);
            }
            else
            {
                SetImage();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!ScreenshotEditForm.ShowDialogAndUpdate(this, m_adapter, m_gameFile, GetFileData()))
                return;

            SetImage();
        }

        private void SetSlideshow(bool set)
        {
            ShowImageUserData(!set);
            m_slideshow = set;

            if (set)
                btnSlideshow.BackColor = SystemColors.Highlight;
            else
                btnSlideshow.BackColor = SystemColors.Control;
        }
    }
}
