namespace DoomLauncher
{
    partial class TextBoxForm
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
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.txtText = new System.Windows.Forms.TextBox();
            this.lblHeader = new DoomLauncher.GrowLabel();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lnk = new System.Windows.Forms.LinkLabel();
            this.chk = new System.Windows.Forms.CheckBox();
            this.tblContainer = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.tblMain.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.tblContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.txtText, 0, 3);
            this.tblMain.Controls.Add(this.lblHeader, 0, 0);
            this.tblMain.Controls.Add(this.flpButtons, 0, 4);
            this.tblMain.Controls.Add(this.lnk, 0, 2);
            this.tblMain.Controls.Add(this.chk, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(4, 45);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 5;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.Size = new System.Drawing.Size(508, 445);
            this.tblMain.TabIndex = 0;
            // 
            // txtText
            // 
            this.txtText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtText.Location = new System.Drawing.Point(4, 152);
            this.txtText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtText.Size = new System.Drawing.Size(500, 239);
            this.txtText.TabIndex = 0;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.IsPath = false;
            this.lblHeader.Location = new System.Drawing.Point(4, 5);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(4, 5, 0, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(39, 20);
            this.lblHeader.TabIndex = 2;
            this.lblHeader.Text = "Text";
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnOK);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 396);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(508, 49);
            this.flpButtons.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(272, 5);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click_1);
            // 
            // lnk
            // 
            this.lnk.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnk.AutoSize = true;
            this.lnk.Location = new System.Drawing.Point(4, 112);
            this.lnk.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnk.Name = "lnk";
            this.lnk.Size = new System.Drawing.Size(38, 20);
            this.lnk.TabIndex = 4;
            this.lnk.TabStop = true;
            this.lnk.Text = "Link";
            this.lnk.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk_LinkClicked);
            // 
            // chk
            // 
            this.chk.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chk.AutoSize = true;
            this.chk.Location = new System.Drawing.Point(4, 61);
            this.chk.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chk.Name = "chk";
            this.chk.Size = new System.Drawing.Size(107, 24);
            this.chk.TabIndex = 5;
            this.chk.Text = "CheckBox";
            this.chk.UseVisualStyleBackColor = true;
            // 
            // tblContainer
            // 
            this.tblContainer.ColumnCount = 1;
            this.tblContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblContainer.Controls.Add(this.tblMain, 0, 1);
            this.tblContainer.Controls.Add(this.titleBar, 0, 0);
            this.tblContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblContainer.Location = new System.Drawing.Point(0, 0);
            this.tblContainer.Margin = new System.Windows.Forms.Padding(0);
            this.tblContainer.Name = "tblContainer";
            this.tblContainer.RowCount = 2;
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblContainer.Size = new System.Drawing.Size(516, 495);
            this.tblContainer.TabIndex = 1;
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(3, 3);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(510, 34);
            this.titleBar.TabIndex = 1;
            this.titleBar.Title = "Title";
            // 
            // TextBoxForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(516, 495);
            this.Controls.Add(this.tblContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TextBoxForm";
            this.ShowIcon = false;
            this.Text = "TextBoxForm";
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.flpButtons.ResumeLayout(false);
            this.tblContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.TextBox txtText;
        private GrowLabel lblHeader;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel lnk;
        private System.Windows.Forms.CheckBox chk;
        private System.Windows.Forms.TableLayoutPanel tblContainer;
        private Controls.TitleBarControl titleBar;
    }
}