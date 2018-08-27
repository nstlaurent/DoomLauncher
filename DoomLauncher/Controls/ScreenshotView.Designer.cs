namespace DoomLauncher
{
    partial class ScreenshotView
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
            this.flpScreenshots = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flpScreenshots
            // 
            this.flpScreenshots.AutoScroll = true;
            this.flpScreenshots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpScreenshots.Location = new System.Drawing.Point(0, 0);
            this.flpScreenshots.Name = "flpScreenshots";
            this.flpScreenshots.Size = new System.Drawing.Size(150, 150);
            this.flpScreenshots.TabIndex = 0;
            // 
            // ScreenshotView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flpScreenshots);
            this.Name = "ScreenshotView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpScreenshots;
    }
}
