namespace DoomLauncher
{
    partial class GameFileEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameFileEditForm));
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.gameFileEdit1 = new DoomLauncher.GameFileEdit();
            this.tblButtons = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveSelect = new System.Windows.Forms.Button();
            this.btnCopyFrom = new System.Windows.Forms.Button();
            this.titleBar = new DoomLauncher.Controls.TitleBarControl();
            this.tblMain.SuspendLayout();
            this.tblButtons.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.gameFileEdit1, 0, 1);
            this.tblMain.Controls.Add(this.tblButtons, 0, 3);
            this.tblMain.Controls.Add(this.titleBar, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblMain.Size = new System.Drawing.Size(645, 960);
            this.tblMain.TabIndex = 1;
            // 
            // gameFileEdit1
            // 
            this.gameFileEdit1.AuthorChecked = true;
            this.gameFileEdit1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.gameFileEdit1.CommentsChecked = true;
            this.gameFileEdit1.DescriptionChecked = true;
            this.gameFileEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameFileEdit1.ForeColor = System.Drawing.Color.White;
            this.gameFileEdit1.Location = new System.Drawing.Point(6, 48);
            this.gameFileEdit1.MapsChecked = true;
            this.gameFileEdit1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.gameFileEdit1.Name = "gameFileEdit1";
            this.gameFileEdit1.RatingChecked = true;
            this.gameFileEdit1.ReleaseDateChecked = true;
            this.gameFileEdit1.Size = new System.Drawing.Size(633, 806);
            this.gameFileEdit1.TabIndex = 0;
            this.gameFileEdit1.TagsChecked = true;
            this.gameFileEdit1.TitleChecked = true;
            // 
            // tblButtons
            // 
            this.tblButtons.ColumnCount = 2;
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblButtons.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tblButtons.Controls.Add(this.btnCopyFrom, 0, 0);
            this.tblButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblButtons.Location = new System.Drawing.Point(0, 911);
            this.tblButtons.Margin = new System.Windows.Forms.Padding(0);
            this.tblButtons.Name = "tblButtons";
            this.tblButtons.RowCount = 1;
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tblButtons.Size = new System.Drawing.Size(645, 49);
            this.tblButtons.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveSelect);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(322, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(322, 49);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(206, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSaveSelect
            // 
            this.btnSaveSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSaveSelect.Location = new System.Drawing.Point(86, 5);
            this.btnSaveSelect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSaveSelect.Name = "btnSaveSelect";
            this.btnSaveSelect.Size = new System.Drawing.Size(112, 35);
            this.btnSaveSelect.TabIndex = 3;
            this.btnSaveSelect.Text = "Save";
            this.btnSaveSelect.UseVisualStyleBackColor = true;
            // 
            // btnCopyFrom
            // 
            this.btnCopyFrom.Location = new System.Drawing.Point(4, 5);
            this.btnCopyFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCopyFrom.Name = "btnCopyFrom";
            this.btnCopyFrom.Size = new System.Drawing.Size(166, 35);
            this.btnCopyFrom.TabIndex = 3;
            this.btnCopyFrom.Text = "Copy From File...";
            this.btnCopyFrom.UseVisualStyleBackColor = true;
            this.btnCopyFrom.Click += new System.EventHandler(this.btnCopyFrom_Click);
            // 
            // titleBar
            // 
            this.titleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(3, 3);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(639, 34);
            this.titleBar.TabIndex = 3;
            this.titleBar.Title = "Edit";
            // 
            // GameFileEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(645, 960);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "GameFileEditForm";
            this.Text = "Edit";
            this.tblMain.ResumeLayout(false);
            this.tblButtons.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GameFileEdit gameFileEdit1;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tblButtons;
        private System.Windows.Forms.Button btnCopyFrom;
        private System.Windows.Forms.Button btnSaveSelect;
        private Controls.TitleBarControl titleBar;
    }
}