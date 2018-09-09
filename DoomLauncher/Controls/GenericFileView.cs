using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using System.Reflection;
using DoomLauncher.DataSources;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;

namespace DoomLauncher
{
    public partial class GenericFileView : BasicFileView
    {
        public GenericFileView()
        {
            InitializeComponent();

            dgvMain.RowHeadersVisible = false;
            dgvMain.AutoGenerateColumns = false;
            dgvMain.ShowCellToolTips = false;
            dgvMain.DefaultCellStyle.SelectionBackColor = Color.Gray;

            SetColumnFields(new Tuple<string, string>[]
            {
                new Tuple<string, string>("Description", "Description"),
                new Tuple<string, string>("DateCreated", "Created"),
                new Tuple<string, string>("SourcePortName", "SourcePort")
            });
        }

        public override void SetData(IGameFile gameFile)
        {
            SetData(dgvMain, gameFile);
        }

        public override void ClearData()
        {
            dgvMain.DataSource = null;
        }

        private void SetColumnFields(IEnumerable<Tuple<string, string>> columnFields)
        {
            if (columnFields.Count() > 0)
            {
                foreach (Tuple<string, string> item in columnFields)
                {
                    DataGridViewColumn col = new DataGridViewTextBoxColumn
                    {
                        HeaderText = item.Item2,
                        Name = item.Item1,
                        DataPropertyName = item.Item1
                    };
                    dgvMain.Columns.Add(col);
                }

                dgvMain.Columns[dgvMain.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void dgvMain_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && e.Button == MouseButtons.Right && dgvMain.SelectedRows.Count < 2)
            {
                dgvMain.ClearSelection();
                dgvMain.Rows[e.RowIndex].Selected = true;
                dgvMain.Rows[e.RowIndex].Cells[0].Selected = true;
            }
        }

        protected override List<IFileData> GetSelectedFiles()
        {
            List<IFileData> files = new List<IFileData>();

            if (dgvMain.SelectedRows.Count > 0)
            {
                PropertyInfo pi = dgvMain.SelectedRows[0].DataBoundItem.GetType().GetProperty("FileData");

                foreach (DataGridViewRow row in dgvMain.SelectedRows)
                    files.Add(pi.GetValue(row.DataBoundItem) as IFileData);
            }

            return files;
        }
    }
}
