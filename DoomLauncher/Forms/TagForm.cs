using DoomLauncher.Interfaces;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TagForm : Form
    {
        private TagControl m_tabCtrl;

        public TagForm()
        {
            InitializeComponent();

            m_tabCtrl = new TagControl();
            m_tabCtrl.Dock = DockStyle.Fill;
            tblMain.Controls.Add(m_tabCtrl, 0, 1);

            Stylizer.Stylize(this, DesignMode);
            MaximizedBounds = Screen.GetWorkingArea(this);
            FormBorderStyle = FormBorderStyle.None;
        }

        public void Init(IDataSourceAdapter adapter)
        {
            m_tabCtrl.Init(adapter);
        }

        public TagControl TagControl
        {
            get { return m_tabCtrl; }
        }
    }
}
