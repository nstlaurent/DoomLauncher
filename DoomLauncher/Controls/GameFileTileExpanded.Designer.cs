namespace DoomLauncher
{
    partial class GameFileTileExpanded
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
            this.gameTile = new DoomLauncher.GameFileTile();
            this.flpMain = new DoomLauncher.FlowLayoutPanelDB();
            this.pnlData = new System.Windows.Forms.Panel();
            this.flpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameTile
            // 
            this.gameTile.DrawBorder = true;
            this.gameTile.Location = new System.Drawing.Point(0, 0);
            this.gameTile.Margin = new System.Windows.Forms.Padding(0);
            this.gameTile.Name = "gameTile";
            this.gameTile.Size = new System.Drawing.Size(300, 200);
            this.gameTile.TabIndex = 0;
            // 
            // flpMain
            // 
            this.flpMain.Controls.Add(this.gameTile);
            this.flpMain.Controls.Add(this.pnlData);
            this.flpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMain.Location = new System.Drawing.Point(0, 0);
            this.flpMain.Margin = new System.Windows.Forms.Padding(0);
            this.flpMain.Name = "flpMain";
            this.flpMain.Size = new System.Drawing.Size(660, 204);
            this.flpMain.TabIndex = 1;
            // 
            // pnlData
            // 
            this.pnlData.Location = new System.Drawing.Point(302, 2);
            this.pnlData.Margin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.pnlData.Name = "pnlData";
            this.pnlData.Size = new System.Drawing.Size(352, 182);
            this.pnlData.TabIndex = 1;
            // 
            // GameFileTileExpanded
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flpMain);
            this.Name = "GameFileTileExpanded";
            this.Size = new System.Drawing.Size(660, 204);
            this.flpMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GameFileTile gameTile;
        private FlowLayoutPanelDB flpMain;
        private System.Windows.Forms.Panel pnlData;
    }
}
