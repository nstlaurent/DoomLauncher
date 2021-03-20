using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class ScreenshotViewerForm : Form
    {
        private string[] m_images = new string[] { };
        private int m_index;
        private bool m_slideshow;

        private SlideShowPictureBox pbMain = new SlideShowPictureBox();

        public ScreenshotViewerForm()
        {
            InitializeComponent();
            pbMain.Dock = DockStyle.Fill;
            tblMain.Controls.Add(pbMain, 0, 0);

            btnSave.Image = Icons.Save;

            KeyPreview = true;
            KeyUp += ScreenshotViewerForm_KeyUp;
            MouseWheel += ScreenshotViewerForm_MouseWheel;
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

        public void SetImages(string[] filenames)
        {
            m_images = filenames.ToArray();
            SetImage();
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
            }
        }

        private string GetImageFilename()
        {
            return m_images[m_index];
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
                dialog.Filter = string.Format("{0}|*.{0}", ext);

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

        private void SetSlideshow(bool set)
        {
            m_slideshow = set;
            if (set)
                btnSlideshow.BackColor = SystemColors.Highlight;
            else
                btnSlideshow.BackColor = SystemColors.Control;
        }
    }
}
