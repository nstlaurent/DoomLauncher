using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DoomLauncher
{
    public interface IGameFileView
    {
        event AddingNewEventHandler ToolTipTextNeeded;
        event EventHandler ItemDoubleClick;
        event EventHandler SelectionChange;
        event KeyPressEventHandler ViewKeyPress;
        event KeyEventHandler ViewKeyDown;
        event DragEventHandler DragEnter;
        event DragEventHandler DragDrop;

        void SetDisplayText(string text);
        void SetContextMenuStrip(ContextMenuStrip menu);
        IEnumerable<IGameFile> DataSource { get; set; }
        IGameFile SelectedItem { get; set; }
        IGameFile[] SelectedItems { get; }
        IGameFile GameFileForIndex(int index);
        void RefreshData();
        bool Focus();

        int ToolTipItemIndex { get; }
        bool MultiSelect { get; set; }
        bool AllowDrop { get; set; }

        void SuspendLayout();
        void ResumeLayout();

        object DoomLauncherParent { get; set; }
    }
}
