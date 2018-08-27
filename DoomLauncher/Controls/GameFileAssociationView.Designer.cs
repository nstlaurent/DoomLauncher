namespace DoomLauncher
{
    partial class GameFileAssociationView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCode("Winform Designer", "VS2015 SP1")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TabPage tabPageScreenshots;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameFileAssociationView));
            this.ctrlScreenshotView = new DoomLauncher.ScreenshotView();
            this.mnuOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFirstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageDemos = new System.Windows.Forms.TabPage();
            this.ctrlDemoView = new DoomLauncher.GenericFileView();
            this.tabPageSaveGames = new System.Windows.Forms.TabPage();
            this.ctrlSaveGameView = new DoomLauncher.GenericFileView();
            this.tabPageStatistics = new System.Windows.Forms.TabPage();
            this.ctrlViewStats = new DoomLauncher.StatisticsView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnCopyAll = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddFile = new System.Windows.Forms.ToolStripButton();
            this.btnOpenFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMoveUp = new System.Windows.Forms.ToolStripButton();
            this.btnMoveDown = new System.Windows.Forms.ToolStripButton();
            this.btnSetFirst = new System.Windows.Forms.ToolStripButton();
            tabPageScreenshots = new System.Windows.Forms.TabPage();
            tabPageScreenshots.SuspendLayout();
            this.mnuOptions.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageDemos.SuspendLayout();
            this.tabPageSaveGames.SuspendLayout();
            this.tabPageStatistics.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageScreenshots
            // 
            tabPageScreenshots.Controls.Add(this.ctrlScreenshotView);
            tabPageScreenshots.Location = new System.Drawing.Point(4, 22);
            tabPageScreenshots.Name = "tabPageScreenshots";
            tabPageScreenshots.Padding = new System.Windows.Forms.Padding(3);
            tabPageScreenshots.Size = new System.Drawing.Size(682, 206);
            tabPageScreenshots.TabIndex = 0;
            tabPageScreenshots.Text = "Screenshots";
            tabPageScreenshots.UseVisualStyleBackColor = true;
            // 
            // ctrlScreenshotView
            // 
            this.ctrlScreenshotView.DataDirectory = null;
            this.ctrlScreenshotView.DataSourceAdapter = null;
            this.ctrlScreenshotView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlScreenshotView.FileType = DoomLauncher.FileType.Unknown;
            this.ctrlScreenshotView.GameFile = null;
            this.ctrlScreenshotView.Location = new System.Drawing.Point(3, 3);
            this.ctrlScreenshotView.Name = "ctrlScreenshotView";
            this.ctrlScreenshotView.Size = new System.Drawing.Size(676, 200);
            this.ctrlScreenshotView.TabIndex = 0;
            // 
            // mnuOptions
            // 
            this.mnuOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFileToolStripMenuItem,
            this.copyAllFilesToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.addFileToolStripMenuItem,
            this.openFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.editDetailsToolStripMenuItem,
            this.toolStripSeparator3,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.setFirstToolStripMenuItem});
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.Size = new System.Drawing.Size(146, 220);
            // 
            // copyFileToolStripMenuItem
            // 
            this.copyFileToolStripMenuItem.Name = "copyFileToolStripMenuItem";
            this.copyFileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.copyFileToolStripMenuItem.Text = "Copy File";
            this.copyFileToolStripMenuItem.Click += new System.EventHandler(this.copyFileToolStripMenuItem_Click);
            // 
            // copyAllFilesToolStripMenuItem
            // 
            this.copyAllFilesToolStripMenuItem.Name = "copyAllFilesToolStripMenuItem";
            this.copyAllFilesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.copyAllFilesToolStripMenuItem.Text = "Copy All Files";
            this.copyAllFilesToolStripMenuItem.Click += new System.EventHandler(this.copyAllFilesToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(142, 6);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            this.addFileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.addFileToolStripMenuItem.Text = "Add File...";
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.openFileToolStripMenuItem.Text = "Open File...";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(142, 6);
            // 
            // editDetailsToolStripMenuItem
            // 
            this.editDetailsToolStripMenuItem.Name = "editDetailsToolStripMenuItem";
            this.editDetailsToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.editDetailsToolStripMenuItem.Text = "Edit Details...";
            this.editDetailsToolStripMenuItem.Click += new System.EventHandler(this.editDetailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(142, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // setFirstToolStripMenuItem
            // 
            this.setFirstToolStripMenuItem.Name = "setFirstToolStripMenuItem";
            this.setFirstToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.setFirstToolStripMenuItem.Text = "Set First";
            this.setFirstToolStripMenuItem.Click += new System.EventHandler(this.setFirstToolStripMenuItem_Click);
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tabControl, 0, 1);
            this.tblMain.Controls.Add(this.toolStrip1, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Size = new System.Drawing.Size(696, 262);
            this.tblMain.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(tabPageScreenshots);
            this.tabControl.Controls.Add(this.tabPageDemos);
            this.tabControl.Controls.Add(this.tabPageSaveGames);
            this.tabControl.Controls.Add(this.tabPageStatistics);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 27);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(690, 232);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageDemos
            // 
            this.tabPageDemos.Controls.Add(this.ctrlDemoView);
            this.tabPageDemos.Location = new System.Drawing.Point(4, 22);
            this.tabPageDemos.Name = "tabPageDemos";
            this.tabPageDemos.Size = new System.Drawing.Size(682, 206);
            this.tabPageDemos.TabIndex = 1;
            this.tabPageDemos.Text = "Demos";
            this.tabPageDemos.UseVisualStyleBackColor = true;
            // 
            // ctrlDemoView
            // 
            this.ctrlDemoView.DataDirectory = null;
            this.ctrlDemoView.DataSourceAdapter = null;
            this.ctrlDemoView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlDemoView.FileType = DoomLauncher.FileType.Unknown;
            this.ctrlDemoView.GameFile = null;
            this.ctrlDemoView.Location = new System.Drawing.Point(0, 0);
            this.ctrlDemoView.Name = "ctrlDemoView";
            this.ctrlDemoView.Size = new System.Drawing.Size(682, 206);
            this.ctrlDemoView.TabIndex = 0;
            // 
            // tabPageSaveGames
            // 
            this.tabPageSaveGames.Controls.Add(this.ctrlSaveGameView);
            this.tabPageSaveGames.Location = new System.Drawing.Point(4, 22);
            this.tabPageSaveGames.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageSaveGames.Name = "tabPageSaveGames";
            this.tabPageSaveGames.Size = new System.Drawing.Size(682, 206);
            this.tabPageSaveGames.TabIndex = 2;
            this.tabPageSaveGames.Text = "Save Games";
            this.tabPageSaveGames.UseVisualStyleBackColor = true;
            // 
            // ctrlSaveGameView
            // 
            this.ctrlSaveGameView.DataDirectory = null;
            this.ctrlSaveGameView.DataSourceAdapter = null;
            this.ctrlSaveGameView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlSaveGameView.FileType = DoomLauncher.FileType.Unknown;
            this.ctrlSaveGameView.GameFile = null;
            this.ctrlSaveGameView.Location = new System.Drawing.Point(0, 0);
            this.ctrlSaveGameView.Name = "ctrlSaveGameView";
            this.ctrlSaveGameView.Size = new System.Drawing.Size(682, 206);
            this.ctrlSaveGameView.TabIndex = 1;
            // 
            // tabPageStatistics
            // 
            this.tabPageStatistics.Controls.Add(this.ctrlViewStats);
            this.tabPageStatistics.Location = new System.Drawing.Point(4, 22);
            this.tabPageStatistics.Name = "tabPageStatistics";
            this.tabPageStatistics.Size = new System.Drawing.Size(682, 206);
            this.tabPageStatistics.TabIndex = 3;
            this.tabPageStatistics.Text = "Statistics";
            this.tabPageStatistics.UseVisualStyleBackColor = true;
            // 
            // ctrlViewStats
            // 
            this.ctrlViewStats.DataSourceAdapter = null;
            this.ctrlViewStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlViewStats.GameFile = null;
            this.ctrlViewStats.Location = new System.Drawing.Point(0, 0);
            this.ctrlViewStats.Name = "ctrlViewStats";
            this.ctrlViewStats.Size = new System.Drawing.Size(682, 206);
            this.ctrlViewStats.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCopy,
            this.btnCopyAll,
            this.btnDelete,
            this.toolStripSeparator7,
            this.btnAddFile,
            this.btnOpenFile,
            this.toolStripSeparator4,
            this.btnEdit,
            this.toolStripSeparator5,
            this.btnMoveUp,
            this.btnMoveDown,
            this.btnSetFirst});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(268, 24);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(23, 21);
            this.btnCopy.Text = "Copy to Clipboard";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopyAll.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyAll.Image")));
            this.btnCopyAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(23, 21);
            this.btnCopyAll.Text = "Copy All to Clipboard";
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 21);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 24);
            // 
            // btnAddFile
            // 
            this.btnAddFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddFile.Image = ((System.Drawing.Image)(resources.GetObject("btnAddFile.Image")));
            this.btnAddFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(23, 21);
            this.btnAddFile.Text = "Add File";
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFile.Image")));
            this.btnOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(23, 21);
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 24);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(23, 21);
            this.btnEdit.Text = "Edit Details";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 24);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.Image")));
            this.btnMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(23, 21);
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.Image")));
            this.btnMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(23, 21);
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnSetFirst
            // 
            this.btnSetFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSetFirst.Image = ((System.Drawing.Image)(resources.GetObject("btnSetFirst.Image")));
            this.btnSetFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSetFirst.Name = "btnSetFirst";
            this.btnSetFirst.Size = new System.Drawing.Size(23, 21);
            this.btnSetFirst.Text = "Set First";
            this.btnSetFirst.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSetFirst.Click += new System.EventHandler(this.btnSetFirst_Click);
            // 
            // GameFileAssociationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Name = "GameFileAssociationView";
            this.Size = new System.Drawing.Size(696, 262);
            tabPageScreenshots.ResumeLayout(false);
            this.mnuOptions.ResumeLayout(false);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageDemos.ResumeLayout(false);
            this.tabPageSaveGames.ResumeLayout(false);
            this.tabPageStatistics.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private ScreenshotView ctrlScreenshotView;
        private System.Windows.Forms.TabPage tabPageDemos;
        private GenericFileView ctrlDemoView;
        private System.Windows.Forms.ContextMenuStrip mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem copyFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllFilesToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageSaveGames;
        private GenericFileView ctrlSaveGameView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setFirstToolStripMenuItem;
        private TableLayoutPanelDB tblMain;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnAddFile;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnMoveUp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton btnSetFirst;
        private System.Windows.Forms.ToolStripButton btnMoveDown;
        private System.Windows.Forms.ToolStripButton btnCopyAll;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton btnOpenFile;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageStatistics;
        private StatisticsView ctrlViewStats;
    }
}
