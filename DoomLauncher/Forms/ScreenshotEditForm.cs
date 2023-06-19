using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher.Forms
{
    public partial class ScreenshotEditForm : Form
    {
        public ScreenshotEditForm()
        {
            InitializeComponent();
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
            SetMaps(gameFile);

            if (fileData == null)
            {
                cmbMap.Enabled = chkMap.Checked = false;
                return;
            }

            TrySetMapData(fileData);

            txtTitle.Text = fileData.UserTitle;
            txtDescription.Text = fileData.UserDescription;
        }

        private void SetMaps(IGameFile gameFile)
        {
            if (!string.IsNullOrEmpty(gameFile.Map))
            {
                AutoCompleteCombo.SetAutoCompleteCustomSource(cmbMap, DataSources.GameFile.GetMaps(gameFile), null, null);
                return;
            }

            int iwadId = -1;
            if (gameFile.IWadID.HasValue)
                iwadId = gameFile.IWadID.Value;
            else
                iwadId = (int)DataCache.Instance.AppConfiguration.GetTypedConfigValue(ConfigType.DefaultIWad, typeof(int));

            var iwad = DataCache.Instance.DataSourceAdapter.GetGameFileIWads().FirstOrDefault(x => x.IWadID == iwadId);
            if (iwad == null)
                return;

            AutoCompleteCombo.SetAutoCompleteCustomSource(cmbMap, DataSources.GameFile.GetMaps(iwad), null, null);
        }

        private void TrySetMapData(IFileData fileData)
        {
            if (cmbMap.DataSource == null || cmbMap.Items.Count == 0)
            {
                cmbMap.Enabled = false;
                chkMap.Checked = false;
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(fileData.Map))
                    cmbMap.SelectedItem = fileData.Map;
                else
                    cmbMap.SelectedIndex = 0;

                cmbMap.Enabled = chkMap.Checked;
                chkMap.Checked = !string.IsNullOrEmpty(fileData.Map);
            }
            catch
            {
                cmbMap.Enabled = false;
                chkMap.Checked = false;
            }
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
