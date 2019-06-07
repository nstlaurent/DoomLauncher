namespace DoomLauncher
{
    partial class PlayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayForm));
            this.cmbSourcePorts = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbIwad = new System.Windows.Forms.ComboBox();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSkill = new System.Windows.Forms.ComboBox();
            this.chkRecord = new System.Windows.Forms.CheckBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cmbDemo = new System.Windows.Forms.ComboBox();
            this.chkDemo = new System.Windows.Forms.CheckBox();
            this.chkMap = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lnkOpenDemo = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtParameters = new System.Windows.Forms.TextBox();
            this.chkPreview = new System.Windows.Forms.CheckBox();
            this.lnkMore = new System.Windows.Forms.LinkLabel();
            this.chkSaveStats = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkScreenFilter = new System.Windows.Forms.CheckBox();
            this.lnkPreviewLaunchParameters = new System.Windows.Forms.LinkLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tblFiles = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pbInfo = new System.Windows.Forms.PictureBox();
            this.flp1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lnkSpecific = new System.Windows.Forms.LinkLabel();
            this.lnkCustomParameters = new System.Windows.Forms.LinkLabel();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.tblInner = new System.Windows.Forms.TableLayoutPanel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.ctrlFiles = new DoomLauncher.FilesCtrl();
            this.lnkFilterSettings = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tblFiles.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).BeginInit();
            this.flp1.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.tblInner.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbSourcePorts
            // 
            this.cmbSourcePorts.DisplayMember = "Name";
            this.cmbSourcePorts.FormattingEnabled = true;
            this.cmbSourcePorts.Location = new System.Drawing.Point(47, 19);
            this.cmbSourcePorts.Name = "cmbSourcePorts";
            this.cmbSourcePorts.Size = new System.Drawing.Size(197, 21);
            this.cmbSourcePorts.TabIndex = 0;
            this.cmbSourcePorts.SelectedIndexChanged += new System.EventHandler(this.cmbSourcePorts_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(41, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(122, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmbIwad
            // 
            this.cmbIwad.DisplayMember = "FileName";
            this.cmbIwad.FormattingEnabled = true;
            this.cmbIwad.Location = new System.Drawing.Point(47, 46);
            this.cmbIwad.Name = "cmbIwad";
            this.cmbIwad.Size = new System.Drawing.Size(197, 21);
            this.cmbIwad.TabIndex = 3;
            this.cmbIwad.ValueMember = "GameFileID";
            this.cmbIwad.SelectedIndexChanged += new System.EventHandler(this.cmbIwad_SelectedIndexChanged);
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Checked = true;
            this.chkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemember.Location = new System.Drawing.Point(9, 9);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(118, 17);
            this.chkRemember.TabIndex = 4;
            this.chkRemember.Text = "Remember Settings";
            this.chkRemember.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbSourcePorts);
            this.groupBox1.Controls.Add(this.cmbIwad);
            this.groupBox1.Location = new System.Drawing.Point(3, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 80);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "IWAD";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Port";
            // 
            // cmbMap
            // 
            this.cmbMap.DisplayMember = "Name";
            this.cmbMap.Enabled = false;
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(86, 19);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(160, 21);
            this.cmbMap.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Skill";
            // 
            // cmbSkill
            // 
            this.cmbSkill.DisplayMember = "Name";
            this.cmbSkill.Enabled = false;
            this.cmbSkill.FormattingEnabled = true;
            this.cmbSkill.Location = new System.Drawing.Point(86, 46);
            this.cmbSkill.Name = "cmbSkill";
            this.cmbSkill.Size = new System.Drawing.Size(160, 21);
            this.cmbSkill.TabIndex = 10;
            // 
            // chkRecord
            // 
            this.chkRecord.AutoSize = true;
            this.chkRecord.Location = new System.Drawing.Point(6, 115);
            this.chkRecord.Name = "chkRecord";
            this.chkRecord.Size = new System.Drawing.Size(61, 17);
            this.chkRecord.TabIndex = 12;
            this.chkRecord.Text = "Record";
            this.chkRecord.UseVisualStyleBackColor = true;
            this.chkRecord.CheckedChanged += new System.EventHandler(this.chkRecord_CheckedChanged);
            // 
            // txtDescription
            // 
            this.txtDescription.Enabled = false;
            this.txtDescription.Location = new System.Drawing.Point(85, 113);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(160, 20);
            this.txtDescription.TabIndex = 13;
            // 
            // cmbDemo
            // 
            this.cmbDemo.DisplayMember = "Description";
            this.cmbDemo.Enabled = false;
            this.cmbDemo.FormattingEnabled = true;
            this.cmbDemo.Location = new System.Drawing.Point(86, 73);
            this.cmbDemo.Name = "cmbDemo";
            this.cmbDemo.Size = new System.Drawing.Size(160, 21);
            this.cmbDemo.TabIndex = 14;
            this.cmbDemo.ValueMember = "FileID";
            this.cmbDemo.SelectedIndexChanged += new System.EventHandler(this.cmbDemo_SelectedIndexChanged);
            // 
            // chkDemo
            // 
            this.chkDemo.AutoSize = true;
            this.chkDemo.Location = new System.Drawing.Point(7, 75);
            this.chkDemo.Name = "chkDemo";
            this.chkDemo.Size = new System.Drawing.Size(77, 17);
            this.chkDemo.TabIndex = 15;
            this.chkDemo.Text = "Play Demo";
            this.chkDemo.UseVisualStyleBackColor = true;
            this.chkDemo.CheckedChanged += new System.EventHandler(this.chkDemo_CheckedChanged);
            // 
            // chkMap
            // 
            this.chkMap.AutoSize = true;
            this.chkMap.Location = new System.Drawing.Point(7, 21);
            this.chkMap.Name = "chkMap";
            this.chkMap.Size = new System.Drawing.Size(47, 17);
            this.chkMap.TabIndex = 16;
            this.chkMap.Text = "Map";
            this.chkMap.UseVisualStyleBackColor = true;
            this.chkMap.CheckedChanged += new System.EventHandler(this.chkMap_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lnkOpenDemo);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtParameters);
            this.groupBox2.Controls.Add(this.cmbMap);
            this.groupBox2.Controls.Add(this.chkMap);
            this.groupBox2.Controls.Add(this.cmbSkill);
            this.groupBox2.Controls.Add(this.chkDemo);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cmbDemo);
            this.groupBox2.Controls.Add(this.chkRecord);
            this.groupBox2.Controls.Add(this.txtDescription);
            this.groupBox2.Location = new System.Drawing.Point(3, 93);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 173);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            // 
            // lnkOpenDemo
            // 
            this.lnkOpenDemo.AutoSize = true;
            this.lnkOpenDemo.Location = new System.Drawing.Point(83, 97);
            this.lnkOpenDemo.Name = "lnkOpenDemo";
            this.lnkOpenDemo.Size = new System.Drawing.Size(92, 13);
            this.lnkOpenDemo.TabIndex = 19;
            this.lnkOpenDemo.TabStop = true;
            this.lnkOpenDemo.Text = "Open Demo File...";
            this.lnkOpenDemo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenDemo_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Extra Params";
            // 
            // txtParameters
            // 
            this.txtParameters.Location = new System.Drawing.Point(84, 139);
            this.txtParameters.Name = "txtParameters";
            this.txtParameters.Size = new System.Drawing.Size(160, 20);
            this.txtParameters.TabIndex = 17;
            // 
            // chkPreview
            // 
            this.chkPreview.AutoSize = true;
            this.chkPreview.Location = new System.Drawing.Point(6, 43);
            this.chkPreview.Name = "chkPreview";
            this.chkPreview.Size = new System.Drawing.Size(159, 17);
            this.chkPreview.TabIndex = 21;
            this.chkPreview.Text = "Preview Launch Parameters";
            this.chkPreview.UseVisualStyleBackColor = true;
            // 
            // lnkMore
            // 
            this.lnkMore.AutoSize = true;
            this.lnkMore.Location = new System.Drawing.Point(108, 20);
            this.lnkMore.Name = "lnkMore";
            this.lnkMore.Size = new System.Drawing.Size(61, 13);
            this.lnkMore.TabIndex = 20;
            this.lnkMore.TabStop = true;
            this.lnkMore.Text = "More Info...";
            this.lnkMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMore_LinkClicked);
            // 
            // chkSaveStats
            // 
            this.chkSaveStats.AutoSize = true;
            this.chkSaveStats.Checked = true;
            this.chkSaveStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveStats.Location = new System.Drawing.Point(6, 20);
            this.chkSaveStats.Name = "chkSaveStats";
            this.chkSaveStats.Size = new System.Drawing.Size(96, 17);
            this.chkSaveStats.TabIndex = 19;
            this.chkSaveStats.Text = "Save Statistics";
            this.chkSaveStats.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lnkFilterSettings);
            this.groupBox4.Controls.Add(this.chkScreenFilter);
            this.groupBox4.Controls.Add(this.lnkPreviewLaunchParameters);
            this.groupBox4.Controls.Add(this.chkPreview);
            this.groupBox4.Controls.Add(this.chkSaveStats);
            this.groupBox4.Controls.Add(this.lnkMore);
            this.groupBox4.Location = new System.Drawing.Point(3, 279);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(256, 93);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            // 
            // chkScreenFilter
            // 
            this.chkScreenFilter.AutoSize = true;
            this.chkScreenFilter.Location = new System.Drawing.Point(6, 65);
            this.chkScreenFilter.Name = "chkScreenFilter";
            this.chkScreenFilter.Size = new System.Drawing.Size(85, 17);
            this.chkScreenFilter.TabIndex = 23;
            this.chkScreenFilter.Text = "Screen Filter";
            this.chkScreenFilter.UseVisualStyleBackColor = true;
            this.chkScreenFilter.CheckedChanged += new System.EventHandler(this.chkScreenFilter_CheckedChanged);
            // 
            // lnkPreviewLaunchParameters
            // 
            this.lnkPreviewLaunchParameters.AutoSize = true;
            this.lnkPreviewLaunchParameters.Location = new System.Drawing.Point(171, 44);
            this.lnkPreviewLaunchParameters.Name = "lnkPreviewLaunchParameters";
            this.lnkPreviewLaunchParameters.Size = new System.Drawing.Size(34, 13);
            this.lnkPreviewLaunchParameters.TabIndex = 22;
            this.lnkPreviewLaunchParameters.TabStop = true;
            this.lnkPreviewLaunchParameters.Text = "Show";
            this.lnkPreviewLaunchParameters.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPreviewLaunchParameters_LinkClicked);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tblFiles);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(265, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(236, 369);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Additional Files / Load Order";
            // 
            // tblFiles
            // 
            this.tblFiles.ColumnCount = 1;
            this.tblFiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFiles.Controls.Add(this.ctrlFiles, 0, 1);
            this.tblFiles.Controls.Add(this.panel1, 0, 0);
            this.tblFiles.Controls.Add(this.flp1, 0, 2);
            this.tblFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFiles.Location = new System.Drawing.Point(3, 16);
            this.tblFiles.Margin = new System.Windows.Forms.Padding(0);
            this.tblFiles.Name = "tblFiles";
            this.tblFiles.RowCount = 3;
            this.tblFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblFiles.Size = new System.Drawing.Size(230, 350);
            this.tblFiles.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblInfo);
            this.panel1.Controls.Add(this.pbInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 40);
            this.panel1.TabIndex = 21;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(25, 7);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(29, 13);
            this.lblInfo.TabIndex = 25;
            this.lblInfo.Text = "label";
            // 
            // pbInfo
            // 
            this.pbInfo.Location = new System.Drawing.Point(3, 7);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(16, 16);
            this.pbInfo.TabIndex = 24;
            this.pbInfo.TabStop = false;
            // 
            // flp1
            // 
            this.flp1.Controls.Add(this.lnkSpecific);
            this.flp1.Controls.Add(this.lnkCustomParameters);
            this.flp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp1.Location = new System.Drawing.Point(0, 326);
            this.flp1.Margin = new System.Windows.Forms.Padding(0);
            this.flp1.Name = "flp1";
            this.flp1.Size = new System.Drawing.Size(230, 24);
            this.flp1.TabIndex = 22;
            // 
            // lnkSpecific
            // 
            this.lnkSpecific.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkSpecific.AutoSize = true;
            this.lnkSpecific.Location = new System.Drawing.Point(3, 3);
            this.lnkSpecific.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lnkSpecific.Name = "lnkSpecific";
            this.lnkSpecific.Size = new System.Drawing.Size(118, 13);
            this.lnkSpecific.TabIndex = 20;
            this.lnkSpecific.TabStop = true;
            this.lnkSpecific.Text = "Select Individual Files...";
            this.lnkSpecific.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSpecific_LinkClicked);
            // 
            // lnkCustomParameters
            // 
            this.lnkCustomParameters.AutoSize = true;
            this.lnkCustomParameters.Location = new System.Drawing.Point(127, 3);
            this.lnkCustomParameters.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lnkCustomParameters.Name = "lnkCustomParameters";
            this.lnkCustomParameters.Size = new System.Drawing.Size(89, 13);
            this.lnkCustomParameters.TabIndex = 21;
            this.lnkCustomParameters.TabStop = true;
            this.lnkCustomParameters.Text = "Custom Params...";
            this.lnkCustomParameters.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCustomParameters_LinkClicked);
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tblInner, 0, 0);
            this.tblMain.Controls.Add(this.pnlBottom, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.Size = new System.Drawing.Size(504, 407);
            this.tblMain.TabIndex = 21;
            // 
            // tblInner
            // 
            this.tblInner.ColumnCount = 2;
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 262F));
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.Controls.Add(this.pnlLeft, 0, 0);
            this.tblInner.Controls.Add(this.groupBox3, 1, 0);
            this.tblInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInner.Location = new System.Drawing.Point(0, 0);
            this.tblInner.Margin = new System.Windows.Forms.Padding(0);
            this.tblInner.Name = "tblInner";
            this.tblInner.RowCount = 1;
            this.tblInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.Size = new System.Drawing.Size(504, 375);
            this.tblInner.TabIndex = 0;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.groupBox1);
            this.pnlLeft.Controls.Add(this.groupBox4);
            this.pnlLeft.Controls.Add(this.groupBox2);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(262, 375);
            this.pnlLeft.TabIndex = 0;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnSaveSettings);
            this.pnlBottom.Controls.Add(this.flpButtons);
            this.pnlBottom.Controls.Add(this.chkRemember);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.Location = new System.Drawing.Point(0, 375);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(504, 32);
            this.pnlBottom.TabIndex = 1;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(133, 5);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(82, 23);
            this.btnSaveSettings.TabIndex = 6;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnOK);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(304, 0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.flpButtons.Size = new System.Drawing.Size(200, 32);
            this.flpButtons.TabIndex = 5;
            // 
            // ctrlFiles
            // 
            this.ctrlFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlFiles.Location = new System.Drawing.Point(3, 43);
            this.ctrlFiles.Name = "ctrlFiles";
            this.ctrlFiles.Size = new System.Drawing.Size(224, 280);
            this.ctrlFiles.TabIndex = 20;
            // 
            // lnkFilterSettings
            // 
            this.lnkFilterSettings.AutoSize = true;
            this.lnkFilterSettings.Location = new System.Drawing.Point(97, 65);
            this.lnkFilterSettings.Name = "lnkFilterSettings";
            this.lnkFilterSettings.Size = new System.Drawing.Size(45, 13);
            this.lnkFilterSettings.TabIndex = 24;
            this.lnkFilterSettings.TabStop = true;
            this.lnkFilterSettings.Text = "Settings";
            this.lnkFilterSettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkFilterSettings_LinkClicked);
            // 
            // PlayForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(504, 407);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PlayForm";
            this.ShowIcon = false;
            this.Text = "Launch";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tblFiles.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).EndInit();
            this.flp1.ResumeLayout(false);
            this.flp1.PerformLayout();
            this.tblMain.ResumeLayout(false);
            this.tblInner.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSourcePorts;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbIwad;
        private System.Windows.Forms.CheckBox chkRemember;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSkill;
        private System.Windows.Forms.CheckBox chkRecord;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ComboBox cmbDemo;
        private System.Windows.Forms.CheckBox chkDemo;
        private System.Windows.Forms.CheckBox chkMap;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtParameters;
        private System.Windows.Forms.CheckBox chkSaveStats;
        private System.Windows.Forms.LinkLabel lnkMore;
        private System.Windows.Forms.CheckBox chkPreview;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private FilesCtrl ctrlFiles;
        private System.Windows.Forms.TableLayoutPanel tblFiles;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.TableLayoutPanel tblInner;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.PictureBox pbInfo;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.LinkLabel lnkOpenDemo;
        private System.Windows.Forms.LinkLabel lnkPreviewLaunchParameters;
        private System.Windows.Forms.FlowLayoutPanel flp1;
        private System.Windows.Forms.LinkLabel lnkSpecific;
        private System.Windows.Forms.LinkLabel lnkCustomParameters;
        private System.Windows.Forms.CheckBox chkScreenFilter;
        private System.Windows.Forms.LinkLabel lnkFilterSettings;
    }
}