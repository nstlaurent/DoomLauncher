namespace DoomLauncher
{
    partial class GameFileTileViewControl
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.flpPaging = new System.Windows.Forms.FlowLayoutPanel();
            this.pagingControl = new DoomLauncher.PagingControl();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMaxItemsPerPage = new System.Windows.Forms.ComboBox();
            this.tblMain.SuspendLayout();
            this.flpPaging.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.flpPaging, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tblMain.Size = new System.Drawing.Size(717, 482);
            this.tblMain.TabIndex = 1;
            // 
            // flpPaging
            // 
            this.flpPaging.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flpPaging.AutoSize = true;
            this.flpPaging.Controls.Add(this.pagingControl);
            this.flpPaging.Controls.Add(this.label1);
            this.flpPaging.Controls.Add(this.cmbMaxItemsPerPage);
            this.flpPaging.Location = new System.Drawing.Point(88, 436);
            this.flpPaging.Margin = new System.Windows.Forms.Padding(0);
            this.flpPaging.Name = "flpPaging";
            this.flpPaging.Size = new System.Drawing.Size(541, 46);
            this.flpPaging.TabIndex = 1;
            this.flpPaging.WrapContents = false;
            // 
            // pagingControl
            // 
            this.pagingControl.Location = new System.Drawing.Point(6, 8);
            this.pagingControl.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.pagingControl.Name = "pagingControl";
            this.pagingControl.Size = new System.Drawing.Size(334, 32);
            this.pagingControl.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Items Per Page";
            // 
            // cmbMaxItemsPerPage
            // 
            this.cmbMaxItemsPerPage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbMaxItemsPerPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaxItemsPerPage.FormattingEnabled = true;
            this.cmbMaxItemsPerPage.Items.AddRange(new object[] {
            "30",
            "60",
            "120"});
            this.cmbMaxItemsPerPage.Location = new System.Drawing.Point(476, 10);
            this.cmbMaxItemsPerPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbMaxItemsPerPage.Name = "cmbMaxItemsPerPage";
            this.cmbMaxItemsPerPage.Size = new System.Drawing.Size(61, 28);
            this.cmbMaxItemsPerPage.TabIndex = 1;
            // 
            // GameFileTileViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "GameFileTileViewControl";
            this.Size = new System.Drawing.Size(717, 482);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.flpPaging.ResumeLayout(false);
            this.flpPaging.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private TableLayoutPanelDB tblMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMaxItemsPerPage;
        private System.Windows.Forms.FlowLayoutPanel flpPaging;
        private PagingControl pagingControl;
    }
}
