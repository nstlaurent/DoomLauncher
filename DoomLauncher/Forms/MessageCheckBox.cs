using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class MessageCheckBox : Form
    {
        private readonly Icon m_icon;
        private readonly int m_height;

        public MessageCheckBox(string title, string text, string checkBoxText, Icon icon)
            : this(title, text, checkBoxText, icon, MessageBoxButtons.OK)
        {
            
        }

        public MessageCheckBox(string title, string text, string checkBoxText, Icon icon, MessageBoxButtons buttons, 
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            if (buttons == MessageBoxButtons.OK)
                btnCancel.Visible = false;

            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                btnOK.Text = "Yes";
                btnCancel.Text = "No";
                btnOK.DialogResult = DialogResult.Yes;
                btnCancel.DialogResult = DialogResult.No;
            }

            if (buttons == MessageBoxButtons.RetryCancel)
            {
                btnOK.Text = "Retry";
                btnCancel.Text = "Cancel";
                btnOK.DialogResult = DialogResult.Retry;
                btnCancel.DialogResult = DialogResult.Cancel;
            }

            if (defaultButton == MessageBoxDefaultButton.Button1)
            {
                TabIndex = 0;
                AcceptButton = btnOK;
            }
            else
            {
                TabIndex = 1;
                AcceptButton = btnCancel;
            }

            Text = title;
            lblText.Text = text;
            checkBox1.Text = checkBoxText;
            m_icon = icon;

            DpiScale dpiScale = new DpiScale(CreateGraphics());
            int textHeight = lblText.Height + dpiScale.ScaleIntY(16);
            int textRowHeight = (int)tblMain.RowStyles[0].Height;
            if (textHeight > textRowHeight)
            {
                int addHeight = textHeight - textRowHeight;
                Height += addHeight;
                tblMain.RowStyles[0].Height += addHeight;
            }

            tblMessage.Paint += tblMessage_Paint;
            lblText.Anchor = AnchorStyles.Left;

            m_height = Height;
            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
        }

        public void SetShowCheckBox(bool set)
        {            
            checkBox1.Visible = set;

            if (set)
            {
                Height = m_height;
                return;
            }

            DpiScale dpiScale = new DpiScale(CreateGraphics());
            Height -= dpiScale.ScaleIntY(28);
        }

        void tblMessage_Paint(object sender, PaintEventArgs e)
        {
            int height = (int)tblMain.RowStyles[0].Height;
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            Rectangle rect = new Rectangle(dpiScale.ScaleIntX(14), (height - dpiScale.ScaleIntY(32)) / 2, 
                dpiScale.ScaleIntX(32), dpiScale.ScaleIntY(32));
            e.Graphics.DrawIcon(m_icon, rect);
        }

        public bool Checked => checkBox1.Checked;
    }
}
