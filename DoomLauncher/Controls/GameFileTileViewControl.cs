using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public partial class GameFileTileViewControl : UserControl, IGameFileView
    {
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
        public Control ToolTipControl => flpMain;

        public event EventHandler ItemDoubleClick;
        public event EventHandler SelectionChange;
        public event KeyPressEventHandler ViewKeyPress;
        public event KeyEventHandler ViewKeyDown;
        public event GameFileEventHandler GameFileEnter;
        public event GameFileEventHandler GameFileLeave;

        private List<IGameFile> m_gameFiles = new List<IGameFile>();

        private List<GameFileTile> m_tiles = new List<GameFileTile>();
        private List<GameFileTile> m_selectedTiles = new List<GameFileTile>();

        private HashSet<IGameFile> m_cachedTiles = new HashSet<IGameFile>();
        private ContextMenuStrip m_menu;

        private ToolTipDisplayHandler m_toolTipDisplayHandler;

        private readonly System.Timers.Timer m_mouseTimer;

        public GameFileTileViewControl()
        {
            InitializeComponent();

            m_toolTipDisplayHandler = new ToolTipDisplayHandler(this, toolTip1);
            
            flpMain.AutoScroll = true;
            flpMain.Click += GameFileTileViewControl_Click;
            flpMain.KeyPress += GameFileTileViewControl_KeyPress;
            flpMain.KeyDown += GameFileTileViewControl_KeyDown;

            m_mouseTimer = new System.Timers.Timer();
            m_mouseTimer.Interval = 100;
            m_mouseTimer.Elapsed += MouseTimer_Elapsed;
            m_mouseTimer.Start();
        }

        static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        private void MouseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(MouseMoveTimer));
        }

        private GameFileTile m_lastHover;

        private void MouseMoveTimer()
        {
            if (MainForm.Instance.GetCurrentViewControl() != this)
                return;

            Point pt = Cursor.Position;

            foreach (var tile in m_tiles)
            {
                var rect = tile.RectangleToScreen(tile.DisplayRectangle);
                if (rect.Contains(pt))
                {
                    if (m_lastHover != tile)
                    {
                        m_lastHover = tile;
                        GameFileEnter?.Invoke(this, new GameFileEventArgs(tile.GameFile));
                    }
                    return;
                }
            }

            if (m_lastHover != null)
                GameFileLeave?.Invoke(this, new GameFileEventArgs(m_lastHover.GameFile));

            m_lastHover = null;
        }

        private void GameFileTileViewControl_KeyDown(object sender, KeyEventArgs e)
        {
            ViewKeyDown?.Invoke(this, e);
        }

        private void GameFileTileViewControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            ViewKeyPress?.Invoke(this, e);
        }

        private void GameFileTileViewControl_Click(object sender, EventArgs e)
        {
            ClearSelection();
            SelectionChange?.Invoke(this, EventArgs.Empty);
        }

        public IGameFile GameFileForIndex(int index)
        {
            if (index >= m_gameFiles.Count)
                return null;

            return m_gameFiles[index];
        }

        public void RefreshData()
        {
            var screenshots = MainForm.Instance.DataSourceAdapter.GetFiles(FileType.Screenshot);
            string screenshotPath = MainForm.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath();
            foreach (var tile in m_tiles)
                SetTileData(tile.GameFile, screenshots, screenshotPath, tile);
        }

        public void SetContextMenuStrip(ContextMenuStrip menu)
        {
            m_menu = menu;
        }

        public void SetDisplayText(string text)
        {

        }

        private void SetData(IEnumerable<IGameFile> gameFiles)
        {
            if (gameFiles == null)
                m_gameFiles.Clear();
            else
                m_gameFiles = gameFiles.OrderBy(x => x.Title).ToList();

            var screenshots = MainForm.Instance.DataSourceAdapter.GetFiles(FileType.Screenshot);
            string screenshotPath = MainForm.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath();

            flpMain.SuspendLayout();

            if (m_gameFiles.Count > flpMain.Controls.Count)
                ExpandLayout(screenshots, screenshotPath);
            else
                ShrinkLayout();

            flpMain.ResumeLayout();
        }

        private void ShrinkLayout()
        {
            List<Control> remove = new List<Control>();

            foreach (GameFileTile tile in flpMain.Controls)
            {
                if (m_gameFiles.Contains(tile.GameFile))
                    continue;

                remove.Add(tile);
            }

            foreach (var tile in remove)
                flpMain.Controls.Remove(tile);
        }

        private void ExpandLayout(IEnumerable<IFileData> screenshots, string screenshotPath)
        {
            var itemsEnum = flpMain.Controls.GetEnumerator();
            itemsEnum.MoveNext();
            int index = 0;

            foreach (var gameFile in m_gameFiles)
            {
                if (itemsEnum.Current == null)
                {
                    flpMain.Controls.Add(GetTile(gameFile, screenshots, screenshotPath));
                }
                else
                {
                    GameFileTile tile = (GameFileTile)itemsEnum.Current;

                    if (tile.GameFile.Equals(gameFile))
                    {
                        itemsEnum.MoveNext();
                    }
                    else
                    {
                        tile = GetTile(gameFile, screenshots, screenshotPath);
                        flpMain.Controls.Add(tile);
                        flpMain.Controls.SetChildIndex(tile, index);
                    }

                    index++;
                }
            }
        }

        private GameFileTile GetTile(IGameFile gameFile, IEnumerable<IFileData> screenshots, string screenshotPath)
        {
            if (m_cachedTiles.Contains(gameFile))
                return m_tiles.FirstOrDefault(x => x.GameFile.Equals(gameFile));

            GameFileTile tile = new GameFileTile
            {
                Margin = new Padding(8, 8, 8, 8),
                GameFile = gameFile
            };

            tile.TileClick += Tile_TileClick;
            tile.TileDoubleClick += Tile_TileDoubleClick;
            SetTileData(gameFile, screenshots, screenshotPath, tile);

            m_tiles.Add(tile);
            m_cachedTiles.Add(gameFile);

            return tile;
        }

        private static void SetTileData(IGameFile gameFile, IEnumerable<IFileData> screenshots, string screenshotPath, GameFileTile tile)
        {
            if (string.IsNullOrEmpty(gameFile.Title))
                tile.SetTitle(gameFile.FileNameNoPath);
            else
                tile.SetTitle(gameFile.Title);

            if (gameFile.GameFileID.HasValue)
            {
                var screenshot = screenshots.FirstOrDefault(x => x.GameFileID == gameFile.GameFileID.Value);
                if (screenshot != null)
                    tile.SetPicture(Path.Combine(screenshotPath, screenshot.FileName));
                else
                    tile.SetPicture(string.Empty);
            }
            else
            {
                tile.SetPicture(string.Empty);
            }
        }

        private void Tile_TileDoubleClick(object sender, EventArgs e)
        {
            ItemDoubleClick?.Invoke(this, e);
        }

        private void Tile_TileClick(object sender, MouseEventArgs e)
        {
            GameFileTile tile = sender as GameFileTile;
            if (tile == null)
                return;

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

        private void SelectRange(GameFileTile tile)
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

            ClearSelection();

            for (int i = start; i <= end; i++)
            {
                m_selectedTiles.Add(m_tiles[i]);
                m_tiles[i].SetSelected(true);
            }

            SelectionChange?.Invoke(this, new EventArgs());
        }

        private int GameFileTileIndex(GameFileTile tile)
        {
            for (int i = 0; i < m_tiles.Count; i++)
            {
                if (tile == m_tiles[i])
                    return i;
            }

            return -1;
        }

        private void ToggleSelection(GameFileTile tile)
        {
            tile.SetSelected(!tile.Selected);
        }

        private void SelectGameFile(IGameFile gameFile)
        {
            ClearSelection();

            if (gameFile == null)
                return;

            foreach (GameFileTile tile in m_tiles)
            {
                if (tile.GameFile.Equals(gameFile))
                {
                    tile.SetSelected(true);
                    m_selectedTiles.Add(tile);
                    break;
                }
            }

            if (m_selectedTiles.Count > 0)
                SelectionChange?.Invoke(this, EventArgs.Empty);
        }

        private void ClearSelection()
        {
            m_selectedTiles.ForEach(x => x.SetSelected(false));
            m_selectedTiles.Clear();
        }
    }
}
