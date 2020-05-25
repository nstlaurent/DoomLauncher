using System;
using System.Drawing;
using System.Windows.Forms;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public abstract class GameFileTileBase : UserControl, IGameFileTile
    {
        public abstract IGameFile GameFile { get; protected set; }
        public abstract bool Selected { get; protected set; }

        public abstract event MouseEventHandler TileClick;
        public abstract event EventHandler TileDoubleClick;

        public abstract void ClearData();
        public abstract void SetData(IGameFile gameFile);
        public abstract void SetImageLocation(string file);
        public abstract void SetImage(Image image);
        public abstract void SetSelected(bool set);
    }
}
