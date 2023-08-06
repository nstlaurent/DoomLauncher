namespace DoomLauncher
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
        [System.CodeDom.Compiler.GeneratedCode("Winform Designer", "VS2015 SP1")]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.tblInfo = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lnkRepository = new System.Windows.Forms.LinkLabel();
            this.lnkThread = new System.Windows.Forms.LinkLabel();
            this.lnkThread2 = new System.Windows.Forms.LinkLabel();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.lblWindowsVersion = new System.Windows.Forms.Label();
            this.tblInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblInfo
            // 
            this.tblInfo.ColumnCount = 2;
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tblInfo.Controls.Add(this.logoPictureBox, 0, 0);
            this.tblInfo.Controls.Add(this.labelProductName, 1, 0);
            this.tblInfo.Controls.Add(this.labelVersion, 1, 1);
            this.tblInfo.Controls.Add(this.okButton, 1, 6);
            this.tblInfo.Controls.Add(this.label1, 1, 4);
            this.tblInfo.Controls.Add(this.lblAuthor, 1, 3);
            this.tblInfo.Controls.Add(this.flowLayoutPanel1, 1, 5);
            this.tblInfo.Controls.Add(this.lblWindowsVersion, 1, 2);
            this.tblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInfo.Location = new System.Drawing.Point(4, 45);
            this.tblInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblInfo.Name = "tblInfo";
            this.tblInfo.RowCount = 7;
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblInfo.Size = new System.Drawing.Size(644, 386);
            this.tblInfo.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(10, 5);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tblInfo.SetRowSpan(this.logoPictureBox, 7);
            this.logoPictureBox.Size = new System.Drawing.Size(192, 197);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Location = new System.Drawing.Point(221, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 26);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(419, 26);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Product Name";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Location = new System.Drawing.Point(221, 49);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 26);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(419, 26);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(528, 346);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(112, 35);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(221, 196);
            this.label1.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(419, 49);
            this.label1.TabIndex = 26;
            this.label1.Text = "Please check out these links to post questions, comments, bugs, and feature reque" +
    "sts!\r\n";
            this.label1.UseMnemonic = false;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAuthor.Location = new System.Drawing.Point(221, 147);
            this.lblAuthor.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(419, 49);
            this.lblAuthor.TabIndex = 28;
            this.lblAuthor.Text = "Author: Nick St Laurent";
            this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lnkRepository);
            this.flowLayoutPanel1.Controls.Add(this.lnkThread);
            this.flowLayoutPanel1.Controls.Add(this.lnkThread2);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(216, 245);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(411, 81);
            this.flowLayoutPanel1.TabIndex = 29;
            // 
            // lnkRepository
            // 
            this.lnkRepository.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkRepository.Location = new System.Drawing.Point(4, 0);
            this.lnkRepository.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkRepository.Name = "lnkRepository";
            this.lnkRepository.Size = new System.Drawing.Size(300, 38);
            this.lnkRepository.TabIndex = 28;
            this.lnkRepository.TabStop = true;
            this.lnkRepository.Text = "GitHub Repository";
            this.lnkRepository.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkRepository.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRepository_LinkClicked);
            // 
            // lnkThread
            // 
            this.lnkThread.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkThread.Location = new System.Drawing.Point(4, 38);
            this.lnkThread.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkThread.Name = "lnkThread";
            this.lnkThread.Size = new System.Drawing.Size(300, 38);
            this.lnkThread.TabIndex = 27;
            this.lnkThread.TabStop = true;
            this.lnkThread.Text = "Doomworld Thread";
            this.lnkThread.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkThread.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkThread_LinkClicked);
            // 
            // lnkThread2
            // 
            this.lnkThread2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkThread2.Location = new System.Drawing.Point(312, 0);
            this.lnkThread2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkThread2.Name = "lnkThread2";
            this.lnkThread2.Size = new System.Drawing.Size(300, 38);
            this.lnkThread2.TabIndex = 26;
            this.lnkThread2.TabStop = true;
            this.lnkThread2.Text = "Realm 667 Forum";
            this.lnkThread2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkThread2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkThread2_LinkClicked);
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Controls.Add(this.tblInfo, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Size = new System.Drawing.Size(652, 436);
            this.tblMain.TabIndex = 1;
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.CanClose = true;
            this.titleBar.ControlBox = true;
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.RememberNormalSize = true;
            this.titleBar.Size = new System.Drawing.Size(652, 40);
            this.titleBar.TabIndex = 0;
            this.titleBar.Title = "Doom Launcher";
            // 
            // lblWindowsVersion
            // 
            this.lblWindowsVersion.AutoSize = true;
            this.lblWindowsVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWindowsVersion.Location = new System.Drawing.Point(265, 118);
            this.lblWindowsVersion.Margin = new System.Windows.Forms.Padding(9, 0, 4, 0);
            this.lblWindowsVersion.Name = "lblWindowsVersion";
            this.lblWindowsVersion.Size = new System.Drawing.Size(503, 59);
            this.lblWindowsVersion.TabIndex = 30;
            this.lblWindowsVersion.Text = "Windows Version";
            this.lblWindowsVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 436);
            this.Controls.Add(this.tblMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutBox";
            this.tblInfo.ResumeLayout(false);
            this.tblInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tblMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblInfo;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.LinkLabel lnkRepository;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.LinkLabel lnkThread;
        private System.Windows.Forms.LinkLabel lnkThread2;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private Controls.TitleBarControl titleBar;
        private System.Windows.Forms.Label lblWindowsVersion;
    }
}
