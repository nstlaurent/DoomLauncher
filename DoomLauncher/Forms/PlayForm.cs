using DoomLauncher.DataSources;
using DoomLauncher.Demo;
using DoomLauncher.Forms;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DoomLauncher
{
    public partial class PlayForm : Form
    {
        private ITabView[] m_additionalFileViews;
        private bool m_demoChangedAdditionalFiles;
        private ISourcePortData m_lastSourcePort;
        private IGameFile m_lastIwad;

        public event EventHandler SaveSettings;
        public event EventHandler OnPreviewLaunchParameters;

        private FileLoadHandler m_handler;

        private readonly AppConfiguration m_appConfig;
        private readonly IDataSourceAdapter m_adapter;
        private ScreenFilter m_filterSettings;
        private bool m_playSessionInProgress;

        private readonly Control[] m_tabControls;

        public PlayForm(AppConfiguration appConfig, IDataSourceAdapter adapter)
        {
            InitializeComponent();
            ctrlFiles.Initialize("GameFileID", "FileNameNoPath");
            ctrlFiles.CellFormatting += ctrlFiles_CellFormatting;
            ctrlFiles.NewItemNeeded += ctrlFiles_NewItemNeeded;
            ctrlFiles.ItemAdded += CtrlFiles_ItemAdded;
            ctrlFiles.ItemRemoving += CtrlFiles_ItemRemoving;
            ctrlFiles.ItemRemoved += CtrlFiles_ItemRemoved;
            Load += PlayForm_Load;

            lnkCustomParameters.Visible = false;

            m_appConfig = appConfig;
            m_adapter = adapter;

            m_filterSettings = GetFilterSettings();
            chkScreenFilter.Checked = m_filterSettings.Enabled;

            newProfileToolStripMenuItem.Image = Icons.File;
            deleteProfileToolStripMenuItem.Image = Icons.Delete;
            editProfileToolStripMenuItem.Image = Icons.Edit;
            toolStripDropDownButton1.Image = Icons.Bars;

            m_tabControls = new Control[]
            {
                cmbProfiles,
                cmbSourcePorts,
                cmbIwad,
                chkMap,
                cmbMap,
                cmbSkill,
                chkDemo,
                cmbDemo,
                chkRecord,
                txtDescription,
                txtParameters,
                chkSaveStats,
                chkLoadLatestSave,
                chkScreenFilter,
                chkExtraParamsOnly,
                chkRemember,
                btnSaveSettings,
                ctrlFiles,
                lnkSpecific,
                lnkCustomParameters,
                btnOK,
                btnCancel
            };

            InitTabIndicies();
        }

        private void InitTabIndicies()
        {
            foreach (var control in this.GetChildElements<Control>())
                control.TabStop = false;

            // Another user had an issue with tab indicies starting 0
            // Starting at 100 mostly fixes it... ugh
            // This still isn't 100% but it's way better than it was
            // The built in functionality for this is very screwed up, about ready to entirely write a custom one
            int index = 100;
            foreach (var control in m_tabControls)
            {
                control.TabStop = true;
                control.TabIndex = index++;
            }
        }

        private void PlayForm_Load(object sender, EventArgs e)
        {
            if (m_tabControls.Length > 0)
            {
                ActiveControl = m_tabControls[0];
                m_tabControls[0].Focus();
            }
        }

        public void Initialize(IEnumerable<ITabView> additionalFileViews, IGameFile gameFile, bool playSessionInProgress)
        {
            m_playSessionInProgress = playSessionInProgress;
            m_additionalFileViews = additionalFileViews.ToArray();

            GameFile = gameFile;

            AutoCompleteCombo.SetAutoCompleteCustomSource(cmbSourcePorts, m_adapter.GetSourcePorts(), typeof(ISourcePortData), "Name");
            AutoCompleteCombo.SetAutoCompleteCustomSource(cmbIwad, Util.GetIWadsDataSource(m_adapter), typeof(IIWadData), "FileName");

            if (gameFile != null)
            {
                Text = "Launch - " + (string.IsNullOrEmpty(gameFile.Title) ? gameFile.FileName : gameFile.Title);
                if (!string.IsNullOrEmpty(gameFile.Map))
                    AutoCompleteCombo.SetAutoCompleteCustomSource(cmbMap, MapSplit(gameFile), null, null);
            }

            AutoCompleteCombo.SetAutoCompleteCustomSource(cmbSkill, Util.GetSkills().ToList(), null, null);
            cmbSkill.SelectedItem = "3";

            LoadProfiles();
        }

        public void SetGameProfile(IGameProfile gameProfile)
        {
            SetIwadInfoLabel();

            UnregisterEvents();
            m_handler = new FileLoadHandler(m_adapter, GameFile, gameProfile);

            SetDefaultSelections();
            GameProfile.ApplyDefaultsToProfile(gameProfile, m_appConfig);
            cmbProfiles.SelectedValue = gameProfile.GameProfileID;

            if (GameFile != null)
            {
                chkSaveStats.Checked = gameProfile.SettingsStat;
                chkLoadLatestSave.Checked = gameProfile.SettingsLoadLatestSave;
                chkExtraParamsOnly.Checked = gameProfile.SettingsExtraParamsOnly;

                if (gameProfile.SourcePortID.HasValue)
                    SelectedSourcePort = m_adapter.GetSourcePort(gameProfile.SourcePortID.Value);

                // Selected GameFile is an IWAD so lock the IWAD selection
                if (IsIwad(GameFile))
                {
                    cmbIwad.Enabled = false;
                    SelectedIWad = GameFile;
                }
                else if (gameProfile.IWadID.HasValue)
                {
                    SelectedIWad = m_adapter.GetGameFileIWads().FirstOrDefault(x => x.IWadID == gameProfile.IWadID);
                }

                if (!string.IsNullOrEmpty(gameProfile.SettingsMap)) SelectedMap = gameProfile.SettingsMap;
                if (!string.IsNullOrEmpty(gameProfile.SettingsSkill)) SelectedSkill = gameProfile.SettingsSkill;
                if (!string.IsNullOrEmpty(gameProfile.SettingsExtraParams)) ExtraParameters = gameProfile.SettingsExtraParams;
                if (!string.IsNullOrEmpty(gameProfile.SettingsSpecificFiles)) SpecificFiles = Util.SplitString(gameProfile.SettingsSpecificFiles);
            }

            bool reset = ShouldRecalculateAdditionalFiles();
            HandleSourcePortSelectionChange(reset);
            HandleIwadSelectionChanged(reset);
            SetAdditionalFiles(reset);
            HandleDemoChange();
            RegisterEvents();               
        }

        private void SetIwadInfoLabel()
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            float infoHeight = dpiScale.ScaleFloatY(40);
            lblInfo.BackColor = SystemColors.Control;
            lblInfo.ForeColor = SystemColors.ControlText;

            if (m_playSessionInProgress)
            {
                tblFiles.RowStyles[0].Height = infoHeight;
                pbInfo.Image = Properties.Resources.bon2b;
                lblInfo.BackColor = SystemColors.Highlight;
                lblInfo.ForeColor = SystemColors.HighlightText;
                lblInfo.Text = string.Format("Play session already in progress. Features{0}like statistics tracking may not function.", Environment.NewLine);
                return;
            }

            if (GameFile != null && IsIwad(GameFile) && SelectedGameProfile is GameFile)
            {
                tblFiles.RowStyles[0].Height = infoHeight;
                pbInfo.Image = Properties.Resources.bon2b;
                lblInfo.Text = string.Format("These files will automatically be added{0} when this IWAD is selected for play.", Environment.NewLine);
            }
            else
            {
                tblFiles.RowStyles[0].Height = 0;
            }
        }

        private void SetDefaultSelections()
        {
            if (cmbMap.Items.Count > 0)
                cmbMap.SelectedIndex = 0;
            else
                cmbMap.SelectedIndex = -1;

            chkMap.Checked = false;

            txtParameters.Text = string.Empty;
            SpecificFiles = null;

            ctrlFiles.SetDataSource(new List<IGameFile>());
        }

        private void RegisterEvents()
        {
            cmbProfiles.SelectedIndexChanged += CmbProfiles_SelectedIndexChanged;
            cmbSourcePorts.SelectedIndexChanged += cmbSourcePorts_SelectedIndexChanged;
            cmbIwad.SelectedIndexChanged += cmbIwad_SelectedIndexChanged;
            cmbDemo.SelectedIndexChanged += cmbDemo_SelectedIndexChanged;

            chkRecord.CheckedChanged += chkRecord_CheckedChanged;
            chkDemo.CheckedChanged += chkDemo_CheckedChanged;
        }

        private void UnregisterEvents()
        {
            cmbProfiles.SelectedIndexChanged -= CmbProfiles_SelectedIndexChanged;
            cmbSourcePorts.SelectedIndexChanged -= cmbSourcePorts_SelectedIndexChanged;
            cmbIwad.SelectedIndexChanged -= cmbIwad_SelectedIndexChanged;
            cmbDemo.SelectedIndexChanged -= cmbDemo_SelectedIndexChanged;

            chkRecord.CheckedChanged -= chkRecord_CheckedChanged;
            chkDemo.CheckedChanged -= chkDemo_CheckedChanged;
        }

        private List<IGameProfile> LoadProfiles()
        {
            if (GameFile.GameFileID.HasValue)
            {
                List<IGameProfile> profiles = m_adapter.GetGameProfiles(GameFile.GameFileID.Value).OrderBy(x => x.Name).ToList();
                profiles.Insert(0, (GameFile)GameFile);
                AutoCompleteCombo.SetAutoCompleteCustomSource(cmbProfiles, profiles, typeof(IGameProfile), "Name");
                return profiles;
            }

            return new List<IGameProfile>();
        }

        private static string[] MapSplit(IGameFile gameFile) => DataSources.GameFile.GetMaps(gameFile);

        private bool IsIwad(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
                return m_adapter.GetIWad(gameFile.GameFileID.Value) != null;

            return false;
        }

        public IGameFile GameFile { get; private set; }
        public IGameProfile SelectedGameProfile => (IGameProfile)cmbProfiles.SelectedItem;

        public ISourcePortData SelectedSourcePort
        {
            get { return cmbSourcePorts.SelectedItem as ISourcePortData; }
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

        public List<IGameFile> GetIWadAdditionalFiles()
        {
            return GetAdditionalFiles().Intersect(m_handler.GetIWadFiles()).ToList();
        }

        public List<IGameFile> GetSourcePortAdditionalFiles()
        {
            return GetAdditionalFiles().Intersect(m_handler.GetSourcePortFiles()).ToList();
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

        public bool LoadLatestSave
        {
            get { return chkLoadLatestSave.Enabled && chkLoadLatestSave.Checked; }
        }

        public bool ExtraParametersOnly
        {
            get { return chkExtraParamsOnly.Checked; }
        }

        public bool ScreenFilter
        {
            get { return chkScreenFilter.Checked; }
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
            HandleSourcePortSelectionChange();
        }

        private void HandleSourcePortSelectionChange(bool resetAdditionalFiles = true)
        {
            if (cmbSourcePorts.SelectedItem != null && GameFile != null)
            {
                ISourcePortData sourcePort = cmbSourcePorts.SelectedItem as ISourcePortData;
                chkSaveStats.Enabled = SaveStatisticsSupported(sourcePort);
                chkLoadLatestSave.Enabled = LoadLatestSaveSupported(sourcePort);
                SetAdditionalFiles(resetAdditionalFiles);
                PopulateDemos();
            }

            m_lastSourcePort = SelectedSourcePort;
        }

        private void PopulateDemos()
        {
            ISourcePortData sourcePort = cmbSourcePorts.SelectedItem as ISourcePortData;

            if (GameFile.GameFileID.HasValue)
            {
                IEnumerable<IFileData> demoFiles = m_adapter.GetFiles(GameFile, FileType.Demo)
                    .Where(x => x.SourcePortID == sourcePort.SourcePortID);

                AutoCompleteCombo.SetAutoCompleteCustomSource(cmbDemo, demoFiles.ToList(), typeof(IFileData), "Description");
            }
        }

        private bool SaveStatisticsSupported(ISourcePortData sourcePort)
        {
            return SourcePortUtil.CreateSourcePort(sourcePort).StatisticsSupported();
        }

        private bool LoadLatestSaveSupported(ISourcePortData sourcePort)
        {
            return SourcePortUtil.CreateSourcePort(sourcePort).LoadSaveGameSupported();
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
                    IGameFile[] selectedFiles = fileSelect.SelectedFiles.Except(new IGameFile[] { GameFile }).Union(GetAdditionalFiles()).ToArray();

                    if (selectedFiles.Length > 0)
                    {
                        e.NewItems = selectedFiles.Cast<object>().ToList();

                        try
                        {
                            ResetSpecificFilesSelections(selectedFiles);
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

                if (tabTo != null && tabTo.GameFileViewControl is IGameFileColumnView columnView)
                    tabTo.SetColumnConfig(columnView.ColumnFields, tabFrom.GetColumnConfig().ToArray());
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

        private void CtrlFiles_ItemAdded(object sender, AdditionalFilesEventArgs e)
        {
        }

        private void CtrlFiles_ItemRemoved(object sender, AdditionalFilesEventArgs e)
        {
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
            return SpecificFilesForm.GetSupportedFiles(m_appConfig.GameFileDirectory.GetFullPath(), gameFile, SourcePortData.GetSupportedExtensions(SelectedSourcePort));
        }

        private void cmbIwad_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleIwadSelectionChanged();
        }

        private void HandleIwadSelectionChanged(bool resetAdditionalFiles = true)
        {
            SetAdditionalFiles(resetAdditionalFiles);

            if (GameFile == null || (GameFile != null && string.IsNullOrEmpty(GameFile.Map)))
                SetMapsFromIwad();

            m_lastIwad = SelectedIWad;
        }

        private void SetMapsFromIwad()
        {
            if (SelectedIWad == null)
                return;

            var gameFileIwad = m_adapter.GetGameFileIWads().FirstOrDefault(x => x.GameFileID == SelectedIWad.GameFileID);
            if (gameFileIwad != null)
                AutoCompleteCombo.SetAutoCompleteCustomSource(cmbMap, MapSplit(gameFileIwad), null, null);
        }

        private void SetAdditionalFiles(bool reset)
        {
            if (m_handler == null)
                return;

            if (reset && m_lastIwad != null && m_lastSourcePort != null)
            {
                m_handler.CalculateAdditionalFiles(m_lastIwad, m_lastSourcePort);
                m_handler.CalculateAdditionalFiles(SelectedIWad, cmbSourcePorts.SelectedItem as ISourcePortData);
                ResetSpecificFilesSelections(m_handler.GetCurrentAdditionalNewFiles().ToArray());
            }

            ctrlFiles.SetDataSource(m_handler.GetCurrentAdditionalFiles());
            ctrlFiles.Refresh(); //the port or iwad in () may have changed so invalidate to force update
        }

        private bool ShouldRecalculateAdditionalFiles()
        {
            //For files that have been launcned before: Selected index changed fires a lot on init, so ignore those
            //If the file has never been launched, then we need to set the additional files on init
            return SelectedGameProfile != null && !SelectedGameProfile.SettingsSaved;
        }

        private void lnkSpecific_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SpecificFilesForm form = new SpecificFilesForm();
            form.StartPosition = FormStartPosition.CenterParent;

            List<IGameFile> gameFiles = new List<IGameFile>();
            gameFiles.AddRange(GetAdditionalFiles());

            form.Initialize(m_appConfig.GameFileDirectory, gameFiles, SourcePortData.GetSupportedExtensions(SelectedSourcePort), SpecificFiles);

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

        private void lnkLoadSaveMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveInfo form = new SaveInfo();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        public bool SettingsValid(out string error)
        {
            error = null;

            if (chkRecord.Checked && string.IsNullOrEmpty(txtDescription.Text))
                error = "Please enter a description for the demo to record.";
            else if (SelectedSourcePort == null)
                error = "A source port must be selected.";
            else if (chkMap.Checked && SelectedMap == null)
                error = "A map must be selected.";
            else if (chkMap.Checked && SelectedSkill == null)
                error = "A skill must be selected";
            else if (chkDemo.Checked && SelectedDemo == null)
                error = "A demo must be selected.";
            else if (SelectedGameProfile == null)
                error = "A profile must be selected.";

            return error == null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Accept();
        }

        public void Accept()
        {
            if (SettingsValid(out string err))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(this, err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void ctrlFiles_CellFormatting(object sender, AdditionalFilesEventArgs e)
        {
            IGameFile gameFile = e.Item as IGameFile;
            IGameFile iwad = SelectedIWad;
            ISourcePortData port = SelectedSourcePort;
            if (iwad != null && m_handler.IsIWadFile(gameFile))
                e.DisplayText = string.Format("{0} ({1})", gameFile.FileName, Path.GetFileNameWithoutExtension(iwad.FileName));
            if (port != null && m_handler.IsSourcePortFile(gameFile))
                e.DisplayText = string.Format("{0} ({1})", gameFile.FileName, port.Name);
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (SelectedGameProfile == null)
                MessageBox.Show(this, "A profile must be selected to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                SaveSettings?.Invoke(this, new EventArgs());
        }

        private void lnkOpenDemo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<IFileData> demoFiles = BasicFileView.CreateFileAssociation(this, m_adapter, m_appConfig.DemoDirectory, FileType.Demo, GameFile,
                cmbSourcePorts.SelectedItem as ISourcePortData);

            if (demoFiles.Count > 0)
            {
                PopulateDemos();
                SelectedSourcePort = m_adapter.GetSourcePort(demoFiles.First().SourcePortID);
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
            if (chkDemo.Checked && cmbDemo.SelectedItem != null)
            {
                var file = cmbDemo.SelectedItem as IFileData;
                var parser = DemoUtil.GetDemoParser(Path.Combine(m_appConfig.DemoDirectory.GetFullPath(), file.FileName));

                if (parser != null)
                {
                    m_handler.Reset();
                    SetAdditionalFiles(true);

                    string[] requiredFiles = parser.GetRequiredFiles();
                    List<string> unavailable = new List<string>();
                    List<IGameFile> iwads = new List<IGameFile>();
                    List<IGameFile> gameFiles = GetGameFiles(requiredFiles, unavailable, iwads);
                    ctrlFiles.SetDataSource(gameFiles);
                    if (iwads.Count > 0)
                        SelectedIWad = iwads.First();

                    if (unavailable.Count > 0)
                    {
                        TextBoxForm form = new TextBoxForm(true, MessageBoxButtons.OK)
                        {
                            StartPosition = FormStartPosition.CenterParent,
                            Text = "Not Found",
                            HeaderText = "The following required files were not found:",
                            DisplayText = string.Join(Environment.NewLine, unavailable.ToArray())
                        };
                        form.ShowDialog(this);
                    }

                    m_demoChangedAdditionalFiles = true;
                    ResetSpecificFilesSelections(ctrlFiles.GetFiles().Cast<IGameFile>().ToArray()); //don't use the handler in this case, we are overriding it
                }
            }
            else
            {
                m_demoChangedAdditionalFiles = false;
            }
        }

        private void lnkPreviewLaunchParameters_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnPreviewLaunchParameters?.Invoke(this, new EventArgs());
        }

        private void lnkCustomParameters_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void lnkFilterSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FilterSettingsForm form;

            try
            {
                form = new FilterSettingsForm(m_filterSettings);
            }
            catch
            {
                m_filterSettings = CreateDefaultFilterSettings(); //this can happen due to an update and the xml not having the property, reset to default
                form = new FilterSettingsForm(m_filterSettings);
            }

            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                m_filterSettings = form.GetFilterSettings();
                m_filterSettings.Enabled = chkScreenFilter.Checked;
                WriteFilterSettings(m_filterSettings);
            }
        }

        private static readonly string s_filterFile = "FilterSettings.xml";

        private static string GetFilterFile()
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), s_filterFile);
        }

        private void WriteFilterSettings(ScreenFilter settings)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(ScreenFilter));
                using (FileStream fs = new FileStream(GetFilterFile(), FileMode.Create))
                    x.Serialize(fs, settings);
            }
            catch
            {
                //oh well, at least we tried
            }
        }

        public ScreenFilter GetFilterSettings()
        {
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(ScreenFilter));
               
                using (FileStream fs = new FileStream(GetFilterFile(), FileMode.Open))
                    return (ScreenFilter)x.Deserialize(fs);
            }
            catch
            {
                if (File.Exists(GetFilterFile()))
                    File.Delete(GetFilterFile());
                return CreateDefaultFilterSettings();
            }
        }

        private ScreenFilter CreateDefaultFilterSettings()
        {
            return new ScreenFilter()
            {
                Type = ScreenFilterType.Ellipse,
                Opacity = 0.5f,
                LineThickness = 1,
                BlockSize = 4,
                SpacingX = 0,
                SpacingY = 0,
                Stagger = true,
                ScanlineSpacing = 4,
                VerticalScanlines = true,
                HorizontalScanlines = true,
                Enabled = chkScreenFilter.Checked
            };
        }

        private void chkScreenFilter_CheckedChanged(object sender, EventArgs e)
        {
            m_filterSettings = GetFilterSettings();
            m_filterSettings.Enabled = chkScreenFilter.Checked;
            WriteFilterSettings(m_filterSettings);
        }

        private bool RenameGameProfile(int gameProfileID, string name)
        {
            if (!IsValidProfileName(gameProfileID, name, out string error))
            {
                MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            SelectedGameProfile.Name = name;
            m_adapter.UpdateGameProfile(SelectedGameProfile);
            cmbProfiles.SelectedIndexChanged -= CmbProfiles_SelectedIndexChanged;
            LoadProfiles();
            cmbProfiles.SelectedValue = gameProfileID;
            cmbProfiles.SelectedIndexChanged += CmbProfiles_SelectedIndexChanged;

            return true;
        }

        private bool IsValidProfileName(int gameProfileID, string name, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(name))
            {
                error = "A name must be entered.";
                return false;
            }

            if (m_adapter.GetGameProfiles(GameFile.GameFileID.Value).Where(x => x.GameProfileID != gameProfileID)
                .Any(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                error = "This profile name already exists.";
                return false;
            }

            return true;
        }

        private static TextBoxForm CreateProfileTextBoxForm(string displayText, bool showCheckBox)
        {
            TextBoxForm form = new TextBoxForm(false, MessageBoxButtons.OKCancel);
            form.SetMaxLength(48);
            form.DisplayText = displayText;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = "Enter Profile Name";
            form.SelectDisplayText(0, form.DisplayText.Length);
            if (showCheckBox)
                form.SetCheckBox("Copy current profile");
            return form;
        }

        public void UpdateGameProfile(IGameProfile gameProfile)
        {
            gameProfile.SettingsSaved = true;
            gameProfile.SourcePortID = gameProfile.IWadID = null;

            if (SelectedSourcePort != null) gameProfile.SourcePortID = SelectedSourcePort.SourcePortID;
            if (SelectedIWad != null) gameProfile.IWadID = SelectedIWad.IWadID;

            if (SelectedMap != null) gameProfile.SettingsMap = SelectedMap;
            else gameProfile.SettingsMap = string.Empty; //this setting can be turned off

            if (SelectedSkill != null) gameProfile.SettingsSkill = SelectedSkill;
            if (ExtraParameters != null) gameProfile.SettingsExtraParams = ExtraParameters;

            gameProfile.SettingsStat = SaveStatistics;
            gameProfile.SettingsLoadLatestSave = LoadLatestSave;
            gameProfile.SettingsExtraParamsOnly = ExtraParametersOnly;

            if (ShouldSaveAdditionalFiles())
            {
                gameProfile.SettingsFiles = string.Join(";", GetAdditionalFiles().Select(x => x.FileName).ToArray());
                gameProfile.SettingsFilesIWAD = string.Join(";", GetIWadAdditionalFiles().Select(x => x.FileName).ToArray());
                gameProfile.SettingsFilesSourcePort = string.Join(";", GetSourcePortAdditionalFiles().Select(x => x.FileName).ToArray());

                if (SpecificFiles != null)
                    gameProfile.SettingsSpecificFiles = string.Join(";", SpecificFiles);
                else
                    gameProfile.SettingsSpecificFiles = string.Empty; //this setting can be turned off
            }
        }

        private void CmbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetGameProfile(SelectedGameProfile);
        }

        private void newProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBoxForm form = CreateProfileTextBoxForm("New Profile", true);
            bool success = false;

            while (!success && form.ShowDialog(this) == DialogResult.OK)
            {
                success = IsValidProfileName(-1, form.DisplayText.Trim(), out string error);

                if (success)
                {
                    GameProfile gameProfile = new GameProfile(GameFile.GameFileID.Value, form.DisplayText);

                    GameProfile.ApplyDefaultsToProfile(gameProfile, m_appConfig);
                    if (GameFile != null && IsIwad(GameFile))
                        gameProfile.IWadID = GameFile.IWadID.Value;

                    if (form.CheckBoxChecked)
                        UpdateGameProfile(gameProfile);
                    
                    m_adapter.InsertGameProfile(gameProfile);

                    cmbProfiles.SelectedIndexChanged -= CmbProfiles_SelectedIndexChanged;
                    var profiles = LoadProfiles();
                    cmbProfiles.SelectedIndexChanged += CmbProfiles_SelectedIndexChanged;
                    cmbProfiles.SelectedValue = profiles.Max(x => x.GameProfileID);
                }
                else
                {
                    MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void editProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedGameProfile is GameFile)
            {
                MessageBox.Show(this, "The default profile cannot be renamed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TextBoxForm form = CreateProfileTextBoxForm(SelectedGameProfile.Name, false);
            int gameProfileID = SelectedGameProfile.GameProfileID;
            bool success = false;

            while (!success && form.ShowDialog(this) == DialogResult.OK)
            {
                success = RenameGameProfile(gameProfileID, form.DisplayText.Trim());
            }
        }

        private void deleteProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedGameProfile != null)
            {
                if (SelectedGameProfile is GameFile)
                {
                    MessageBox.Show(this, "The default profile cannot be deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show(this, string.Format("Are you sure you want to delete {0}?", SelectedGameProfile.Name), 
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    m_adapter.DeleteGameProfile(SelectedGameProfile.GameProfileID);
                    LoadProfiles();
                }
            }
        }

        private List<IGameFile> GetGameFiles(string[] fileNames, List<string> unavailable, List<IGameFile> iwads)
        {        
            List<IGameFile> gameFiles = new List<IGameFile>();
            var knowniwads = m_adapter.GetGameFileIWads();

            foreach (string file in fileNames)
            {
                // This currently doesn't work for unmanaged files
                var gameFile = m_adapter.GetGameFile(file.Replace(Path.GetExtension(file), ".zip"));
                if (gameFile == null)
                {
                    unavailable.Add(file);
                    continue;
                }

                if (knowniwads.Any(x => x.FileName.Equals(gameFile.FileName, StringComparison.InvariantCultureIgnoreCase)))
                    iwads.Add(gameFile);
                else
                    gameFiles.Add(gameFile);
            }

            return gameFiles;
        }

        private void TxtParameters_Click(object sender, EventArgs e)
        {
            TextBoxForm form = new TextBoxForm(true, MessageBoxButtons.OKCancel)
            {
                Text = "Extra Parameters",
                DisplayText = txtParameters.Text,
                StartPosition = FormStartPosition.CenterParent,
                AcceptButton = null
            };

            form.SelectDisplayText(0, 0);
            if (form.ShowDialog(this) == DialogResult.OK)
                txtParameters.Text = form.DisplayText;
        }
    }
}
