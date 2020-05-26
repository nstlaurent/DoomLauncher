using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public interface IGameFileTile
    {
        event MouseEventHandler TileClick;
        event EventHandler TileDoubleClick;

        IGameFile GameFile { get; }
        bool Selected { get; }

        void SetSelected(bool set);
        void SetData(IGameFile gameFile, IEnumerable<ITagData> tags);
        void SetImageLocation(string file);
        void SetImage(Image image);
        void ClearData();
    }
}
