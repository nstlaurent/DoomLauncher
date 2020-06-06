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
            this.tblMain.Controls.Add(this.flpButtons, 0, 2);
            this.tblMain.Controls.Add(this.clbFiles, 0, 1);
            this.tblMain.Controls.Add(this.tblInner, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.Size = new System.Drawing.Size(314, 361);
            this.tblMain.TabIndex = 0;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnOK);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 329);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(314, 32);
            this.flpButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(236, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(155, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
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
            this.clbFiles.Location = new System.Drawing.Point(3, 83);
            this.clbFiles.Name = "clbFiles";
            this.clbFiles.Size = new System.Drawing.Size(308, 243);
            this.clbFiles.TabIndex = 1;
            // 
            // tblInner
            // 
            this.tblInner.ColumnCount = 2;
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tblInner.Controls.Add(this.btnSearch, 1, 1);
            this.tblInner.Controls.Add(this.txtSearch, 0, 1);
            this.tblInner.Controls.Add(this.pnlLinks, 1, 0);
            this.tblInner.Controls.Add(this.pnl, 0, 0);
            this.tblInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInner.Location = new System.Drawing.Point(0, 0);
            this.tblInner.Margin = new System.Windows.Forms.Padding(0);
            this.tblInner.Name = "tblInner";
            this.tblInner.RowCount = 2;
            this.tblInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tblInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.Size = new System.Drawing.Size(314, 80);
            this.tblInner.TabIndex = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(157, 53);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(3, 53);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(148, 20);
            this.txtSearch.TabIndex = 7;
            // 
            // pnlLinks
            // 
            this.pnlLinks.Controls.Add(this.chkPkContents);
            this.pnlLinks.Controls.Add(this.chkSupported);
            this.pnlLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLinks.Location = new System.Drawing.Point(154, 0);
            this.pnlLinks.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLinks.Name = "pnlLinks";
            this.pnlLinks.Size = new System.Drawing.Size(160, 50);
            this.pnlLinks.TabIndex = 4;
            // 
            // chkPkContents
            // 
            this.chkPkContents.AutoSize = true;
            this.chkPkContents.Location = new System.Drawing.Point(3, 25);
            this.chkPkContents.Name = "chkPkContents";
            this.chkPkContents.Size = new System.Drawing.Size(119, 17);
            this.chkPkContents.TabIndex = 4;
            this.chkPkContents.Text = "Show pk3 Contents";
            this.chkPkContents.UseVisualStyleBackColor = true;
            this.chkPkContents.CheckedChanged += new System.EventHandler(this.chkPkContents_CheckedChanged);
            // 
            // chkSupported
            // 
            this.chkSupported.AutoSize = true;
            this.chkSupported.Location = new System.Drawing.Point(3, 5);
            this.chkSupported.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.chkSupported.Name = "chkSupported";
            this.chkSupported.Size = new System.Drawing.Size(153, 17);
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
            this.pnl.Size = new System.Drawing.Size(154, 50);
            this.pnl.TabIndex = 8;
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.Location = new System.Drawing.Point(3, 26);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(54, 13);
            this.lblLoading.TabIndex = 3;
            this.lblLoading.Text = "Loading...";
            // 
            // lnkSelect
            // 
            this.lnkSelect.AutoSize = true;
            this.lnkSelect.Location = new System.Drawing.Point(3, 6);
            this.lnkSelect.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lnkSelect.Name = "lnkSelect";
            this.lnkSelect.Size = new System.Drawing.Size(98, 13);
            this.lnkSelect.TabIndex = 2;
            this.lnkSelect.TabStop = true;
            this.lnkSelect.Text = "Select/Unselect All";
            this.lnkSelect.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelect_LinkClicked);
            // 
            // SpecificFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(314, 361);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 300);
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
    }
}