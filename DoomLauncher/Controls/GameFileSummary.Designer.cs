namespace DoomLauncher
{
    partial class GameFileSummary
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
            this.tblTags = new DoomLauncher.TableLayoutPanelDB();
            this.lblTagsText = new DoomLauncher.GrowLabel();
            this.lblTags = new System.Windows.Forms.Label();
            this.tblLastMap = new DoomLauncher.TableLayoutPanelDB();
            this.lblLastMap = new System.Windows.Forms.Label();
            this.lblLastMapText = new System.Windows.Forms.Label();
            this.lblTitle = new DoomLauncher.GrowLabel();
            this.ctrlStats = new DoomLauncher.StatsControl();
            this.txtDescription = new DoomLauncher.Controls.CRichTextBox();
            this.txtComments = new DoomLauncher.Controls.CRichTextBox();
            this.tblTimePlayed = new DoomLauncher.TableLayoutPanelDB();
            this.lblTimePlayed = new System.Windows.Forms.Label();
            this.lblTimePlayedText = new System.Windows.Forms.Label();
            this.tblMain.SuspendLayout();
            this.tblTags.SuspendLayout();
            this.tblLastMap.SuspendLayout();
            this.tblTimePlayed.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tblTags, 0, 4);
            this.tblMain.Controls.Add(this.tblLastMap, 0, 3);
            this.tblMain.Controls.Add(this.lblTitle, 0, 0);
            this.tblMain.Controls.Add(this.ctrlStats, 0, 5);
            this.tblMain.Controls.Add(this.txtDescription, 0, 6);
            this.tblMain.Controls.Add(this.txtComments, 0, 7);
            this.tblMain.Controls.Add(this.tblTimePlayed, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 8;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 308F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tblMain.Size = new System.Drawing.Size(306, 1006);
            this.tblMain.TabIndex = 0;
            // 
            // tblTags
            // 
            this.tblTags.ColumnCount = 3;
            this.tblTags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tblTags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tblTags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTags.Controls.Add(this.lblTagsText, 1, 0);
            this.tblTags.Controls.Add(this.lblTags, 2, 0);
            this.tblTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblTags.Location = new System.Drawing.Point(1, 437);
            this.tblTags.Margin = new System.Windows.Forms.Padding(0);
            this.tblTags.Name = "tblTags";
            this.tblTags.RowCount = 1;
            this.tblTags.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTags.Size = new System.Drawing.Size(304, 31);
            this.tblTags.TabIndex = 15;
            // 
            // lblTagsText
            // 
            this.lblTagsText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTagsText.AutoSize = true;
            this.lblTagsText.IsPath = false;
            this.lblTagsText.Location = new System.Drawing.Point(16, 5);
            this.lblTagsText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTagsText.Name = "lblTagsText";
            this.lblTagsText.Size = new System.Drawing.Size(48, 20);
            this.lblTagsText.TabIndex = 5;
            this.lblTagsText.Text = "Tags:";
            // 
            // lblTags
            // 
            this.lblTags.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTags.AutoSize = true;
            this.lblTags.Location = new System.Drawing.Point(151, 5);
            this.lblTags.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(40, 20);
            this.lblTags.TabIndex = 13;
            this.lblTags.Text = "tags";
            // 
            // tblLastMap
            // 
            this.tblLastMap.ColumnCount = 3;
            this.tblLastMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tblLastMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tblLastMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLastMap.Controls.Add(this.lblLastMap, 2, 0);
            this.tblLastMap.Controls.Add(this.lblLastMapText, 1, 0);
            this.tblLastMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLastMap.Location = new System.Drawing.Point(1, 405);
            this.tblLastMap.Margin = new System.Windows.Forms.Padding(0);
            this.tblLastMap.Name = "tblLastMap";
            this.tblLastMap.RowCount = 1;
            this.tblLastMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLastMap.Size = new System.Drawing.Size(304, 31);
            this.tblLastMap.TabIndex = 14;
            // 
            // lblLastMap
            // 
            this.lblLastMap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLastMap.AutoSize = true;
            this.lblLastMap.Location = new System.Drawing.Point(151, 5);
            this.lblLastMap.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastMap.Name = "lblLastMap";
            this.lblLastMap.Size = new System.Drawing.Size(69, 20);
            this.lblLastMap.TabIndex = 12;
            this.lblLastMap.Text = "last map";
            // 
            // lblLastMapText
            // 
            this.lblLastMapText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLastMapText.AutoSize = true;
            this.lblLastMapText.Location = new System.Drawing.Point(16, 5);
            this.lblLastMapText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastMapText.Name = "lblLastMapText";
            this.lblLastMapText.Size = new System.Drawing.Size(79, 20);
            this.lblLastMapText.TabIndex = 11;
            this.lblLastMapText.Text = "Last Map:";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.IsPath = false;
            this.lblTitle.Location = new System.Drawing.Point(121, 17);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(63, 29);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Title";
            this.lblTitle.UseMnemonic = false;
            // 
            // ctrlStats
            // 
            this.ctrlStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStats.Location = new System.Drawing.Point(1, 469);
            this.ctrlStats.Margin = new System.Windows.Forms.Padding(0);
            this.ctrlStats.Name = "ctrlStats";
            this.ctrlStats.Size = new System.Drawing.Size(304, 120);
            this.ctrlStats.TabIndex = 8;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(5, 595);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(296, 321);
            this.txtDescription.TabIndex = 9;
            this.txtDescription.Text = "";
            this.txtDescription.WarnLinkClick = true;
            // 
            // txtComments
            // 
            this.txtComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtComments.Location = new System.Drawing.Point(5, 927);
            this.txtComments.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(296, 73);
            this.txtComments.TabIndex = 10;
            this.txtComments.Text = "";
            this.txtComments.WarnLinkClick = true;
            // 
            // tblTimePlayed
            // 
            this.tblTimePlayed.ColumnCount = 3;
            this.tblTimePlayed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tblTimePlayed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tblTimePlayed.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTimePlayed.Controls.Add(this.lblTimePlayed, 2, 0);
            this.tblTimePlayed.Controls.Add(this.lblTimePlayedText, 1, 0);
            this.tblTimePlayed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblTimePlayed.Location = new System.Drawing.Point(1, 373);
            this.tblTimePlayed.Margin = new System.Windows.Forms.Padding(0);
            this.tblTimePlayed.Name = "tblTimePlayed";
            this.tblTimePlayed.RowCount = 1;
            this.tblTimePlayed.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblTimePlayed.Size = new System.Drawing.Size(304, 31);
            this.tblTimePlayed.TabIndex = 13;
            // 
            // lblTimePlayed
            // 
            this.lblTimePlayed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTimePlayed.AutoSize = true;
            this.lblTimePlayed.Location = new System.Drawing.Point(151, 5);
            this.lblTimePlayed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimePlayed.Name = "lblTimePlayed";
            this.lblTimePlayed.Size = new System.Drawing.Size(89, 20);
            this.lblTimePlayed.TabIndex = 8;
            this.lblTimePlayed.Text = "time played";
            // 
            // lblTimePlayedText
            // 
            this.lblTimePlayedText.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTimePlayedText.AutoSize = true;
            this.lblTimePlayedText.Location = new System.Drawing.Point(16, 5);
            this.lblTimePlayedText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimePlayedText.Name = "lblTimePlayedText";
            this.lblTimePlayedText.Size = new System.Drawing.Size(98, 20);
            this.lblTimePlayedText.TabIndex = 9;
            this.lblTimePlayedText.Text = "Time Played:";
            // 
            // GameFileSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "GameFileSummary";
            this.Size = new System.Drawing.Size(306, 1006);
            this.ClientSizeChanged += new System.EventHandler(this.GameFileSummary_ClientSizeChanged);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.tblTags.ResumeLayout(false);
            this.tblTags.PerformLayout();
            this.tblLastMap.ResumeLayout(false);
            this.tblLastMap.PerformLayout();
            this.tblTimePlayed.ResumeLayout(false);
            this.tblTimePlayed.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DoomLauncher.TableLayoutPanelDB tblMain;
        private GrowLabel lblTitle;
        private GrowLabel lblTagsText;
        private StatsControl ctrlStats;
        private Controls.CRichTextBox txtDescription;
        private Controls.CRichTextBox txtComments;
        private System.Windows.Forms.Label lblLastMapText;
        private System.Windows.Forms.Label lblLastMap;
        private System.Windows.Forms.Label lblTimePlayed;
        private System.Windows.Forms.Label lblTimePlayedText;
        private System.Windows.Forms.Label lblTags;
        private TableLayoutPanelDB tblTimePlayed;
        private TableLayoutPanelDB tblTags;
        private TableLayoutPanelDB tblLastMap;
    }
}
