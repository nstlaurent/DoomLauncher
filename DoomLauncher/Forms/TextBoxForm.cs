using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TextBoxForm : Form
    {
        private static readonly int s_headerRow = 0;
        private static readonly int s_checkBoxRow = 1;
        private static readonly int s_linkRow = 2;
        private string m_url;

        const int TitleBarHeight = 40;

        public TextBoxForm()
            : this(true, MessageBoxButtons.OK)
        {

        }

        public TextBoxForm(bool multiline, MessageBoxButtons buttons)
        {
            InitializeComponent();
            DpiScale dpiScale = new DpiScale(CreateGraphics());

            if (buttons != MessageBoxButtons.OK && buttons != MessageBoxButtons.OKCancel)
                throw new NotSupportedException(buttons.ToString() + " not supported");

            btnCancel.Visible = buttons == MessageBoxButtons.OKCancel;

            HeaderText = string.Empty;
            txtText.Multiline = multiline;

            if (!multiline)
            {
                Height = dpiScale.ScaleIntY(100 + TitleBarHeight);
                Width = dpiScale.ScaleIntX(300);
            }

            tblMain.RowStyles[s_checkBoxRow].Height = 0;
            tblMain.RowStyles[s_linkRow].Height = 0;
            lnk.Visible = false;
            chk.Visible = false;

            AcceptButton = btnOK;
            CancelButton = btnCancel;

            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
        }

        public string Title
        {
            get { return titleBar.Title; }
            set
            {
                titleBar.Title = value;
                Text = value;
            }
        }

        public void SetCheckBox(string text)
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            chk.Visible = true;
            chk.Text = text;
            tblMain.RowStyles[s_checkBoxRow].Height = dpiScale.ScaleIntY(32);
            Height += dpiScale.ScaleIntY(32);
        }

        public void SetLink(string text, string url)
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            lnk.Visible = true;
            lnk.Text = text;
            m_url = url;
            tblMain.RowStyles[s_linkRow].Height = dpiScale.ScaleIntY(32);
            Height += dpiScale.ScaleIntY(32);
        }

        public void SetMaxLength(int length)
        {
            txtText.MaxLength = length;
        }

        public void SelectDisplayText(int start, int length)
        {
            txtText.Select(start, length);
        }

        public string DisplayText
        {
            get { return txtText.Text; }
            set { txtText.Text = value; }
        }

        public bool CheckBoxChecked
        {
            get { return chk.Checked; }
            set { chk.Checked = value; }
        }

        public void AppendText(string text)
        {
            txtText.Text += text;
        }

        public string HeaderText
        {
            get { return lblHeader.Text; }
            set
            {
                lblHeader.Text = value;
                tblMain.RowStyles[s_headerRow].Height = lblHeader.Height + 3;
            }
        }

        private void lnk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(m_url);
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
