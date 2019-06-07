namespace DoomLauncher.Forms
{
    partial class FilterSettingsForm
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
            this.chkStagger = new System.Windows.Forms.CheckBox();
            this.numBlockSize = new System.Windows.Forms.NumericUpDown();
            this.numSpacingX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numSpacingY = new System.Windows.Forms.NumericUpDown();
            this.grpEllipse = new System.Windows.Forms.GroupBox();
            this.grpScanline = new System.Windows.Forms.GroupBox();
            this.chkVertical = new System.Windows.Forms.CheckBox();
            this.chkHorizontal = new System.Windows.Forms.CheckBox();
            this.numScanlineSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPreviewScanline = new System.Windows.Forms.Button();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBlockSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpacingX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpacingY)).BeginInit();
            this.grpEllipse.SuspendLayout();
            this.grpScanline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScanlineSize)).BeginInit();
            this.SuspendLayout();
            // 
            // chkStagger
            // 
            this.chkStagger.AutoSize = true;
            this.chkStagger.Location = new System.Drawing.Point(15, 17);
            this.chkStagger.Name = "chkStagger";
            this.chkStagger.Size = new System.Drawing.Size(63, 17);
            this.chkStagger.TabIndex = 0;
            this.chkStagger.Text = "Stagger";
            this.chkStagger.UseVisualStyleBackColor = true;
            // 
            // numBlockSize
            // 
            this.numBlockSize.Location = new System.Drawing.Point(89, 40);
            this.numBlockSize.Name = "numBlockSize";
            this.numBlockSize.Size = new System.Drawing.Size(83, 20);
            this.numBlockSize.TabIndex = 1;
            this.numBlockSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // numSpacingX
            // 
            this.numSpacingX.DecimalPlaces = 1;
            this.numSpacingX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSpacingX.Location = new System.Drawing.Point(89, 66);
            this.numSpacingX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numSpacingX.Name = "numSpacingX";
            this.numSpacingX.Size = new System.Drawing.Size(83, 20);
            this.numSpacingX.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Block Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "X Spacing";
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(15, 118);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(157, 23);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview For 3 Seconds";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Y Spacing";
            // 
            // numSpacingY
            // 
            this.numSpacingY.DecimalPlaces = 1;
            this.numSpacingY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSpacingY.Location = new System.Drawing.Point(89, 92);
            this.numSpacingY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numSpacingY.Name = "numSpacingY";
            this.numSpacingY.Size = new System.Drawing.Size(83, 20);
            this.numSpacingY.TabIndex = 8;
            // 
            // grpEllipse
            // 
            this.grpEllipse.Controls.Add(this.btnPreview);
            this.grpEllipse.Controls.Add(this.label3);
            this.grpEllipse.Controls.Add(this.chkStagger);
            this.grpEllipse.Controls.Add(this.numSpacingY);
            this.grpEllipse.Controls.Add(this.numBlockSize);
            this.grpEllipse.Controls.Add(this.numSpacingX);
            this.grpEllipse.Controls.Add(this.label2);
            this.grpEllipse.Controls.Add(this.label1);
            this.grpEllipse.Location = new System.Drawing.Point(12, 44);
            this.grpEllipse.Name = "grpEllipse";
            this.grpEllipse.Size = new System.Drawing.Size(274, 157);
            this.grpEllipse.TabIndex = 10;
            this.grpEllipse.TabStop = false;
            this.grpEllipse.Text = "Ellipse";
            // 
            // grpScanline
            // 
            this.grpScanline.Controls.Add(this.btnPreviewScanline);
            this.grpScanline.Controls.Add(this.numScanlineSize);
            this.grpScanline.Controls.Add(this.label4);
            this.grpScanline.Controls.Add(this.chkHorizontal);
            this.grpScanline.Controls.Add(this.chkVertical);
            this.grpScanline.Location = new System.Drawing.Point(12, 207);
            this.grpScanline.Name = "grpScanline";
            this.grpScanline.Size = new System.Drawing.Size(274, 137);
            this.grpScanline.TabIndex = 11;
            this.grpScanline.TabStop = false;
            this.grpScanline.Text = "Scanline";
            // 
            // chkVertical
            // 
            this.chkVertical.AutoSize = true;
            this.chkVertical.Checked = true;
            this.chkVertical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVertical.Location = new System.Drawing.Point(6, 41);
            this.chkVertical.Name = "chkVertical";
            this.chkVertical.Size = new System.Drawing.Size(61, 17);
            this.chkVertical.TabIndex = 1;
            this.chkVertical.Text = "Vertical";
            this.chkVertical.UseVisualStyleBackColor = true;
            // 
            // chkHorizontal
            // 
            this.chkHorizontal.AutoSize = true;
            this.chkHorizontal.Checked = true;
            this.chkHorizontal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHorizontal.Location = new System.Drawing.Point(6, 64);
            this.chkHorizontal.Name = "chkHorizontal";
            this.chkHorizontal.Size = new System.Drawing.Size(73, 17);
            this.chkHorizontal.TabIndex = 12;
            this.chkHorizontal.Text = "Horizontal";
            this.chkHorizontal.UseVisualStyleBackColor = true;
            // 
            // numScanlineSize
            // 
            this.numScanlineSize.Location = new System.Drawing.Point(89, 19);
            this.numScanlineSize.Name = "numScanlineSize";
            this.numScanlineSize.Size = new System.Drawing.Size(83, 20);
            this.numScanlineSize.TabIndex = 13;
            this.numScanlineSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Size";
            // 
            // btnPreviewScanline
            // 
            this.btnPreviewScanline.Location = new System.Drawing.Point(15, 97);
            this.btnPreviewScanline.Name = "btnPreviewScanline";
            this.btnPreviewScanline.Size = new System.Drawing.Size(157, 23);
            this.btnPreviewScanline.TabIndex = 10;
            this.btnPreviewScanline.Text = "Preview For 3 Seconds";
            this.btnPreviewScanline.UseVisualStyleBackColor = true;
            this.btnPreviewScanline.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // cmbMode
            // 
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Items.AddRange(new object[] {
            "Ellipse",
            "Scanline"});
            this.cmbMode.Location = new System.Drawing.Point(63, 17);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(121, 21);
            this.cmbMode.TabIndex = 12;
            this.cmbMode.SelectedIndexChanged += new System.EventHandler(this.cmbMode_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Mode";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(213, 368);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(132, 368);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // FilterSettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 393);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbMode);
            this.Controls.Add(this.grpScanline);
            this.Controls.Add(this.grpEllipse);
            this.Name = "FilterSettingsForm";
            this.Text = "Filter Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numBlockSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpacingX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpacingY)).EndInit();
            this.grpEllipse.ResumeLayout(false);
            this.grpEllipse.PerformLayout();
            this.grpScanline.ResumeLayout(false);
            this.grpScanline.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScanlineSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkStagger;
        private System.Windows.Forms.NumericUpDown numBlockSize;
        private System.Windows.Forms.NumericUpDown numSpacingX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numSpacingY;
        private System.Windows.Forms.GroupBox grpEllipse;
        private System.Windows.Forms.GroupBox grpScanline;
        private System.Windows.Forms.CheckBox chkVertical;
        private System.Windows.Forms.CheckBox chkHorizontal;
        private System.Windows.Forms.NumericUpDown numScanlineSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPreviewScanline;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}