using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    class GameFileTileManager
    {
        public int MaxItems = 28;
        public static GameFileTileManager Instance { get; private set; } = new GameFileTileManager();

        public List<GameFileTile> Tiles = new List<GameFileTile>();
        public FlowLayoutPanelDB FlowLayoutPanel = new FlowLayoutPanelDB();
        public Image DefaultImage { get; private set; }

        public GameFileTileManager()
        {
            DefaultImage = Image.FromFile("TileImages\\DoomLauncherTile.png");

            for (int i = 0; i < MaxItems; i++)
            {
                GameFileTile tile = new GameFileTile
                {
                    Margin = new Padding(8, 8, 8, 8)
                };

                Tiles.Add(tile);
                FlowLayoutPanel.Controls.Add(tile);
            }
        }
    }
}
