using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher
{
    class GameFileTileManager
    {
        public event EventHandler TilesRecreated;

        public static GameFileTileManager Instance { get; private set; } = new GameFileTileManager();

        public List<GameFileTileBase> Tiles = new List<GameFileTileBase>();
        public List<GameFileTileBase> OldTiles = new List<GameFileTileBase>();
        public FlowLayoutPanelDB TileLayout = new FlowLayoutPanelDB();
        public int MaxItems => DataCache.Instance.AppConfiguration.ItemsPerPage;

        private GameFileViewFactory m_factory;

        public void Init(GameFileViewFactory factory)
        {
            m_factory = factory;
            TileLayout.AutoScroll = true;

            ResetLayout();
        }

        private void ResetLayout()
        {
            OldTiles.AddRange(Tiles);

            TileLayout.SuspendLayout();

            if (m_factory.IsUsingTileView)
            {
                GameFileTileBase testTile = m_factory.CreateTile();

                if (Tiles.Count == 0 || Tiles[0].GetType() != testTile.GetType())
                    RecreateLayout();
                else
                    UpdateLayout();
            }

            TileLayout.ResumeLayout();

            TilesRecreated?.Invoke(this, EventArgs.Empty);
            OldTiles.Clear();
        }

        private void UpdateLayout()
        {
            if (Tiles.Count > MaxItems)
            {
                Tiles = Tiles.Take(MaxItems).ToList();
                
                while (TileLayout.Controls.Count > MaxItems)
                    TileLayout.Controls.RemoveAt(TileLayout.Controls.Count - 1);
            }
            else
            {
                int addCount = MaxItems - Tiles.Count;
                for (int i = 0; i < addCount; i++)
                {
                    GameFileTileBase tile = m_factory.CreateTile();
                    Tiles.Add(tile);
                    TileLayout.Controls.Add(tile);
                }
            }
        }

        private void RecreateLayout()
        {
            TileLayout.Controls.Clear();
            Tiles.Clear();

            for (int i = 0; i < MaxItems; i++)
            {
                GameFileTileBase tile = m_factory.CreateTile();
                Tiles.Add(tile);
                TileLayout.Controls.Add(tile);
            }
        }

        public void SetMaxItems(int max)
        {
            DataCache.Instance.AppConfiguration.ItemsPerPage = max;
            ResetLayout();
        }
    }
}
