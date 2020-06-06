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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lnkRepository = new System.Windows.Forms.LinkLabel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lnkThread = new System.Windows.Forms.LinkLabel();
            this.lnkThread2 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.lnkRepository, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.okButton, 1, 6);
            this.tableLayoutPanel.Controls.Add(this.label1, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.lblAuthor, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 1, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(417, 265);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // lnkRepository
            // 
            this.lnkRepository.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkRepository.AutoSize = true;
            this.lnkRepository.Location = new System.Drawing.Point(140, 134);
            this.lnkRepository.Name = "lnkRepository";
            this.lnkRepository.Size = new System.Drawing.Size(93, 13);
            this.lnkRepository.TabIndex = 28;
            this.lnkRepository.TabStop = true;
            this.lnkRepository.Text = "GitHub Repository";
            this.lnkRepository.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkRepository.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRepository_LinkClicked);
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(4, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 7);
            this.logoPictureBox.Size = new System.Drawing.Size(128, 128);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.Location = new System.Drawing.Point(143, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(271, 17);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Product Name";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.Location = new System.Drawing.Point(143, 32);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(271, 17);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(339, 239);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(140, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(274, 32);
            this.label1.TabIndex = 26;
            this.label1.Text = "Please check out these links to post questions, comments, bugs, and feature reque" +
    "sts!\r\n";
            this.label1.UseMnemonic = false;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAuthor.Location = new System.Drawing.Point(143, 64);
            this.lblAuthor.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(271, 32);
            this.lblAuthor.TabIndex = 28;
            this.lblAuthor.Text = "Author: hobomaster22";
            this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lnkThread);
            this.flowLayoutPanel1.Controls.Add(this.lnkThread2);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(137, 157);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(209, 22);
            this.flowLayoutPanel1.TabIndex = 29;
            // 
            // lnkThread
            // 
            this.lnkThread.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkThread.AutoSize = true;
            this.lnkThread.Location = new System.Drawing.Point(3, 0);
            this.lnkThread.Name = "lnkThread";
            this.lnkThread.Size = new System.Drawing.Size(97, 13);
            this.lnkThread.TabIndex = 27;
            this.lnkThread.TabStop = true;
            this.lnkThread.Text = "Doomworld Thread";
            this.lnkThread.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkThread.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkThread_LinkClicked);
            // 
            // lnkThread2
            // 
            this.lnkThread2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lnkThread2.AutoSize = true;
            this.lnkThread2.Location = new System.Drawing.Point(106, 0);
            this.lnkThread2.Name = "lnkThread2";
            this.lnkThread2.Size = new System.Drawing.Size(90, 13);
            this.lnkThread2.TabIndex = 26;
            this.lnkThread2.TabStop = true;
            this.lnkThread2.Text = "Realm 667 Forum";
            this.lnkThread2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkThread2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkThread2_LinkClicked);
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 283);
            this.Controls.Add(this.tableLayoutPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutBox";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
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
    }
}
