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
            this.tblContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tblData = new System.Windows.Forms.TableLayoutPanel();
            this.statisticsView = new DoomLauncher.StatisticsView();
            this.statsControl = new DoomLauncher.StatsControl();
            this.lblTitle = new DoomLauncher.GrowLabel();
            this.flpData = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDescription = new DoomLauncher.GrowLabel();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.tblButtons = new System.Windows.Forms.TableLayoutPanel();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new DoomLauncher.FormButton();
            this.btnSlideshow = new DoomLauncher.FormButton();
            this.btnEdit = new DoomLauncher.FormButton();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.tblMain.SuspendLayout();
            this.tblContainer.SuspendLayout();
            this.tblData.SuspendLayout();
            this.flpData.SuspendLayout();
            this.tblButtons.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tblContainer, 0, 1);
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Controls.Add(this.tblButtons, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.Size = new System.Drawing.Size(1378, 844);
            this.tblMain.TabIndex = 1;
            // 
            // tblContainer
            // 
            this.tblContainer.AutoScroll = true;
            this.tblContainer.ColumnCount = 2;
            this.tblContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblContainer.Controls.Add(this.tblData, 1, 0);
            this.tblContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblContainer.Location = new System.Drawing.Point(0, 40);
            this.tblContainer.Margin = new System.Windows.Forms.Padding(0);
            this.tblContainer.Name = "tblContainer";
            this.tblContainer.RowCount = 1;
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblContainer.Size = new System.Drawing.Size(1378, 760);
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
            this.tblData.Size = new System.Drawing.Size(689, 760);
            this.tblData.TabIndex = 0;
            // 
            // statisticsView
            // 
            this.statisticsView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.statisticsView.DataSourceAdapter = null;
            this.statisticsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statisticsView.ForeColor = System.Drawing.Color.White;
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
            this.flpData.Size = new System.Drawing.Size(689, 380);
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
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(1378, 40);
            this.titleBar.TabIndex = 3;
            this.titleBar.Title = "Screenshot";
            // 
            // tblButtons
            // 
            this.tblButtons.ColumnCount = 4;
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.Controls.Add(this.flpButtons, 0, 0);
            this.tblButtons.Controls.Add(this.btnPrev, 1, 0);
            this.tblButtons.Controls.Add(this.btnNext, 2, 0);
            this.tblButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblButtons.Location = new System.Drawing.Point(0, 800);
            this.tblButtons.Margin = new System.Windows.Forms.Padding(0);
            this.tblButtons.Name = "tblButtons";
            this.tblButtons.RowCount = 1;
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblButtons.Size = new System.Drawing.Size(1378, 44);
            this.tblButtons.TabIndex = 4;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnSave);
            this.flpButtons.Controls.Add(this.btnSlideshow);
            this.flpButtons.Controls.Add(this.btnEdit);
            this.flpButtons.Location = new System.Drawing.Point(0, 0);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(134, 40);
            this.flpButtons.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::DoomLauncher.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(0, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(40, 40);
            this.btnSave.TabIndex = 0;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSlideshow
            // 
            this.btnSlideshow.Image = global::DoomLauncher.Properties.Resources.Video;
            this.btnSlideshow.Location = new System.Drawing.Point(40, 0);
            this.btnSlideshow.Margin = new System.Windows.Forms.Padding(0);
            this.btnSlideshow.Name = "btnSlideshow";
            this.btnSlideshow.Size = new System.Drawing.Size(40, 40);
            this.btnSlideshow.TabIndex = 1;
            this.btnSlideshow.UseVisualStyleBackColor = true;
            this.btnSlideshow.Click += new System.EventHandler(this.btnSlideshow_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Image = global::DoomLauncher.Properties.Resources.Edit;
            this.btnEdit.Location = new System.Drawing.Point(80, 0);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(40, 40);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrev.Font = new System.Drawing.Font("MS Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrev.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPrev.Location = new System.Drawing.Point(609, 0);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(77, 44);
            this.btnPrev.TabIndex = 1;
            this.btnPrev.Text = "⇦";
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("MS Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNext.Location = new System.Drawing.Point(692, 0);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(77, 44);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "⇨";
            this.btnNext.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // ScreenshotViewerForm
            // 
            this.ClientSize = new System.Drawing.Size(1378, 844);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScreenshotViewerForm";
            this.tblMain.ResumeLayout(false);
            this.tblContainer.ResumeLayout(false);
            this.tblData.ResumeLayout(false);
            this.tblData.PerformLayout();
            this.flpData.ResumeLayout(false);
            this.flpData.PerformLayout();
            this.tblButtons.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanelDB tblMain;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.TableLayoutPanel tblContainer;
        private System.Windows.Forms.TableLayoutPanel tblData;
        private GrowLabel lblDescription;
        private GrowLabel lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flpData;
        private StatisticsView statisticsView;
        private StatsControl statsControl;
        private Controls.TitleBarControl titleBar;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private FormButton btnSave;
        private FormButton btnSlideshow;
        private FormButton btnEdit;
        private System.Windows.Forms.TableLayoutPanel tblButtons;
    }
}