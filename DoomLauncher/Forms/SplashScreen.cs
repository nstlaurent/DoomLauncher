using System.Drawing;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            TransparencyKey = Color.Magenta;
            InitializeComponent();
        }
    }
}
