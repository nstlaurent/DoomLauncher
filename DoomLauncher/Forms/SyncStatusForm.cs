using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SyncStatusForm : Form
    {
        public SyncStatusForm()
        {
            InitializeComponent();

            GameFileViewControl.StyleGrid(dgvFiles);
            dgvFiles.Columns[dgvFiles.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Stylizer.Stylize(this, DesignMode);
        }

        public void SetHeaderText(string text)
        {
            lblHeader.Text = text;
        }

        public void SetData(IEnumerable<string> files, IEnumerable<string> dropDownOptions)
        {
            List<SyncFileData> syncFiles = files.Select(x => new SyncFileData(x)).OrderBy(x => x.FileName).ToList();
            dgvFiles.DataSource = syncFiles;

            cmbOptions.DataSource = dropDownOptions.ToList();
        }

        public List<string> GetSelectedFiles()
        {
            List<SyncFileData> syncFiles = dgvFiles.DataSource as List<SyncFileData>;
            return syncFiles.Where(x => x.Selected).Select(x => x.FileName).ToList();
        }

        public int SelectedOptionIndex
        {
            get { return cmbOptions.SelectedIndex; }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            List<SyncFileData> syncFiles = dgvFiles.DataSource as List<SyncFileData>;

            if (syncFiles.Count > 0)
            {
                bool select = !syncFiles[0].Selected;

                foreach (SyncFileData file in syncFiles)
                    file.Selected = select;

                dgvFiles.DataSource = null;
                dgvFiles.DataSource = syncFiles;

                dgvFiles.Columns[dgvFiles.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvFiles.Columns[dgvFiles.Columns.Count - 1].HeaderText = string.Empty;
            }
        }
    }
}
