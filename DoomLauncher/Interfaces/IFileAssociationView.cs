using System.Windows.Forms;

namespace DoomLauncher.Interfaces
{
    interface IFileAssociationView
    {
        void SetContextMenu(ContextMenuStrip menu);
        void SetData(IGameFile gameFile);
        void ClearData();

        bool DeleteAllowed { get; }
        bool CopyAllowed { get; }
        bool NewAllowed { get; }
        bool EditAllowed { get; }
        bool ViewAllowed { get; }
        bool ChangeOrderAllowed { get; }

        bool New();
        bool Edit();
        bool Delete();
        void CopyToClipboard();
        void CopyAllToClipboard();
        void View();
        bool MoveFileOrderUp();
        bool MoveFileOrderDown();
        bool SetFileOrderFirst();

        IGameFile GameFile { get; set; }
    }
}
