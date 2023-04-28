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
            Stylizer.Stylize(this, DesignMode);
        }

        public static bool ShowDialogAndUpdate(IWin32Window owner, IDataSourceAdapter adapter, IGameFile gameFile, IFileData fileData)
        {
            ScreenshotEditForm screenshotEditForm = new ScreenshotEditForm();
            screenshotEditForm.StartPosition = FormStartPosition.CenterParent;
            screenshotEditForm.SetData(gameFile, fileData);

            if (screenshotEditForm.ShowDialog(owner) == DialogResult.OK)
            {
                fileData.UserTitle = screenshotEditForm.Title;
                fileData.UserDescription = screenshotEditForm.Description;
                fileData.Map = screenshotEditForm.Map;
                if (!screenshotEditForm.HasMap)
                    fileData.Map = null;

                adapter.UpdateFile(fileData);
                return true;
            }

            return false;
        }

        public void SetData(IGameFile gameFile, IFileData fileData)
        {
            AutoCompleteCombo.SetAutoCompleteCustomSource(cmbMap, DataSources.GameFile.GetMaps(gameFile), null, null);

            if (fileData == null)
            {
                cmbMap.Enabled = chkMap.Checked = false;
                return;
            }

            if (!string.IsNullOrEmpty(fileData.Map))
                cmbMap.SelectedItem = fileData.Map;
            else
                cmbMap.SelectedIndex = 0;

            cmbMap.Enabled = chkMap.Checked;
            chkMap.Checked = !string.IsNullOrEmpty(fileData.Map);
            txtTitle.Text = fileData.UserTitle;
            txtDescription.Text = fileData.UserDescription;
        }

        public bool HasMap => chkMap.Checked;

        public string Map => chkMap.Checked ? cmbMap.SelectedItem as string : null;

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
