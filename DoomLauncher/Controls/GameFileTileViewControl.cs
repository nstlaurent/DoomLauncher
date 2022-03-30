using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class GameFileTileViewControl : UserControl, IGameFileView, IGameFileSortableView
    {
        public event EventHandler ItemClick;
        public event EventHandler ItemDoubleClick;
        public event EventHandler SelectionChange;
        public event KeyPressEventHandler ViewKeyPress;
        public event KeyEventHandler ViewKeyDown;
        public event GameFileEventHandler GameFileEnter;
        public event GameFileEventHandler GameFileLeave;

        private readonly int LayoutRow = 0;

        private List<IGameFile> m_gameFiles = new List<IGameFile>();
        private readonly List<GameFileTileBase> m_selectedTiles = new List<GameFileTileBase>();
        private readonly Label m_label = new Label();

        private ContextMenuStrip m_menu;
        private bool m_visible;
        private bool m_menuShowing;
        private bool m_tilesRecreated;

        private readonly System.Timers.Timer m_mouseTimer;
        private int m_lastScrollPos;
        private GameFileTileBase m_lastHover;

        private string m_sortedColumn;
        private SortOrder m_sortedColumnOrder;

        private FlowLayoutPanelDB flpMain = new FlowLayoutPanelDB();

        public GameFileTileViewControl()
        {
            InitializeComponent();

            BackColor = ColorTheme.Current.Window;

            SetItemsPerPage(GameFileTileManager.Instance.MaxItems);
            pagingControl.PageIndexChanged += M_pagingControl_PageIndexChanged;

            m_mouseTimer = new System.Timers.Timer();
            m_mouseTimer.Interval = 100;
            m_mouseTimer.Elapsed += MouseTimer_Elapsed;
            m_mouseTimer.Start();

            m_label.Visible = false;
            GameFileViewControl.StyleDisplayLabel(m_label);

            cmbMaxItemsPerPage.KeyDown += CmbMaxItemsPerPage_KeyDown;

            GameFileTileManager.Instance.TilesRecreated += Instance_TilesRecreated;
            InitTiles();

            Stylizer.StylizeControl(this, DesignMode);
        }

        private void SetItemsPerPage(int maxItems)
        {
            cmbMaxItemsPerPage.SelectedIndexChanged -= CmbMaxItemsPerPage_SelectedIndexChanged;
            cmbMaxItemsPerPage.SelectedItem = maxItems.ToString();
            if (cmbMaxItemsPerPage.SelectedIndex == -1)
                cmbMaxItemsPerPage.SelectedIndex = 0;
            cmbMaxItemsPerPage.SelectedIndexChanged += CmbMaxItemsPerPage_SelectedIndexChanged;
        }

        public IEnumerable<IGameFile> DataSource
        {
            get => m_gameFiles;
            set => SetData(value);
        }

        public IGameFile SelectedItem
        {
            get
            {
                if (m_selectedTiles.Count > 0)
                    return m_selectedTiles[0].GameFile;
                return null;
            }
            set
            {
                SelectGameFile(value);
            }
        }

        public IGameFile[] SelectedItems => m_selectedTiles.Select(x => x.GameFile).ToArray();

        public bool MultiSelect { get; set; }
        public object DoomLauncherParent { get; set; }

        public void SetVisible(bool set)
        {
            m_visible = set;

            if (set)
            {
                flpMain = GameFileTileManager.Instance.TileLayout;
                flpMain.TabIndex = 1;
                flpMain.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                flpMain.Dock = DockStyle.Fill;

                if (m_label.Visible)
                {
                    // Data in the view has changed since last displayed
                    if (m_gameFiles.Count > 0)
                        ClearDisplayLabel();
                }
                else
                {
                    tblMain.Controls.Add(flpMain, 0, LayoutRow);
                }

                flpMain.BackColor = ColorTheme.Current.Window;
                flpMain.KeyPress += GameFileTileViewControl_KeyPress;
                flpMain.KeyDown += GameFileTileViewControl_KeyDown;

                SetPageData(pagingControl.PageIndex, false);
                SetSavedScrollValue();
                flpMain.PerformLayout();

                SetDefaultSelection();
            }
            else
            {
                flpMain.KeyPress -= GameFileTileViewControl_KeyPress;
                flpMain.KeyDown -= GameFileTileViewControl_KeyDown;

                m_selectedTiles.ForEach(x => x.SetSelected(false));
                m_lastScrollPos = flpMain.VerticalScroll.Value;
                Controls.Remove(flpMain);
            }
        }

        private void SetSavedScrollValue()
        {
            if (m_lastScrollPos < flpMain.VerticalScroll.Minimum)
                flpMain.VerticalScroll.Value = flpMain.VerticalScroll.Minimum;
            else if (m_lastScrollPos > flpMain.VerticalScroll.Maximum)
                flpMain.VerticalScroll.Value = flpMain.VerticalScroll.Maximum;
            else
                flpMain.VerticalScroll.Value = m_lastScrollPos;
        }

        public string GetSortedColumnKey()
        {
            return m_sortedColumn;
        }

        public SortOrder GetColumnSort(string key)
        {
            if (m_sortedColumn == key)
                return m_sortedColumnOrder;

            return SortOrder.None;
        }

        public void SetSortedColumn(string key, SortOrder sort)
        {
            m_sortedColumn = key;
            m_sortedColumnOrder = sort;
        }

        private void SetDefaultSelection()
        {
            if (SelectedItem == null && m_gameFiles.Count > 0)
            {
                int index = pagingControl.PageIndex * GameFileTileManager.Instance.MaxItems;
                if (index < m_gameFiles.Count)
                {
                    SelectGameFile(m_gameFiles[index]);
                    return;
                }
            }

            SelectionChange?.Invoke(this, EventArgs.Empty);
        }

        private void InitTiles()
        {
            m_lastScrollPos = 0;

            foreach (var tile in GameFileTileManager.Instance.OldTiles)
            {
                tile.TileClick -= Tile_TileClick;
                tile.TileDoubleClick -= Tile_TileDoubleClick;
            }

            foreach (var tile in GameFileTileManager.Instance.Tiles)
            {
                tile.TileClick += Tile_TileClick;
                tile.TileDoubleClick += Tile_TileDoubleClick;
            }
        }

        private void CmbMaxItemsPerPage_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void Instance_TilesRecreated(object sender, EventArgs e)
        {
            m_tilesRecreated = true;
            pagingControl.Init(m_gameFiles.Count, GameFileTileManager.Instance.MaxItems);

            SetItemsPerPage(GameFileTileManager.Instance.MaxItems);

            InitTiles();
            RefreshData();
            SetDefaultSelection();
        }

        private void M_pagingControl_PageIndexChanged(object sender, EventArgs e)
        {
            SetPageData(pagingControl.PageIndex, true);
            SetDefaultSelection();
        }

        private void MouseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!IsDisposed && InvokeRequired)
                    Invoke(new Action(MouseMoveTimer));
            }
            catch (ObjectDisposedException)
            {
                m_mouseTimer.Stop();
            }
        }

        private void MouseMoveTimer()
        {
            if (!m_visible || m_menuShowing)
                return;

            // PointToClient will return null if the view is obscured at the current position
            Point pt = Cursor.Position;
            Control test = tblMain.GetChildAtPoint(tblMain.PointToClient(pt));
            if (test != null && test == flpMain)
            {
                flpMain.Focus();

                foreach (var tile in GameFileTileManager.Instance.Tiles)
                {
                    if (!tile.Visible)
                        continue;

                    var rect = tile.RectangleToScreen(tile.DisplayRectangle);
                    if (rect.Contains(pt))
                    {
                        if (m_lastHover != tile && tile.IsVisibleAtPoint(pt))
                        {
                            m_lastHover = tile;
                            GameFileEnter?.Invoke(this, new GameFileEventArgs(tile.GameFile));
                        }
                        return;
                    }
                }
            }

            ClearHover();
        }

        private void GameFileTileViewControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_selectedTiles.Count > 0 && (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
            {
                int baseIndex = pagingControl.PageIndex * GameFileTileManager.Instance.MaxItems;
                int originalIndex = GameFileTileIndex(m_selectedTiles[0]) + baseIndex;

                int newIndex = GetNextTileIndex(originalIndex, e.KeyCode);
                if (newIndex == originalIndex)
                    return;
                   
                if (newIndex >= baseIndex && newIndex < baseIndex + GameFileTileManager.Instance.MaxItems && newIndex < m_gameFiles.Count)
                {
                    ClearSelection();
                    SelectGameFile(m_gameFiles[newIndex]);
                }
            }

            ViewKeyDown?.Invoke(this, e);
        }

        private int GetNextTileIndex(int index, Keys keycode)
        {
            GameFileTileBase tile = GameFileTileManager.Instance.Tiles[index];
            int baseIndex = pagingControl.PageIndex * GameFileTileManager.Instance.MaxItems;

            int startIndex;
            int endIndex;

            if (keycode == Keys.Down || keycode == Keys.Right)
            {
                startIndex = index;
                endIndex = baseIndex + GameFileTileManager.Instance.MaxItems;
            }
            else if (keycode == Keys.Up || keycode == Keys.Left)
            {
                startIndex = index;
                endIndex = baseIndex - 1;
            }
            else
            {
                return index;
            }

            while (startIndex != endIndex)
            {
                GameFileTileBase nextTile = GameFileTileManager.Instance.Tiles[startIndex];

                if (keycode == Keys.Down && nextTile.Location.Y > tile.Location.Y && nextTile.Location.X == tile.Location.X)
                    return startIndex;
                else if (keycode == Keys.Up && nextTile.Location.Y < tile.Location.Y && nextTile.Location.X == tile.Location.X)
                    return startIndex;
                else if (keycode == Keys.Right && nextTile.Location.X > tile.Location.X)
                    return startIndex;
                else if (keycode == Keys.Left && nextTile.Location.X < tile.Location.X)
                    return startIndex;

                if (keycode == Keys.Down || keycode == Keys.Right)
                    startIndex++;
                else
                    startIndex--;
            }

            if (keycode == Keys.Left && index - 1 > startIndex)
                return index - 1;
            else if (keycode == Keys.Right && index + 1 < endIndex)
                return index + 1;

            return index;
        }

        private void GameFileTileViewControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            ViewKeyPress?.Invoke(this, e);
        }

        public IGameFile GameFileForIndex(int index)
        {
            if (index >= m_gameFiles.Count)
                return null;

            return m_gameFiles[index];
        }

        public void UpdateGameFile(IGameFile gameFile)
        {
            foreach (var tile in GameFileTileManager.Instance.Tiles)
            {
                if (ShouldSkipTile(tile))
                    break;

                if (tile.GameFile.Equals(gameFile))
                {
                    var screenshots = DataCache.Instance.DataSourceAdapter.GetFiles(FileType.Screenshot);
                    var thumbnails = DataCache.Instance.DataSourceAdapter.GetFiles(FileType.Thumbnail);
                    SetTileData(tile, gameFile, screenshots, thumbnails, DataCache.Instance.TagMapLookup.GetTags(tile.GameFile), true);
                }
            }
        }

        public void RefreshData()
        {
            if (!m_visible)
                return;

            if (m_tilesRecreated)
            {
                m_tilesRecreated = false;
                SetPageData(pagingControl.PageIndex, true);
                return;
            }

            var screenshots = DataCache.Instance.DataSourceAdapter.GetFiles(FileType.Screenshot);
            var thumbnails = DataCache.Instance.DataSourceAdapter.GetFiles(FileType.Thumbnail);

            foreach (var tile in GameFileTileManager.Instance.Tiles)
            {
                if (ShouldSkipTile(tile))
                    break;

                SetTileData(tile, tile.GameFile, screenshots, thumbnails, DataCache.Instance.TagMapLookup.GetTags(tile.GameFile), false);
            }
        }

        private static bool ShouldSkipTile(GameFileTileBase tile) => !tile.Visible || tile.GameFile == null;

        public void SetContextMenuStrip(ContextMenuStrip menu)
        {
            m_menu = menu;
            m_menu.Opened += M_menu_Opened;
            m_menu.Closed += M_menu_Closed;
        }

        public void SetDisplayText(string text)
        {
            tblMain.Controls.Remove(flpMain);
            tblMain.Controls.Add(m_label, 0, LayoutRow);
            m_label.Text = text;
            m_label.Visible = true;
        }

        private void SetData(IEnumerable<IGameFile> gameFiles)
        {
            ClearHover();
            ClearDisplayLabel();

            bool update = false;
            int saveIndex = pagingControl.PageIndex;

            if (gameFiles == null)
            {
                m_gameFiles.Clear();
            }
            else
            {
                var diff = m_gameFiles.Except(gameFiles);
                // Basically a hack for when deleting a single item to keep the current page
                update = diff.Count() == 1;
                m_gameFiles = gameFiles.ToList();
            }

            pagingControl.Init(m_gameFiles.Count, GameFileTileManager.Instance.MaxItems);

            if (!m_visible)
                return;

            if (update)
            {
                if (!pagingControl.SetPageIndex(saveIndex))
                    SetPageData(saveIndex, false);
                SetDefaultSelection();
            }
            else
            {
                SetPageData(0, true);
                SetDefaultSelection();
            }
        }

        private void ClearDisplayLabel()
        {
            if (m_label.Visible)
            {
                m_label.Visible = false;
                tblMain.Controls.Remove(m_label);
                tblMain.Controls.Add(GameFileTileManager.Instance.TileLayout, 0, LayoutRow);
            }
        }

        private void ClearHover()
        {
            if (m_lastHover != null)
                GameFileLeave?.Invoke(this, new GameFileEventArgs(m_lastHover.GameFile));

            m_lastHover = null;
        }

        private void SetPageData(int pageIndex, bool dataChange)
        {
            GameFileTileManager.Instance.Tiles.ForEach(x => x.SetSelected(false));

            var screenshots = DataCache.Instance.DataSourceAdapter.GetFiles(FileType.Screenshot);
            var thumbnails = DataCache.Instance.DataSourceAdapter.GetFiles(FileType.Thumbnail);

            var gameFiles = m_gameFiles.Skip(pageIndex * GameFileTileManager.Instance.MaxItems).Take(GameFileTileManager.Instance.MaxItems).ToList();
            SetLayout(gameFiles, screenshots, thumbnails);

            if (dataChange)
            {
                ClearSelection(false);
                flpMain.VerticalScroll.Value = flpMain.VerticalScroll.Minimum;
                flpMain.PerformLayout();
            }
            else
            {
                m_selectedTiles.ForEach(x => x.SetSelected(true));
            }
        }

        private void SetLayout(List<IGameFile> gameFiles, IEnumerable<IFileData> screenshots, IEnumerable<IFileData> thumbnails)
        {
            flpMain.SuspendLayout();
            int tileIndex = 0;

            foreach (var gameFile in gameFiles)
            {
                SetTileData(GameFileTileManager.Instance.Tiles[tileIndex], gameFile, screenshots, thumbnails, DataCache.Instance.TagMapLookup.GetTags(gameFile), false);
                GameFileTileManager.Instance.Tiles[tileIndex].Visible = true;
                tileIndex++;
            }

            for (int i = gameFiles.Count; i < GameFileTileManager.Instance.Tiles.Count; i++)
            {
                GameFileTileManager.Instance.Tiles[i].ClearData();
                GameFileTileManager.Instance.Tiles[i].Visible = false;
            }

            flpMain.ResumeLayout();
        }

        private static void SetTileData(GameFileTileBase tile, IGameFile gameFile, IEnumerable<IFileData> screenshots, IEnumerable<IFileData> thumbnails, IEnumerable<ITagData> tags, bool forceRefresh)
        {
            if (gameFile == null || (!forceRefresh && gameFile.Equals(tile.GameFile)))
                return;

            tile.SetData(gameFile, tags);

            if (!gameFile.GameFileID.HasValue)
            {
                tile.SetImage(DataCache.Instance.DefaultImage);
                return;
            }

            IFileData thumbnail = ThumbnailManager.GetOrCreateThumbnail(gameFile, screenshots, thumbnails);

            if (thumbnail != null)
                tile.SetImageLocation(Path.Combine(DataCache.Instance.AppConfiguration.ThumbnailDirectory.GetFullPath(), thumbnail.FileName));
            else
                tile.SetImage(DataCache.Instance.DefaultImage);
        }

        private void M_menu_Opened(object sender, EventArgs e)
        {
            m_menuShowing = true;
        }

        private void M_menu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            m_menuShowing = false;
        }

        private void CmbMaxItemsPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameFileTileManager.Instance.SetMaxItems(Convert.ToInt32(cmbMaxItemsPerPage.SelectedItem));
        }

        private void Tile_TileDoubleClick(object sender, EventArgs e)
        {
            if (!m_visible)
                return;

            ItemDoubleClick?.Invoke(this, e);
        }

        private void Tile_TileClick(object sender, MouseEventArgs e)
        {
            if (!(sender is GameFileTileBase tile) || !m_visible)
                return;

            ItemClick?.Invoke(this, EventArgs.Empty);

            if (e.Button == MouseButtons.Left)
            {
                if ((ModifierKeys & Keys.Control) == Keys.Control)
                    ToggleSelection(tile);
                else if ((ModifierKeys & Keys.Shift) == Keys.Shift && m_selectedTiles.Count > 0)
                    SelectRange(tile);
                else
                    SelectGameFile(tile.GameFile);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (!m_selectedTiles.Contains(tile))
                    SelectGameFile(tile.GameFile);
                Point pt =  PointToScreen(new Point(Location.X + tile.Location.X + e.X, Location.Y + tile.Location.Y + e.Y));
                m_menu.Show(pt);
            }
        }

        private void SelectRange(GameFileTileBase tile)
        {
            int start = GameFileTileIndex(m_selectedTiles.First());
            int end = GameFileTileIndex(m_selectedTiles.Last());

            if (start > end)
            {
                int temp = start;
                end = start;
                start = temp;
            }

            int index = GameFileTileIndex(tile);

            if (index < start)
                start = index;
            if (index > end)
                end = index;

            ClearSelection(false);

            for (int i = start; i <= end; i++)
            {
                m_selectedTiles.Add(GameFileTileManager.Instance.Tiles[i]);
                GameFileTileManager.Instance.Tiles[i].SetSelected(true);
            }

            SelectionChange?.Invoke(this, EventArgs.Empty);
        }

        private int GameFileTileIndex(GameFileTileBase tile)
        {
            for (int i = 0; i < GameFileTileManager.Instance.Tiles.Count; i++)
            {
                if (tile == GameFileTileManager.Instance.Tiles[i])
                    return i;
            }

            return -1;
        }

        private void ToggleSelection(GameFileTileBase tile)
        {
            tile.SetSelected(!tile.Selected);
            if (tile.Selected)
            {
                m_selectedTiles.Add(tile);
            }
            else
            {
                m_selectedTiles.Remove(tile);
            }
        }

        private void SelectGameFile(IGameFile gameFile)
        {
            ClearSelection(false);

            if (gameFile == null)
                return;

            foreach (GameFileTileBase tile in GameFileTileManager.Instance.Tiles)
            {
                if (ShouldSkipTile(tile))
                    break;

                if (tile.GameFile.Equals(gameFile))
                {
                    tile.SetSelected(true);
                    m_selectedTiles.Add(tile);
                    break;
                }
            }

            if (m_selectedTiles.Count > 0)
            {
                flpMain.ScrollControlIntoView(m_selectedTiles[0]);
                SelectionChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ClearSelection(bool fireEvent = true)
        {
            if (m_selectedTiles.Count == 0)
                return;

            m_selectedTiles.ForEach(x => x.SetSelected(false));
            m_selectedTiles.Clear();
            if (fireEvent)
                SelectionChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
