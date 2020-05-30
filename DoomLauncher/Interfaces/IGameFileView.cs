using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class GameFileEventArgs
    {
        public IGameFile GameFile { get; private set; }
        public GameFileEventArgs(IGameFile gameFile)
        {
            GameFile = gameFile;
        }
    }

    public delegate void GameFileEventHandler(object sender, GameFileEventArgs e);

    public interface IGameFileView
    {
        event EventHandler ItemClick;
        event EventHandler ItemDoubleClick;
        event EventHandler SelectionChange;
        event KeyPressEventHandler ViewKeyPress;
        event KeyEventHandler ViewKeyDown;
        event DragEventHandler DragEnter;
        event DragEventHandler DragDrop;
        event GameFileEventHandler GameFileEnter;
        event GameFileEventHandler GameFileLeave;

        void SetDisplayText(string text);
        void SetContextMenuStrip(ContextMenuStrip menu);
        IEnumerable<IGameFile> DataSource { get; set; }
        IGameFile SelectedItem { get; set; }
        IGameFile[] SelectedItems { get; }
        IGameFile GameFileForIndex(int index);
        void RefreshData();
        bool Focus();
        void UpdateGameFile(IGameFile gameFile);

        bool MultiSelect { get; set; }
        bool AllowDrop { get; set; }

        void SuspendLayout();
        void ResumeLayout();

        object DoomLauncherParent { get; set; }

        void SetVisible(bool set);
    }
}
