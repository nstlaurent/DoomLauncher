namespace DoomLauncher
{
    partial class CumulativeStats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CumulativeStats));
            this.btnOK = new System.Windows.Forms.Button();
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.lblNote = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDisplay2 = new System.Windows.Forms.Label();
            this.lblDisplay1 = new System.Windows.Forms.Label();
            this.lblTimeLaunched = new System.Windows.Forms.Label();
            this.lblTimePlayed = new System.Windows.Forms.Label();
            this.ctrlStats = new DoomLauncher.StatsControl();
            this.tblMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(399, 297);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // tblMain
            // 
            this.tblMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.lblNote, 0, 0);
            this.tblMain.Controls.Add(this.btnOK, 0, 3);
            this.tblMain.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tblMain.Controls.Add(this.ctrlStats, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tblMain.Size = new System.Drawing.Size(516, 342);
            this.tblMain.TabIndex = 2;
            // 
            // lblNote
            // 
            this.lblNote.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNote.AutoSize = true;
            this.lblNote.Location = new System.Drawing.Point(5, 15);
            this.lblNote.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(47, 20);
            this.lblNote.TabIndex = 10;
            this.lblNote.Text = "Note:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblDisplay2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblDisplay1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeLaunched, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblTimePlayed, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 51);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(514, 75);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // lblDisplay2
            // 
            this.lblDisplay2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDisplay2.AutoSize = true;
            this.lblDisplay2.Location = new System.Drawing.Point(4, 46);
            this.lblDisplay2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplay2.Name = "lblDisplay2";
            this.lblDisplay2.Size = new System.Drawing.Size(150, 20);
            this.lblDisplay2.TabIndex = 11;
            this.lblDisplay2.Text = "Time Played (Stats):";
            // 
            // lblDisplay1
            // 
            this.lblDisplay1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDisplay1.AutoSize = true;
            this.lblDisplay1.Location = new System.Drawing.Point(3, 7);
            this.lblDisplay1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplay1.Name = "lblDisplay1";
            this.lblDisplay1.Size = new System.Drawing.Size(183, 20);
            this.lblDisplay1.TabIndex = 7;
            this.lblDisplay1.Text = "Time Played (Launched):";
            // 
            // lblTimeLaunched
            // 
            this.lblTimeLaunched.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTimeLaunched.AutoSize = true;
            this.lblTimeLaunched.Location = new System.Drawing.Point(203, 8);
            this.lblTimeLaunched.Name = "lblTimeLaunched";
            this.lblTimeLaunched.Size = new System.Drawing.Size(80, 20);
            this.lblTimeLaunched.TabIndex = 12;
            this.lblTimeLaunched.Text = "Launched";
            // 
            // lblTimePlayed
            // 
            this.lblTimePlayed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTimePlayed.AutoSize = true;
            this.lblTimePlayed.Location = new System.Drawing.Point(203, 46);
            this.lblTimePlayed.Name = "lblTimePlayed";
            this.lblTimePlayed.Size = new System.Drawing.Size(56, 20);
            this.lblTimePlayed.TabIndex = 13;
            this.lblTimePlayed.Text = "Played";
            // 
            // ctrlStats
            // 
            this.ctrlStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStats.Location = new System.Drawing.Point(5, 132);
            this.ctrlStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ctrlStats.Name = "ctrlStats";
            this.ctrlStats.Size = new System.Drawing.Size(506, 150);
            this.ctrlStats.TabIndex = 13;
            // 
            // CumulativeStats
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 342);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "CumulativeStats";
            this.Text = "Cumulative Stats";
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanelDB tblMain;
        private System.Windows.Forms.Label lblDisplay1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Label lblDisplay2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblTimeLaunched;
        private System.Windows.Forms.Label lblTimePlayed;
        private StatsControl ctrlStats;
    }
}