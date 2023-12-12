using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoomLauncher.Handlers;

namespace DoomLauncher
{
    public partial class GameFileAssociationView : UserControl
    {
        public event EventHandler FileAdded;
        public event EventHandler FileDeleted;
        public event EventHandler FileOrderChanged;
        public event EventHandler FileDetailsChanged;
        public event EventHandler<RequestScreenshotsEventArgs> RequestScreenshots;

        private readonly ToolTipGroup m_toolTipGroup = new ToolTipGroup();
        private IGameFile m_gameFile;

        public GameFileAssociationView()
        {
            InitializeComponent();

            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;

            ctrlScreenshotView.FileType = FileType.Screenshot;
            ctrlSaveGameView.FileType = FileType.SaveGame;
            ctrlDemoView.FileType = FileType.Demo;

            ctrlScreenshotView.RequestScreenshots += CtrlScreenshotView_RequestScreenshots;
            SetButtonsAllButtonsEnabled(false);

            Icons.DpiScale = new DpiScale(CreateGraphics());
            btnDelete.Image = Icons.Delete;
            btnAddFile.Image = Icons.File;
            btnCopy.Image = Icons.Export;
            btnCopyAll.Image = Icons.ExportAll;
            btnEdit.Image = Icons.Edit;
            btnMoveUp.Image = Icons.ArrowUp;
            btnMoveDown.Image = Icons.ArrowDown;
            btnSetFirst.Image = Icons.StepBack;
            btnOpenFile.Image = Icons.FolderOpen;

            m_toolTipGroup.SetToolTip(btnDelete, "Delete");
            m_toolTipGroup.SetToolTip(btnAddFile, "Add File");
            m_toolTipGroup.SetToolTip(btnCopy, "Export");
            m_toolTipGroup.SetToolTip(btnCopyAll, "Export All");
            m_toolTipGroup.SetToolTip(btnEdit, "Edit");
            m_toolTipGroup.SetToolTip(btnMoveUp, "Move Up");
            m_toolTipGroup.SetToolTip(btnMoveDown, "Move Down");
            m_toolTipGroup.SetToolTip(btnSetFirst, "Set First");
            m_toolTipGroup.SetToolTip(btnOpenFile, "Open File");

            Stylizer.StylizeControl(this, DesignMode);
            Stylizer.StylizeControl(mnuOptions, DesignMode);
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
            ctrlScreenshotView.SetPictureWidth(config.ScreenshotPreviewSize);

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
            SetButtonsEnabled(CurrentView);
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
            SetButtonsAllButtonsEnabled(false);
        }

        public IDataSourceAdapter DataSourceAdapter { get; set; }
        public LauncherPath ScreenshotDirectory { get; set; }
        public LauncherPath SaveGameDirectory { get; set; }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentView != null && m_gameFile != null)
                SetButtonsEnabled(CurrentView);
        }

        private void SetButtonsEnabled(IFileAssociationView view)
        {
            SetIconButtonEnabled(btnAddFile, IconImage.File, view.NewAllowed);
            SetIconButtonEnabled(btnCopy, IconImage.Export, view.CopyOrExportAllowed);
            SetIconButtonEnabled(btnCopyAll, IconImage.ExportAll, view.CopyOrExportAllowed);
            SetIconButtonEnabled(btnDelete, IconImage.Delete, view.DeleteAllowed);
            SetIconButtonEnabled(btnEdit, IconImage.Edit, view.EditAllowed);
            SetIconButtonEnabled(btnMoveDown, IconImage.DownArrow, view.EditAllowed);
            SetIconButtonEnabled(btnMoveUp, IconImage.UpArrow, view.EditAllowed);
            SetIconButtonEnabled(btnSetFirst, IconImage.StepBack, view.EditAllowed);
            SetIconButtonEnabled(btnOpenFile, IconImage.FolderOpen, view.ViewAllowed);
        }

        public void SetButtonsAllButtonsEnabled(bool enabled)
        {
            SetIconButtonEnabled(btnAddFile, IconImage.File, enabled);
            SetIconButtonEnabled(btnCopy, IconImage.Export, enabled);
            SetIconButtonEnabled(btnCopyAll, IconImage.ExportAll, enabled);
            SetIconButtonEnabled(btnDelete, IconImage.Delete, enabled);
            SetIconButtonEnabled(btnEdit, IconImage.Edit, enabled);
            SetIconButtonEnabled(btnMoveDown, IconImage.DownArrow, enabled);
            SetIconButtonEnabled(btnMoveUp, IconImage.UpArrow, enabled);
            SetIconButtonEnabled(btnSetFirst, IconImage.StepBack, enabled);
            SetIconButtonEnabled(btnOpenFile, IconImage.FolderOpen, enabled);
        }

        private void SetIconButtonEnabled(FormButton button, IconImage iconImage, bool enabled)
        {
            button.Enabled = enabled;
            button.Image = Icons.GetIcon(iconImage, enabled ?  ColorTheme.Current.Text : ColorTheme.Current.DisabledText);
        }

        private IFileAssociationView CurrentView =>
            tabControl.SelectedTab.Controls[0] as IFileAssociationView;
        
        private void copyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCopy();
        }

        private void HandleCopy()
        {
            if (CurrentView != null && CurrentView.CopyOrExportAllowed)
                CurrentView.CopyToClipboard();
        }

        private void copyAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCopyAll();
        }

        private void HandleCopyAll()
        {
            if (CurrentView != null && CurrentView.CopyOrExportAllowed)
                CurrentView.CopyAllToClipboard();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleDelete();
        }

        private void HandleDelete()
        {
            if (CurrentView != null && CurrentView.DeleteAllowed && CurrentView.Delete())
                FileDeleted?.Invoke(this, EventArgs.Empty);
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAdd();
        }

        private void HandleAdd()
        {
            if (CurrentView != null && CurrentView.NewAllowed && CurrentView.New())
            {
                FileAdded?.Invoke(this, EventArgs.Empty);
                SetData(m_gameFile);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleExport();
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleExportAll();
        }

        private void HandleExport()
        {
            if (CurrentView != null && CurrentView.CopyOrExportAllowed)
                CurrentView.Export();
        }

        private void HandleExportAll()
        {
            if (CurrentView != null && CurrentView.CopyOrExportAllowed)
                CurrentView.ExportAll();
        }

        private void editDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void HandleEdit()
        {
            if (CurrentView != null && CurrentView.EditAllowed && CurrentView.Edit())
            {
                SetData(m_gameFile);
                FileDetailsChanged?.Invoke(this, EventArgs.Empty);
            }
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
                FileOrderChanged?.Invoke(this, EventArgs.Empty);
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
                FileOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            HandleExport();
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            HandleExportAll();
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

            if (view.CopyOrExportAllowed)
            {
                CreateMenuItem(menu, "Export...", exportToolStripMenuItem_Click);
                CreateMenuItem(menu, "Export All...", exportAllToolStripMenuItem_Click);
                AddSeperator(menu);
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

            var customMenuOptions = view.MenuOptions;
            if (customMenuOptions.Count > 0)
            {
                AddSeperator(menu);
                foreach (var option in customMenuOptions)
                {
                    CreateMenuItem(menu, option.Title, (sender, eventArgs) =>
                    {
                        if (!option.Action())
                            return;
                        // Update data and assume the file order changed
                        SetData(m_gameFile);
                        FileOrderChanged?.Invoke(this, EventArgs.Empty);
                    });
                }
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
