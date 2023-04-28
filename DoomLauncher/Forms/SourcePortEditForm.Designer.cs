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
            this.tblMain.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tblMain.Controls.Add(this.groupBox2, 0, 0);
            this.tblMain.Controls.Add(this.grpAdditionalFiles, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 244F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 246F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 246F));
            this.tblMain.Size = new System.Drawing.Size(475, 535);
            this.tblMain.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 494);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(475, 242);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(371, 4);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
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
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
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
            this.groupBox2.Location = new System.Drawing.Point(4, 4);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(467, 236);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // sourcePortEdit1
            // 
            this.sourcePortEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcePortEdit1.Location = new System.Drawing.Point(4, 19);
            this.sourcePortEdit1.Margin = new System.Windows.Forms.Padding(5);
            this.sourcePortEdit1.Name = "sourcePortEdit1";
            this.sourcePortEdit1.Size = new System.Drawing.Size(459, 213);
            this.sourcePortEdit1.TabIndex = 0;
            // 
            // grpAdditionalFiles
            // 
            this.grpAdditionalFiles.Controls.Add(this.lblInfo);
            this.grpAdditionalFiles.Controls.Add(this.pbInfo);
            this.grpAdditionalFiles.Controls.Add(this.ctrlFiles);
            this.grpAdditionalFiles.Location = new System.Drawing.Point(4, 248);
            this.grpAdditionalFiles.Margin = new System.Windows.Forms.Padding(4);
            this.grpAdditionalFiles.Name = "grpAdditionalFiles";
            this.grpAdditionalFiles.Padding = new System.Windows.Forms.Padding(4);
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
            this.pbInfo.Margin = new System.Windows.Forms.Padding(4);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(21, 20);
            this.pbInfo.TabIndex = 22;
            this.pbInfo.TabStop = false;
            // 
            // ctrlFiles
            // 
            this.ctrlFiles.Location = new System.Drawing.Point(12, 63);
            this.ctrlFiles.Margin = new System.Windows.Forms.Padding(5);
            this.ctrlFiles.Name = "ctrlFiles";
            this.ctrlFiles.Size = new System.Drawing.Size(443, 169);
            this.ctrlFiles.TabIndex = 21;
            // 
            // SourcePortEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(475, 535);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
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

    }
}