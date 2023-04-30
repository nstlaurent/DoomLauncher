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
            this.tabControl = new DoomLauncher.CTabControl();
            this.tabPageDemos = new System.Windows.Forms.TabPage();
            this.ctrlDemoView = new DoomLauncher.GenericFileView();
            this.tabPageSaveGames = new System.Windows.Forms.TabPage();
            this.ctrlSaveGameView = new DoomLauncher.GenericFileView();
            this.tabPageStatistics = new System.Windows.Forms.TabPage();
            this.ctrlViewStats = new DoomLauncher.StatisticsView();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCopy = new DoomLauncher.FormButton();
            this.btnCopyAll = new DoomLauncher.FormButton();
            this.btnDelete = new DoomLauncher.FormButton();
            this.btnAddFile = new DoomLauncher.FormButton();
            this.btnOpenFile = new DoomLauncher.FormButton();
            this.btnEdit = new DoomLauncher.FormButton();
            this.btnMoveUp = new DoomLauncher.FormButton();
            this.btnMoveDown = new DoomLauncher.FormButton();
            this.btnSetFirst = new DoomLauncher.FormButton();
            tabPageScreenshots = new System.Windows.Forms.TabPage();
            tabPageScreenshots.SuspendLayout();
            this.mnuOptions.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageDemos.SuspendLayout();
            this.tabPageSaveGames.SuspendLayout();
            this.tabPageStatistics.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageScreenshots
            // 
            tabPageScreenshots.Controls.Add(this.ctrlScreenshotView);
            tabPageScreenshots.Location = new System.Drawing.Point(4, 29);
            tabPageScreenshots.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPageScreenshots.Name = "tabPageScreenshots";
            tabPageScreenshots.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPageScreenshots.Size = new System.Drawing.Size(1028, 320);
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
            this.ctrlScreenshotView.Location = new System.Drawing.Point(4, 5);
            this.ctrlScreenshotView.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.ctrlScreenshotView.Name = "ctrlScreenshotView";
            this.ctrlScreenshotView.Size = new System.Drawing.Size(1020, 310);
            this.ctrlScreenshotView.TabIndex = 0;
            // 
            // mnuOptions
            // 
            this.mnuOptions.ImageScalingSize = new System.Drawing.Size(24, 24);
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
            this.mnuOptions.Size = new System.Drawing.Size(191, 310);
            // 
            // copyFileToolStripMenuItem
            // 
            this.copyFileToolStripMenuItem.Name = "copyFileToolStripMenuItem";
            this.copyFileToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.copyFileToolStripMenuItem.Text = "Copy File";
            this.copyFileToolStripMenuItem.Click += new System.EventHandler(this.copyFileToolStripMenuItem_Click);
            // 
            // copyAllFilesToolStripMenuItem
            // 
            this.copyAllFilesToolStripMenuItem.Name = "copyAllFilesToolStripMenuItem";
            this.copyAllFilesToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.copyAllFilesToolStripMenuItem.Text = "Copy All Files";
            this.copyAllFilesToolStripMenuItem.Click += new System.EventHandler(this.copyAllFilesToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            this.addFileToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.addFileToolStripMenuItem.Text = "Add File...";
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.openFileToolStripMenuItem.Text = "Open File...";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(187, 6);
            // 
            // editDetailsToolStripMenuItem
            // 
            this.editDetailsToolStripMenuItem.Name = "editDetailsToolStripMenuItem";
            this.editDetailsToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.editDetailsToolStripMenuItem.Text = "Edit Details...";
            this.editDetailsToolStripMenuItem.Click += new System.EventHandler(this.editDetailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(187, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // setFirstToolStripMenuItem
            // 
            this.setFirstToolStripMenuItem.Name = "setFirstToolStripMenuItem";
            this.setFirstToolStripMenuItem.Size = new System.Drawing.Size(190, 32);
            this.setFirstToolStripMenuItem.Text = "Set First";
            this.setFirstToolStripMenuItem.Click += new System.EventHandler(this.setFirstToolStripMenuItem_Click);
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tabControl, 0, 1);
            this.tblMain.Controls.Add(this.flpButtons, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.Size = new System.Drawing.Size(1044, 403);
            this.tblMain.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(tabPageScreenshots);
            this.tabControl.Controls.Add(this.tabPageDemos);
            this.tabControl.Controls.Add(this.tabPageSaveGames);
            this.tabControl.Controls.Add(this.tabPageStatistics);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.Location = new System.Drawing.Point(4, 45);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1036, 353);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            // 
            // tabPageDemos
            // 
            this.tabPageDemos.Controls.Add(this.ctrlDemoView);
            this.tabPageDemos.Location = new System.Drawing.Point(4, 29);
            this.tabPageDemos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPageDemos.Name = "tabPageDemos";
            this.tabPageDemos.Size = new System.Drawing.Size(1027, 324);
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
            this.ctrlDemoView.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.ctrlDemoView.Name = "ctrlDemoView";
            this.ctrlDemoView.Size = new System.Drawing.Size(1027, 324);
            this.ctrlDemoView.TabIndex = 0;
            // 
            // tabPageSaveGames
            // 
            this.tabPageSaveGames.Controls.Add(this.ctrlSaveGameView);
            this.tabPageSaveGames.Location = new System.Drawing.Point(4, 29);
            this.tabPageSaveGames.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageSaveGames.Name = "tabPageSaveGames";
            this.tabPageSaveGames.Size = new System.Drawing.Size(1027, 324);
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
            this.ctrlSaveGameView.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.ctrlSaveGameView.Name = "ctrlSaveGameView";
            this.ctrlSaveGameView.Size = new System.Drawing.Size(1027, 324);
            this.ctrlSaveGameView.TabIndex = 1;
            // 
            // tabPageStatistics
            // 
            this.tabPageStatistics.Controls.Add(this.ctrlViewStats);
            this.tabPageStatistics.Location = new System.Drawing.Point(4, 29);
            this.tabPageStatistics.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPageStatistics.Name = "tabPageStatistics";
            this.tabPageStatistics.Size = new System.Drawing.Size(1027, 324);
            this.tabPageStatistics.TabIndex = 3;
            this.tabPageStatistics.Text = "Statistics";
            this.tabPageStatistics.UseVisualStyleBackColor = true;
            // 
            // ctrlViewStats
            // 
            this.ctrlViewStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.ctrlViewStats.DataSourceAdapter = null;
            this.ctrlViewStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlViewStats.ForeColor = System.Drawing.Color.White;
            this.ctrlViewStats.GameFile = null;
            this.ctrlViewStats.Location = new System.Drawing.Point(0, 0);
            this.ctrlViewStats.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.ctrlViewStats.Name = "ctrlViewStats";
            this.ctrlViewStats.Size = new System.Drawing.Size(1027, 324);
            this.ctrlViewStats.TabIndex = 0;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnCopy);
            this.flpButtons.Controls.Add(this.btnCopyAll);
            this.flpButtons.Controls.Add(this.btnDelete);
            this.flpButtons.Controls.Add(this.btnAddFile);
            this.flpButtons.Controls.Add(this.btnOpenFile);
            this.flpButtons.Controls.Add(this.btnEdit);
            this.flpButtons.Controls.Add(this.btnMoveUp);
            this.flpButtons.Controls.Add(this.btnMoveDown);
            this.flpButtons.Controls.Add(this.btnSetFirst);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.Location = new System.Drawing.Point(0, 0);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(1044, 40);
            this.flpButtons.TabIndex = 2;
            // 
            // btnCopy
            // 
            this.btnCopy.Image = global::DoomLauncher.Properties.Resources.Export;
            this.btnCopy.Location = new System.Drawing.Point(0, 0);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(0);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(40, 40);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.Image = global::DoomLauncher.Properties.Resources.ExportAll;
            this.btnCopyAll.Location = new System.Drawing.Point(40, 0);
            this.btnCopyAll.Margin = new System.Windows.Forms.Padding(0);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(40, 40);
            this.btnCopyAll.TabIndex = 1;
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::DoomLauncher.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(80, 0);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(40, 40);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.Image = global::DoomLauncher.Properties.Resources.File;
            this.btnAddFile.Location = new System.Drawing.Point(120, 0);
            this.btnAddFile.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(40, 40);
            this.btnAddFile.TabIndex = 3;
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Image = global::DoomLauncher.Properties.Resources.FolderOpen;
            this.btnOpenFile.Location = new System.Drawing.Point(160, 0);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(40, 40);
            this.btnOpenFile.TabIndex = 4;
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Image = global::DoomLauncher.Properties.Resources.Edit;
            this.btnEdit.Location = new System.Drawing.Point(200, 0);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(40, 40);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Image = global::DoomLauncher.Properties.Resources.UpArrow;
            this.btnMoveUp.Location = new System.Drawing.Point(240, 0);
            this.btnMoveUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(40, 40);
            this.btnMoveUp.TabIndex = 6;
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Image = global::DoomLauncher.Properties.Resources.DownArrow;
            this.btnMoveDown.Location = new System.Drawing.Point(280, 0);
            this.btnMoveDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(40, 40);
            this.btnMoveDown.TabIndex = 7;
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnSetFirst
            // 
            this.btnSetFirst.Image = global::DoomLauncher.Properties.Resources.StepBack;
            this.btnSetFirst.Location = new System.Drawing.Point(320, 0);
            this.btnSetFirst.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetFirst.Name = "btnSetFirst";
            this.btnSetFirst.Size = new System.Drawing.Size(40, 40);
            this.btnSetFirst.TabIndex = 8;
            this.btnSetFirst.UseVisualStyleBackColor = true;
            this.btnSetFirst.Click += new System.EventHandler(this.btnSetFirst_Click);
            // 
            // GameFileAssociationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "GameFileAssociationView";
            this.Size = new System.Drawing.Size(1044, 403);
            tabPageScreenshots.ResumeLayout(false);
            this.mnuOptions.ResumeLayout(false);
            this.tblMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageDemos.ResumeLayout(false);
            this.tabPageSaveGames.ResumeLayout(false);
            this.tabPageStatistics.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CTabControl tabControl;
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
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageStatistics;
        private StatisticsView ctrlViewStats;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private FormButton btnCopy;
        private FormButton btnCopyAll;
        private FormButton btnDelete;
        private FormButton btnAddFile;
        private FormButton btnOpenFile;
        private FormButton btnEdit;
        private FormButton btnMoveUp;
        private FormButton btnMoveDown;
        private FormButton btnSetFirst;
    }
}
