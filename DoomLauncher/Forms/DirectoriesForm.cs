using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class DirectoriesForm : Form
    {
        public DirectoriesForm()
        {
            InitializeComponent();
            lstDirectories.HeaderStyle = ColumnHeaderStyle.None;
            lstDirectories.Columns[0].Width = lstDirectories.Width;
        }

        public void SetDirectories(string[] directories)
        {
            Array.ForEach(directories, x => lstDirectories.Items.Add(x));
        }

        public List<string> GetDirectories()
        {
            List<string> ret = new List<string>();

            foreach (ListViewItem item in lstDirectories.Items)
                ret.Add(item.Text);

            return ret;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                lstDirectories.Items.Add(dialog.SelectedPath);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstDirectories.SelectedItems.Count > 0)
                lstDirectories.Items.Remove(lstDirectories.SelectedItems[0]);
        }
    }
}
