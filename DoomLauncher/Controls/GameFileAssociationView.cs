using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;

namespace DoomLauncher
{
    public partial class GameFileAssociationView : UserControl
    {
        public event EventHandler FileDeleted;
        public event EventHandler FileOrderChanged;
        public event EventHandler<RequestScreenshotsEventArgs> RequestScreenshots;

        private IGameFile m_gameFile;

        public GameFileAssociationView()
        {
            InitializeComponent();

            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;

            ctrlScreenshotView.FileType = FileType.Screenshot;
            ctrlSaveGameView.FileType = FileType.SaveGame;
            ctrlDemoView.FileType = FileType.Demo;

            ctrlScreenshotView.RequestScreenshots += CtrlScreenshotView_RequestScreenshots;
        }

        private void CtrlScreenshotView_RequestScreenshots(object sender, RequestScreenshotsEventArgs e)
        {
            RequestScreenshots?.Invoke(this, e);
        }

        public void SetScreenshots(List<IFileData> screenshots)
        {
            ctrlScreenshotView.SetScreenshots(screenshots);
        }

        public void Initialize(IDataSourceAdapter adapter, AppConfiguration config)
        {
            DataSourceAdapter = adapter;
            ScreenshotDirectory = config.ScreenshotDirectory;
            SaveGameDirectory = config.SaveGameDirectory;

            ctrlScreenshotView.DataSourceAdapter = DataSourceAdapter;
            ctrlScreenshotView.DataDirectory = ScreenshotDirectory;
            ctrlScreenshotView.FileType = FileType.Screenshot;
            ctrlScreenshotView.SetContextMenu(BuildContextMenuStrip(ctrlScreenshotView));
            ctrlScreenshotView.SetPictureWidth(Util.GetPreviewScreenshotWidth(config.ScreenshotPreviewSize));

            ctrlSaveGameView.DataSourceAdapter = DataSourceAdapter;
            ctrlSaveGameView.DataDirectory = SaveGameDirectory;
            ctrlSaveGameView.FileType = FileType.SaveGame;
            ctrlSaveGameView.SetContextMenu(BuildContextMenuStrip(ctrlSaveGameView));

            ctrlDemoView.DataSourceAdapter = DataSourceAdapter;
            ctrlDemoView.DataDirectory = config.DemoDirectory;
            ctrlDemoView.FileType = FileType.Demo;
            ctrlDemoView.SetContextMenu(BuildContextMenuStrip(ctrlDemoView));

            ctrlViewStats.DataSourceAdapter = DataSourceAdapter;
            ctrlViewStats.SetContextMenu(BuildContextMenuStrip(ctrlViewStats));

            SetButtonsEnabled(CurrentView);
        }

        private IFileAssociationView[] GetViews()
        {
            return new IFileAssociationView[] { ctrlScreenshotView, ctrlDemoView, ctrlSaveGameView, ctrlViewStats };
        }

        public void SetData(IGameFile gameFile)
        {
            m_gameFile = gameFile;
            Array.ForEach(GetViews(), x => SetViewData(x, gameFile));
        }

        private void SetViewData(IFileAssociationView view, IGameFile gameFile)
        {
            view.GameFile = gameFile;
            view.SetData(gameFile);
        }

        public void ClearData()
        {
            m_gameFile = null;
            Array.ForEach(GetViews(), x => x.ClearData());
        }

        public IDataSourceAdapter DataSourceAdapter { get; set; }
        public LauncherPath ScreenshotDirectory { get; set; }
        public LauncherPath SaveGameDirectory { get; set; }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentView != null)
            {
                SetButtonsEnabled(CurrentView);
            }
        }

        private void SetButtonsEnabled(IFileAssociationView view)
        {
            btnAddFile.Enabled = view.NewAllowed;
            btnCopy.Enabled = btnCopyAll.Enabled = view.CopyAllowed;
            btnDelete.Enabled = view.DeleteAllowed;
            btnEdit.Enabled = view.EditAllowed;
            btnMoveDown.Enabled = btnMoveUp.Enabled = btnSetFirst.Enabled = view.ChangeOrderAllowed;
            btnOpenFile.Enabled = view.ViewAllowed;
        }

        private IFileAssociationView CurrentView
        {
            get
            {
                return tabControl.SelectedTab.Controls[0] as IFileAssociationView;
            }
        }

        private void copyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCopy();
        }

        private void HandleCopy()
        {
            IFileAssociationView view = CurrentView;

            if (view != null)
                view.CopyToClipboard();
        }

        private void copyAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCopyAll();
        }

        private void HandleCopyAll()
        {
            if (CurrentView != null && CurrentView.CopyAllowed)
            {
                CurrentView.CopyAllToClipboard();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleDelete();
        }

        private void HandleDelete()
        {
            if (CurrentView != null && CurrentView.DeleteAllowed && CurrentView.Delete())
                FileDeleted?.Invoke(this, new EventArgs());
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAdd();
        }

        private void HandleAdd()
        {
            if (CurrentView != null && CurrentView.NewAllowed && CurrentView.New())
                SetData(m_gameFile);
        }

        private void editDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void HandleEdit()
        {
            if (CurrentView != null && CurrentView.EditAllowed && CurrentView.Edit())
                SetData(m_gameFile);
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilePriority(true);
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilePriority(false);
        }

        private void setFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleSetFirst();
        }

        private void HandleSetFirst()
        {
            if (CurrentView != null && CurrentView.ChangeOrderAllowed && 
                CurrentView.SetFileOrderFirst())
            {
                SetData(m_gameFile);
                FileOrderChanged?.Invoke(this, new EventArgs());
            }
        }

        private void SetFilePriority(bool up)
        {
            bool success = false;

            if (CurrentView != null && CurrentView.ChangeOrderAllowed)
            {
                if (up)
                    success = CurrentView.MoveFileOrderUp();
                else
                    success = CurrentView.MoveFileOrderDown();
            }

            if (success)
            {
                SetData(m_gameFile);
                FileOrderChanged?.Invoke(this, new EventArgs());
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            HandleCopy();
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            HandleCopyAll();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            HandleDelete();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            HandleAdd();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            SetFilePriority(true);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            SetFilePriority(false);
        }

        private void btnSetFirst_Click(object sender, EventArgs e)
        {
            HandleSetFirst();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            HandleView();
        }

        private void HandleView()
        {
            if (CurrentView != null && CurrentView.ViewAllowed)
            {
                CurrentView.View();
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleView();
        }

        private ContextMenuStrip BuildContextMenuStrip(IFileAssociationView view)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            if (view.CopyAllowed)
            {
                CreateMenuItem(menu, "Copy", copyFileToolStripMenuItem_Click);
                CreateMenuItem(menu, "Copy All", copyAllFilesToolStripMenuItem_Click);
            }

            if (view.DeleteAllowed)
                CreateMenuItem(menu, "Delete", deleteToolStripMenuItem_Click);

            AddSeperator(menu);

            if (view.NewAllowed)
                CreateMenuItem(menu, "Add File...", addFileToolStripMenuItem_Click);
            if (view.ViewAllowed)
                CreateMenuItem(menu, "Open File...", openFileToolStripMenuItem_Click);

            AddSeperator(menu);

            if (view.EditAllowed)
                CreateMenuItem(menu, "Edit Details...", editDetailsToolStripMenuItem_Click);

            AddSeperator(menu);

            if (view.ChangeOrderAllowed)
            {
                CreateMenuItem(menu, "Move Up", moveUpToolStripMenuItem_Click);
                CreateMenuItem(menu, "Move Down", moveDownToolStripMenuItem_Click);
                CreateMenuItem(menu, "Set First", setFirstToolStripMenuItem_Click);
            }

            FinalizeMenu(menu);

            return menu;
        }

        private void FinalizeMenu(System.Windows.Forms.ContextMenuStrip menu)
        {
            if (menu.Items.Count > 0 && menu.Items[menu.Items.Count - 1] is ToolStripSeparator)
                menu.Items.Remove(menu.Items[menu.Items.Count - 1]);
        }

        private static void AddSeperator(ContextMenuStrip menu)
        {
            if (menu.Items.Count > 0 && !(menu.Items[menu.Items.Count - 1] is ToolStripSeparator))
                menu.Items.Add(new ToolStripSeparator());
        }

        private static void CreateMenuItem(ContextMenuStrip menu, string text, EventHandler handler)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Click += handler;
            menu.Items.Add(item);
        }
    }
}
