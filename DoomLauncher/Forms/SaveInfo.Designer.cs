
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
            this.tblContainer = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.tblInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo1)).BeginInit();
            this.tblContainer.SuspendLayout();
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
            this.tblInfo.Location = new System.Drawing.Point(0, 26);
            this.tblInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tblInfo.Name = "tblInfo";
            this.tblInfo.RowCount = 2;
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Size = new System.Drawing.Size(384, 85);
            this.tblInfo.TabIndex = 4;
            // 
            // lblSave
            // 
            this.lblSave.AutoSize = true;
            this.lblSave.Location = new System.Drawing.Point(35, 40);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(322, 26);
            this.lblSave.TabIndex = 3;
            this.lblSave.Text = "Loading a save file is supported for all ports, except the Doomsday Engine.";
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
            // tblContainer
            // 
            this.tblContainer.ColumnCount = 1;
            this.tblContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblContainer.Controls.Add(this.titleBar, 0, 0);
            this.tblContainer.Controls.Add(this.tblInfo, 0, 1);
            this.tblContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblContainer.Location = new System.Drawing.Point(0, 0);
            this.tblContainer.Margin = new System.Windows.Forms.Padding(0);
            this.tblContainer.Name = "tblContainer";
            this.tblContainer.RowCount = 2;
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblContainer.Size = new System.Drawing.Size(384, 111);
            this.tblContainer.TabIndex = 5;
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.CanClose = true;
            this.titleBar.ControlBox = true;
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.RememberNormalSize = true;
            this.titleBar.Size = new System.Drawing.Size(384, 26);
            this.titleBar.TabIndex = 0;
            this.titleBar.Title = "Load Latest Save";
            // 
            // SaveInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 111);
            this.Controls.Add(this.tblContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SaveInfo";
            this.Text = "Load Latest Save";
            this.tblInfo.ResumeLayout(false);
            this.tblInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo1)).EndInit();
            this.tblContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblInfo;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.PictureBox pbInfo1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tblContainer;
        private Controls.TitleBarControl titleBar;
    }
}