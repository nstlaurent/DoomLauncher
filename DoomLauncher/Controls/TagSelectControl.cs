using System;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace DoomLauncher.Controls
{
    public partial class TagSelectControl : UserControl
    {
        public event EventHandler<ITagData> TagSelectionChanged;
        public event EventHandler<string> StaticSelectionChanged;
        public event EventHandler PinChanged;
        public event EventHandler ManageTags;

        public bool Pinned { get; private set; }

        private TagSelectOptions m_options = new TagSelectOptions();
        private List<ITagData> m_checkedTags = new List<ITagData>();
        private IEnumerable<ITagData> m_customSource;
        private bool m_loaded;

        public TagSelectControl()
        {
            InitializeComponent();

            btnSearch.Image = Icons.Search;
            btnPin.Image = Icons.Pin;

            SetPinned(false);
            Resize += TagSelectControl_Resize;
        }

        private void TagSelectControl_Resize(object sender, EventArgs e)
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            int offset = btnPin.Width + dpiScale.ScaleIntX(12);

            if (flpSearch.Width < dpiScale.ScaleIntX(60))
                return;

            if (flpSearch.Width < txtSearch.MaximumSize.Width + offset)
                txtSearch.Width = flpSearch.Width - offset;
            else
                txtSearch.Width = txtSearch.MaximumSize.Width;
        }

        public void Init(TagSelectOptions options)
        {
            m_loaded = true;
            m_options = options;

            btnPin.Visible = options.ShowPin;

            if (options.ShowMenu)
                dgvTags.ContextMenuStrip = menu;

            InitGrid(dgvTags);
            EnableSelection();

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
            int index = GetCheckBoxCellIndex();
            if (index == -1)
                return;

            HashSet<ITagData> tagHash = new HashSet<ITagData>(m_checkedTags);
            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                ITagData tag = row.DataBoundItem as ITagData;
                if (!(dgvTags.Rows[row.Index].Cells[index] is DataGridViewCheckBoxCell checkCell))
                    continue;

                checkCell.Value = tagHash.Contains(tag);
            }
        }

        private int GetCheckBoxCellIndex()
        {
            int index = 0;

            foreach(DataGridViewColumn column in dgvTags.Columns)
            {
                if (column is DataGridViewCheckBoxColumn)
                    return index;
                index++;
            }

            return -1;
        }

        public List<ITagData> GetCheckedTags() => m_checkedTags;

        public ITagData SelectedItem => dgvTags.SelectedRows.Count == 0 ? null : dgvTags.SelectedRows[0].DataBoundItem as ITagData;

        public void SetSelectedItem(ITagData tag)
        {
            ClearSelections();

            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                if (row.DataBoundItem is ITagData tagData && tagData.Equals(tag))
                {
                    dgvTags.FirstDisplayedScrollingRowIndex = row.Index;
                    row.Selected = true;
                    break;
                }
            }
        }

        public void SetDataSource(IEnumerable<ITagData> tags)
        {
            if (!m_options.CustomSetData)
                throw new InvalidOperationException("Can only call SetDataSource with CustomSetData in TagSelectOptions.");

            m_customSource = tags;
            SetTags();
        }

        private void SetTags()
        {
            DisableSelection();

            IEnumerable<ITagData> tags;

            if (m_options.CustomSetData)
            {
                if (m_customSource == null)
                    return;

                tags = m_customSource;
            }
            else
            {
                tags = DataCache.Instance.Tags;
            }

            if (m_options.HasTabOnly)
                tags = tags.Where(x => x.HasTab);
            if (!string.IsNullOrEmpty(txtSearch.Text))
                tags = tags.Where(x => x.Name.IndexOf(txtSearch.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);

            List<ITagData> allTags;

            if (m_options.ShowStatic)
            {
                allTags = GetStaticTags();
                allTags.AddRange(tags);
            }
            else
            {
                allTags = tags.ToList();
            }

            dgvTags.DataSource = allTags;

            SetCheckedTags();

            ClearSelections();
            EnableSelection();
        }

        private List<ITagData> GetStaticTags()
        {
            List<ITagData> tags = new List<ITagData>();
            HashSet<string> visibleViews = DataCache.Instance.AppConfiguration.VisibleViews;

            foreach (var key in TabKeys.KeyNames)
            {
                if (key != TabKeys.LocalKey && !visibleViews.Contains(key))
                    continue;

                tags.Add(new StaticTagData() { Name = key, Favorite = true });
            }

            return tags;
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

            view.ColumnHeadersVisible = m_options.ShowTagData;
            view.MultiSelect = false;
            view.AllowUserToResizeRows = false;
            view.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (m_options.ShowCheckBoxes)
                view.Columns.Add(new DataGridViewCheckBoxColumn() { ReadOnly = false, Width = dpiScale.ScaleIntX(32) });

            view.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Name = nameof(ITagData.Name),
                DataPropertyName = nameof(ITagData.FavoriteName),
                Width = dpiScale.ScaleIntX(200)
            });

            if (m_options.ShowTagData)
            {
                view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Display Tab",
                    Name = nameof(ITagData.HasTab),
                    DataPropertyName = nameof(ITagData.HasTab)
                });

                view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Exclude",
                    Name = nameof(ITagData.ExcludeFromOtherTabs),
                    DataPropertyName = nameof(ITagData.ExcludeFromOtherTabs)
                });

                view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Favorite",
                    Name = nameof(ITagData.Favorite),
                    DataPropertyName = nameof(ITagData.Favorite)
                });                

                view.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Display Color",
                    Name = nameof(ITagData.HasColor),
                    DataPropertyName = nameof(ITagData.HasColor)
                });
            }

            view.Columns[view.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void View_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView view = sender as DataGridView;
            if (view.SelectedRows.Count == 0)
                return;

            int checkIndex = GetCheckBoxCellIndex();

            if (m_options.ShowCheckBoxes && checkIndex != -1 && view.SelectedRows[0].Cells[checkIndex] is DataGridViewCheckBoxCell checkBoxCell && 
                checkBoxCell.Value != null)
            {
                bool set = !(bool)checkBoxCell.Value;
                checkBoxCell.Value = set;

                if (set)
                    m_checkedTags.Add(view.SelectedRows[0].DataBoundItem as ITagData);
                else
                    m_checkedTags.Remove(view.SelectedRows[0].DataBoundItem as ITagData);
            }

            if (dgvTags.SelectedRows[0].DataBoundItem is StaticTagData staticTag)
                StaticSelectionChanged?.Invoke(this, staticTag.Name);
            else
                TagSelectionChanged?.Invoke(this, view.SelectedRows[0].DataBoundItem as ITagData);

            if (!m_options.AllowRowSelect)
                view.ClearSelection();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SetTags();
        }

        public void ClearSelections() => dgvTags.ClearSelection();
        private void DisableSelection() => dgvTags.SelectionChanged -= View_SelectionChanged;

        private void EnableSelection()
        {
            if (m_loaded)
                dgvTags.SelectionChanged += View_SelectionChanged;
        }

        private void btnPin_Click(object sender, EventArgs e)
        {
            SetPinned(!Pinned);
            PinChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetPinned(bool pinned)
        {
            Pinned = pinned;

            DpiScale dpiScale = new DpiScale(CreateGraphics());
            Image img = Icons.Pin;
            if (!Pinned)
                img = img.Rotate(90);

            btnPin.Image = img;
            btnPin.Image = img.FixedSize((int)(img.Width * .8), (int)(img.Height * .8), Color.Transparent);
            btnPin.Width = img.Width + dpiScale.ScaleIntX(2);
            btnPin.Height = img.Height + dpiScale.ScaleIntY(4);
        }

        private void manageTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageTags?.Invoke(this, EventArgs.Empty);
        }
    }

    public class TagSelectOptions
    {
        public bool ShowStatic { get; set; }
        public bool HasTabOnly { get; set; }
        public bool ShowCheckBoxes { get; set; }
        public bool AllowRowSelect { get; set; }
        public bool ShowPin { get; set; }
        public bool ShowTagData { get; set; }
        public bool CustomSetData { get; set; }
        public bool ShowMenu { get; set; }
    }
}
