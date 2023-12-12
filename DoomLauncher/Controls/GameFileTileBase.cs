using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public abstract class GameFileTileBase : UserControl, IGameFileTile
    {
        public abstract event MouseEventHandler TileClick;
        public abstract event EventHandler TileDoubleClick;

        public abstract int ImageWidth { get; protected set; }
        public abstract IGameFile GameFile { get; protected set; }
        public abstract bool Selected { get; protected set; }

        public abstract void ClearData();
        public abstract void SetData(IGameFile gameFile, IEnumerable<ITagData> tags);
        public abstract void SetImageLocation(string file);
        public abstract void SetImage(Image image);
        public abstract void SetSelected(bool set);
    }
}
