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

        public GameFileTileExpanded()
        {
            InitializeComponent();
            //Height = gameTile.Height;
            gameTile.TileClick += GameTile_TileClick;
            gameTile.TileDoubleClick += GameTile_TileDoubleClick;
            Paint += GameFileTileExpanded_Paint;
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
            gameTile.SetData(gameFile, tags);
            lblFilename.Text = gameFile.FileNameNoPath;
            lblTitle.Text = gameFile.Title;
            lblAuthor.Text = gameFile.Author;
            lblTags.Text = string.Join(", ", tags.Select(x => x.Name));
            if (gameFile.MapCount.HasValue)
                lblMaps.Text = gameFile.MapCount.ToString();
            else
                lblMaps.Text = "0";
            if (gameFile.ReleaseDate.HasValue)
                lblReleaseDate.Text = gameFile.ReleaseDate.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            else
                lblReleaseDate.Text = string.Empty;
            if (gameFile.LastPlayed.HasValue)
                lblLastPlayed.Text = gameFile.LastPlayed.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            else
                lblLastPlayed.Text = string.Empty;
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
