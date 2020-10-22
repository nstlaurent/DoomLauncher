using System;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
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

        public bool Pinned { get; private set; }

        private TagSelectOptions m_options = new TagSelectOptions();
        private List<ITagData> m_checkedTags = new List<ITagData>();
        private bool m_loaded;

        public TagSelectControl()
        {
            InitializeComponent();

            btnSearch.Image = Icons.Search;
            btnPin.Image = Icons.Pin;

            SetPinned(false);
            Load += TagSelectControl_Load;
        }

        private void TagSelectControl_Load(object sender, EventArgs e)
        {
            m_loaded = true;
            ClearSelections();
            EnableSelection();
            SetCheckedTags();
        }

        public void Init(TagSelectOptions options)
        {
            m_options = options;

            btnPin.Visible = options.ShowPin;

            InitGrid(dgvCustom);

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

            dgvCustom.DataSource = allTags;

            SetCheckedTags();

            ClearSelections();
            EnableSelection();
        }

        private List<ITagData> GetStaticTags()
        {
            List<ITagData> tags = new List<ITagData>();

            foreach (var key in TabKeys.KeyNames)
                tags.Add(new StaticTagData() { Name = key, Favorite = true });

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

            view.ColumnHeadersVisible = false;
            view.MultiSelect = false;
            view.AllowUserToResizeColumns = false;
            view.AllowUserToResizeRows = false;
            view.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (m_options.ShowCheckBoxes)
                view.Columns.Add(new DataGridViewCheckBoxColumn() { ReadOnly = false, Width = dpiScale.ScaleIntX(32) });

            view.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = string.Empty,
                Name = nameof(ITagData.Name),
                DataPropertyName = nameof(ITagData.FavoriteName)
            });

            view.Columns[view.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void View_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView view = sender as DataGridView;
            if (view.SelectedRows.Count == 0)
                return;

            if (m_options.ShowCheckBoxes && view.SelectedRows[0].Cells[0] is DataGridViewCheckBoxCell checkBoxCell && checkBoxCell.Value != null)
            {
                bool set = !(bool)checkBoxCell.Value;
                checkBoxCell.Value = set;

                if (set)
                    m_checkedTags.Add(view.SelectedRows[0].DataBoundItem as ITagData);
                else
                    m_checkedTags.Remove(view.SelectedRows[0].DataBoundItem as ITagData);
            }

            if (dgvCustom.SelectedRows[0].DataBoundItem is StaticTagData staticTag)
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

        public void ClearSelections() => dgvCustom.ClearSelection();
        private void DisableSelection() => dgvCustom.SelectionChanged -= View_SelectionChanged;

        private void EnableSelection()
        {
            if (m_loaded)
                dgvCustom.SelectionChanged += View_SelectionChanged;
        }

        private void btnPin_Click(object sender, EventArgs e)
        {
            SetPinned(!Pinned);
            PinChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetPinned(bool pinned)
        {
            Pinned = pinned;

            Image img = Icons.Pin;
            if (!Pinned)
                img = Util.RotateImage(img, 90);

            btnPin.Image = img;
            btnPin.Image = ThumbnailManager.FixedSize(img, (int)(img.Width * .8), (int)(img.Height * .8), Color.Transparent);
            btnPin.Width = img.Width + 2;
            btnPin.Height = img.Height + 4;
        }
    }

    public class TagSelectOptions
    {
        public bool ShowStatic { get; set; }
        public bool HasTabOnly { get; set; }
        public bool ShowCheckBoxes { get; set; }
        public bool AllowRowSelect { get; set; }
        public bool ShowPin { get; set; }
    }
}
