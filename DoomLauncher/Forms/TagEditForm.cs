using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class TagEditForm : Form
    {
        private TagEdit m_tagEdit;

        public TagEditForm()
        {
            InitializeComponent();

            m_tagEdit = new TagEdit();
            m_tagEdit.Dock = DockStyle.Fill;
            tblMain.Controls.Add(m_tagEdit, 0, 1);

            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
        }

        public TagEdit TagEditControl
        {
            get { return m_tagEdit; }
        }
    }
}
