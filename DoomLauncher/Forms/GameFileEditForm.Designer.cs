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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gameFileEdit1 = new DoomLauncher.GameFileEdit();
            this.tblButtons = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveSelect = new System.Windows.Forms.Button();
            this.btnCopyFrom = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tblButtons.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gameFileEdit1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tblButtons, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(430, 544);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // gameFileEdit1
            // 
            this.gameFileEdit1.AuthorChecked = true;
            this.gameFileEdit1.CommentsChecked = true;
            this.gameFileEdit1.DescriptionChecked = true;
            this.gameFileEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameFileEdit1.Location = new System.Drawing.Point(3, 3);
            this.gameFileEdit1.Name = "gameFileEdit1";
            this.gameFileEdit1.RatingChecked = true;
            this.gameFileEdit1.ReleaseDateChecked = true;
            this.gameFileEdit1.Size = new System.Drawing.Size(424, 474);
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
            this.tblButtons.Location = new System.Drawing.Point(0, 512);
            this.tblButtons.Margin = new System.Windows.Forms.Padding(0);
            this.tblButtons.Name = "tblButtons";
            this.tblButtons.RowCount = 1;
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblButtons.Size = new System.Drawing.Size(430, 32);
            this.tblButtons.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveSelect);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(215, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(215, 32);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(137, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSaveSelect
            // 
            this.btnSaveSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSaveSelect.Location = new System.Drawing.Point(56, 3);
            this.btnSaveSelect.Name = "btnSaveSelect";
            this.btnSaveSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSelect.TabIndex = 3;
            this.btnSaveSelect.Text = "Save";
            this.btnSaveSelect.UseVisualStyleBackColor = true;
            // 
            // btnCopyFrom
            // 
            this.btnCopyFrom.Location = new System.Drawing.Point(3, 3);
            this.btnCopyFrom.Name = "btnCopyFrom";
            this.btnCopyFrom.Size = new System.Drawing.Size(111, 23);
            this.btnCopyFrom.TabIndex = 3;
            this.btnCopyFrom.Text = "Copy From File...";
            this.btnCopyFrom.UseVisualStyleBackColor = true;
            this.btnCopyFrom.Click += new System.EventHandler(this.btnCopyFrom_Click);
            // 
            // GameFileEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(430, 544);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameFileEditForm";
            this.Text = "Edit";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tblButtons.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GameFileEdit gameFileEdit1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tblButtons;
        private System.Windows.Forms.Button btnCopyFrom;
        private System.Windows.Forms.Button btnSaveSelect;
    }
}