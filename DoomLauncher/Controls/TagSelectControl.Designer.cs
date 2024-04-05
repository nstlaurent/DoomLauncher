namespace DoomLauncher.Controls
{
    partial class TagSelectControl
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvTags = new System.Windows.Forms.DataGridView();
            this.tblTop = new System.Windows.Forms.TableLayoutPanel();
            this.flpSearch = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnPin = new System.Windows.Forms.Button();
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.manageTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbSearch = new System.Windows.Forms.PictureBox();
            this.tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTags)).BeginInit();
            this.tblTop.SuspendLayout();
            this.flpSearch.SuspendLayout();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.dgvTags, 0, 1);
            this.tblMain.Controls.Add(this.tblTop, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tblMain.Size = new System.Drawing.Size(290, 516);
            this.tblMain.TabIndex = 0;
            // 
            // dgvTags
            // 
            this.dgvTags.AllowUserToAddRows = false;
            this.dgvTags.AllowUserToDeleteRows = false;
            this.dgvTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTags.Location = new System.Drawing.Point(4, 43);
            this.dgvTags.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvTags.Name = "dgvTags";
            this.dgvTags.ReadOnly = true;
            this.dgvTags.RowHeadersWidth = 62;
            this.dgvTags.Size = new System.Drawing.Size(282, 469);
            this.dgvTags.TabIndex = 2;
            // 
            // tblTop
            // 
            this.tblTop.ColumnCount = 2;
            this.tblTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tblTop.Controls.Add(this.flpSearch, 0, 0);
            this.tblTop.Controls.Add(this.btnPin, 1, 0);
            this.tblTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblTop.Location = new System.Drawing.Point(0, 0);
            this.tblTop.Margin = new System.Windows.Forms.Padding(0);
            this.tblTop.Name = "tblTop";
            this.tblTop.RowCount = 1;
            this.tblTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTop.Size = new System.Drawing.Size(290, 39);
            this.tblTop.TabIndex = 4;
            // 
            // flpSearch
            // 
            this.flpSearch.Controls.Add(this.pbSearch);
            this.flpSearch.Controls.Add(this.txtSearch);
            this.flpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpSearch.Location = new System.Drawing.Point(0, 0);
            this.flpSearch.Margin = new System.Windows.Forms.Padding(0);
            this.flpSearch.Name = "flpSearch";
            this.flpSearch.Size = new System.Drawing.Size(263, 39);
            this.flpSearch.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(26, 6);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(0, 6, 0, 4);
            this.txtSearch.MaximumSize = new System.Drawing.Size(169, 26);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(169, 22);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnPin
            // 
            this.btnPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPin.FlatAppearance.BorderSize = 0;
            this.btnPin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPin.Image = global::DoomLauncher.Properties.Resources.Pin;
            this.btnPin.Location = new System.Drawing.Point(263, 0);
            this.btnPin.Margin = new System.Windows.Forms.Padding(0);
            this.btnPin.Name = "btnPin";
            this.btnPin.Size = new System.Drawing.Size(27, 25);
            this.btnPin.TabIndex = 4;
            this.btnPin.UseVisualStyleBackColor = true;
            this.btnPin.Click += new System.EventHandler(this.btnPin_Click);
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageTagsToolStripMenuItem});
            this.menu.Name = "contextMenuStrip1";
            this.menu.Size = new System.Drawing.Size(175, 28);
            // 
            // manageTagsToolStripMenuItem
            // 
            this.manageTagsToolStripMenuItem.Name = "manageTagsToolStripMenuItem";
            this.manageTagsToolStripMenuItem.Size = new System.Drawing.Size(174, 24);
            this.manageTagsToolStripMenuItem.Text = "Manage Tags...";
            this.manageTagsToolStripMenuItem.Click += new System.EventHandler(this.manageTagsToolStripMenuItem_Click);
            // 
            // pbSearch
            // 
            this.pbSearch.Location = new System.Drawing.Point(6, 11);
            this.pbSearch.Margin = new System.Windows.Forms.Padding(6, 11, 0, 3);
            this.pbSearch.Name = "pbSearch";
            this.pbSearch.Size = new System.Drawing.Size(20, 15);
            this.pbSearch.TabIndex = 4;
            this.pbSearch.TabStop = false;
            // 
            // TagSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TagSelectControl";
            this.Size = new System.Drawing.Size(290, 516);
            this.tblMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTags)).EndInit();
            this.tblTop.ResumeLayout(false);
            this.flpSearch.ResumeLayout(false);
            this.flpSearch.PerformLayout();
            this.menu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.FlowLayoutPanel flpSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvTags;
        private System.Windows.Forms.TableLayoutPanel tblTop;
        private System.Windows.Forms.Button btnPin;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem manageTagsToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbSearch;
    }
}
