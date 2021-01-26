using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class SaveInfo : Form
    {
        public SaveInfo()
        {
            InitializeComponent();

            pbInfo1.Image = DoomLauncher.Properties.Resources.bon2b;
            lblSave.Text += "\n\nChocolate Doom \nCNDoom\nCrispy Doom\nPrBoom-Plus\nWoof!\nZDoom Variants";
        }
    }
}
