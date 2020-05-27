using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    //public partial class GameFileTileExpanded : UserControl
    //{
    //    public GameFileTileExpanded()
    //    {
    //        InitializeComponent();
    //    }
    //}

    public partial class GameFileTileExpanded : GameFileTileBase
    {
        public override event MouseEventHandler TileClick;
        public override event EventHandler TileDoubleClick;

        public override IGameFile GameFile { get { return gameTile.GameFile; } protected set { } }
        public override bool Selected { get { return gameTile.Selected; } protected set { } }

        private string m_tags;
        private string m_maps;
        private string m_release;
        private string m_played;

        public GameFileTileExpanded()
        {
            InitializeComponent();

            gameTile.TileClick += GameTile_TileClick;
            gameTile.TileDoubleClick += GameTile_TileDoubleClick;
            Paint += GameFileTileExpanded_Paint;
            pnlData.Paint += PnlData_Paint;
        }

        private void PnlData_Paint(object sender, PaintEventArgs e)
        {
            if (GameFile == null)
                return;

            Font f = new Font(Font.FontFamily, 10);

            int xPos = gameTile.Location.X + 122;
            int yPos = 8;
            int offset = 22;

            e.Graphics.DrawString(GameFile.FileNameNoPath, f, Brushes.Black, xPos, yPos);
            yPos += offset;
            e.Graphics.DrawString(GameFile.Title, f, Brushes.Black, xPos, yPos);
            yPos += offset;
            e.Graphics.DrawString(GameFile.Author, f, Brushes.Black, xPos, yPos);
            yPos += offset;
            e.Graphics.DrawString(m_release, f, Brushes.Black, xPos, yPos);
            yPos += offset;
            e.Graphics.DrawString(m_played, f, Brushes.Black, xPos, yPos);
            yPos += offset;
            e.Graphics.DrawString(m_maps, f, Brushes.Black, xPos, yPos);
            yPos += offset;
            e.Graphics.DrawString(m_tags, f, Brushes.Black, xPos, yPos);
        }

        private void GameFileTileExpanded_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawLine(new Pen(Color.Red, 1.0f), new Point(2, Height - 2), new Point(Width - 2, Height - 2));
        }

        private void GameTile_TileDoubleClick(object sender, EventArgs e)
        {
            TileDoubleClick?.Invoke(this, e);
        }

        private void GameTile_TileClick(object sender, MouseEventArgs e)
        {
            TileClick?.Invoke(this, e);
        }

        public override void ClearData()
        {
            gameTile.ClearData();
        }

        public override void SetData(IGameFile gameFile, IEnumerable<ITagData> tags)
        {
            if (gameFile.Equals(GameFile))
                return;

            gameTile.SetData(gameFile, tags);

            m_tags = string.Join(", ", tags.Select(x => x.Name));
            if (gameFile.MapCount.HasValue)
                m_maps = gameFile.MapCount.ToString();
            else
                m_maps = "0";

            if (gameFile.ReleaseDate.HasValue)
                m_release = gameFile.ReleaseDate.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            else
                m_release = string.Empty;

            if (gameFile.LastPlayed.HasValue)
                m_played = gameFile.LastPlayed.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            else
                m_played = string.Empty;

            pnlData.Invalidate();
        }

        public override void SetImageLocation(string file)
        {
            gameTile.SetImageLocation(file);
        }

        public override void SetImage(Image image)
        {
            gameTile.SetImage(image);
        }

        public override void SetSelected(bool set)
        {
            gameTile.SetSelected(set);
        }
    }
}
