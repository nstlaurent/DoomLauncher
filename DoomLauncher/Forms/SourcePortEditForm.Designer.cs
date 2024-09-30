namespace DoomLauncher
{
    partial class SourcePortEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourcePortEditForm));
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.sourcePortEdit1 = new DoomLauncher.SourcePortEdit();
            this.grpAdditionalFiles = new System.Windows.Forms.GroupBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pbInfo = new System.Windows.Forms.PictureBox();
            this.ctrlFiles = new DoomLauncher.FilesCtrl();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.flpInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.pbInfo2 = new System.Windows.Forms.PictureBox();
            this.table2 = new DoomLauncher.TableLayoutPanelDB();
            this.tblReplacementVars = new DoomLauncher.TableLayoutPanelDB();
            this.lblReplacementInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tblMain.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpAdditionalFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).BeginInit();
            this.flpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo2)).BeginInit();
            this.table2.SuspendLayout();
            this.tblReplacementVars.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tblMain.Controls.Add(this.groupBox2, 0, 1);
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Controls.Add(this.grpAdditionalFiles, 0, 3);
            this.tblMain.Controls.Add(this.flpInfo, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 5;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 244F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 246F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.Size = new System.Drawing.Size(475, 658);
            this.tblMain.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 610);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(475, 48);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(371, 4);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(263, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.sourcePortEdit1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(4, 36);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(467, 236);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // sourcePortEdit1
            // 
            this.sourcePortEdit1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.sourcePortEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcePortEdit1.ForeColor = System.Drawing.Color.White;
            this.sourcePortEdit1.Location = new System.Drawing.Point(4, 19);
            this.sourcePortEdit1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.sourcePortEdit1.Name = "sourcePortEdit1";
            this.sourcePortEdit1.Size = new System.Drawing.Size(459, 213);
            this.sourcePortEdit1.TabIndex = 0;
            // 
            // grpAdditionalFiles
            // 
            this.grpAdditionalFiles.Controls.Add(this.lblInfo);
            this.grpAdditionalFiles.Controls.Add(this.pbInfo);
            this.grpAdditionalFiles.Controls.Add(this.ctrlFiles);
            this.grpAdditionalFiles.Location = new System.Drawing.Point(4, 364);
            this.grpAdditionalFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpAdditionalFiles.Name = "grpAdditionalFiles";
            this.grpAdditionalFiles.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpAdditionalFiles.Size = new System.Drawing.Size(467, 238);
            this.grpAdditionalFiles.TabIndex = 3;
            this.grpAdditionalFiles.TabStop = false;
            this.grpAdditionalFiles.Text = "Additional Files";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(41, 23);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(37, 16);
            this.lblInfo.TabIndex = 23;
            this.lblInfo.Text = "label";
            // 
            // pbInfo
            // 
            this.pbInfo.Location = new System.Drawing.Point(12, 23);
            this.pbInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(21, 20);
            this.pbInfo.TabIndex = 22;
            this.pbInfo.TabStop = false;
            // 
            // ctrlFiles
            // 
            this.ctrlFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.ctrlFiles.ForeColor = System.Drawing.Color.White;
            this.ctrlFiles.Location = new System.Drawing.Point(12, 63);
            this.ctrlFiles.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ctrlFiles.Name = "ctrlFiles";
            this.ctrlFiles.Size = new System.Drawing.Size(443, 169);
            this.ctrlFiles.TabIndex = 21;
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
            this.titleBar.Size = new System.Drawing.Size(475, 32);
            this.titleBar.TabIndex = 5;
            this.titleBar.Title = "Source Port";
            // 
            // flpInfo
            // 
            this.flpInfo.Controls.Add(this.pbInfo2);
            this.flpInfo.Controls.Add(this.table2);
            this.flpInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpInfo.Location = new System.Drawing.Point(0, 276);
            this.flpInfo.Margin = new System.Windows.Forms.Padding(0);
            this.flpInfo.Name = "flpInfo";
            this.flpInfo.Size = new System.Drawing.Size(475, 84);
            this.flpInfo.TabIndex = 6;
            // 
            // pbInfo2
            // 
            this.pbInfo2.Location = new System.Drawing.Point(8, 8);
            this.pbInfo2.Margin = new System.Windows.Forms.Padding(8, 8, 4, 4);
            this.pbInfo2.Name = "pbInfo2";
            this.pbInfo2.Size = new System.Drawing.Size(21, 20);
            this.pbInfo2.TabIndex = 24;
            this.pbInfo2.TabStop = false;
            // 
            // table2
            // 
            this.table2.ColumnCount = 1;
            this.table2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table2.Controls.Add(this.lblReplacementInfo, 0, 0);
            this.table2.Controls.Add(this.tblReplacementVars, 0, 1);
            this.table2.Location = new System.Drawing.Point(33, 0);
            this.table2.Margin = new System.Windows.Forms.Padding(0);
            this.table2.Name = "table2";
            this.table2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.table2.RowCount = 2;
            this.table2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.table2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table2.Size = new System.Drawing.Size(442, 80);
            this.table2.TabIndex = 25;
            // 
            // tblReplacementVars
            // 
            this.tblReplacementVars.ColumnCount = 2;
            this.tblReplacementVars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tblReplacementVars.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblReplacementVars.Controls.Add(this.label4, 1, 1);
            this.tblReplacementVars.Controls.Add(this.label3, 0, 1);
            this.tblReplacementVars.Controls.Add(this.label2, 1, 0);
            this.tblReplacementVars.Controls.Add(this.label1, 0, 0);
            this.tblReplacementVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblReplacementVars.Location = new System.Drawing.Point(0, 32);
            this.tblReplacementVars.Margin = new System.Windows.Forms.Padding(0);
            this.tblReplacementVars.Name = "tblReplacementVars";
            this.tblReplacementVars.RowCount = 2;
            this.tblReplacementVars.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblReplacementVars.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblReplacementVars.Size = new System.Drawing.Size(442, 44);
            this.tblReplacementVars.TabIndex = 0;
            // 
            // lblReplacementInfo
            // 
            this.lblReplacementInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblReplacementInfo.AutoSize = true;
            this.lblReplacementInfo.Location = new System.Drawing.Point(4, 8);
            this.lblReplacementInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReplacementInfo.Name = "lblReplacementInfo";
            this.lblReplacementInfo.Size = new System.Drawing.Size(149, 16);
            this.lblReplacementInfo.TabIndex = 26;
            this.lblReplacementInfo.Text = "Replacement Variables";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "$filename";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "FIle name of the launched file";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "$iwad";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(103, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "IWAD file name";
            // 
            // SourcePortEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(475, 658);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SourcePortEditForm";
            this.Text = "Source Port";
            this.tblMain.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.grpAdditionalFiles.ResumeLayout(false);
            this.grpAdditionalFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).EndInit();
            this.flpInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo2)).EndInit();
            this.table2.ResumeLayout(false);
            this.table2.PerformLayout();
            this.tblReplacementVars.ResumeLayout(false);
            this.tblReplacementVars.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpAdditionalFiles;
        private FilesCtrl ctrlFiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private SourcePortEdit sourcePortEdit1;
        private System.Windows.Forms.PictureBox pbInfo;
        private System.Windows.Forms.Label lblInfo;
        private Controls.TitleBarControl titleBar;
        private System.Windows.Forms.FlowLayoutPanel flpInfo;
        private System.Windows.Forms.PictureBox pbInfo2;
        private TableLayoutPanelDB table2;
        private System.Windows.Forms.Label lblReplacementInfo;
        private TableLayoutPanelDB tblReplacementVars;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}