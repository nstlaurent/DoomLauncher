using DoomLauncher.DataSources;
using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SourcePortViewForm : Form
    {
        public event EventHandler SourcePortLaunched;

        private readonly ITabView[] m_tabViews;
        private readonly SourcePortLaunchType m_launchType;
        private readonly IDataSourceAdapter m_adapter;
        private readonly AppConfiguration m_appConfig;

        public SourcePortViewForm(IDataSourceAdapter adapter, AppConfiguration appConfig, ITabView[] tabViews, SourcePortLaunchType type)
        {
            InitializeComponent();

            dgvSourcePorts.MultiSelect = false;

            m_adapter = adapter;
            m_appConfig = appConfig;
            m_tabViews = tabViews;
            m_launchType = type;

            ResetData();
            dgvSourcePorts.Columns[dgvSourcePorts.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Stylizer.Stylize(this, DesignMode);
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
            bool loadArchived = chkShowArchived.Checked;
            IEnumerable<ISourcePortData> data;
            if (m_launchType == SourcePortLaunchType.Utility)
            {
                Text = "Utilities";
                data = m_adapter.GetUtilities(loadArchived);
            }
            else
            {
                Text = "Source Ports";
                data = m_adapter.GetSourcePorts(loadArchived);
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

                return GetSourcePortFromRow(dgvSourcePorts.SelectedRows[0]);
            }
        }

        private ISourcePortData GetSourcePortFromRow(DataGridViewRow row)
        {
            object item = row.DataBoundItem;
            PropertyInfo pi = item.GetType().GetProperty("SourcePort");
            return pi.GetValue(item) as ISourcePortData;
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
                    SelectSourcePort(sourcePort);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SourcePortEditForm editForm = new SourcePortEditForm(m_adapter, m_tabViews, m_launchType);

            IEnumerable<string> extensions = new string[] { ".wad" };

            if (m_launchType == SourcePortLaunchType.SourcePort)
                extensions = extensions.Union(Util.GetDehackedExtensions().Union(Util.GetSourcePortPkExtensions()));
            else
                extensions = extensions.Union(Util.GetSourcePortPkExtensions());

            editForm.SetSupportedExtensions(string.Join(",", extensions));

            editForm.StartPosition = FormStartPosition.CenterParent;

            if (editForm.ShowDialog(this) == DialogResult.OK)
            {
                SourcePortData sourcePort = new SourcePortData();
                editForm.UpdateDataSource(sourcePort);
                sourcePort.LaunchType = m_launchType;
                m_adapter.InsertSourcePort(sourcePort);
                ResetData();

                SelectSourcePort(sourcePort);
            }
        }

        private void SelectSourcePort(ISourcePortData sourcePort)
        {
            // Source ports are ordered by name so we have to search for it
            // The identity (SourcePortID) is saved after the insert so just use the name to find it when the ID is zero
            foreach (DataGridViewRow row in dgvSourcePorts.Rows)
            {
                if (sourcePort.SourcePortID > 0)
                {
                    if (sourcePort.SourcePortID == GetSourcePortFromRow(row).SourcePortID)
                    {
                        row.Selected = true;
                        break;
                    }
                }
                else if (sourcePort.Executable == GetSourcePortFromRow(row).Executable)
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageCheckBox messageBox = new MessageCheckBox("Confirm", GetDeleteConfirm(), "Delete save games, demos, and statistics associated with this port",
                SystemIcons.Exclamation, MessageBoxButtons.OKCancel);

            messageBox.SetShowCheckBox(m_launchType == SourcePortLaunchType.SourcePort);

            if (SelectedItem != null && messageBox.ShowDialog(this) == DialogResult.OK)
            {
                int index = dgvSourcePorts.SelectedRows[0].Index;
                try
                {
                    if (m_launchType == SourcePortLaunchType.SourcePort)
                    {
                        m_adapter.UpdateGameFiles(GameFileFieldType.SourcePortID, GameFileFieldType.SourcePortID, SelectedItem.SourcePortID, null);

                        if (messageBox.Checked)
                            DeleteSourcePortFiles();
                        else
                            UnlinkFilesFromSourcePort();
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

                if (index >= dgvSourcePorts.Rows.Count)
                    index = dgvSourcePorts.Rows.Count - 1;

                if (dgvSourcePorts.Rows.Count > 0)
                    dgvSourcePorts.Rows[index].Selected = true;
            }
        }

        private static FileType[] GetDeleteFileTypes()
        {
            return new FileType[] { FileType.SaveGame, FileType.Demo  };
        }

        private void DeleteSourcePortFiles()
        {
            var deleteFileTypes = GetDeleteFileTypes();
            var files = m_adapter.GetFiles().Where(x => x.SourcePortID == SelectedItem.SourcePortID && deleteFileTypes.Contains(x.FileTypeID));

            foreach(var fileType in deleteFileTypes)
                m_adapter.DeleteFiles(SelectedItem, fileType);
   
            m_adapter.DeleteStats(SelectedItem);
            m_adapter.UpdateFiles(SelectedItem.SourcePortID, -1); //Since we didn't delete screenshots unlink them

            foreach(var file in files)
            {
                try
                {
                    string deleteFile = Path.Combine(m_appConfig.PathForFileType(file.FileTypeID).GetFullPath(), file.FileName);
                    if (File.Exists(deleteFile))
                        File.Delete(deleteFile);
                }
                catch
                { 
                    //we tried 
                }
            }
        }

        private void UnlinkFilesFromSourcePort()
        {
            m_adapter.UpdateFiles(SelectedItem.SourcePortID, -1);

            var stats = m_adapter.GetStats().Where(x => x.SourcePortID == SelectedItem.SourcePortID);
            foreach (var stat in stats)
            {
                stat.SourcePortID = -1;
                m_adapter.UpdateStats(stat);
            }
        }

        private string GetDeleteConfirm()
        {
            if (m_launchType == SourcePortLaunchType.SourcePort)
                return "Deleting this source port will orphan demo files and save games associated with it. Are you sure you want to continue?\n\nConsider archiving this port instead!";
            else
                return "Are you sure you want to delete this utility?";
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            if (IsInitSetup)
                Close();
            else
                SourcePortLaunched?.Invoke(this, new EventArgs());
        }

        public ISourcePortData GetSelectedSourcePort()
        {
             return SelectedItem;
        }

        private void chkShowArchived_CheckedChanged(object sender, EventArgs e)
        {
            ResetData();
        }
    }
}
