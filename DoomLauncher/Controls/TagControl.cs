using DoomLauncher.Controls;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.Stylize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TagControl : UserControl
    {
        private IDataSourceAdapter m_adapter;
        private readonly List<ITagData> m_addTags = new List<ITagData>();
        private readonly List<ITagData> m_editTags = new List<ITagData>();
        private readonly List<ITagData> m_deleteTags = new List<ITagData>();

        public TagControl()
        {
            InitializeComponent();
            Stylizer.StylizeControl(this, DesignMode);
        }

        public void Init(IDataSourceAdapter adapter)
        {
            m_adapter = adapter;
            tagSelectCtrl.Init(new TagSelectOptions() { AllowRowSelect = true, ShowTagData = true, CustomSetData = true });
            SetTagData();
        }

        private void SetTagData()
        {
            tagSelectCtrl.SetDataSource(DataCache.Instance.SortTags(m_adapter.GetTags()).ToList());
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
                    StyledMessageBox.Show(this, "Tag name must be unique and not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    m_adapter.InsertTag(tag);
                    SetTagData();

                    IEnumerable<ITagData> check = m_adapter.GetTags().Where(x => x.Name == tag.Name);

                    if (check.Any())
                    {
                        if (m_addTags.Contains(check.First()))
                            m_addTags.Remove(check.First());

                        m_addTags.Add(check.First());
                        tagSelectCtrl.SetSelectedItem(check.First());
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (tagSelectCtrl.SelectedItem != null)
            {
                ITagData tag = tagSelectCtrl.SelectedItem;
                TagEditForm form = new TagEditForm();
                form.TagEditControl.SetDataSource(tag);
                form.StartPosition = FormStartPosition.CenterParent;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    form.TagEditControl.GetDataSource(tag);

                    if (!IsTagNameUnique(tag))
                    {
                        StyledMessageBox.Show(this, "Tag name must be unique and not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SetTagData();
                    }
                    else
                    {
                        m_adapter.UpdateTag(tag);
                        SetTagData();

                        if (m_editTags.Contains(tag))
                            m_editTags.Remove(tag);

                        m_editTags.Add(tag);
                    }

                    tagSelectCtrl.SetSelectedItem(tag);
                }
            }
        }

        private bool IsTagNameUnique(ITagData tag)
        {
            IEnumerable<ITagData> check = m_adapter.GetTags().Where(x => x.Name.Equals(tag.Name, StringComparison.CurrentCultureIgnoreCase) && !x.Equals(tag));
            return !(string.IsNullOrEmpty(tag.Name) || check.Any() || TabKeys.KeyNames.ToList().FindAll(x=> tag.Name.Equals(x, StringComparison.CurrentCultureIgnoreCase)).Any());
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (tagSelectCtrl.SelectedItem != null && StyledMessageBox.Show(this, "Are you sure you want to delete this tag?",
                "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                ITagData tag = tagSelectCtrl.SelectedItem;
                m_adapter.DeleteTag(tag);
                m_adapter.DeleteTagMapping(tag.TagID);

                SetTagData();

                if (m_deleteTags.Contains(tag))
                    m_deleteTags.Remove(tag);

                m_deleteTags.Add(tag);
            }
        }
    }
}
