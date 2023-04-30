using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class SaveInfo : Form
    {
        public SaveInfo()
        {
            InitializeComponent();

            pbInfo1.Image = DoomLauncher.Properties.Resources.bon2b;
            lblSave.Text += "\n\nHelion\nChocolate Doom \nCNDoom\nCrispy Doom\nEternity\nPrBoom-Plus\nWoof!\nZDoom Variants";
            Stylizer.Stylize(this, DesignMode, StylizerOptions.RemoveTitleBar);
        }
    }
}
