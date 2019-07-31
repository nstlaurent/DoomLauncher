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
            Controls.Add(m_tabCtrl);
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
