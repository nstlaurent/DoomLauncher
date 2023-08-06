using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
            lblInfo.BackColor = Color.Transparent;
            lblWelcome.BackColor = Color.Transparent;
            lblInfo.ForeColor = Color.White;
            lblWelcome.ForeColor = Color.White;
            lnkHelp.LinkColor = Color.White;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("Help.pdf");
            }
            catch
            {
                MessageBox.Show(this, "Could not open the help file. Did you copy all the files to your Doom Launcher folder correctly?", "Help Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
