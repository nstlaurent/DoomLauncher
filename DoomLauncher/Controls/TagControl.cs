using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;

namespace DoomLauncher
{
    public partial class TagControl : UserControl
    {
        private IDataSourceAdapter m_adapter;
        private List<ITagData> m_addTags = new List<ITagData>();
        private List<ITagData> m_editTags = new List<ITagData>();
        private List<ITagData> m_deleteTags = new List<ITagData>();

        public TagControl()
        {
            InitializeComponent();

            dgvTags.RowHeadersVisible = false;
            dgvTags.AutoGenerateColumns = false;
            dgvTags.DefaultCellStyle.SelectionBackColor = Color.Gray;

            DataGridViewColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Name";
            col.Name = "Name";
            col.DataPropertyName = "Name";
            dgvTags.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Display Tab";
            col.Name = "HasTab";
            col.DataPropertyName = "HasTab";
            dgvTags.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Display Color";
            col.Name = "HasColor";
            col.DataPropertyName = "HasColor";
            dgvTags.Columns.Add(col);

            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void Init(IDataSourceAdapter adapter)
        {
            m_adapter = adapter;
            dgvTags.DataSource = adapter.GetTags().OrderBy(x => x.Name).ToList();
        }

        public ITagData[] AddedTags
        {
            get { return m_addTags.ToArray();  }
        }

        public ITagData[] EditedTags
        {
            get { return m_editTags.ToArray(); }
        }

        public ITagData[] DeletedTags
        {
            get { return m_deleteTags.ToArray(); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TagEditForm form = new TagEditForm();
            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                TagData tag = new TagData();
                form.TagEditControl.GetDataSource(tag);

                if (!IsTagNameUnique(tag))
                {
                    MessageBox.Show(this, "Tag name must be unique and not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    m_adapter.InsertTag(tag);
                    Init(m_adapter);

                    IEnumerable<ITagData> check = m_adapter.GetTags().Where(x => x.Name == tag.Name);

                    if (check.Any())
                    {
                        if (m_addTags.Contains(check.First()))
                            m_addTags.Remove(check.First());

                        m_addTags.Add(check.First());
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTags.SelectedRows.Count > 0)
            {
                ITagData tag = dgvTags.SelectedRows[0].DataBoundItem as ITagData;

                if (tag != null)
                {
                    TagEditForm form = new TagEditForm();
                    form.TagEditControl.SetDataSource(tag);
                    form.StartPosition = FormStartPosition.CenterParent;
                    
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        form.TagEditControl.GetDataSource(tag);

                        if (!IsTagNameUnique(tag))
                        {
                            MessageBox.Show(this, "Tag name must be unique and not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Init(m_adapter);
                        }
                        else
                        {
                            m_adapter.UpdateTag(tag);
                            Init(m_adapter);

                            if (m_editTags.Contains(tag))
                                m_editTags.Remove(tag);

                            m_editTags.Add(tag);
                        }
                    }
                }
            }
        }

        private bool IsTagNameUnique(ITagData tag)
        {
            IEnumerable<ITagData> check = m_adapter.GetTags().Where(x => x.Name.Equals(tag.Name, StringComparison.CurrentCultureIgnoreCase) && !x.Equals(tag));
            return !(string.IsNullOrEmpty(tag.Name) || check.Any() || MainForm.GetBaseTabs().ToList().FindAll(x=> tag.Name.Equals(x, StringComparison.CurrentCultureIgnoreCase)).Any());
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTags.SelectedRows.Count > 0 && MessageBox.Show(this, "Are you sure you want to delete this tag?", 
                "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                ITagData tag = dgvTags.SelectedRows[0].DataBoundItem as ITagData;

                if (tag != null)
                {
                    m_adapter.DeleteTag(tag);
                    m_adapter.DeleteTagMapping(tag.TagID);

                    Init(m_adapter);

                    if (m_deleteTags.Contains(tag))
                        m_deleteTags.Remove(tag);

                    m_deleteTags.Add(tag);
                }
            }
        }
    }
}
