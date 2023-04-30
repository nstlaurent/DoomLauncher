namespace DoomLauncher
{
    partial class FilesCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCode("Winform Designer", "VS2015 SP1")]
        private void InitializeComponent()
        {
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvAdditionalFiles = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new DoomLauncher.FormButton();
            this.btnDelete = new DoomLauncher.FormButton();
            this.btnMoveUp = new DoomLauncher.FormButton();
            this.btnMoveDown = new DoomLauncher.FormButton();
            this.tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditionalFiles)).BeginInit();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.dgvAdditionalFiles, 0, 1);
            this.tblMain.Controls.Add(this.flpButtons, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.Size = new System.Drawing.Size(370, 482);
            this.tblMain.TabIndex = 23;
            // 
            // dgvAdditionalFiles
            // 
            this.dgvAdditionalFiles.AllowUserToAddRows = false;
            this.dgvAdditionalFiles.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvAdditionalFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAdditionalFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName});
            this.dgvAdditionalFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAdditionalFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvAdditionalFiles.Location = new System.Drawing.Point(4, 45);
            this.dgvAdditionalFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvAdditionalFiles.Name = "dgvAdditionalFiles";
            this.dgvAdditionalFiles.RowHeadersWidth = 62;
            this.dgvAdditionalFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAdditionalFiles.Size = new System.Drawing.Size(362, 432);
            this.dgvAdditionalFiles.TabIndex = 2;
            this.dgvAdditionalFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAdditionalFiles_KeyDown);
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileName.DataPropertyName = "FileName";
            this.FileName.HeaderText = "File";
            this.FileName.MinimumWidth = 8;
            this.FileName.Name = "FileName";
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnAdd);
            this.flpButtons.Controls.Add(this.btnDelete);
            this.flpButtons.Controls.Add(this.btnMoveUp);
            this.flpButtons.Controls.Add(this.btnMoveDown);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.Location = new System.Drawing.Point(0, 0);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(370, 40);
            this.flpButtons.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::DoomLauncher.Properties.Resources.File;
            this.btnAdd.Location = new System.Drawing.Point(0, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(40, 37);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = global::DoomLauncher.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(40, 0);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(40, 37);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Image = global::DoomLauncher.Properties.Resources.UpArrow;
            this.btnMoveUp.Location = new System.Drawing.Point(80, 0);
            this.btnMoveUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(40, 37);
            this.btnMoveUp.TabIndex = 2;
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Image = global::DoomLauncher.Properties.Resources.DownArrow;
            this.btnMoveDown.Location = new System.Drawing.Point(120, 0);
            this.btnMoveDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(40, 37);
            this.btnMoveDown.TabIndex = 3;
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // FilesCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FilesCtrl";
            this.Size = new System.Drawing.Size(370, 482);
            this.tblMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditionalFiles)).EndInit();
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.DataGridView dgvAdditionalFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private FormButton btnDelete;
        private FormButton btnMoveUp;
        private FormButton btnMoveDown;
        private FormButton btnAdd;
    }
}
