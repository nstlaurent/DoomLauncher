using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class FlowLayoutPanelDB : FlowLayoutPanel
    {
        public FlowLayoutPanelDB()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }
}
