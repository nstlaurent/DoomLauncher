using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MetaDataForm : Form
    {
        public MetaDataForm()
        {
            InitializeComponent();
            gameFileEdit1.SetShowCheckBoxes(true);
            gameFileEdit1.SetShowMaps(false);

            Stylizer.Stylize(this, DesignMode, StylizerOptions.RemoveTitleBar);
        }

        public GameFileEdit GameFileEdit
        {
            get { return gameFileEdit1; }
        }
    }
}
