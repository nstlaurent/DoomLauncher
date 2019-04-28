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
            this.lblTimePlayed = new System.Windows.Forms.Label();
            this.lblTitle = new DoomLauncher.GrowLabel();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lblTags = new DoomLauncher.GrowLabel();
            this.ctrlStats = new DoomLauncher.StatsControl();
            this.txtDescription = new DoomLauncher.Controls.CRichTextBox();
            this.txtComments = new DoomLauncher.Controls.CRichTextBox();
            this.lblLastMap = new System.Windows.Forms.Label();
            this.tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.lblTimePlayed, 0, 2);
            this.tblMain.Controls.Add(this.lblTitle, 0, 0);
            this.tblMain.Controls.Add(this.pbImage, 0, 1);
            this.tblMain.Controls.Add(this.lblTags, 0, 5);
            this.tblMain.Controls.Add(this.ctrlStats, 0, 4);
            this.tblMain.Controls.Add(this.txtDescription, 0, 6);
            this.tblMain.Controls.Add(this.txtComments, 0, 7);
            this.tblMain.Controls.Add(this.lblLastMap, 0, 3);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 8;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblMain.Size = new System.Drawing.Size(204, 654);
            this.tblMain.TabIndex = 0;
            // 
            // lblTimePlayed
            // 
            this.lblTimePlayed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTimePlayed.AutoSize = true;
            this.lblTimePlayed.Location = new System.Drawing.Point(4, 246);
            this.lblTimePlayed.Name = "lblTimePlayed";
            this.lblTimePlayed.Size = new System.Drawing.Size(68, 13);
            this.lblTimePlayed.TabIndex = 7;
            this.lblTimePlayed.Text = "Time Played:";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(81, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(41, 19);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Title";
            this.lblTitle.UseMnemonic = false;
            // 
            // pbImage
            // 
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.Location = new System.Drawing.Point(4, 45);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(196, 194);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImage.TabIndex = 2;
            this.pbImage.TabStop = false;
            // 
            // lblTags
            // 
            this.lblTags.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTags.AutoSize = true;
            this.lblTags.Location = new System.Drawing.Point(4, 367);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(34, 13);
            this.lblTags.TabIndex = 5;
            this.lblTags.Text = "Tags:";
            // 
            // ctrlStats
            // 
            this.ctrlStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStats.Location = new System.Drawing.Point(1, 285);
            this.ctrlStats.Margin = new System.Windows.Forms.Padding(0);
            this.ctrlStats.Name = "ctrlStats";
            this.ctrlStats.Size = new System.Drawing.Size(202, 78);
            this.ctrlStats.TabIndex = 8;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(4, 388);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(196, 207);
            this.txtDescription.TabIndex = 9;
            this.txtDescription.Text = "";
            this.txtDescription.WarnLinkClick = true;
            // 
            // txtComments
            // 
            this.txtComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtComments.Location = new System.Drawing.Point(4, 602);
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(196, 48);
            this.txtComments.TabIndex = 10;
            this.txtComments.Text = "";
            this.txtComments.WarnLinkClick = true;
            // 
            // lblLastMap
            // 
            this.lblLastMap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLastMap.AutoSize = true;
            this.lblLastMap.Location = new System.Drawing.Point(4, 267);
            this.lblLastMap.Name = "lblLastMap";
            this.lblLastMap.Size = new System.Drawing.Size(54, 13);
            this.lblLastMap.TabIndex = 11;
            this.lblLastMap.Text = "Last Map:";
            // 
            // GameFileSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMain);
            this.Name = "GameFileSummary";
            this.Size = new System.Drawing.Size(204, 654);
            this.ClientSizeChanged += new System.EventHandler(this.GameFileSummary_ClientSizeChanged);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DoomLauncher.TableLayoutPanelDB tblMain;
        private System.Windows.Forms.PictureBox pbImage;
        private GrowLabel lblTitle;
        private GrowLabel lblTags;
        private System.Windows.Forms.Label lblTimePlayed;
        private StatsControl ctrlStats;
        private Controls.CRichTextBox txtDescription;
        private Controls.CRichTextBox txtComments;
        private System.Windows.Forms.Label lblLastMap;
    }
}
