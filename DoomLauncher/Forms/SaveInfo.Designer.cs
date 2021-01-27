
namespace DoomLauncher.Forms
{
    partial class SaveInfo
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveInfo));
            this.tblInfo = new System.Windows.Forms.TableLayoutPanel();
            this.lblSave = new System.Windows.Forms.Label();
            this.pbInfo1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tblInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo1)).BeginInit();
            this.SuspendLayout();
            // 
            // tblInfo
            // 
            this.tblInfo.ColumnCount = 2;
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Controls.Add(this.lblSave, 1, 1);
            this.tblInfo.Controls.Add(this.pbInfo1, 0, 0);
            this.tblInfo.Controls.Add(this.label3, 1, 0);
            this.tblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInfo.Location = new System.Drawing.Point(0, 0);
            this.tblInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tblInfo.Name = "tblInfo";
            this.tblInfo.RowCount = 2;
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Size = new System.Drawing.Size(384, 235);
            this.tblInfo.TabIndex = 4;
            // 
            // lblSave
            // 
            this.lblSave.AutoSize = true;
            this.lblSave.Location = new System.Drawing.Point(35, 40);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(262, 13);
            this.lblSave.TabIndex = 3;
            this.lblSave.Text = "Loading a save file is supported for the following ports:";
            // 
            // pbInfo1
            // 
            this.pbInfo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbInfo1.InitialImage = null;
            this.pbInfo1.Location = new System.Drawing.Point(0, 0);
            this.pbInfo1.Margin = new System.Windows.Forms.Padding(0);
            this.pbInfo1.Name = "pbInfo1";
            this.pbInfo1.Size = new System.Drawing.Size(32, 40);
            this.pbInfo1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbInfo1.TabIndex = 0;
            this.pbInfo1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "The \'Load Latest Save\' option will become available when a supported source port " +
    "is selected.";
            // 
            // SaveInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 235);
            this.Controls.Add(this.tblInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SaveInfo";
            this.Text = "Load Latest Save";
            this.tblInfo.ResumeLayout(false);
            this.tblInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblInfo;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.PictureBox pbInfo1;
        private System.Windows.Forms.Label label3;
    }
}