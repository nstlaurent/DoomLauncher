using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class FilesCtrl : UserControl
    {
        public event AdditionalFilesEventHanlder CellFormatting;
        public event AdditionalFilesEventHanlder NewItemNeeded;
        public event AdditionalFilesEventHanlder ItemRemoved;
        public event AdditionalFilesEventHanlder ItemPriorityUp;
        public event AdditionalFilesEventHanlder ItemPriorityDown;
        public event AdditionalFilesEventHanlder ItemRemoving;

        private List<object> m_files = new List<object>();
        private string m_keyProperty, m_dataProperty;

        public FilesCtrl()
        {
            InitializeComponent();
            dgvAdditionalFiles.BackgroundColor = SystemColors.Window;
            dgvAdditionalFiles.AutoGenerateColumns = false;
            dgvAdditionalFiles.RowHeadersVisible = false;
            dgvAdditionalFiles.MultiSelect = false;
            dgvAdditionalFiles.CellFormatting += dgvAdditionalFiles_CellFormatting;

            btnAdd.Image = Icons.File;
            btnDelete.Image = Icons.Delete;
            btnMoveUp.Image = Icons.ArrowDown;
            btnMoveDown.Image = Icons.ArrowUp;
        }

        public void Initialize(string keyProperty, string dataProperty)
        {
            dgvAdditionalFiles.Columns[0].DataPropertyName = dataProperty;
            dgvAdditionalFiles.Columns[0].Name = dataProperty;

            m_keyProperty = keyProperty;
            m_dataProperty = dataProperty;
        }

        public List<object> GetFiles()
        {
            return m_files.ToList();
        }

        public void SetDataSource(object dataSource)
        {
            dgvAdditionalFiles.DataSource = null;
            dgvAdditionalFiles.DataSource = dataSource;

            m_files.Clear();
            foreach (DataGridViewRow dgvr in dgvAdditionalFiles.Rows)
                m_files.Add(dgvr.DataBoundItem);
        }

        public string GetAdditionalFilesString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (object file in m_files)
            {
                PropertyInfo pi = file.GetType().GetProperty(m_dataProperty);
                sb.Append(pi.GetValue(file).ToString());
                sb.Append(';');
            }

            return sb.ToString();
        }

        private void dgvAdditionalFiles_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (CellFormatting != null)
            {
                object item = dgvAdditionalFiles.Rows[e.RowIndex].DataBoundItem;
                AdditionalFilesEventArgs args = new AdditionalFilesEventArgs(item, (string)e.Value);
                CellFormatting(this, args);
                e.Value = args.DisplayText;
            }
        }

        private object SelectedItem
        {
            get
            {
                if (dgvAdditionalFiles.SelectedRows.Count > 0)
                    return dgvAdditionalFiles.SelectedRows[0].DataBoundItem;

                return null;
            }
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            AdditionalFilesEventArgs args = new AdditionalFilesEventArgs(null);
            NewItemNeeded?.Invoke(this, args);

            if (args.NewItems != null)
            {
                m_files.AddRange(args.NewItems);
                Rebind();
                if (dgvAdditionalFiles.Rows.Count > 0)
                    dgvAdditionalFiles.Rows[dgvAdditionalFiles.Rows.Count - 1].Selected = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            HandleDelete();
        }

        private void dgvAdditionalFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                HandleDelete();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveFile(true);
            object item = SelectedItem;
            if (item != null && ItemPriorityUp != null)
                ItemPriorityUp(this, new AdditionalFilesEventArgs(item));
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveFile(false);
            object item = SelectedItem;
            if (item != null && ItemPriorityDown != null)
                ItemPriorityDown(this, new AdditionalFilesEventArgs(item));
        }

        private void MoveFile(bool up)
        {
            if (dgvAdditionalFiles.SelectedRows.Count > 0)
            {
                object file = dgvAdditionalFiles.SelectedRows[0].DataBoundItem;
                int priority = GetFilePriority(file);

                if (up && priority > 0)
                    priority--;

                if (!up && priority < m_files.Count - 1)
                    priority++;

                m_files.Remove(file);
                m_files.Insert(priority, file);

                Rebind();

                dgvAdditionalFiles.ClearSelection();
                dgvAdditionalFiles.Rows[priority].Selected = true;
            }
        }

        private void Rebind()
        {
            m_files = m_files.Distinct().ToList();
            SetDataSource(m_files.ToList());
        }

        private int GetFilePriority(object file)
        {
            int count = 0;
            PropertyInfo pi = file.GetType().GetProperty(m_keyProperty);

            foreach (object fileCheck in m_files)
            {
                if (pi.GetValue(fileCheck).Equals(pi.GetValue(file)))
                    break;

                count++;
            }

            return count;
        }

        private void HandleDelete()
        {
            object item = SelectedItem;
            if (item != null)
            {
                int index = dgvAdditionalFiles.SelectedRows[0].Index;
                AdditionalFilesEventArgs cancelEvent = new AdditionalFilesEventArgs(item);
                ItemRemoving?.Invoke(this, cancelEvent);

                if (!cancelEvent.Cancel)
                {
                    m_files.Remove(item);
                    Rebind();

                    if (dgvAdditionalFiles.Rows.Count > 0)
                    {
                        if (index >= dgvAdditionalFiles.Rows.Count)
                            index = dgvAdditionalFiles.Rows.Count - 1;
                        dgvAdditionalFiles.Rows[index].Selected = true;
                    }

                    ItemRemoved?.Invoke(this, new AdditionalFilesEventArgs(item));
                }
            }
        }
    }

    public delegate void AdditionalFilesEventHanlder(object sender, AdditionalFilesEventArgs e);
    public class AdditionalFilesEventArgs : EventArgs
    {
        public AdditionalFilesEventArgs(object item)
        {
            Item = item;
            Cancel = false;
        }

        public AdditionalFilesEventArgs(object item, string displayText)
        {
            Item = item;
            DisplayText = displayText;
            Cancel = false;
        }

        public object Item { get; set; }
        public string DisplayText { get; set; }
        public List<object> NewItems { get; set; }
        public bool Cancel { get; set; }
    }
}
