using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class ScreenshotViewerForm : Form
    {
        private string[] m_images = new string[] { };
        private int m_index = 0;

        public ScreenshotViewerForm()
        {
            InitializeComponent();

            KeyPreview = true;
            KeyUp += ScreenshotViewerForm_KeyUp;
            MouseWheel += ScreenshotViewerForm_MouseWheel;
        }

        private void ScreenshotViewerForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                SetPreviousImage();
            else if (e.Delta > 0)
                SetNextImage();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //this is to set the focus of the left/right buttons
            if (!msg.HWnd.Equals(this.Handle) && (keyData == Keys.Left || keyData == Keys.Right))
            {
                switch (keyData)
                {
                    case Keys.Right:
                        btnNext.Focus();
                        break;
                    case Keys.Left:
                        btnPrev.Focus();
                        break;
                }
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
            if (pbMain.Image != null)
                pbMain.Image.Dispose();
            pbMain.Image = Image.FromFile(GetImageFilename());
            Text = string.Format("Screenshot Viewer - {0}/{1}", m_index + 1, m_images.Length);
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
    }
}
