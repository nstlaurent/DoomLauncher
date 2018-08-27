using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TextBoxForm : Form
    {
        private string m_url;

        public TextBoxForm()
            : this(true, MessageBoxButtons.OK)
        {

        }

        public TextBoxForm(bool multiline, MessageBoxButtons buttons)
        {
            InitializeComponent();

            if (buttons != MessageBoxButtons.OK && buttons != MessageBoxButtons.OKCancel)
                throw new NotSupportedException(buttons.ToString() + " not supported");

            btnCancel.Visible = buttons == MessageBoxButtons.OKCancel;

            HeaderText = string.Empty;
            txtText.Multiline = multiline;

            if (!multiline)
            {
                this.Height = 100;
                this.Width = 300;
            }

            tblMain.RowStyles[1].Height = 0;
        }

        public void SetLink(string text, string url)
        {
            lnk.Text = text;
            m_url = url;
            tblMain.RowStyles[1].Height = 32;
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
                tblMain.RowStyles[0].Height = lblHeader.Height + 3;
            }
        }

        private void lnk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(m_url);
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            Close();
        }
    }
}
