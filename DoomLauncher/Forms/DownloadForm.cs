using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class DownloadForm : Form
    {
        public DownloadForm()
        {
            InitializeComponent();

            FormClosing += DownloadForm_FormClosing;
            Shown += DownloadForm_Shown;
        }

        void DownloadForm_Shown(object sender, EventArgs e)
        {
            Showing = true;
        }

        void DownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Showing = false;
            Hide();
        }

        public bool Showing
        {
            get;
            private set;
        }

        public DownloadView DownloadView
        {
            get
            {
                return downloadView;
            }
        }
    }
}
