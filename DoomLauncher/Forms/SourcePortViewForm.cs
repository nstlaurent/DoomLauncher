using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SourcePortViewForm : Form
    {
        public event EventHandler SourcePortLaunched;

        private readonly ITabView[] m_tabViews;
        private readonly SourcePortLaunchType m_launchType;
        private readonly IDataSourceAdapter m_adapter;

        public SourcePortViewForm(IDataSourceAdapter adapter, ITabView[] tabViews, SourcePortLaunchType type)
        {
            InitializeComponent();

            dgvSourcePorts.DefaultCellStyle.NullValue = "N/A";
            dgvSourcePorts.RowHeadersVisible = false;
            dgvSourcePorts.AutoGenerateColumns = false;
            dgvSourcePorts.DefaultCellStyle.SelectionBackColor = Color.Gray;

            m_adapter = adapter;
            m_tabViews = tabViews;
            m_launchType = type;

            ResetData();
            dgvSourcePorts.Columns[dgvSourcePorts.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void DisplayInitSetupButton()
        {
            IsInitSetup = true;

            btnNext.ForeColor = Color.DarkGreen;
            btnNext.Text = "Next >>";
            btnNext.Visible = true;
        }

        private bool IsInitSetup { get; set; }
        public void ShowPlayButton(bool set)
        {
            btnNext.Visible = set;
        }

        private void ResetData()
        {
            IEnumerable<ISourcePortData> data;
            if (m_launchType == SourcePortLaunchType.Utility)
            {
                Text = "Utilities";
                data = m_adapter.GetUtilities();
            }
            else
            {
                Text = "Source Ports";
                data = m_adapter.GetSourcePorts();
            }

            SetDataSource(data);
        }

        private void SetDataSource(IEnumerable<ISourcePortData> sourcePorts)
        {
            dgvSourcePorts.DataSource =
                (from item in sourcePorts
                 select new { item.SourcePortID, item.Name, item.Executable, Directory = item.Directory.GetPossiblyRelativePath(), SourcePort = item }).ToList();
        }

        private ISourcePortData SelectedItem
        {
            get
            {
                if (dgvSourcePorts.SelectedRows.Count == 0)
                    return null;

                object item = dgvSourcePorts.SelectedRows[0].DataBoundItem;
                PropertyInfo pi = item.GetType().GetProperty("SourcePort");

                return pi.GetValue(item) as ISourcePortData;
            }
        }

        private void dgvSourcePorts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            HandleEdit();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void HandleEdit()
        {
            if (SelectedItem != null)
            {
                SourcePortEditForm editForm = new SourcePortEditForm(m_adapter, m_tabViews, m_launchType);
                ISourcePortData sourcePort = SelectedItem;
                editForm.SetDataSource(sourcePort);
                editForm.StartPosition = FormStartPosition.CenterParent;

                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    editForm.UpdateDataSource(sourcePort);
                    m_adapter.UpdateSourcePort(sourcePort);
                    ResetData();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SourcePortEditForm editForm = new SourcePortEditForm(m_adapter, m_tabViews, m_launchType);

            if (m_launchType == SourcePortLaunchType.SourcePort)
                editForm.SetSupportedExtensions(string.Format(".wad,{0},.deh,.bex", Util.GetPkExtensionsCsv()));
            else
                editForm.SetSupportedExtensions(string.Format(".wad,{0}", Util.GetPkExtensionsCsv()));

            editForm.StartPosition = FormStartPosition.CenterParent;

            if (editForm.ShowDialog(this) == DialogResult.OK)
            {
                SourcePortData sourcePort = new SourcePortData();
                editForm.UpdateDataSource(sourcePort);
                sourcePort.LaunchType = m_launchType;
                m_adapter.InsertSourcePort(sourcePort);
                ResetData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null && MessageBox.Show(this, GetDeleteConfirm(), "Confirm",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                try
                {
                    if (m_launchType == SourcePortLaunchType.SourcePort)
                    {
                        m_adapter.UpdateGameFiles(GameFileFieldType.SourcePortID, GameFileFieldType.SourcePortID, SelectedItem.SourcePortID, null);
                        m_adapter.UpdateFiles(SelectedItem.SourcePortID, -1);
                    }

                    m_adapter.DeleteSourcePort(SelectedItem);
                }
                catch (IOException)
                {
                    MessageBox.Show(this, "This file appears to be in use and cannot be deleted.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    Util.DisplayUnexpectedException(this, ex);
                }

                ResetData();
            }
        }

        private string GetDeleteConfirm()
        {
            if (m_launchType == SourcePortLaunchType.SourcePort)
                return "Deleting this source port will orphan demo files and save games associated with it. Are you sure you want to continue?";
            else
                return "Are you sure you want to delete this utility?";
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            if (IsInitSetup)
            {
                Close();
            }
            else
            {
                if (SourcePortLaunched != null)
                {
                    SourcePortLaunched(this, new EventArgs());
                }
            }
        }

        public ISourcePortData GetSelectedSourcePort()
        {
             return SelectedItem;
        }
    }
}
