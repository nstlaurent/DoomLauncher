using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class DownloadViewItem : UserControl
    {
        public event EventHandler Cancelled;

        public DownloadViewItem()
        {
            InitializeComponent();
        }

        public object Key
        {
            get;
            set;
        }

        public string DisplayText
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        public int ProgressValue
        {
            get { return progressBar.Value; }
            set { progressBar.Value = value; }
        }

        public bool CancelVisible
        {
            get { return btnCancel.Visible; }
            set { btnCancel.Visible = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled?.Invoke(this, new EventArgs());
        }
    }
}
