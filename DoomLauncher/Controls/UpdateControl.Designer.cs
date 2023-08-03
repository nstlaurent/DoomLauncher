namespace DoomLauncher
{
    partial class UpdateControl
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
            this.btnInstall = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lnkPage = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(161, 76);
            this.btnInstall.Margin = new System.Windows.Forms.Padding(4);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(208, 28);
            this.btnInstall.TabIndex = 0;
            this.btnInstall.Text = "Download and Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(4, 12);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(46, 17);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "label1";
            // 
            // lnkPage
            // 
            this.lnkPage.AutoSize = true;
            this.lnkPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkPage.Location = new System.Drawing.Point(4, 42);
            this.lnkPage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkPage.Name = "lnkPage";
            this.lnkPage.Size = new System.Drawing.Size(134, 17);
            this.lnkPage.TabIndex = 2;
            this.lnkPage.TabStop = true;
            this.lnkPage.Text = "Release Information";
            this.lnkPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPage_LinkClicked);
            // 
            // UpdateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lnkPage);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnInstall);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "UpdateControl";
            this.Size = new System.Drawing.Size(397, 110);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel lnkPage;
    }
}
