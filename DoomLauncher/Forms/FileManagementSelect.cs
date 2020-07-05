using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class FileManagementSelect : Form
    {
        public FileManagementSelect()
        {
            InitializeComponent();
            cmbFileManagement.DataSource = new string[] { "Managed", "Unmanaged" };
        }

        public FileManagement GetSelectedFileManagement()
        {
            if (cmbFileManagement.SelectedIndex == 0)
                return FileManagement.Managed;

            return FileManagement.Unmanaged;
        }

        private void BtnOK_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
