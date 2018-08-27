namespace DoomLauncher
{
    partial class BasicTabViewCtrl
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
            this.ctrlView = new DoomLauncher.GameFileViewControl();
            this.SuspendLayout();
            // 
            // ctrlView
            // 
            this.ctrlView.DataSource = null;
            this.ctrlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlView.Location = new System.Drawing.Point(0, 0);
            this.ctrlView.Name = "ctrlView";
            this.ctrlView.SelectedItem = null;
            this.ctrlView.Size = new System.Drawing.Size(150, 150);
            this.ctrlView.TabIndex = 0;
            // 
            // BasicTabViewCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctrlView);
            this.Name = "BasicTabViewCtrl";
            this.ResumeLayout(false);

        }

        #endregion

        private GameFileViewControl ctrlView;
    }
}
