namespace DoomLauncher.Forms
{
    partial class ScreenshotViewerForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenshotViewerForm));
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.tblButtons = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSlideshow = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.tblContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tblData = new System.Windows.Forms.TableLayoutPanel();
            this.statisticsView = new DoomLauncher.StatisticsView();
            this.statsControl = new DoomLauncher.StatsControl();
            this.lblTitle = new DoomLauncher.GrowLabel();
            this.flpData = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDescription = new DoomLauncher.GrowLabel();
            this.tblMain.SuspendLayout();
            this.tblButtons.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tblContainer.SuspendLayout();
            this.tblData.SuspendLayout();
            this.flpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tblButtons, 0, 1);
            this.tblMain.Controls.Add(this.tblContainer, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tblMain.Size = new System.Drawing.Size(1378, 844);
            this.tblMain.TabIndex = 1;
            // 
            // tblButtons
            // 
            this.tblButtons.BackColor = System.Drawing.SystemColors.Control;
            this.tblButtons.ColumnCount = 4;
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.Controls.Add(this.toolStrip1, 0, 0);
            this.tblButtons.Controls.Add(this.btnNext, 2, 0);
            this.tblButtons.Controls.Add(this.btnPrev, 1, 0);
            this.tblButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblButtons.Location = new System.Drawing.Point(0, 800);
            this.tblButtons.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.tblButtons.Name = "tblButtons";
            this.tblButtons.RowCount = 1;
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblButtons.Size = new System.Drawing.Size(1378, 41);
            this.tblButtons.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnSlideshow,
            this.btnEdit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 4);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(120, 33);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::DoomLauncher.Properties.Resources.Save;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(34, 28);
            this.btnSave.Text = "Save Image";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSlideshow
            // 
            this.btnSlideshow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSlideshow.Image = global::DoomLauncher.Properties.Resources.Video;
            this.btnSlideshow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSlideshow.Name = "btnSlideshow";
            this.btnSlideshow.Size = new System.Drawing.Size(34, 28);
            this.btnSlideshow.Text = "Slideshow";
            this.btnSlideshow.Click += new System.EventHandler(this.btnSlideshow_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = global::DoomLauncher.Properties.Resources.Edit;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(34, 28);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnNext
            // 
            this.btnNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("MS Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btnNext.Location = new System.Drawing.Point(692, 0);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(77, 41);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "⇨";
            this.btnNext.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrev.Font = new System.Drawing.Font("MS Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrev.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btnPrev.Location = new System.Drawing.Point(609, 0);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(77, 41);
            this.btnPrev.TabIndex = 1;
            this.btnPrev.Text = "⇦";
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // tblContainer
            // 
            this.tblContainer.ColumnCount = 2;
            this.tblContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblContainer.Controls.Add(this.tblData, 1, 0);
            this.tblContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblContainer.Location = new System.Drawing.Point(0, 0);
            this.tblContainer.Margin = new System.Windows.Forms.Padding(0);
            this.tblContainer.Name = "tblContainer";
            this.tblContainer.RowCount = 1;
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblContainer.Size = new System.Drawing.Size(1378, 800);
            this.tblContainer.TabIndex = 2;
            // 
            // tblData
            // 
            this.tblData.ColumnCount = 1;
            this.tblData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblData.Controls.Add(this.statisticsView, 0, 2);
            this.tblData.Controls.Add(this.statsControl, 0, 1);
            this.tblData.Controls.Add(this.lblTitle, 0, 0);
            this.tblData.Controls.Add(this.flpData, 0, 3);
            this.tblData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblData.Location = new System.Drawing.Point(689, 0);
            this.tblData.Margin = new System.Windows.Forms.Padding(0);
            this.tblData.Name = "tblData";
            this.tblData.RowCount = 4;
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblData.Size = new System.Drawing.Size(689, 800);
            this.tblData.TabIndex = 0;
            // 
            // statisticsView
            // 
            this.statisticsView.DataSourceAdapter = null;
            this.statisticsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statisticsView.GameFile = null;
            this.statisticsView.Location = new System.Drawing.Point(4, 190);
            this.statisticsView.Margin = new System.Windows.Forms.Padding(4, 10, 4, 5);
            this.statisticsView.Name = "statisticsView";
            this.statisticsView.Size = new System.Drawing.Size(681, 185);
            this.statisticsView.TabIndex = 2;
            // 
            // statsControl
            // 
            this.statsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsControl.Location = new System.Drawing.Point(4, 65);
            this.statsControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statsControl.Name = "statsControl";
            this.statsControl.Size = new System.Drawing.Size(681, 110);
            this.statsControl.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.IsPath = false;
            this.lblTitle.Location = new System.Drawing.Point(313, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(63, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            this.lblTitle.UseMnemonic = false;
            // 
            // flpData
            // 
            this.flpData.Controls.Add(this.lblDescription);
            this.flpData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpData.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpData.Location = new System.Drawing.Point(0, 380);
            this.flpData.Margin = new System.Windows.Forms.Padding(0);
            this.flpData.Name = "flpData";
            this.flpData.Size = new System.Drawing.Size(689, 420);
            this.flpData.TabIndex = 2;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.IsPath = false;
            this.lblDescription.Location = new System.Drawing.Point(6, 20);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(6, 20, 6, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(109, 25);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description";
            // 
            // ScreenshotViewerForm
            // 
            this.ClientSize = new System.Drawing.Size(1378, 844);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScreenshotViewerForm";
            this.tblMain.ResumeLayout(false);
            this.tblButtons.ResumeLayout(false);
            this.tblButtons.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tblContainer.ResumeLayout(false);
            this.tblData.ResumeLayout(false);
            this.tblData.PerformLayout();
            this.flpData.ResumeLayout(false);
            this.flpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanelDB tblMain;
        private System.Windows.Forms.TableLayoutPanel tblButtons;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnSlideshow;
        private System.Windows.Forms.TableLayoutPanel tblContainer;
        private System.Windows.Forms.TableLayoutPanel tblData;
        private GrowLabel lblDescription;
        private GrowLabel lblTitle;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.FlowLayoutPanel flpData;
        private StatisticsView statisticsView;
        private StatsControl statsControl;
    }
}