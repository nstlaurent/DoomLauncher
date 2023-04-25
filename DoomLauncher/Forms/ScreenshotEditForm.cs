using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class ScreenshotEditForm : Form
    {
        public ScreenshotEditForm()
        {
            InitializeComponent();
        }

        public void SetData(IGameFile gameFile, IFileData fileData)
        {
            AutoCompleteCombo.SetAutoCompleteCustomSource(cmbMap, DataSources.GameFile.GetMaps(gameFile), null, null);

            if (!string.IsNullOrEmpty(fileData.Map))
                cmbMap.SelectedItem = fileData.Map;
            else
                cmbMap.SelectedIndex = -1;

            cmbMap.Enabled = cmbMap.SelectedItem != null;
            chkMap.Checked = !string.IsNullOrEmpty(fileData.Map);
            txtTitle.Text = fileData.UserTitle;
            txtDescription.Text = fileData.UserDescription;
        }

        public bool HasMap => chkMap.Checked;

        public string Map => cmbMap.SelectedItem as string;

        public string Title
        {
            get => txtTitle.Text;
        }

        public string Description
        {
            get => txtDescription.Text;
        }

        private void chkMap_CheckedChanged(object sender, System.EventArgs e)
        {
            cmbMap.Enabled = chkMap.Checked;
        }
    }
}
