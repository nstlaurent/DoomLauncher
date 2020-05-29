using System;
using System.Collections.Generic;
using System.Drawing;

namespace DoomLauncher
{
    class GameFileTileManager
    {
        public event EventHandler TilesRecreated;

        public static readonly int MaxItems = 30;
        public static GameFileTileManager Instance { get; private set; } = new GameFileTileManager();

        public List<GameFileTileBase> Tiles = new List<GameFileTileBase>();
        public List<GameFileTileBase> OldTiles = new List<GameFileTileBase>();
        public FlowLayoutPanelDB TileLayout = new FlowLayoutPanelDB();
        public Image DefaultImage { get; private set; }

        public void Init(GameFileViewFactory factory)
        {
            TileLayout.AutoScroll = true;
            DefaultImage = Image.FromFile("TileImages\\DoomLauncherTile.png");

            OldTiles.AddRange(Tiles);
            Tiles.Clear();

            TileLayout.SuspendLayout();
            TileLayout.Controls.Clear();

            if (factory.IsUsingTileView)
            {
                for (int i = 0; i < MaxItems; i++)
                {
                    GameFileTileBase tile = factory.CreateTile();
                    Tiles.Add(tile);
                    TileLayout.Controls.Add(tile);
                }
            }

            TileLayout.ResumeLayout();

            TilesRecreated?.Invoke(this, EventArgs.Empty);
            OldTiles.Clear();
        }
    }
}
