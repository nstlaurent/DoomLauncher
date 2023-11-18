using DoomLauncher.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher.Stylize
{
    public static class StyledMessageBox
    {
        public static DialogResult Show(IWin32Window window, string text, string caption, MessageBoxButtons buttons, 
            MessageBoxIcon icon = MessageBoxIcon.Error,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            var systemIcon = SystemIcons.Information;
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    systemIcon = SystemIcons.Error;
                    break;
                case MessageBoxIcon.Warning:
                    systemIcon = SystemIcons.Warning;
                    break;
                case MessageBoxIcon.Question:
                    systemIcon = SystemIcons.Question;
                    break;
                case MessageBoxIcon.Asterisk:
                    systemIcon = SystemIcons.Asterisk;
                    break;
            }

            var form = new MessageCheckBox(caption, text, string.Empty, systemIcon, buttons, defaultButton);
            form.SetShowCheckBox(false);
            form.StartPosition = FormStartPosition.CenterParent;
            return form.ShowDialog(window);
        }
    }
}
