using System;
using System.Collections.Generic;
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
            Icons.DpiScale = new DpiScale(CreateGraphics());
            btnAdd.Image = Icons.File;
            btnDelete.Image = Icons.Delete;

            Stylizer.Stylize(this, DesignMode);
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
