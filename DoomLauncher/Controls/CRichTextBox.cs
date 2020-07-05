using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DoomLauncher.Controls
{
    public partial class CRichTextBox : RichTextBox
    {
        public CRichTextBox()
        {
            InitializeComponent();
            WarnLinkClick = true;
            LinkClicked += CRichTextBox_LinkClicked;
        }

        public bool WarnLinkClick { get; set; }

        private void CRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (ShouldNavigate(e))
                Process.Start(e.LinkText);
        }

        private bool ShouldNavigate(LinkClickedEventArgs e)
        {
            if (WarnLinkClick)
            {
                return MessageBox.Show(this, string.Format("You are about to navigate to:{1}{0} {1}{1}Continue?", e.LinkText, Environment.NewLine),
                    "Navigate", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
            }

            return true;
        }
    }
}
