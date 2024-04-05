namespace DoomLauncher
{
    partial class SearchControl
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
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFilters = new DoomLauncher.FormButton();
            this.pbSearch = new System.Windows.Forms.PictureBox();
            this.tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 3;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblMain.Controls.Add(this.txtSearch, 1, 0);
            this.tblMain.Controls.Add(this.btnFilters, 2, 0);
            this.tblMain.Controls.Add(this.pbSearch, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 1;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Size = new System.Drawing.Size(415, 23);
            this.tblMain.TabIndex = 2;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(26, 0);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(359, 22);
            this.txtSearch.TabIndex = 1;
            // 
            // btnFilters
            // 
            this.btnFilters.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilters.Location = new System.Drawing.Point(385, 0);
            this.btnFilters.Margin = new System.Windows.Forms.Padding(0);
            this.btnFilters.Name = "btnFilters";
            this.btnFilters.Size = new System.Drawing.Size(30, 23);
            this.btnFilters.TabIndex = 2;
            this.btnFilters.Text = "▼";
            this.btnFilters.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFilters.UseVisualStyleBackColor = true;
            this.btnFilters.Click += new System.EventHandler(this.btnFilters_Click);
            // 
            // pbSearch
            // 
            this.pbSearch.Location = new System.Drawing.Point(3, 5);
            this.pbSearch.Margin = new System.Windows.Forms.Padding(3, 5, 0, 3);
            this.pbSearch.Name = "pbSearch";
            this.pbSearch.Size = new System.Drawing.Size(20, 15);
            this.pbSearch.TabIndex = 3;
            this.pbSearch.TabStop = false;
            // 
            // SearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SearchControl";
            this.Size = new System.Drawing.Size(415, 23);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox txtSearch;
        private TableLayoutPanelDB tblMain;
        private FormButton btnFilters;
        private System.Windows.Forms.PictureBox pbSearch;
    }
}
