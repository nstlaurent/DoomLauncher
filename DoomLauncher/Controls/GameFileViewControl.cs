using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Equin.ApplicationFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GameFileViewControl : UserControl, IGameFileColumnView
    {
        public event EventHandler ItemClick;
        public event EventHandler ItemDoubleClick;
        public event EventHandler SelectionChange;
        public event CancelEventHandler CustomRowPaint;
        public event KeyPressEventHandler ViewKeyPress;
        public event KeyEventHandler ViewKeyDown;
        public event GameFileEventHandler GameFileEnter;
        public event GameFileEventHandler GameFileLeave;

        private readonly Label m_label = new Label();
        private readonly Dictionary<int, PropertyInfo> m_properties = new Dictionary<int, PropertyInfo>();
        private BindingListView<IGameFile> m_datasource;
        private bool m_binding = false;

        public GameFileViewControl()
        {
            InitializeComponent();

            SetupGridView();

            StyleDisplayLabel(m_label);
            Controls.Add(m_label);
            dgvMain.KeyDown += dgvMain_KeyDown;
        }

        public static void StyleGrid(DataGridView view)
        {
            view.RowsDefaultCellStyle.ForeColor = SystemColors.WindowText;
            view.AlternatingRowsDefaultCellStyle.ForeColor = SystemColors.WindowText;
            view.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;

            if (SystemInformation.HighContrast)
            {
                view.AlternatingRowsDefaultCellStyle.BackColor = view.DefaultCellStyle.BackColor;
            }
            else
            {
                Color color = view.DefaultCellStyle.BackColor;
                view.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(color.R - 32, color.G - 32, color.B - 32);
            }

            view.DefaultCellStyle.NullValue = "N/A";
            view.RowHeadersVisible = false;
            view.AutoGenerateColumns = false;
            view.ShowCellToolTips = false;
            view.BackgroundColor = SystemColors.Window;
        }

        public static void StyleDisplayLabel(Label label)
        {
            label.AutoSize = true;
            label.Visible = false;
            label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            label.Margin = new Padding(12);
            label.Font = new Font(label.Font.FontFamily, 12.0f, FontStyle.Bold);
        }

        public void SetVisible(bool set)
        {
            // Not required
        }

        public bool MultiSelect
        {
            get { return dgvMain.MultiSelect; }
            set { dgvMain.MultiSelect = value; }
        }

        public void SetDisplayText(string text)
        {
            m_label.Text = text;
            m_label.Visible = true;
            BorderStyle = BorderStyle.FixedSingle;

            Controls.Remove(dgvMain);
        }

        public void SetContextMenuStrip(ContextMenuStrip menu)
        {
            dgvMain.ContextMenuStrip = menu;
        }

        public void SetColumnFields(IEnumerable<ColumnField> columnFields)
        {
            bool resetDataSource = dgvMain.Columns.Count > 0;
            m_orderLookup.Clear();
            ColumnFields = columnFields.ToArray();
            dgvMain.Columns.Clear();

            if (columnFields.Any())
            {            
                foreach (var item in columnFields)
                {
                    DataGridViewColumn col = new DataGridViewTextBoxColumn
                    {
                        HeaderText = item.Title,
                        Name = item.DataKey,
                        DataPropertyName = item.DataKey
                    };
                    dgvMain.Columns.Add(col);
                    m_orderLookup.Add(item.DataKey.ToLower(), col);
                }
                
                dgvMain.Columns[dgvMain.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (resetDataSource) //need to reset datasource so columns show in correct order
                    SetDataSource(m_datasource);
            }
        }

        public void SetColumnFormat(string colName, string format)
        {
            if (dgvMain.Columns[colName] != null)
                dgvMain.Columns[colName].DefaultCellStyle.Format = format;
        }

        public List<Tuple<string, string>> GetColumnFormats()
        {
            List<Tuple<string, string>> formats = new List<Tuple<string, string>>();

            foreach (DataGridViewColumn col in dgvMain.Columns)
            {
                if (!string.IsNullOrEmpty(col.DefaultCellStyle.Format))
                    formats.Add(new Tuple<string, string>(col.Name, col.DefaultCellStyle.Format));
            }

            return formats;
        }

        public IEnumerable<IGameFile> DataSource
        {
            get
            {
                if (m_datasource != null)
                    return m_datasource;
                else
                    return new IGameFile[] { };
            }
            set
            {
                if (value != null)
                    SetDataSource(new BindingListView<IGameFile>(value.ToList()));
                else
                    SetDataSource(new BindingListView<IGameFile>(new GameFile[] { }));
            }
        }

        private void SetDataSource(object datasource)
        {
            m_binding = true;
            m_datasource = (BindingListView<IGameFile>)datasource;

            if (m_datasource == null)
            {
                dgvMain.RowCount = 0;
            }
            else
            {
                dgvMain.RowCount = m_datasource.Count;
                DataGridViewColumn col = dgvMain.Columns.Cast<DataGridViewColumn>()
                    .FirstOrDefault(x => x.HeaderCell.SortGlyphDirection != SortOrder.None);
                if (col != null)
                    HandleSort(col.Index, false);
            }

            if (!Controls.Contains(dgvMain))
            {
                Controls.Add(dgvMain);
                m_label.Visible = false;
                BorderStyle = BorderStyle.None;
            }

            Refresh();

            m_binding = false;
        }

        public void UpdateGameFile(IGameFile gameFile)
        {
            // Grid view inherently supports this
        }

        public IGameFile SelectedItem
        {
            get
            {
                if (dgvMain.SelectedRows.Count > 0)
                {
                    DataGridViewRow dgvr = dgvMain.SelectedRows[0];
                    return m_datasource[dgvr.Index].Object;
                }

                return null;
            }
            set
            {
                dgvMain.ClearSelection();
                int rowIndex = 0;

                foreach (DataGridViewRow dgvr in dgvMain.Rows)
                {
                    if (m_datasource[dgvr.Index].Equals(value))
                    {
                        dgvr.Selected = true;
                        dgvr.Cells[0].Selected = true;
                        dgvMain.FirstDisplayedScrollingRowIndex = rowIndex;
                        break;
                    }

                    rowIndex++;
                }
            }
        }

        public IGameFile[] SelectedItems
        {
            get
            {
                if (m_datasource != null && dgvMain.SelectedRows.Count > 0)
                {
                    List<IGameFile> ret = new List<IGameFile>(dgvMain.Rows.Count);
                    
                    foreach (DataGridViewRow dgvr in dgvMain.SelectedRows)
                    {
                        if (m_datasource.Count > dgvr.Index)
                            ret.Add(m_datasource[dgvr.Index].Object);
                    }

                    return ret.ToArray();
                }

                return new IGameFile[] { };
            }
        }

        public IGameFile GameFileForIndex(int index)
        {
            if (index > -1 && index < m_datasource.Count)
                return m_datasource[index].Object;

            return null;
        }

        public void RefreshData()
        {
            Refresh();
        }

        private void SetupGridView()
        {
            StyleGrid(dgvMain);

            dgvMain.SelectionChanged += dgvMain_SelectionChanged;
            dgvMain.CellClick += dgvMain_CellClick;
            dgvMain.ColumnDisplayIndexChanged += dgvMain_ColumnDisplayIndexChanged;
            
            dgvMain.VirtualMode = true;
            dgvMain.CellValueNeeded += dgvMain_CellValueNeeded;

            dgvMain.ColumnHeaderMouseClick += dgvMain_ColumnHeaderMouseClick;

            dgvMain.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvMain.AllowUserToResizeRows = false;
        }

        private void dgvMain_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            HandleSort(e.ColumnIndex, true);
        }

        private void HandleSort(int columnIndex, bool flip)
        {
            if (m_datasource != null)
            {
                DataGridViewColumn dgvcSet = dgvMain.Columns[columnIndex];

                foreach (DataGridViewColumn dgvc in dgvMain.Columns)
                {
                    if (dgvc != dgvcSet)
                        dgvc.HeaderCell.SortGlyphDirection = SortOrder.None;
                }

                if (flip)
                {
                    if (dgvcSet.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                        dgvcSet.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    else
                        dgvcSet.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                }

                string sortOrder;
                if (dgvcSet.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                    sortOrder = "ASC";
                else
                    sortOrder = "DESC";

                m_datasource.ApplySort(string.Concat(dgvcSet.Name, " ", sortOrder));
                dgvMain.Invalidate();
            }
        }

        void dgvMain_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (m_datasource != null && m_datasource.Count > e.RowIndex)
            {
                IGameFile gameFile = m_datasource[e.RowIndex].Object;

                if (!m_properties.ContainsKey(e.ColumnIndex))
                    m_properties.Add(e.ColumnIndex, gameFile.GetType().GetProperty(dgvMain.Columns[e.ColumnIndex].DataPropertyName));

                e.Value = m_properties[e.ColumnIndex].GetValue(gameFile);
            }
        }

        void dgvMain_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ColumnReorderIndex = e.Column.DisplayIndex;
        }

        void dgvMain_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ItemClick?.Invoke(this, EventArgs.Empty);
            HandleSelection();
        }

        void dgvMain_SelectionChanged(object sender, EventArgs e)
        {
            HandleSelection();
        }

        private void HandleSelection()
        {
            if (m_setting) return;
            SelectionChange?.Invoke(this, new EventArgs());
        }

        private bool m_setting = false;

        private void dgvMain_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && e.Button == MouseButtons.Right && dgvMain.SelectedRows.Count < 2)
            {
                m_setting = true;
                dgvMain.ClearSelection();
                dgvMain.Rows[e.RowIndex].Selected = true;
                dgvMain.Rows[e.RowIndex].Cells[0].Selected = true;

                SelectionChange?.Invoke(this, new EventArgs());
                m_setting = false;
            }
        }

        private IGameFile m_lastEnter;

        private void dgvMain_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (m_binding)
                return;

            var gameFile = GameFileForIndex(e.RowIndex);
            if (gameFile != m_lastEnter)
            {
                m_lastEnter = gameFile;
                GameFileEnter?.Invoke(this, new GameFileEventArgs(gameFile));
            }
        }

        private void dgvMain_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            GameFileLeave?.Invoke(this, new GameFileEventArgs(m_lastEnter));
            m_lastEnter = null;
        }

        private void dgvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ItemDoubleClick?.Invoke(this, new EventArgs());
        }

        public void SetColumnWidth(string key, int width)
        {
            if (width <= 0)
                return;

            key = key.ToLower();
            m_orderLookup[key].Width = width;
        }

        public int GetColumnWidth(string key)
        {
            key = key.ToLower();
            return m_orderLookup[key].Width;
        }

        public SortOrder GetColumnSort(string key)
        {
            foreach(DataGridViewColumn column in dgvMain.Columns)
            {
                if (column.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    return column.HeaderCell.SortGlyphDirection;
            }
            return SortOrder.None;
        }

        public string GetSortedColumnKey()
        {
            foreach (DataGridViewColumn column in dgvMain.Columns)
            {
                if (column.HeaderCell.SortGlyphDirection != SortOrder.None)
                    return column.Name;
            }

            return string.Empty;
        }

        public string[] GetColumnKeyOrder()
        {
            string[] items = new string[m_orderLookup.Count];

            int count = 0;
            foreach (KeyValuePair<string, DataGridViewColumn> item in m_orderLookup)
            {
                items[count] = m_orderLookup.First(x => x.Value.DisplayIndex == count).Key;
                count++;
            }

            return items;
        }

        public void SetSortedColumn(string column, SortOrder sort)
        {
            DataGridViewColumn col = dgvMain.Columns.Cast<DataGridViewColumn>()
                .FirstOrDefault(x => x.Name.Equals(column, StringComparison.CurrentCultureIgnoreCase));

            if (col != null)
            {
                col.HeaderCell.SortGlyphDirection = sort;
                if (m_datasource != null)
                    HandleSort(col.Index, false);
            }
        }

        private void dgvMain_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (CustomRowColorPaint && CustomRowPaint != null && !m_binding)
            {
                CustomRowPaintDataBoundItem = GameFileForIndex(e.RowIndex);

                CancelEventArgs args = new CancelEventArgs();
                CustomRowPaint(this, args);

                if (!args.Cancel)
                    dgvMain.Rows[e.RowIndex].DefaultCellStyle.ForeColor = CustomRowPaintForeColor;
            }
        }

        public ColumnField[] ColumnFields { get; private set; }
        public int ToolTipItemIndex { get; private set; }
        public int ColumnReorderIndex { get; private set; }

        public bool CustomRowColorPaint { get; set; }
        public IGameFile CustomRowPaintDataBoundItem { get; private set; }
        public Color CustomRowPaintForeColor { get; set; }

        public object DoomLauncherParent { get; set; }

        private Dictionary<string, DataGridViewColumn> m_orderLookup = new Dictionary<string, DataGridViewColumn>();

        private void dgvMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            ViewKeyPress?.Invoke(this, e);
        }

        private void dgvMain_KeyDown(object sender, KeyEventArgs e)
        {
            ViewKeyDown?.Invoke(this, e);
        }
    }
}
