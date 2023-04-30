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
            DoomLauncher.Controls.TitleBarControl titleBar;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterSettingsForm));
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
            this.numScanlineSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.chkHorizontal = new System.Windows.Forms.CheckBox();
            this.chkVertical = new System.Windows.Forms.CheckBox();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.numOpacity = new System.Windows.Forms.NumericUpDown();
            this.numThickness = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.tblInfo = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.pbInfo = new System.Windows.Forms.PictureBox();
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            titleBar = new DoomLauncher.Controls.TitleBarControl();
            ((System.ComponentModel.ISupportInitialize)(this.numBlockSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpacingX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpacingY)).BeginInit();
            this.grpEllipse.SuspendLayout();
            this.grpScanline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numScanlineSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThickness)).BeginInit();
            this.tblInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).BeginInit();
            this.tblMain.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // titleBar
            // 
            titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            titleBar.ForeColor = System.Drawing.Color.White;
            titleBar.Location = new System.Drawing.Point(0, 0);
            titleBar.Margin = new System.Windows.Forms.Padding(0);
            titleBar.Name = "titleBar";
            titleBar.Size = new System.Drawing.Size(419, 34);
            titleBar.TabIndex = 2;
            titleBar.Title = "Filter Settings";
            // 
            // chkStagger
            // 
            this.chkStagger.AutoSize = true;
            this.chkStagger.Location = new System.Drawing.Point(22, 26);
            this.chkStagger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkStagger.Name = "chkStagger";
            this.chkStagger.Size = new System.Drawing.Size(92, 24);
            this.chkStagger.TabIndex = 0;
            this.chkStagger.Text = "Stagger";
            this.chkStagger.UseVisualStyleBackColor = true;
            // 
            // numBlockSize
            // 
            this.numBlockSize.Location = new System.Drawing.Point(134, 62);
            this.numBlockSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numBlockSize.Name = "numBlockSize";
            this.numBlockSize.Size = new System.Drawing.Size(156, 26);
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
            this.numSpacingX.Location = new System.Drawing.Point(134, 102);
            this.numSpacingX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numSpacingX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numSpacingX.Name = "numSpacingX";
            this.numSpacingX.Size = new System.Drawing.Size(156, 26);
            this.numSpacingX.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Block Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 102);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "X Spacing";
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(59, 212);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(243, 35);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview For 3 Seconds";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 142);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
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
            this.numSpacingY.Location = new System.Drawing.Point(134, 142);
            this.numSpacingY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numSpacingY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numSpacingY.Name = "numSpacingY";
            this.numSpacingY.Size = new System.Drawing.Size(156, 26);
            this.numSpacingY.TabIndex = 8;
            // 
            // grpEllipse
            // 
            this.grpEllipse.Controls.Add(this.label3);
            this.grpEllipse.Controls.Add(this.chkStagger);
            this.grpEllipse.Controls.Add(this.numSpacingY);
            this.grpEllipse.Controls.Add(this.numBlockSize);
            this.grpEllipse.Controls.Add(this.numSpacingX);
            this.grpEllipse.Controls.Add(this.label2);
            this.grpEllipse.Controls.Add(this.label1);
            this.grpEllipse.Location = new System.Drawing.Point(6, 259);
            this.grpEllipse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEllipse.Name = "grpEllipse";
            this.grpEllipse.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpEllipse.Size = new System.Drawing.Size(406, 188);
            this.grpEllipse.TabIndex = 10;
            this.grpEllipse.TabStop = false;
            this.grpEllipse.Text = "Ellipse";
            // 
            // grpScanline
            // 
            this.grpScanline.Controls.Add(this.numScanlineSize);
            this.grpScanline.Controls.Add(this.label4);
            this.grpScanline.Controls.Add(this.chkHorizontal);
            this.grpScanline.Controls.Add(this.chkVertical);
            this.grpScanline.Location = new System.Drawing.Point(6, 456);
            this.grpScanline.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpScanline.Name = "grpScanline";
            this.grpScanline.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpScanline.Size = new System.Drawing.Size(406, 132);
            this.grpScanline.TabIndex = 11;
            this.grpScanline.TabStop = false;
            this.grpScanline.Text = "Scanline";
            // 
            // numScanlineSize
            // 
            this.numScanlineSize.Location = new System.Drawing.Point(134, 29);
            this.numScanlineSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numScanlineSize.Name = "numScanlineSize";
            this.numScanlineSize.Size = new System.Drawing.Size(156, 26);
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
            this.label4.Location = new System.Drawing.Point(22, 32);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 20);
            this.label4.TabIndex = 14;
            this.label4.Text = "Size";
            // 
            // chkHorizontal
            // 
            this.chkHorizontal.AutoSize = true;
            this.chkHorizontal.Checked = true;
            this.chkHorizontal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHorizontal.Location = new System.Drawing.Point(27, 95);
            this.chkHorizontal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkHorizontal.Name = "chkHorizontal";
            this.chkHorizontal.Size = new System.Drawing.Size(107, 24);
            this.chkHorizontal.TabIndex = 12;
            this.chkHorizontal.Text = "Horizontal";
            this.chkHorizontal.UseVisualStyleBackColor = true;
            // 
            // chkVertical
            // 
            this.chkVertical.AutoSize = true;
            this.chkVertical.Checked = true;
            this.chkVertical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVertical.Location = new System.Drawing.Point(26, 60);
            this.chkVertical.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkVertical.Name = "chkVertical";
            this.chkVertical.Size = new System.Drawing.Size(88, 24);
            this.chkVertical.TabIndex = 1;
            this.chkVertical.Text = "Vertical";
            this.chkVertical.UseVisualStyleBackColor = true;
            // 
            // cmbMode
            // 
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Items.AddRange(new object[] {
            "Ellipse",
            "Scanline"});
            this.cmbMode.Location = new System.Drawing.Point(141, 91);
            this.cmbMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(154, 28);
            this.cmbMode.TabIndex = 12;
            this.cmbMode.SelectedIndexChanged += new System.EventHandler(this.cmbMode_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 95);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "Mode";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(303, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(183, 5);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 132);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "Opacity";
            // 
            // numOpacity
            // 
            this.numOpacity.DecimalPlaces = 2;
            this.numOpacity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numOpacity.Location = new System.Drawing.Point(141, 132);
            this.numOpacity.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numOpacity.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.numOpacity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numOpacity.Name = "numOpacity";
            this.numOpacity.Size = new System.Drawing.Size(156, 26);
            this.numOpacity.TabIndex = 17;
            this.numOpacity.Value = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            // 
            // numThickness
            // 
            this.numThickness.Location = new System.Drawing.Point(141, 172);
            this.numThickness.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numThickness.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.numThickness.Name = "numThickness";
            this.numThickness.Size = new System.Drawing.Size(156, 26);
            this.numThickness.TabIndex = 19;
            this.numThickness.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 172);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "Thickness";
            // 
            // tblInfo
            // 
            this.tblInfo.ColumnCount = 2;
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Controls.Add(this.label8, 1, 0);
            this.tblInfo.Controls.Add(this.pbInfo, 0, 0);
            this.tblInfo.Location = new System.Drawing.Point(0, 0);
            this.tblInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tblInfo.Name = "tblInfo";
            this.tblInfo.RowCount = 1;
            this.tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInfo.Size = new System.Drawing.Size(411, 75);
            this.tblInfo.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(52, 7);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(344, 60);
            this.label8.TabIndex = 3;
            this.label8.Text = "Works best with ports that use a software renderer and do not change your resolut" +
    "ion like Chocolate/Crispy Doom or Odamex.";
            // 
            // pbInfo
            // 
            this.pbInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbInfo.Location = new System.Drawing.Point(4, 5);
            this.pbInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(40, 65);
            this.pbInfo.TabIndex = 0;
            this.pbInfo.TabStop = false;
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tblMain.Controls.Add(this.panel1, 0, 1);
            this.tblMain.Controls.Add(titleBar, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tblMain.Size = new System.Drawing.Size(425, 705);
            this.tblMain.TabIndex = 21;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 648);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(419, 54);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tblInfo);
            this.panel1.Controls.Add(this.grpEllipse);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.grpScanline);
            this.panel1.Controls.Add(this.numThickness);
            this.panel1.Controls.Add(this.cmbMode);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.numOpacity);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(419, 599);
            this.panel1.TabIndex = 1;
            // 
            // FilterSettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(425, 705);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThickness)).EndInit();
            this.tblInfo.ResumeLayout(false);
            this.tblInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).EndInit();
            this.tblMain.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numOpacity;
        private System.Windows.Forms.NumericUpDown numThickness;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel tblInfo;
        private System.Windows.Forms.PictureBox pbInfo;
        private System.Windows.Forms.Label label8;
        private TableLayoutPanelDB tblMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}