using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    class GameFileTileManager
    {
        public int MaxItems = 30;
        public static GameFileTileManager Instance { get; private set; } = new GameFileTileManager();

        public List<GameFileTileBase> Tiles = new List<GameFileTileBase>();
        public FlowLayoutPanelDB TileLayout = new FlowLayoutPanelDB();
        public Image DefaultImage { get; private set; }

        public GameFileTileManager()
        {
            TileLayout.AutoScroll = true;
            DefaultImage = Image.FromFile("TileImages\\DoomLauncherTile.png");

            for (int i = 0; i < MaxItems; i++)
            {
                GameFileTileExpanded tile = new GameFileTileExpanded
                {
                    Margin = new Padding(8, 8, 8, 8)
                };

                Tiles.Add(tile);
                TileLayout.Controls.Add(tile);
            }
        }
    }
}
