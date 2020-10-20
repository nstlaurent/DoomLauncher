using System;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
using System.Linq;
using System.Collections.Generic;

namespace DoomLauncher.Controls
{
    public partial class TagSelectControl : UserControl
    {
        public event EventHandler<ITagData> TagSelectionChanged;
        public event EventHandler<string> StaticSelectionChanged;

        private TagSelectOptions m_options = new TagSelectOptions();
        private readonly DataGridView[] m_views;
        private List<ITagData> m_checkedTags = new List<ITagData>();
        private bool m_loaded;

        public TagSelectControl()
        {
            InitializeComponent();

            m_views = new DataGridView[] { dgvStatic, dgvCustom };

            Load += TagSelectControl_Load;
        }

        private void TagSelectControl_Load(object sender, EventArgs e)
        {
            m_loaded = true;
            ClearSelections();
            EnableSelection();
            SetCheckedTags();
        }

        private object GetStaticDataSource()
        {
            return TabKeys.KeyNames.Select(x => new TagData() { Name = x }).ToArray();
        }

        public void Init(TagSelectOptions options)
        {
            m_options = options;

            Array.ForEach(m_views, x => InitGrid(x));
            dgvStatic.DataSource = GetStaticDataSource();
            dgvStatic.ScrollBars = ScrollBars.None;

            if (m_options.ShowStatic)
            {
                DpiScale dpiScale = new DpiScale(CreateGraphics());
                int height = dpiScale.ScaleIntY(8);
                foreach (DataGridViewRow row in dgvStatic.Rows)
                    height += row.Height;

                tblMain.RowStyles[1].Height = height;
            }
            else
            {
                tblMain.RowStyles[1].Height = 0;
            }

            DataCache.Instance.TagsChanged += DataCache_TagsChanged;
            SetTags();
        }

        public void SetCheckedTags(IEnumerable<ITagData> tags)
        {
            m_checkedTags = tags.ToList();
            SetTags();
        }

        public void SetCheckedTags()
        {
            if (dgvCustom.Columns.Count < 2)
                return;

            HashSet<ITagData> tagHash = new HashSet<ITagData>(m_checkedTags);
            foreach (DataGridViewRow row in dgvCustom.Rows)
            {
                ITagData tag = row.DataBoundItem as ITagData;
                if (!(dgvCustom.Rows[row.Index].Cells[0] is DataGridViewCheckBoxCell checkCell))
                    continue;

                checkCell.Value = tagHash.Contains(tag);
            }
        }

        public List<ITagData> GetCheckedTags() => m_checkedTags;

        private void SetTags()
        {
            DisableSelection();

            IEnumerable<ITagData> tags;

            if (m_options.HasTabOnly)
                tags = DataCache.Instance.Tags.Where(x => x.HasTab);
            else
                tags = DataCache.Instance.Tags;

            if (!string.IsNullOrEmpty(txtSearch.Text))
                tags = DataCache.Instance.Tags.Where(x => x.Name.IndexOf(txtSearch.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);

            dgvCustom.DataSource = tags.ToArray();

            SetCheckedTags();

            ClearSelections();
            EnableSelection();
        }

        private void DataCache_TagsChanged(object sender, EventArgs e)
        {
            SetTags();
        }

        private void InitGrid(DataGridView view)
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            GameFileViewControl.StyleGrid(view);

            view.Columns.Clear();

            view.ColumnHeadersVisible = false;
            view.MultiSelect = false;
            view.AllowUserToResizeColumns = false;
            view.AllowUserToResizeRows = false;
            view.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (view != dgvStatic && m_options.ShowCheckBoxes)
                view.Columns.Add(new DataGridViewCheckBoxColumn() { ReadOnly = false, Width = dpiScale.ScaleIntX(32) });

            view.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = string.Empty,
                Name = nameof(ITagData.Name),
                DataPropertyName = nameof(ITagData.Name)
            });

            view.Columns[view.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void View_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView view = sender as DataGridView;
            if (view.SelectedRows.Count == 0)
                return;

            if (view == dgvCustom)
            {
                if (m_options.ShowCheckBoxes && view.SelectedRows[0].Cells[0] is DataGridViewCheckBoxCell checkBoxCell && checkBoxCell.Value != null)
                {
                    bool set = !(bool)checkBoxCell.Value;
                    checkBoxCell.Value = set;

                    if (set)
                        m_checkedTags.Add(view.SelectedRows[0].DataBoundItem as ITagData);
                    else
                        m_checkedTags.Remove(view.SelectedRows[0].DataBoundItem as ITagData);
                }

                TagSelectionChanged?.Invoke(this, view.SelectedRows[0].DataBoundItem as ITagData);
                dgvStatic.ClearSelection();
            }
            else
            {
                StaticSelectionChanged?.Invoke(this, ((ITagData)view.SelectedRows[0].DataBoundItem).Name);
                dgvCustom.ClearSelection();
            }

            if (!m_options.AllowRowSelect)
                view.ClearSelection();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SetTags();
        }

        public void ClearSelections() => Array.ForEach(m_views, x => x.ClearSelection());
        private void DisableSelection() => Array.ForEach(m_views, x => x.SelectionChanged -= View_SelectionChanged);

        private void EnableSelection()
        {
            if (m_loaded)
                Array.ForEach(m_views, x => x.SelectionChanged += View_SelectionChanged);
        }
    }

    public class TagSelectOptions
    {
        public bool ShowStatic { get; set; }
        public bool HasTabOnly { get; set; }
        public bool ShowCheckBoxes { get; set; }
        public bool AllowRowSelect { get; set; }
    }
}
