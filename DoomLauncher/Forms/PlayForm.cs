using DoomLauncher.DataSources;
using DoomLauncher.Demo;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class PlayForm : Form
    {
        private ITabView[] m_additionalFileViews;
        private bool m_init, m_demoChangedAdditionalFiles;
        private ISourcePort m_lastSourcePort;
        private IGameFile m_lastIwad;

        public event EventHandler SaveSettings;
        public event EventHandler OnPreviewLaunchParameters;

        private FileLoadHandler m_handler;

        private readonly AppConfiguration m_appConfig;
        private readonly IDataSourceAdapter m_adapter;

        private enum AddFilesType
        {
            SourcePort,
            IWAD
        }

        public PlayForm(AppConfiguration appConfig, IDataSourceAdapter adapter)
        {
            InitializeComponent();
            ctrlFiles.Initialize("GameFileID", "FileName");
            ctrlFiles.CellFormatting += ctrlFiles_CellFormatting;
            ctrlFiles.NewItemNeeded += ctrlFiles_NewItemNeeded;
            ctrlFiles.ItemRemoving += CtrlFiles_ItemRemoving;

            lnkCustomParameters.Visible = false;

            m_appConfig = appConfig;
            m_adapter = adapter;
        }

        public void Initialize(IEnumerable<ITabView> additionalFileViews, IGameFile gameFile)
        {
            m_init = true;
            m_additionalFileViews = additionalFileViews.ToArray();

            GameFile = gameFile;

            m_handler = new FileLoadHandler(m_adapter, gameFile);

            cmbSourcePorts.DataSource = m_adapter.GetSourcePorts();
            cmbIwad.DataSource = Util.GetIWadsDataSource(m_adapter);

            if (gameFile != null)
            {
                Text = "Launch - " + (string.IsNullOrEmpty(gameFile.Title) ? gameFile.FileName : gameFile.Title);
                if (!string.IsNullOrEmpty(gameFile.Map))
                    cmbMap.DataSource = gameFile.Map.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
                chkSaveStats.Checked = gameFile.SettingsStat;
            }

            cmbSkill.DataSource = Util.GetSkills();
            cmbSkill.SelectedItem = "3";

            if (gameFile != null && IsIwad(gameFile))
            {
                pbInfo.Image = DoomLauncher.Properties.Resources.bon2b;
                lblInfo.Text = string.Format("These files will automatically be added{0} when this IWAD is selected for play.", Environment.NewLine);
            }
            else
            {
                tblFiles.RowStyles[0].Height = 0;
            }
        }

        public void InitializeComplete()
        {
            IGameFile gameIwad = SelectedIWad;
            if (gameIwad != null && gameIwad.Equals(GameFile))
                cmbIwad.Enabled = false;

            ctrlFiles.SetDataSource(m_handler.GetCurrentAdditionalFiles());           
            AddExtraAdditionalFiles();

            m_init = false;
        }

        private bool IsIwad(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
                return m_adapter.GetIWad(gameFile.GameFileID.Value) != null;

            return false;
        }

        public IGameFile GameFile { get; private set; }

        public ISourcePort SelectedSourcePort
        {
            get { return cmbSourcePorts.SelectedItem as ISourcePort; }
            set { cmbSourcePorts.SelectedItem = value; }
        }

        public IGameFile SelectedIWad
        {
            get
            {
                if (cmbIwad.SelectedItem != null)
                {
                    return m_adapter.GetGameFileIWads().FirstOrDefault(x => x.GameFileID == ((IIWadData)cmbIwad.SelectedItem).GameFileID);
                }

                return null;
            }
            set
            {
                if (value == null)
                    cmbIwad.SelectedIndex = 0;
                else
                    cmbIwad.SelectedItem = Util.GetIWadsDataSource(m_adapter).FirstOrDefault(x => x.IWadID == value.IWadID);
            }
        }

        public IFileData SelectedDemo
        {
            get { return cmbDemo.SelectedItem as IFileData; }
            set { cmbDemo.SelectedItem = value; }
        }

        public List<IGameFile> GetAdditionalFiles()
        {
            //return all the files in order, the user can determine the order of any file, whether it was added by source port or iwad selection
            return ctrlFiles.GetFiles().Cast<IGameFile>().ToList();
        }

        public string SelectedMap
        {
            get
            {
                if (!chkMap.Checked) return null;
                return cmbMap.SelectedItem as string;
            }
            set
            {
                if (value == null)
                {
                    chkMap.Checked = false;
                }
                else
                {
                    chkMap.Checked = true;
                    cmbMap.SelectedItem = value;
                }
            }
        }

        public string SelectedSkill
        {
            get { return cmbSkill.SelectedItem as string; }
            set { cmbSkill.SelectedItem = value; }
        }

        public bool RememberSettings
        {
            get { return chkRemember.Checked; }
        }

        public bool Record
        {
            get { return chkRecord.Checked; }
        }

        public bool PlayDemo
        {
            get { return chkDemo.Checked; }
        }

        public string RecordDescriptionText
        {
            get { return txtDescription.Text; }
        }

        public string ExtraParameters
        {
            get { return txtParameters.Text; }
            set { txtParameters.Text = value; }
        }

        public string[] SpecificFiles { get; set; }

        public bool SaveStatistics
        {
            get { return chkSaveStats.Enabled && chkSaveStats.Checked; }
        }

        public bool PreviewLaunchParameters
        {
            get { return chkPreview.Checked; }
        }

        public bool ShouldSaveAdditionalFiles()
        {
            return !m_demoChangedAdditionalFiles;
        }

        private void chkRecord_CheckedChanged(object sender, EventArgs e)
        {
            txtDescription.Enabled = chkRecord.Checked;
            cmbDemo.Enabled = false;
            chkDemo.CheckedChanged -= chkDemo_CheckedChanged;
            chkDemo.Checked = false;
            chkDemo.CheckedChanged += chkDemo_CheckedChanged;
        }

        private void chkDemo_CheckedChanged(object sender, EventArgs e)
        {
            txtDescription.Enabled = false;
            cmbDemo.Enabled = chkDemo.Checked;
            chkRecord.CheckedChanged -= chkRecord_CheckedChanged;
            chkRecord.Checked = false;
            chkRecord.CheckedChanged += chkRecord_CheckedChanged;
            HandleDemoChange();
        }

        private void cmbSourcePorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSourcePorts.SelectedItem != null && GameFile != null)
            {
                PopulateDemos();
                ISourcePort sourcePort = cmbSourcePorts.SelectedItem as ISourcePort;
                chkSaveStats.Enabled = SaveStatisticsSupported(sourcePort);
                AddExtraAdditionalFiles();
            }

            m_lastSourcePort = SelectedSourcePort;
        }

        private void PopulateDemos()
        {
            ISourcePort sourcePort = cmbSourcePorts.SelectedItem as ISourcePort;

            if (GameFile.GameFileID.HasValue)
            {
                IEnumerable<IFileData> demoFiles = m_adapter.GetFiles(GameFile, FileType.Demo)
                    .Where(x => x.SourcePortID == sourcePort.SourcePortID);

                cmbDemo.DataSource = demoFiles.ToList();
            }
        }

        private bool SaveStatisticsSupported(ISourcePort sourcePort)
        {
            if (BoomStatsReader.Supported(sourcePort))
                return true;
            if (ZDoomStatsReader.Supported(sourcePort))
                return true;
            if (CNDoomStatsReader.Supported(sourcePort))
                return true;

            return false;
        }

        private void chkMap_CheckedChanged(object sender, EventArgs e)
        {
            cmbMap.Enabled = cmbSkill.Enabled = chkMap.Checked;
        }

        private void ctrlFiles_NewItemNeeded(object sender, AdditionalFilesEventArgs e)
        {
            using (FileSelectForm fileSelect = new FileSelectForm())
            {
                fileSelect.Initialize(m_adapter, m_additionalFileViews);
                fileSelect.MultiSelect = true;
                fileSelect.StartPosition = FormStartPosition.CenterParent;

                if (fileSelect.ShowDialog(this) == DialogResult.OK)
                {
                    IGameFile[] selectedFiles = fileSelect.SelectedFiles.Except(new IGameFile[] { GameFile }).ToArray();

                    if (selectedFiles.Length > 0)
                    {
                        e.NewItems = selectedFiles.Cast<object>().ToList();

                        try
                        {
                            ResetSpecificFilesSelections(new IGameFile[] { selectedFiles.First() });
                        }
                        catch (FileNotFoundException ex)
                        {
                            MessageBox.Show(this, string.Format("The Game File {0} is missing from the library.", ex.FileName), "File Not Found");
                        }
                        catch (Exception ex)
                        {
                            Util.DisplayUnexpectedException(this, ex);
                        }
                    }
                }

                SetColumnConfigToMain(fileSelect.TabViews);
            }
        }

        private void SetColumnConfigToMain(ITabView[] tabViews)
        {
            foreach (ITabView tabFrom in tabViews)
            {
                var tabTo = m_additionalFileViews.FirstOrDefault(x => x.Title == tabFrom.Title);

                if (tabTo != null)
                    tabTo.SetColumnConfig(tabTo.GameFileViewControl.ColumnFields, tabFrom.GetColumnConfig().ToArray());
            }
        }

        private void CtrlFiles_ItemRemoving(object sender, AdditionalFilesEventArgs e)
        {
            if (e.Item.Equals(GameFile))
            {
                MessageBox.Show(this, string.Format("Cannot remove {0}. This is the file you will be launching!", GameFile.FileName), 
                    "Cannot Remove", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else
            {
                if (SpecificFiles != null)
                    SpecificFiles = SpecificFiles.Except(GetSupportedFiles((IGameFile)e.Item)).ToArray();
            }
        }

        private void ResetSpecificFilesSelections(IGameFile[] selectedFiles)
        {
            foreach (IGameFile gameFile in selectedFiles)
            {
                if (SpecificFiles == null)
                    SpecificFiles = GetSupportedFiles(GameFile);

                SpecificFiles = SpecificFiles.Union(GetSupportedFiles(gameFile)).ToArray();
            }
        }

        private string[] GetSupportedFiles(IGameFile gameFile)
        {
            return SpecificFilesForm.GetSupportedFiles(m_appConfig.GameFileDirectory.GetFullPath(), gameFile, SourcePort.GetSupportedExtensions(SelectedSourcePort));
        }

        private void cmbIwad_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddExtraAdditionalFiles();

            if (GameFile == null && SelectedIWad != null)
            {
                var gameFileIwad = m_adapter.GetGameFileIWads().FirstOrDefault(x => x.GameFileID == SelectedIWad.GameFileID);
                if (gameFileIwad != null)
                    cmbMap.DataSource = gameFileIwad.Map.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            m_lastIwad = SelectedIWad;
        }

        private void AddExtraAdditionalFiles()
        {
            m_handler.CalculateAdditionalFiles(SelectedIWad, cmbSourcePorts.SelectedItem as ISourcePort);

            if (InitAddFilesCheck())
            {              
                ResetSpecificFilesSelections(m_handler.GetCurrentAdditionalNewFiles().ToArray());
                ctrlFiles.SetDataSource(m_handler.GetCurrentAdditionalFiles());
            }

            ctrlFiles.Refresh(); //the port or iwad in () may have changed so invalidate to force update
        }

        private bool InitAddFilesCheck()
        {
            //For files that have been launcned before: Selected index changed fires a lot on init, so ignore those
            //If the file has never been launched, then we need to set the additional files on init
            //Having files in SettingsSpecificFiles indicates if the file has been launched before
            if (m_init)
                return GameFile != null && GameFile.SettingsSpecificFiles.Length == 0;

            return true;
        }

        private void lnkSpecific_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SpecificFilesForm form = new SpecificFilesForm();
            form.StartPosition = FormStartPosition.CenterParent;

            List<IGameFile> gameFiles = new List<IGameFile>();
            gameFiles.AddRange(GetAdditionalFiles());

            form.Initialize(m_appConfig.GameFileDirectory, gameFiles, SourcePort.GetSupportedExtensions(SelectedSourcePort), SpecificFiles);

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                SpecificFiles = form.GetSpecificFiles();
            }
        }

        private void lnkMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StatsInfo form = new StatsInfo();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string err = null;

            if (chkRecord.Checked && string.IsNullOrEmpty(txtDescription.Text))
            {
                err = "Please enter a description for the demo to record.";
            }

            if (err == null)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(this, err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        private void ctrlFiles_CellFormatting(object sender, AdditionalFilesEventArgs e)
        {
            IGameFile gameFile = e.Item as IGameFile;
            IGameFile iwad = SelectedIWad;
            ISourcePort port = cmbSourcePorts.SelectedValue as ISourcePort;
            if (m_handler.IsIWadFile(gameFile))
                e.DisplayText = string.Format("{0} ({1})", gameFile.FileName, Util.RemoveExtension(iwad.FileName));
            if (m_handler.IsSourcePortFile(gameFile))
                e.DisplayText = string.Format("{0} ({1})", gameFile.FileName, port.Name);
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings?.Invoke(this, new EventArgs());
        }

        private void lnkOpenDemo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<IFileData> demoFiles = BasicFileView.CreateFileAssociation(this, m_adapter, m_appConfig.DemoDirectory, FileType.Demo, GameFile,
                cmbSourcePorts.SelectedItem as ISourcePort);

            if (demoFiles.Count > 0)
            {
                PopulateDemos();
                cmbDemo.SelectedValue = demoFiles.First().FileID;
                chkDemo.Checked = true; //will trigger HandleDemoChange
            }
        }

        private void cmbDemo_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleDemoChange();
        }

        private void HandleDemoChange()
        {
            ResetAdditionalFiles();

            if (chkDemo.Checked && cmbDemo.SelectedItem != null)
            {
                var file = cmbDemo.SelectedItem as IFileData;
                var parser = DemoUtil.GetDemoParser(Path.Combine(m_appConfig.DemoDirectory.GetFullPath(), file.FileName));

                if (parser != null)
                {
                    string[] requiredFiles = parser.GetRequiredFiles();
                    List<string> unavailable = new List<string>();
                    List<IGameFile> iwads = new List<IGameFile>();
                    List<IGameFile> gameFiles = GetGameFiles(requiredFiles, unavailable, iwads);
                    ctrlFiles.SetDataSource(gameFiles);
                    if (iwads.Count > 0)
                        SelectedIWad = iwads.First();

                    if (unavailable.Count > 0)
                    {
                        TextBoxForm form = new TextBoxForm(true, MessageBoxButtons.OK);
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.Text = "Not Found";
                        form.HeaderText = "The following required files were not found:";
                        form.DisplayText = string.Join(Environment.NewLine, unavailable.ToArray());
                        form.ShowDialog(this);
                    }

                    m_demoChangedAdditionalFiles = true;
                }
            }
            else
            {
                m_demoChangedAdditionalFiles = false;
            }
        }

        private void ResetAdditionalFiles()
        {
            m_handler.Reset();
            InitializeComplete();
        }

        private void lnkPreviewLaunchParameters_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnPreviewLaunchParameters?.Invoke(this, new EventArgs());
        }

        private void lnkCustomParameters_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private List<IGameFile> GetGameFiles(string[] fileNames, List<string> unavailable, List<IGameFile> iwads)
        {        
            List<IGameFile> gameFiles = new List<IGameFile>();
            var knowniwads = m_adapter.GetGameFileIWads();

            foreach (string file in fileNames)
            {
                var gameFile = m_adapter.GetGameFile(file.Replace(Path.GetExtension(file), ".zip"));

                if (gameFile == null)
                {
                    unavailable.Add(file);
                }
                else
                {
                    if (knowniwads.Any(x => x.FileName.Equals(gameFile.FileName, StringComparison.InvariantCultureIgnoreCase)))
                        iwads.Add(gameFile);
                    else
                        gameFiles.Add(gameFile);
                }

            }

            return gameFiles;
        }
    }
}
