namespace DoomLauncher
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tblOuter = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.tabPageDefault = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblLaunchSettings = new System.Windows.Forms.Label();
            this.cmbSkill = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSourcePorts = new System.Windows.Forms.ComboBox();
            this.cmbIwad = new System.Windows.Forms.ComboBox();
            this.tabPageFileManagement = new System.Windows.Forms.TabPage();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tblFileOptions = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbFileManagement = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPageView = new System.Windows.Forms.TabPage();
            this.pnlViewRestart = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbViewType = new System.Windows.Forms.ComboBox();
            this.tblOuter.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageDefault.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPageFileManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tblFileOptions.SuspendLayout();
            this.tabPageView.SuspendLayout();
            this.pnlViewRestart.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblOuter
            // 
            this.tblOuter.ColumnCount = 1;
            this.tblOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblOuter.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tblOuter.Controls.Add(this.tabControl, 0, 0);
            this.tblOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblOuter.Location = new System.Drawing.Point(0, 0);
            this.tblOuter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tblOuter.Name = "tblOuter";
            this.tblOuter.RowCount = 2;
            this.tblOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblOuter.Size = new System.Drawing.Size(450, 291);
            this.tblOuter.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 252);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(450, 39);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(346, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(238, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageConfig);
            this.tabControl.Controls.Add(this.tabPageDefault);
            this.tabControl.Controls.Add(this.tabPageFileManagement);
            this.tabControl.Controls.Add(this.tabPageView);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(4, 4);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(442, 244);
            this.tabControl.TabIndex = 1;
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageConfig.Location = new System.Drawing.Point(4, 25);
            this.tabPageConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageConfig.Size = new System.Drawing.Size(434, 215);
            this.tabPageConfig.TabIndex = 0;
            this.tabPageConfig.Text = "Configuration";
            // 
            // tabPageDefault
            // 
            this.tabPageDefault.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDefault.Controls.Add(this.pictureBox1);
            this.tabPageDefault.Controls.Add(this.lblLaunchSettings);
            this.tabPageDefault.Controls.Add(this.cmbSkill);
            this.tabPageDefault.Controls.Add(this.label4);
            this.tabPageDefault.Controls.Add(this.label2);
            this.tabPageDefault.Controls.Add(this.label1);
            this.tabPageDefault.Controls.Add(this.cmbSourcePorts);
            this.tabPageDefault.Controls.Add(this.cmbIwad);
            this.tabPageDefault.Location = new System.Drawing.Point(4, 25);
            this.tabPageDefault.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageDefault.Name = "tabPageDefault";
            this.tabPageDefault.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageDefault.Size = new System.Drawing.Size(529, 284);
            this.tabPageDefault.TabIndex = 1;
            this.tabPageDefault.Text = "Launch Settings";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DoomLauncher.Properties.Resources.bon2b;
            this.pictureBox1.Location = new System.Drawing.Point(13, 124);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(28, 22);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // lblLaunchSettings
            // 
            this.lblLaunchSettings.AutoSize = true;
            this.lblLaunchSettings.Location = new System.Drawing.Point(49, 124);
            this.lblLaunchSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLaunchSettings.Name = "lblLaunchSettings";
            this.lblLaunchSettings.Size = new System.Drawing.Size(33, 16);
            this.lblLaunchSettings.TabIndex = 15;
            this.lblLaunchSettings.Text = "Text";
            // 
            // cmbSkill
            // 
            this.cmbSkill.DisplayMember = "Name";
            this.cmbSkill.FormattingEnabled = true;
            this.cmbSkill.Location = new System.Drawing.Point(64, 82);
            this.cmbSkill.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSkill.Name = "cmbSkill";
            this.cmbSkill.Size = new System.Drawing.Size(261, 24);
            this.cmbSkill.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 86);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Skill";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "IWAD";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Port";
            // 
            // cmbSourcePorts
            // 
            this.cmbSourcePorts.DisplayMember = "Name";
            this.cmbSourcePorts.FormattingEnabled = true;
            this.cmbSourcePorts.Location = new System.Drawing.Point(64, 16);
            this.cmbSourcePorts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSourcePorts.Name = "cmbSourcePorts";
            this.cmbSourcePorts.Size = new System.Drawing.Size(261, 24);
            this.cmbSourcePorts.TabIndex = 9;
            this.cmbSourcePorts.ValueMember = "SourcePortID";
            // 
            // cmbIwad
            // 
            this.cmbIwad.DisplayMember = "FileName";
            this.cmbIwad.FormattingEnabled = true;
            this.cmbIwad.Location = new System.Drawing.Point(64, 49);
            this.cmbIwad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbIwad.Name = "cmbIwad";
            this.cmbIwad.Size = new System.Drawing.Size(261, 24);
            this.cmbIwad.TabIndex = 10;
            this.cmbIwad.ValueMember = "IWadID";
            // 
            // tabPageFileManagement
            // 
            this.tabPageFileManagement.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageFileManagement.Controls.Add(this.pictureBox2);
            this.tabPageFileManagement.Controls.Add(this.tblFileOptions);
            this.tabPageFileManagement.Controls.Add(this.cmbFileManagement);
            this.tabPageFileManagement.Controls.Add(this.label10);
            this.tabPageFileManagement.Location = new System.Drawing.Point(4, 25);
            this.tabPageFileManagement.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageFileManagement.Name = "tabPageFileManagement";
            this.tabPageFileManagement.Size = new System.Drawing.Size(529, 284);
            this.tabPageFileManagement.TabIndex = 2;
            this.tabPageFileManagement.Text = "File Management";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::DoomLauncher.Properties.Resources.bon2b;
            this.pictureBox2.Location = new System.Drawing.Point(19, 217);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(28, 22);
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            // 
            // tblFileOptions
            // 
            this.tblFileOptions.ColumnCount = 2;
            this.tblFileOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tblFileOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFileOptions.Controls.Add(this.label9, 1, 2);
            this.tblFileOptions.Controls.Add(this.label8, 1, 1);
            this.tblFileOptions.Controls.Add(this.label3, 0, 0);
            this.tblFileOptions.Controls.Add(this.label5, 0, 1);
            this.tblFileOptions.Controls.Add(this.label6, 0, 2);
            this.tblFileOptions.Controls.Add(this.label7, 1, 0);
            this.tblFileOptions.Location = new System.Drawing.Point(19, 68);
            this.tblFileOptions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tblFileOptions.Name = "tblFileOptions";
            this.tblFileOptions.RowCount = 4;
            this.tblFileOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblFileOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblFileOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblFileOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tblFileOptions.Size = new System.Drawing.Size(488, 142);
            this.tblFileOptions.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(124, 89);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(266, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "Prompted to choose when file(s) are added.";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(124, 42);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(360, 32);
            this.label8.TabIndex = 4;
            this.label8.Text = "Files are referenced by their original path and not managed by Doom Launcher.";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Managed";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(4, 50);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "Unmanaged";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(4, 89);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "Prompt";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(124, 3);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(330, 32);
            this.label7.TabIndex = 3;
            this.label7.Text = "Doom Launcher manages and compresses files in the GameFiles directory for you.";
            // 
            // cmbFileManagement
            // 
            this.cmbFileManagement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFileManagement.FormattingEnabled = true;
            this.cmbFileManagement.Location = new System.Drawing.Point(19, 17);
            this.cmbFileManagement.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbFileManagement.Name = "cmbFileManagement";
            this.cmbFileManagement.Size = new System.Drawing.Size(261, 24);
            this.cmbFileManagement.TabIndex = 10;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(55, 220);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(359, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "Downloaded files are always managed by Doom Launcher.";
            // 
            // tabPageView
            // 
            this.tabPageView.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageView.Controls.Add(this.pnlViewRestart);
            this.tabPageView.Controls.Add(this.label11);
            this.tabPageView.Controls.Add(this.cmbViewType);
            this.tabPageView.Location = new System.Drawing.Point(4, 25);
            this.tabPageView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageView.Name = "tabPageView";
            this.tabPageView.Size = new System.Drawing.Size(529, 284);
            this.tabPageView.TabIndex = 3;
            this.tabPageView.Text = "View";
            // 
            // pnlViewRestart
            // 
            this.pnlViewRestart.Controls.Add(this.label12);
            this.pnlViewRestart.Location = new System.Drawing.Point(0, 54);
            this.pnlViewRestart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlViewRestart.Name = "pnlViewRestart";
            this.pnlViewRestart.Size = new System.Drawing.Size(501, 170);
            this.pnlViewRestart.TabIndex = 13;
            this.pnlViewRestart.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(76, 22);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(375, 16);
            this.label12.TabIndex = 0;
            this.label12.Text = "Changing from tile view to grid view will restart Doom Launcher.";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 21);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 16);
            this.label11.TabIndex = 12;
            this.label11.Text = "View Type";
            // 
            // cmbViewType
            // 
            this.cmbViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbViewType.FormattingEnabled = true;
            this.cmbViewType.Location = new System.Drawing.Point(117, 17);
            this.cmbViewType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbViewType.Name = "cmbViewType";
            this.cmbViewType.Size = new System.Drawing.Size(261, 24);
            this.cmbViewType.TabIndex = 11;
            this.cmbViewType.SelectedIndexChanged += new System.EventHandler(this.CmbViewType_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(540, 349);
            this.Controls.Add(this.tblOuter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.tblOuter.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageDefault.ResumeLayout(false);
            this.tabPageDefault.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPageFileManagement.ResumeLayout(false);
            this.tabPageFileManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tblFileOptions.ResumeLayout(false);
            this.tblFileOptions.PerformLayout();
            this.tabPageView.ResumeLayout(false);
            this.tabPageView.PerformLayout();
            this.pnlViewRestart.ResumeLayout(false);
            this.pnlViewRestart.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblOuter;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageConfig;
        private System.Windows.Forms.TabPage tabPageDefault;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSourcePorts;
        private System.Windows.Forms.ComboBox cmbIwad;
        private System.Windows.Forms.ComboBox cmbSkill;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblLaunchSettings;
        private System.Windows.Forms.TabPage tabPageFileManagement;
        private System.Windows.Forms.ComboBox cmbFileManagement;
        private System.Windows.Forms.TableLayoutPanel tblFileOptions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TabPage tabPageView;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbViewType;
        private System.Windows.Forms.Panel pnlViewRestart;
        private System.Windows.Forms.Label label12;
    }
}