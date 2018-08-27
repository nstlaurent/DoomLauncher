namespace DoomLauncher
{
    partial class GameFileFolderViewControl
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
            this.ctrlView = new DoomLauncher.GameFileViewControl();
            this.SuspendLayout();
            // 
            // ctrlView
            // 
            this.ctrlView.CustomRowColorPaint = false;
            this.ctrlView.CustomRowPaintForeColor = System.Drawing.Color.Empty;
            this.ctrlView.DataSource = null;
            this.ctrlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlView.Location = new System.Drawing.Point(0, 0);
            this.ctrlView.Name = "ctrlView";
            this.ctrlView.SelectedItem = null;
            this.ctrlView.Size = new System.Drawing.Size(322, 150);
            this.ctrlView.TabIndex = 0;
            // 
            // GameFileFolderViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctrlView);
            this.Name = "GameFileFolderViewControl";
            this.Size = new System.Drawing.Size(322, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private GameFileViewControl ctrlView;

    }
}
