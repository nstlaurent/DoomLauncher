using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class MessageCheckBox : Form
    {
        private Icon m_icon;
        private int m_height;

        public MessageCheckBox(string title, string text, string checkBoxText, Icon icon)
            : this(title, text, checkBoxText, icon, MessageBoxButtons.OK)
        {
            
        }

        public MessageCheckBox(string title, string text, string checkBoxText, Icon icon, MessageBoxButtons buttons)
        {
            InitializeComponent();

            if (buttons != MessageBoxButtons.OK && buttons != MessageBoxButtons.OKCancel)
                throw new NotSupportedException("Only MessageBoxButtons OK or OKCancel are supported");

            StartPosition = FormStartPosition.CenterParent;

            if (buttons == MessageBoxButtons.OK)
                btnCancel.Visible = false;

            Text = title;
            lblText.Text = text;
            checkBox1.Text = checkBoxText;
            m_icon = icon;

            tblMessage.Paint += tblMessage_Paint;
            lblText.Anchor = AnchorStyles.Left;

            m_height = this.Height;
            Stylizer.Stylize(this, DesignMode);
        }

        public void SetShowCheckBox(bool set)
        {
            checkBox1.Visible = set;

            if (set)
                this.Height = m_height;
            else
                this.Height -= 24;
        }

        void tblMessage_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawIcon(m_icon, 12, 12);
        }

        public bool Checked
        {
            get { return checkBox1.Checked; }
        }
    }
}
