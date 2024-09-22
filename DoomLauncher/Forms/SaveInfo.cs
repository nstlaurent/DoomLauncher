using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class SaveInfo : Form
    {
        public SaveInfo()
        {
            InitializeComponent();

            pbInfo1.Image = DoomLauncher.Properties.Resources.bon2b;
            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
        }
    }
}
