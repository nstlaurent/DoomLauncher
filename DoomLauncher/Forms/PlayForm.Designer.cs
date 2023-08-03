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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tblProfile = new System.Windows.Forms.TableLayoutPanel();
            this.lblProfile = new System.Windows.Forms.Label();
            this.cmbProfiles = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.profileToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProfileMenu = new DoomLauncher.FormButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSkill = new System.Windows.Forms.ComboBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cmbDemo = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lnkOpenDemo = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtParameters = new System.Windows.Forms.TextBox();
            this.chkMap = new DoomLauncher.CCheckBox();
            this.chkDemo = new DoomLauncher.CCheckBox();
            this.chkRecord = new DoomLauncher.CCheckBox();
            this.lnkMore = new System.Windows.Forms.LinkLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lnkLoadSaveMore = new System.Windows.Forms.LinkLabel();
            this.chkLoadLatestSave = new DoomLauncher.CCheckBox();
            this.lnkFilterSettings = new System.Windows.Forms.LinkLabel();
            this.chkScreenFilter = new DoomLauncher.CCheckBox();
            this.lnkPreviewLaunchParameters = new System.Windows.Forms.LinkLabel();
            this.chkExtraParamsOnly = new DoomLauncher.CCheckBox();
            this.chkSaveStats = new DoomLauncher.CCheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tblFiles = new DoomLauncher.TableLayoutPanelDB();
            this.ctrlFiles = new DoomLauncher.FilesCtrl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pbInfo = new System.Windows.Forms.PictureBox();
            this.flp1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lnkSpecific = new System.Windows.Forms.LinkLabel();
            this.lnkCustomParameters = new System.Windows.Forms.LinkLabel();
            this.tblMain = new DoomLauncher.TableLayoutPanelDB();
            this.tblInner = new DoomLauncher.TableLayoutPanelDB();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.flpButtons = new DoomLauncher.FlowLayoutPanelDB();
            this.chkRemember = new DoomLauncher.CCheckBox();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.groupBox1.SuspendLayout();
            this.tblProfile.SuspendLayout();
            this.panel2.SuspendLayout();
            this.profileToolStrip.SuspendLayout();
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
            this.cmbSourcePorts.Location = new System.Drawing.Point(68, 57);
            this.cmbSourcePorts.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSourcePorts.Name = "cmbSourcePorts";
            this.cmbSourcePorts.Size = new System.Drawing.Size(256, 24);
            this.cmbSourcePorts.TabIndex = 0;
            this.cmbSourcePorts.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(55, 6);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(163, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmbIwad
            // 
            this.cmbIwad.DisplayMember = "FileName";
            this.cmbIwad.FormattingEnabled = true;
            this.cmbIwad.Location = new System.Drawing.Point(68, 91);
            this.cmbIwad.Margin = new System.Windows.Forms.Padding(4);
            this.cmbIwad.Name = "cmbIwad";
            this.cmbIwad.Size = new System.Drawing.Size(256, 24);
            this.cmbIwad.TabIndex = 3;
            this.cmbIwad.TabStop = false;
            this.cmbIwad.ValueMember = "GameFileID";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tblProfile);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbSourcePorts);
            this.groupBox1.Controls.Add(this.cmbIwad);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(341, 131);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // tblProfile
            // 
            this.tblProfile.ColumnCount = 3;
            this.tblProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tblProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tblProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblProfile.Controls.Add(this.lblProfile, 0, 0);
            this.tblProfile.Controls.Add(this.cmbProfiles, 1, 0);
            this.tblProfile.Controls.Add(this.panel2, 2, 0);
            this.tblProfile.Location = new System.Drawing.Point(4, 19);
            this.tblProfile.Margin = new System.Windows.Forms.Padding(0);
            this.tblProfile.Name = "tblProfile";
            this.tblProfile.RowCount = 1;
            this.tblProfile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblProfile.Size = new System.Drawing.Size(321, 31);
            this.tblProfile.TabIndex = 9;
            // 
            // lblProfile
            // 
            this.lblProfile.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProfile.AutoSize = true;
            this.lblProfile.Location = new System.Drawing.Point(4, 7);
            this.lblProfile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(45, 16);
            this.lblProfile.TabIndex = 10;
            this.lblProfile.Text = "Profile";
            // 
            // cmbProfiles
            // 
            this.cmbProfiles.DisplayMember = "Name";
            this.cmbProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbProfiles.FormattingEnabled = true;
            this.cmbProfiles.Location = new System.Drawing.Point(64, 4);
            this.cmbProfiles.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.cmbProfiles.Name = "cmbProfiles";
            this.cmbProfiles.Size = new System.Drawing.Size(221, 24);
            this.cmbProfiles.TabIndex = 9;
            this.cmbProfiles.TabStop = false;
            this.cmbProfiles.ValueMember = "GameProfileID";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.profileToolStrip);
            this.panel2.Controls.Add(this.btnProfileMenu);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(285, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(36, 31);
            this.panel2.TabIndex = 14;
            // 
            // profileToolStrip
            // 
            this.profileToolStrip.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.profileToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.profileToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.profileToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.profileToolStrip.Location = new System.Drawing.Point(0, 4);
            this.profileToolStrip.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.profileToolStrip.Name = "profileToolStrip";
            this.profileToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.profileToolStrip.Size = new System.Drawing.Size(39, 27);
            this.profileToolStrip.TabIndex = 12;
            this.profileToolStrip.Text = "Options";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProfileToolStripMenuItem,
            this.editProfileToolStripMenuItem,
            this.deleteProfileToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::DoomLauncher.Properties.Resources.Bars;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 24);
            this.toolStripDropDownButton1.Text = "Options";
            // 
            // newProfileToolStripMenuItem
            // 
            this.newProfileToolStripMenuItem.Image = global::DoomLauncher.Properties.Resources.File;
            this.newProfileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.newProfileToolStripMenuItem.Name = "newProfileToolStripMenuItem";
            this.newProfileToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.newProfileToolStripMenuItem.Text = "New Profile...";
            this.newProfileToolStripMenuItem.Click += new System.EventHandler(this.newProfileToolStripMenuItem_Click);
            // 
            // editProfileToolStripMenuItem
            // 
            this.editProfileToolStripMenuItem.Image = global::DoomLauncher.Properties.Resources.Edit;
            this.editProfileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.editProfileToolStripMenuItem.Name = "editProfileToolStripMenuItem";
            this.editProfileToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.editProfileToolStripMenuItem.Text = "Rename Profile...";
            this.editProfileToolStripMenuItem.Click += new System.EventHandler(this.editProfileToolStripMenuItem_Click);
            // 
            // deleteProfileToolStripMenuItem
            // 
            this.deleteProfileToolStripMenuItem.Image = global::DoomLauncher.Properties.Resources.Delete;
            this.deleteProfileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.deleteProfileToolStripMenuItem.Name = "deleteProfileToolStripMenuItem";
            this.deleteProfileToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.deleteProfileToolStripMenuItem.Text = "Delete Profile";
            this.deleteProfileToolStripMenuItem.Click += new System.EventHandler(this.deleteProfileToolStripMenuItem_Click);
            // 
            // btnProfileMenu
            // 
            this.btnProfileMenu.Image = global::DoomLauncher.Properties.Resources.Bars;
            this.btnProfileMenu.Location = new System.Drawing.Point(0, 6);
            this.btnProfileMenu.Margin = new System.Windows.Forms.Padding(0);
            this.btnProfileMenu.Name = "btnProfileMenu";
            this.btnProfileMenu.Size = new System.Drawing.Size(30, 26);
            this.btnProfileMenu.TabIndex = 13;
            this.btnProfileMenu.UseVisualStyleBackColor = true;
            this.btnProfileMenu.Click += new System.EventHandler(this.btnProfileMenu_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 95);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "IWAD";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Port";
            // 
            // cmbMap
            // 
            this.cmbMap.DisplayMember = "Name";
            this.cmbMap.Enabled = false;
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(115, 23);
            this.cmbMap.Margin = new System.Windows.Forms.Padding(4);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(212, 24);
            this.cmbMap.TabIndex = 6;
            this.cmbMap.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 60);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Skill";
            // 
            // cmbSkill
            // 
            this.cmbSkill.DisplayMember = "Name";
            this.cmbSkill.Enabled = false;
            this.cmbSkill.FormattingEnabled = true;
            this.cmbSkill.Location = new System.Drawing.Point(115, 57);
            this.cmbSkill.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSkill.Name = "cmbSkill";
            this.cmbSkill.Size = new System.Drawing.Size(212, 24);
            this.cmbSkill.TabIndex = 10;
            this.cmbSkill.TabStop = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Enabled = false;
            this.txtDescription.Location = new System.Drawing.Point(115, 138);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(212, 22);
            this.txtDescription.TabIndex = 13;
            this.txtDescription.TabStop = false;
            // 
            // cmbDemo
            // 
            this.cmbDemo.DisplayMember = "Description";
            this.cmbDemo.Enabled = false;
            this.cmbDemo.FormattingEnabled = true;
            this.cmbDemo.Location = new System.Drawing.Point(115, 90);
            this.cmbDemo.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDemo.Name = "cmbDemo";
            this.cmbDemo.Size = new System.Drawing.Size(212, 24);
            this.cmbDemo.TabIndex = 14;
            this.cmbDemo.TabStop = false;
            this.cmbDemo.ValueMember = "FileID";
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
            this.groupBox2.Location = new System.Drawing.Point(4, 142);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(341, 213);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            // 
            // lnkOpenDemo
            // 
            this.lnkOpenDemo.AutoSize = true;
            this.lnkOpenDemo.Location = new System.Drawing.Point(111, 119);
            this.lnkOpenDemo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkOpenDemo.Name = "lnkOpenDemo";
            this.lnkOpenDemo.Size = new System.Drawing.Size(114, 16);
            this.lnkOpenDemo.TabIndex = 19;
            this.lnkOpenDemo.TabStop = true;
            this.lnkOpenDemo.Text = "Open Demo File...";
            this.lnkOpenDemo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenDemo_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 175);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Extra Params";
            // 
            // txtParameters
            // 
            this.txtParameters.Location = new System.Drawing.Point(115, 171);
            this.txtParameters.Margin = new System.Windows.Forms.Padding(4);
            this.txtParameters.Name = "txtParameters";
            this.txtParameters.Size = new System.Drawing.Size(212, 22);
            this.txtParameters.TabIndex = 17;
            this.txtParameters.TabStop = false;
            this.txtParameters.Click += new System.EventHandler(this.TxtParameters_Click);
            // 
            // chkMap
            // 
            this.chkMap.AutoSize = true;
            this.chkMap.Location = new System.Drawing.Point(9, 26);
            this.chkMap.Margin = new System.Windows.Forms.Padding(4);
            this.chkMap.Name = "chkMap";
            this.chkMap.Size = new System.Drawing.Size(56, 20);
            this.chkMap.TabIndex = 16;
            this.chkMap.TabStop = false;
            this.chkMap.Text = "Map";
            this.chkMap.CheckedChanged += new System.EventHandler(this.chkMap_CheckedChanged);
            // 
            // chkDemo
            // 
            this.chkDemo.AutoSize = true;
            this.chkDemo.Location = new System.Drawing.Point(9, 92);
            this.chkDemo.Margin = new System.Windows.Forms.Padding(4);
            this.chkDemo.Name = "chkDemo";
            this.chkDemo.Size = new System.Drawing.Size(96, 20);
            this.chkDemo.TabIndex = 15;
            this.chkDemo.TabStop = false;
            this.chkDemo.Text = "Play Demo";
            this.chkDemo.CheckedChanged += new System.EventHandler(this.chkDemo_CheckedChanged);
            // 
            // chkRecord
            // 
            this.chkRecord.AutoSize = true;
            this.chkRecord.Location = new System.Drawing.Point(8, 142);
            this.chkRecord.Margin = new System.Windows.Forms.Padding(4);
            this.chkRecord.Name = "chkRecord";
            this.chkRecord.Size = new System.Drawing.Size(74, 20);
            this.chkRecord.TabIndex = 12;
            this.chkRecord.TabStop = false;
            this.chkRecord.Text = "Record";
            this.chkRecord.CheckedChanged += new System.EventHandler(this.chkRecord_CheckedChanged);
            // 
            // lnkMore
            // 
            this.lnkMore.AutoSize = true;
            this.lnkMore.Location = new System.Drawing.Point(218, 26);
            this.lnkMore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkMore.Name = "lnkMore";
            this.lnkMore.Size = new System.Drawing.Size(71, 16);
            this.lnkMore.TabIndex = 20;
            this.lnkMore.TabStop = true;
            this.lnkMore.Text = "More Info...";
            this.lnkMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMore_LinkClicked);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lnkLoadSaveMore);
            this.groupBox4.Controls.Add(this.chkLoadLatestSave);
            this.groupBox4.Controls.Add(this.lnkFilterSettings);
            this.groupBox4.Controls.Add(this.chkScreenFilter);
            this.groupBox4.Controls.Add(this.lnkPreviewLaunchParameters);
            this.groupBox4.Controls.Add(this.chkExtraParamsOnly);
            this.groupBox4.Controls.Add(this.chkSaveStats);
            this.groupBox4.Controls.Add(this.lnkMore);
            this.groupBox4.Location = new System.Drawing.Point(4, 362);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(341, 142);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            // 
            // lnkLoadSaveMore
            // 
            this.lnkLoadSaveMore.AutoSize = true;
            this.lnkLoadSaveMore.Location = new System.Drawing.Point(218, 54);
            this.lnkLoadSaveMore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkLoadSaveMore.Name = "lnkLoadSaveMore";
            this.lnkLoadSaveMore.Size = new System.Drawing.Size(71, 16);
            this.lnkLoadSaveMore.TabIndex = 26;
            this.lnkLoadSaveMore.TabStop = true;
            this.lnkLoadSaveMore.Text = "More Info...";
            this.lnkLoadSaveMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLoadSaveMore_LinkClicked);
            // 
            // chkLoadLatestSave
            // 
            this.chkLoadLatestSave.AutoSize = true;
            this.chkLoadLatestSave.Location = new System.Drawing.Point(8, 53);
            this.chkLoadLatestSave.Margin = new System.Windows.Forms.Padding(4);
            this.chkLoadLatestSave.Name = "chkLoadLatestSave";
            this.chkLoadLatestSave.Size = new System.Drawing.Size(134, 20);
            this.chkLoadLatestSave.TabIndex = 25;
            this.chkLoadLatestSave.TabStop = false;
            this.chkLoadLatestSave.Text = "Load Latest Save";
            // 
            // lnkFilterSettings
            // 
            this.lnkFilterSettings.AutoSize = true;
            this.lnkFilterSettings.Location = new System.Drawing.Point(218, 81);
            this.lnkFilterSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkFilterSettings.Name = "lnkFilterSettings";
            this.lnkFilterSettings.Size = new System.Drawing.Size(55, 16);
            this.lnkFilterSettings.TabIndex = 24;
            this.lnkFilterSettings.TabStop = true;
            this.lnkFilterSettings.Text = "Settings";
            this.lnkFilterSettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkFilterSettings_LinkClicked);
            // 
            // chkScreenFilter
            // 
            this.chkScreenFilter.AutoSize = true;
            this.chkScreenFilter.Location = new System.Drawing.Point(8, 81);
            this.chkScreenFilter.Margin = new System.Windows.Forms.Padding(4);
            this.chkScreenFilter.Name = "chkScreenFilter";
            this.chkScreenFilter.Size = new System.Drawing.Size(104, 20);
            this.chkScreenFilter.TabIndex = 23;
            this.chkScreenFilter.TabStop = false;
            this.chkScreenFilter.Text = "Screen Filter";
            this.chkScreenFilter.CheckedChanged += new System.EventHandler(this.chkScreenFilter_CheckedChanged);
            // 
            // lnkPreviewLaunchParameters
            // 
            this.lnkPreviewLaunchParameters.AutoSize = true;
            this.lnkPreviewLaunchParameters.Location = new System.Drawing.Point(218, 109);
            this.lnkPreviewLaunchParameters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkPreviewLaunchParameters.Name = "lnkPreviewLaunchParameters";
            this.lnkPreviewLaunchParameters.Size = new System.Drawing.Size(113, 16);
            this.lnkPreviewLaunchParameters.TabIndex = 22;
            this.lnkPreviewLaunchParameters.TabStop = true;
            this.lnkPreviewLaunchParameters.Text = "Show Parameters";
            this.lnkPreviewLaunchParameters.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPreviewLaunchParameters_LinkClicked);
            // 
            // chkExtraParamsOnly
            // 
            this.chkExtraParamsOnly.AutoSize = true;
            this.chkExtraParamsOnly.Location = new System.Drawing.Point(8, 109);
            this.chkExtraParamsOnly.Margin = new System.Windows.Forms.Padding(4);
            this.chkExtraParamsOnly.Name = "chkExtraParamsOnly";
            this.chkExtraParamsOnly.Size = new System.Drawing.Size(139, 20);
            this.chkExtraParamsOnly.TabIndex = 21;
            this.chkExtraParamsOnly.TabStop = false;
            this.chkExtraParamsOnly.Text = "Extra Params Only";
            // 
            // chkSaveStats
            // 
            this.chkSaveStats.AutoSize = true;
            this.chkSaveStats.Checked = true;
            this.chkSaveStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveStats.Location = new System.Drawing.Point(8, 25);
            this.chkSaveStats.Margin = new System.Windows.Forms.Padding(4);
            this.chkSaveStats.Name = "chkSaveStats";
            this.chkSaveStats.Size = new System.Drawing.Size(117, 20);
            this.chkSaveStats.TabIndex = 19;
            this.chkSaveStats.TabStop = false;
            this.chkSaveStats.Text = "Save Statistics";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tblFiles);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(353, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(315, 500);
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
            this.tblFiles.Location = new System.Drawing.Point(4, 19);
            this.tblFiles.Margin = new System.Windows.Forms.Padding(0);
            this.tblFiles.Name = "tblFiles";
            this.tblFiles.RowCount = 3;
            this.tblFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblFiles.Size = new System.Drawing.Size(307, 477);
            this.tblFiles.TabIndex = 0;
            // 
            // ctrlFiles
            // 
            this.ctrlFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.ctrlFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlFiles.ForeColor = System.Drawing.Color.White;
            this.ctrlFiles.Location = new System.Drawing.Point(5, 54);
            this.ctrlFiles.Margin = new System.Windows.Forms.Padding(5);
            this.ctrlFiles.Name = "ctrlFiles";
            this.ctrlFiles.Size = new System.Drawing.Size(297, 388);
            this.ctrlFiles.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblInfo);
            this.panel1.Controls.Add(this.pbInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 49);
            this.panel1.TabIndex = 21;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(33, 9);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(37, 16);
            this.lblInfo.TabIndex = 25;
            this.lblInfo.Text = "label";
            // 
            // pbInfo
            // 
            this.pbInfo.Location = new System.Drawing.Point(4, 9);
            this.pbInfo.Margin = new System.Windows.Forms.Padding(4);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(21, 20);
            this.pbInfo.TabIndex = 24;
            this.pbInfo.TabStop = false;
            // 
            // flp1
            // 
            this.flp1.Controls.Add(this.lnkSpecific);
            this.flp1.Controls.Add(this.lnkCustomParameters);
            this.flp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp1.Location = new System.Drawing.Point(0, 447);
            this.flp1.Margin = new System.Windows.Forms.Padding(0);
            this.flp1.Name = "flp1";
            this.flp1.Size = new System.Drawing.Size(307, 30);
            this.flp1.TabIndex = 22;
            // 
            // lnkSpecific
            // 
            this.lnkSpecific.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkSpecific.AutoSize = true;
            this.lnkSpecific.Location = new System.Drawing.Point(4, 4);
            this.lnkSpecific.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.lnkSpecific.Name = "lnkSpecific";
            this.lnkSpecific.Size = new System.Drawing.Size(146, 16);
            this.lnkSpecific.TabIndex = 20;
            this.lnkSpecific.TabStop = true;
            this.lnkSpecific.Text = "Select Individual Files...";
            this.lnkSpecific.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSpecific_LinkClicked);
            // 
            // lnkCustomParameters
            // 
            this.lnkCustomParameters.AutoSize = true;
            this.lnkCustomParameters.Location = new System.Drawing.Point(158, 4);
            this.lnkCustomParameters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.lnkCustomParameters.Name = "lnkCustomParameters";
            this.lnkCustomParameters.Size = new System.Drawing.Size(111, 16);
            this.lnkCustomParameters.TabIndex = 21;
            this.lnkCustomParameters.TabStop = true;
            this.lnkCustomParameters.Text = "Custom Params...";
            this.lnkCustomParameters.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCustomParameters_LinkClicked);
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tblInner, 0, 1);
            this.tblMain.Controls.Add(this.pnlBottom, 0, 2);
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblMain.Size = new System.Drawing.Size(672, 579);
            this.tblMain.TabIndex = 21;
            // 
            // tblInner
            // 
            this.tblInner.ColumnCount = 2;
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 349F));
            this.tblInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.Controls.Add(this.pnlLeft, 0, 0);
            this.tblInner.Controls.Add(this.groupBox3, 1, 0);
            this.tblInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInner.Location = new System.Drawing.Point(0, 32);
            this.tblInner.Margin = new System.Windows.Forms.Padding(0);
            this.tblInner.Name = "tblInner";
            this.tblInner.RowCount = 1;
            this.tblInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInner.Size = new System.Drawing.Size(672, 508);
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
            this.pnlLeft.Size = new System.Drawing.Size(349, 508);
            this.pnlLeft.TabIndex = 0;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnSaveSettings);
            this.pnlBottom.Controls.Add(this.flpButtons);
            this.pnlBottom.Controls.Add(this.chkRemember);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.Location = new System.Drawing.Point(0, 540);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(672, 39);
            this.pnlBottom.TabIndex = 1;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(177, 6);
            this.btnSaveSettings.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(109, 28);
            this.btnSaveSettings.TabIndex = 6;
            this.btnSaveSettings.TabStop = false;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // flpButtons
            // 
            this.flpButtons.CaptureArrowKeys = false;
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnOK);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(405, 0);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(4);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.flpButtons.Size = new System.Drawing.Size(267, 39);
            this.flpButtons.TabIndex = 5;
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Checked = true;
            this.chkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemember.Location = new System.Drawing.Point(12, 11);
            this.chkRemember.Margin = new System.Windows.Forms.Padding(4);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(148, 20);
            this.chkRemember.TabIndex = 4;
            this.chkRemember.TabStop = false;
            this.chkRemember.Text = "Remember Settings";
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(672, 32);
            this.titleBar.TabIndex = 2;
            this.titleBar.Title = "Launch";
            // 
            // PlayForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(672, 579);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PlayForm";
            this.Text = "Launch";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tblProfile.ResumeLayout(false);
            this.tblProfile.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.profileToolStrip.ResumeLayout(false);
            this.profileToolStrip.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSkill;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ComboBox cmbDemo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtParameters;
        private System.Windows.Forms.LinkLabel lnkMore;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private FilesCtrl ctrlFiles;
        private System.Windows.Forms.TableLayoutPanel tblFiles;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.PictureBox pbInfo;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.LinkLabel lnkOpenDemo;
        private System.Windows.Forms.LinkLabel lnkPreviewLaunchParameters;
        private System.Windows.Forms.FlowLayoutPanel flp1;
        private System.Windows.Forms.LinkLabel lnkSpecific;
        private System.Windows.Forms.LinkLabel lnkCustomParameters;
        private System.Windows.Forms.LinkLabel lnkFilterSettings;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.ComboBox cmbProfiles;
        private System.Windows.Forms.TableLayoutPanel tblProfile;
        private System.Windows.Forms.ToolStrip profileToolStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem newProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteProfileToolStripMenuItem;
        private System.Windows.Forms.LinkLabel lnkLoadSaveMore;
        private Controls.TitleBarControl titleBar;
        private FormButton btnProfileMenu;
        private System.Windows.Forms.Panel panel2;
        private CCheckBox chkRemember;
        private CCheckBox chkRecord;
        private CCheckBox chkDemo;
        private CCheckBox chkMap;
        private CCheckBox chkSaveStats;
        private CCheckBox chkExtraParamsOnly;
        private CCheckBox chkScreenFilter;
        private CCheckBox chkLoadLatestSave;
        private TableLayoutPanelDB tblMain;
        private TableLayoutPanelDB tblInner;
        private FlowLayoutPanelDB flpButtons;
    }
}