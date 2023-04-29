namespace DoomLauncher
{
    partial class SpecificFilesForm
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
        [System.CodeDom.Compiler.GeneratedCode("Winform Designer", "VS2015 SP1")]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecificFilesForm));
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.clbFiles = new System.Windows.Forms.CheckedListBox();
            this.tblInner = new System.Windows.Forms.TableLayoutPanel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlLinks = new System.Windows.Forms.Panel();
            this.chkPkContents = new System.Windows.Forms.CheckBox();
            this.chkSupported = new System.Windows.Forms.CheckBox();
            this.pnl = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.lnkSelect = new System.Windows.Forms.LinkLabel();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.tblMain.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.tblInner.SuspendLayout();
            this.pnlLinks.SuspendLayout();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.flpButtons, 0, 3);
            this.tblMain.Controls.Add(this.clbFiles, 0, 2);
            this.tblMain.Controls.Add(this.tblInner, 0, 1);
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.Size = new System.Drawing.Size(471, 555);
            this.tblMain.TabIndex = 0;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnOK);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 506);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(471, 49);
            this.flpButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(355, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(235, 5);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // clbFiles
            // 
            this.clbFiles.CheckOnClick = true;
            this.clbFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbFiles.FormattingEnabled = true;
            this.clbFiles.Location = new System.Drawing.Point(4, 167);
            this.clbFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clbFiles.Name = "clbFiles";
            this.clbFiles.Size = new System.Drawing.Size(463, 334);
            this.clbFiles.TabIndex = 1;
            // 
            // tblInner
            // 
            this.tblInner.ColumnCount = 2;
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tblInner.Controls.Add(this.btnSearch, 1, 1);
            this.tblInner.Controls.Add(this.txtSearch, 0, 1);
            this.tblInner.Controls.Add(this.pnlLinks, 1, 0);
            this.tblInner.Controls.Add(this.pnl, 0, 0);
            this.tblInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInner.Location = new System.Drawing.Point(0, 40);
            this.tblInner.Margin = new System.Windows.Forms.Padding(0);
            this.tblInner.Name = "tblInner";
            this.tblInner.RowCount = 2;
            this.tblInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tblInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.Size = new System.Drawing.Size(471, 122);
            this.tblInner.TabIndex = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(235, 83);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(112, 34);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(4, 83);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(223, 26);
            this.txtSearch.TabIndex = 7;
            // 
            // pnlLinks
            // 
            this.pnlLinks.Controls.Add(this.chkPkContents);
            this.pnlLinks.Controls.Add(this.chkSupported);
            this.pnlLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLinks.Location = new System.Drawing.Point(231, 0);
            this.pnlLinks.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLinks.Name = "pnlLinks";
            this.pnlLinks.Size = new System.Drawing.Size(240, 78);
            this.pnlLinks.TabIndex = 4;
            // 
            // chkPkContents
            // 
            this.chkPkContents.AutoSize = true;
            this.chkPkContents.Location = new System.Drawing.Point(4, 39);
            this.chkPkContents.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkPkContents.Name = "chkPkContents";
            this.chkPkContents.Size = new System.Drawing.Size(174, 24);
            this.chkPkContents.TabIndex = 4;
            this.chkPkContents.Text = "Show pk3 Contents";
            this.chkPkContents.UseVisualStyleBackColor = true;
            this.chkPkContents.CheckedChanged += new System.EventHandler(this.chkPkContents_CheckedChanged);
            // 
            // chkSupported
            // 
            this.chkSupported.AutoSize = true;
            this.chkSupported.Location = new System.Drawing.Point(4, 8);
            this.chkSupported.Margin = new System.Windows.Forms.Padding(36, 0, 0, 0);
            this.chkSupported.Name = "chkSupported";
            this.chkSupported.Size = new System.Drawing.Size(227, 24);
            this.chkSupported.TabIndex = 3;
            this.chkSupported.Text = "Supported Extensions Only";
            this.chkSupported.UseVisualStyleBackColor = true;
            this.chkSupported.CheckedChanged += new System.EventHandler(this.chkSupported_CheckedChanged);
            // 
            // pnl
            // 
            this.pnl.Controls.Add(this.lblLoading);
            this.pnl.Controls.Add(this.lnkSelect);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Margin = new System.Windows.Forms.Padding(0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(231, 78);
            this.pnl.TabIndex = 8;
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.Location = new System.Drawing.Point(4, 40);
            this.lblLoading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(78, 20);
            this.lblLoading.TabIndex = 3;
            this.lblLoading.Text = "Loading...";
            // 
            // lnkSelect
            // 
            this.lnkSelect.AutoSize = true;
            this.lnkSelect.Location = new System.Drawing.Point(4, 9);
            this.lnkSelect.Margin = new System.Windows.Forms.Padding(4, 9, 4, 0);
            this.lnkSelect.Name = "lnkSelect";
            this.lnkSelect.Size = new System.Drawing.Size(142, 20);
            this.lnkSelect.TabIndex = 2;
            this.lnkSelect.TabStop = true;
            this.lnkSelect.Text = "Select/Unselect All";
            this.lnkSelect.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelect_LinkClicked);
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(3, 3);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(465, 34);
            this.titleBar.TabIndex = 6;
            this.titleBar.Title = "Select Files";
            // 
            // SpecificFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(471, 555);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(440, 434);
            this.Name = "SpecificFilesForm";
            this.Text = "Select Files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpecificFilesForm_FormClosing);
            this.tblMain.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.tblInner.ResumeLayout(false);
            this.tblInner.PerformLayout();
            this.pnlLinks.ResumeLayout(false);
            this.pnlLinks.PerformLayout();
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckedListBox clbFiles;
        private System.Windows.Forms.LinkLabel lnkSelect;
        private System.Windows.Forms.CheckBox chkSupported;
        private System.Windows.Forms.Panel pnlLinks;
        private System.Windows.Forms.CheckBox chkPkContents;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TableLayoutPanel tblInner;
        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.Label lblLoading;
        private Controls.TitleBarControl titleBar;
    }
}