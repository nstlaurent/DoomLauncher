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
            this.titleBarControl1 = new DoomLauncher.Controls.TitleBarControl();
            this.tblMain.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpAdditionalFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.tblMain.Controls.Add(this.groupBox2, 0, 1);
            this.tblMain.Controls.Add(this.grpAdditionalFiles, 0, 2);
            this.tblMain.Controls.Add(this.titleBarControl1, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 305F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 308F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 308F));
            this.tblMain.Size = new System.Drawing.Size(534, 669);
            this.tblMain.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 658);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(534, 303);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(418, 5);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(298, 5);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 35);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.sourcePortEdit1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(4, 45);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(526, 295);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // sourcePortEdit1
            // 
            this.sourcePortEdit1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.sourcePortEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcePortEdit1.ForeColor = System.Drawing.Color.White;
            this.sourcePortEdit1.Location = new System.Drawing.Point(4, 24);
            this.sourcePortEdit1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.sourcePortEdit1.Name = "sourcePortEdit1";
            this.sourcePortEdit1.Size = new System.Drawing.Size(518, 266);
            this.sourcePortEdit1.TabIndex = 0;
            // 
            // grpAdditionalFiles
            // 
            this.grpAdditionalFiles.Controls.Add(this.lblInfo);
            this.grpAdditionalFiles.Controls.Add(this.pbInfo);
            this.grpAdditionalFiles.Controls.Add(this.ctrlFiles);
            this.grpAdditionalFiles.Location = new System.Drawing.Point(4, 350);
            this.grpAdditionalFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpAdditionalFiles.Name = "grpAdditionalFiles";
            this.grpAdditionalFiles.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpAdditionalFiles.Size = new System.Drawing.Size(525, 298);
            this.grpAdditionalFiles.TabIndex = 3;
            this.grpAdditionalFiles.TabStop = false;
            this.grpAdditionalFiles.Text = "Additional Files";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(46, 29);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(42, 20);
            this.lblInfo.TabIndex = 23;
            this.lblInfo.Text = "label";
            // 
            // pbInfo
            // 
            this.pbInfo.Location = new System.Drawing.Point(14, 29);
            this.pbInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(24, 25);
            this.pbInfo.TabIndex = 22;
            this.pbInfo.TabStop = false;
            // 
            // ctrlFiles
            // 
            this.ctrlFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.ctrlFiles.ForeColor = System.Drawing.Color.White;
            this.ctrlFiles.Location = new System.Drawing.Point(14, 79);
            this.ctrlFiles.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ctrlFiles.Name = "ctrlFiles";
            this.ctrlFiles.Size = new System.Drawing.Size(498, 211);
            this.ctrlFiles.TabIndex = 21;
            // 
            // titleBarControl1
            // 
            this.titleBarControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBarControl1.ForeColor = System.Drawing.Color.White;
            this.titleBarControl1.Location = new System.Drawing.Point(3, 3);
            this.titleBarControl1.Name = "titleBarControl1";
            this.titleBarControl1.Size = new System.Drawing.Size(528, 34);
            this.titleBarControl1.TabIndex = 5;
            this.titleBarControl1.Title = "Source Port";
            // 
            // SourcePortEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(534, 669);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SourcePortEditForm";
            this.Text = "Source Port";
            this.tblMain.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.grpAdditionalFiles.ResumeLayout(false);
            this.grpAdditionalFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).EndInit();
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
        private Controls.TitleBarControl titleBarControl1;
    }
}