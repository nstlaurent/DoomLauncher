namespace DoomLauncher.Forms
{
    partial class MessageCheckBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageCheckBox));
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tblMessage = new System.Windows.Forms.TableLayoutPanel();
            this.lblText = new System.Windows.Forms.Label();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tblMain.SuspendLayout();
            this.tblMessage.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.checkBox1, 0, 2);
            this.tblMain.Controls.Add(this.tblMessage, 0, 0);
            this.tblMain.Controls.Add(this.flpButtons, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblMain.Size = new System.Drawing.Size(400, 126);
            this.tblMain.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 103);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(151, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Don\'t show this error again";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tblMessage
            // 
            this.tblMessage.ColumnCount = 2;
            this.tblMessage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tblMessage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMessage.Controls.Add(this.lblText, 1, 0);
            this.tblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMessage.Location = new System.Drawing.Point(3, 3);
            this.tblMessage.Name = "tblMessage";
            this.tblMessage.RowCount = 1;
            this.tblMessage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMessage.Size = new System.Drawing.Size(394, 58);
            this.tblMessage.TabIndex = 2;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(67, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(0, 13);
            this.lblText.TabIndex = 0;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnOK);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 64);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(400, 32);
            this.flpButtons.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(322, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(241, 6);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // MessageCheckBox
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(400, 126);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageCheckBox";
            this.Text = "MessageCheckBox";
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.tblMessage.ResumeLayout(false);
            this.tblMessage.PerformLayout();
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TableLayoutPanel tblMessage;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnCancel;
    }
}