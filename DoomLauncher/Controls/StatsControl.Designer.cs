namespace DoomLauncher
{
    partial class StatsControl
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
            this.tblStats = new System.Windows.Forms.TableLayoutPanel();
            this.lblItems = new System.Windows.Forms.Label();
            this.lblSecrets = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblKills = new System.Windows.Forms.Label();
            this.pbKills = new System.Windows.Forms.PictureBox();
            this.pbSecrets = new System.Windows.Forms.PictureBox();
            this.pbItems = new System.Windows.Forms.PictureBox();
            this.tblStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbKills)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSecrets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbItems)).BeginInit();
            this.SuspendLayout();
            // 
            // tblStats
            // 
            this.tblStats.ColumnCount = 3;
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136F));
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblStats.Controls.Add(this.lblItems, 2, 2);
            this.tblStats.Controls.Add(this.lblSecrets, 2, 1);
            this.tblStats.Controls.Add(this.label1, 1, 0);
            this.tblStats.Controls.Add(this.label2, 1, 1);
            this.tblStats.Controls.Add(this.label3, 1, 2);
            this.tblStats.Controls.Add(this.lblKills, 2, 0);
            this.tblStats.Controls.Add(this.pbKills, 0, 0);
            this.tblStats.Controls.Add(this.pbSecrets, 0, 1);
            this.tblStats.Controls.Add(this.pbItems, 0, 2);
            this.tblStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblStats.Location = new System.Drawing.Point(0, 0);
            this.tblStats.Margin = new System.Windows.Forms.Padding(0);
            this.tblStats.Name = "tblStats";
            this.tblStats.RowCount = 4;
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblStats.Size = new System.Drawing.Size(262, 150);
            this.tblStats.TabIndex = 7;
            this.tblStats.Paint += new System.Windows.Forms.PaintEventHandler(this.tblStats_Paint);
            // 
            // lblItems
            // 
            this.lblItems.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblItems.AutoSize = true;
            this.lblItems.Location = new System.Drawing.Point(129, 53);
            this.lblItems.Name = "lblItems";
            this.lblItems.Size = new System.Drawing.Size(30, 13);
            this.lblItems.TabIndex = 5;
            this.lblItems.Text = "0 / 0";
            // 
            // lblSecrets
            // 
            this.lblSecrets.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSecrets.AutoSize = true;
            this.lblSecrets.Location = new System.Drawing.Point(129, 29);
            this.lblSecrets.Name = "lblSecrets";
            this.lblSecrets.Size = new System.Drawing.Size(30, 13);
            this.lblSecrets.TabIndex = 4;
            this.lblSecrets.Text = "0 / 0";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kills:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Secrets:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Items:";
            // 
            // lblKills
            // 
            this.lblKills.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblKills.AutoSize = true;
            this.lblKills.Location = new System.Drawing.Point(129, 5);
            this.lblKills.Name = "lblKills";
            this.lblKills.Size = new System.Drawing.Size(30, 13);
            this.lblKills.TabIndex = 3;
            this.lblKills.Text = "0 / 0";
            // 
            // pbKills
            // 
            this.pbKills.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbKills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbKills.Location = new System.Drawing.Point(0, 0);
            this.pbKills.Margin = new System.Windows.Forms.Padding(0);
            this.pbKills.Name = "pbKills";
            this.pbKills.Size = new System.Drawing.Size(50, 24);
            this.pbKills.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbKills.TabIndex = 6;
            this.pbKills.TabStop = false;
            // 
            // pbSecrets
            // 
            this.pbSecrets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbSecrets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSecrets.Location = new System.Drawing.Point(0, 24);
            this.pbSecrets.Margin = new System.Windows.Forms.Padding(0);
            this.pbSecrets.Name = "pbSecrets";
            this.pbSecrets.Size = new System.Drawing.Size(50, 24);
            this.pbSecrets.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSecrets.TabIndex = 7;
            this.pbSecrets.TabStop = false;
            // 
            // pbItems
            // 
            this.pbItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbItems.Location = new System.Drawing.Point(0, 48);
            this.pbItems.Margin = new System.Windows.Forms.Padding(0);
            this.pbItems.Name = "pbItems";
            this.pbItems.Size = new System.Drawing.Size(50, 24);
            this.pbItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbItems.TabIndex = 8;
            this.pbItems.TabStop = false;
            // 
            // StatsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblStats);
            this.Name = "StatsControl";
            this.Size = new System.Drawing.Size(262, 150);
            this.tblStats.ResumeLayout(false);
            this.tblStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbKills)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSecrets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblStats;
        private System.Windows.Forms.Label lblItems;
        private System.Windows.Forms.Label lblSecrets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblKills;
        private System.Windows.Forms.PictureBox pbKills;
        private System.Windows.Forms.PictureBox pbSecrets;
        private System.Windows.Forms.PictureBox pbItems;
    }
}
