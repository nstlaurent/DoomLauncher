namespace DoomLauncher
{
    partial class SourcePortViewForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourcePortViewForm));
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvSourcePorts = new System.Windows.Forms.DataGridView();
            this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Executable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Directory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanelDB1 = new DoomLauncher.TableLayoutPanelDB();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.flpTop = new System.Windows.Forms.FlowLayoutPanel();
            this.chkShowArchived = new DoomLauncher.CCheckBox();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSourcePorts)).BeginInit();
            this.tableLayoutPanelDB1.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.flpTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.dgvSourcePorts, 0, 2);
            this.tblMain.Controls.Add(this.tableLayoutPanelDB1, 0, 3);
            this.tblMain.Controls.Add(this.flpTop, 0, 1);
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.Size = new System.Drawing.Size(936, 680);
            this.tblMain.TabIndex = 0;
            // 
            // dgvSourcePorts
            // 
            this.dgvSourcePorts.AllowUserToAddRows = false;
            this.dgvSourcePorts.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvSourcePorts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSourcePorts.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvSourcePorts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvSourcePorts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSourcePorts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColName,
            this.Executable,
            this.Directory});
            this.dgvSourcePorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSourcePorts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvSourcePorts.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgvSourcePorts.Location = new System.Drawing.Point(4, 83);
            this.dgvSourcePorts.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvSourcePorts.Name = "dgvSourcePorts";
            this.dgvSourcePorts.RowHeadersWidth = 51;
            this.dgvSourcePorts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSourcePorts.Size = new System.Drawing.Size(928, 543);
            this.dgvSourcePorts.TabIndex = 0;
            this.dgvSourcePorts.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSourcePorts_CellDoubleClick);
            // 
            // ColName
            // 
            this.ColName.DataPropertyName = "Name";
            this.ColName.HeaderText = "Name";
            this.ColName.MinimumWidth = 6;
            this.ColName.Name = "ColName";
            this.ColName.Width = 125;
            // 
            // Executable
            // 
            this.Executable.DataPropertyName = "Executable";
            this.Executable.HeaderText = "Executable";
            this.Executable.MinimumWidth = 6;
            this.Executable.Name = "Executable";
            this.Executable.Width = 125;
            // 
            // Directory
            // 
            this.Directory.DataPropertyName = "Directory";
            this.Directory.HeaderText = "Directory";
            this.Directory.MinimumWidth = 6;
            this.Directory.Name = "Directory";
            this.Directory.Width = 125;
            // 
            // tableLayoutPanelDB1
            // 
            this.tableLayoutPanelDB1.ColumnCount = 2;
            this.tableLayoutPanelDB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDB1.Controls.Add(this.flpButtons, 0, 0);
            this.tableLayoutPanelDB1.Controls.Add(this.btnNext, 1, 0);
            this.tableLayoutPanelDB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDB1.Location = new System.Drawing.Point(0, 631);
            this.tableLayoutPanelDB1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelDB1.Name = "tableLayoutPanelDB1";
            this.tableLayoutPanelDB1.RowCount = 1;
            this.tableLayoutPanelDB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanelDB1.Size = new System.Drawing.Size(936, 49);
            this.tableLayoutPanelDB1.TabIndex = 1;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnNew);
            this.flpButtons.Controls.Add(this.btnEdit);
            this.flpButtons.Controls.Add(this.btnDelete);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.Location = new System.Drawing.Point(0, 0);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(468, 49);
            this.flpButtons.TabIndex = 2;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(4, 5);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(112, 35);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(124, 5);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(112, 35);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(244, 5);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(112, 35);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(815, 5);
            this.btnNext.Margin = new System.Windows.Forms.Padding(4, 5, 9, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(112, 35);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Visible = false;
            this.btnNext.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // flpTop
            // 
            this.flpTop.Controls.Add(this.chkShowArchived);
            this.flpTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTop.Location = new System.Drawing.Point(3, 40);
            this.flpTop.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.flpTop.Name = "flpTop";
            this.flpTop.Size = new System.Drawing.Size(933, 38);
            this.flpTop.TabIndex = 2;
            // 
            // chkShowArchived
            // 
            this.chkShowArchived.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkShowArchived.AutoSize = true;
            this.chkShowArchived.Location = new System.Drawing.Point(3, 4);
            this.chkShowArchived.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkShowArchived.Name = "chkShowArchived";
            this.chkShowArchived.Size = new System.Drawing.Size(140, 24);
            this.chkShowArchived.TabIndex = 0;
            this.chkShowArchived.Text = "Show Archived";
            this.chkShowArchived.UseVisualStyleBackColor = true;
            this.chkShowArchived.CheckedChanged += new System.EventHandler(this.chkShowArchived_CheckedChanged);
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(936, 40);
            this.titleBar.TabIndex = 3;
            this.titleBar.Title = "Title";
            // 
            // SourcePortViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 680);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SourcePortViewForm";
            this.Text = "Source Ports";
            this.tblMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSourcePorts)).EndInit();
            this.tableLayoutPanelDB1.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.flpTop.ResumeLayout(false);
            this.flpTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.DataGridView dgvSourcePorts;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Executable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Directory;
        private TableLayoutPanelDB tableLayoutPanelDB1;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.FlowLayoutPanel flpTop;
        private System.Windows.Forms.CheckBox chkShowArchived;
        private Controls.TitleBarControl titleBar;
    }
}