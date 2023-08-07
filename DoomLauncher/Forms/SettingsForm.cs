using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class SettingsForm : Form
    {
        private readonly List<Tuple<IConfigurationData, object>> m_configValues = new List<Tuple<IConfigurationData, object>>();
        private TextBox m_gameFileDirectory, m_screenshotDirectories;
        private Label m_lblScreenshotWidth;
        private TrackBar m_screenshotTrackBar;

        private readonly IDataSourceAdapter m_adapter;
        private readonly AppConfiguration m_appConfig;
        private Padding m_controlMargin = new Padding(4, 0, 0, 0);

        private static readonly int TextBoxWidth = 190;
        private static readonly int FullControlWidth = 270;

        public SettingsForm(IDataSourceAdapter adapter, AppConfiguration appConfig)
        {
            InitializeComponent();

            lblLaunchSettings.Text = string.Concat("These are the default settings for a game file", Environment.NewLine, 
                " that does not have a specific configuration saved.");

            m_adapter = adapter;
            m_appConfig = appConfig;

            pnlViewRestart.Paint += PnlViewRestart_Paint;

            PopulateDefaultSettings(m_adapter);
            PopulateConfiguration();
            UpdateScreenshotWidth(m_screenshotTrackBar);

            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
            PopulateViews();
        }

        private void PopulateViews()
        {
            HashSet<string> visibleViews = DataCache.Instance.AppConfiguration.VisibleViews;
            foreach (string key in TabKeys.KeyNames.Except(new string[] { TabKeys.LocalKey }))
                chkListViews.Items.Add(key, visibleViews.Contains(key));

            chkListViews.ItemCheck += chkListViews_ItemCheck;
        }

        private void PnlViewRestart_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawIcon(SystemIcons.Warning, 8, 8);
        }

        public void SetToLaunchSettingsTab()
        {
            tabControl.SelectedIndex = 1;
        }

        public void SetCancelAllowed(bool set)
        {
            btnCancel.Visible = set;
            titleBar.SetControlBox(set);
        }

        private int GetDefaultControlWidth()
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            return dpiScale.ScaleIntX(FullControlWidth);
        }

        private void PopulateConfiguration()
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            m_controlMargin = new Padding(dpiScale.ScaleIntX(4), 0, 0, 0);
            IEnumerable<IConfigurationData> configItems = m_adapter.GetConfiguration().Where(x => x.UserCanModify);

            TableLayoutPanel tblMain = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset,
            };

            int height = dpiScale.ScaleIntY(8);
            int labelWidth = dpiScale.ScaleIntX(134);
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelWidth));

            int height32 = dpiScale.ScaleIntY(32);
            int rowPadding = dpiScale.ScaleIntX(1);

            foreach (IConfigurationData config in configItems)
            {
                GrowLabel lbl = new GrowLabel
                {
                    Anchor = AnchorStyles.Left,
                    Text = AddSpaceBetweenWords(config.Name),
                    Width = labelWidth
                };

                tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, lbl.Height < height32 ? height32 : lbl.Height));
                tblMain.Controls.Add(lbl, 0, tblMain.RowStyles.Count - 1);

                if (!string.IsNullOrEmpty(config.AvailableValues))
                    HandleComboBox(tblMain, config);
                else if (config.Name == AppConfiguration.ScreenshotPreviewSizeName) //special case for TrackBar
                    HandleScreenshotPreviewSize(tblMain, config, dpiScale);
                else
                    HandleTextBox(tblMain, config);

                height += height32 + rowPadding;
            }

            tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tblMain.Height = height + dpiScale.ScaleIntY(8);

            tabControl.TabPages[0].Controls.Add(tblMain);
            Height = tblMain.Height + dpiScale.ScaleIntY(108+32);
            Width = dpiScale.ScaleIntX(452);
        }

        private void HandleTextBox(TableLayoutPanel tblMain, IConfigurationData config)
        {
            TextBox txt = new TextBox
            {
                Anchor = AnchorStyles.Left,
                Text = config.Value,
                Width = GetDefaultControlWidth(),
                Margin = m_controlMargin
            };
            m_configValues.Add(new Tuple<IConfigurationData, object>(config, txt));

            if (config.Name == "GameFileDirectory")
                HandleGameFileDirectory(tblMain, txt);
            else if (config.Name == "ScreenshotCaptureDirectories")
                HandleScreenshotCaptureDirectories(tblMain, txt);
            else
                tblMain.Controls.Add(txt, 1, tblMain.RowStyles.Count - 1);
        }

        private void HandleComboBox(TableLayoutPanel tblMain, IConfigurationData config)
        {
            ComboBox cmb = new ComboBox
            {
                Anchor = AnchorStyles.Left,
                Width = GetDefaultControlWidth(),
                Margin = m_controlMargin
            };

            string[] items = Util.SplitString(config.AvailableValues);
            List<Tuple<string, string>> cmbDataSource = new List<Tuple<string, string>>();

            for (int i = 0; i < items.Length - 1; i += 2)
                cmbDataSource.Add(new Tuple<string, string>(items[i], items[i + 1]));

            cmb.ValueMember = "Item1";
            cmb.DataSource = cmbDataSource;
            cmb.BindingContext = new BindingContext();
            cmb.SelectedItem = cmbDataSource.FirstOrDefault(x => x.Item2 == config.Value);

            tblMain.Controls.Add(cmb, 1, tblMain.RowStyles.Count - 1);
            m_configValues.Add(new Tuple<IConfigurationData, object>(config, cmb));
        }

        private void HandleScreenshotPreviewSize(TableLayoutPanel tblMain, IConfigurationData config, DpiScale dpiScale)
        {
            m_lblScreenshotWidth = new Label
            {
                Width = dpiScale.ScaleIntX(68),
                Height = dpiScale.ScaleIntY(16),
                Margin = new Padding(dpiScale.ScaleIntX(4), dpiScale.ScaleIntX(8), 0, 0)
            };

            m_screenshotTrackBar = new TrackBar
            {
                Minimum = -8,
                Maximum = 8,
                Value = Convert.ToInt32(config.Value),
                Width = dpiScale.ScaleIntX(200)
            };
            m_screenshotTrackBar.ValueChanged += Trk_ValueChanged;

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill
            };
            flp.Controls.Add(m_screenshotTrackBar);
            flp.Controls.Add(m_lblScreenshotWidth);

            tblMain.Controls.Add(flp, 1, tblMain.RowStyles.Count - 1);
            m_configValues.Add(new Tuple<IConfigurationData, object>(config, m_screenshotTrackBar));
        }

        private void Trk_ValueChanged(object sender, EventArgs e)
        {
            UpdateScreenshotWidth(((TrackBar)sender));
        }

        private void UpdateScreenshotWidth(TrackBar trackBar)
        {
            m_lblScreenshotWidth.Text = string.Concat("Width: ", Util.GetPreviewScreenshotWidth(trackBar.Value));
        }

        private void HandleScreenshotCaptureDirectories(TableLayoutPanel tblMain, TextBox txt)
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            m_screenshotDirectories = txt;
            m_screenshotDirectories.Width = dpiScale.ScaleIntX(TextBoxWidth);
            m_screenshotDirectories.Enabled = false;
            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            flp.Controls.Add(txt);

            Button changeButton = new Button
            {
                Text = "Change..."
            };

            changeButton.Width = dpiScale.ScaleIntX(changeButton.Width);
            changeButton.Height = dpiScale.ScaleIntY(changeButton.Height);
            changeButton.Click += ChangeButton_Click;
            flp.Controls.Add(changeButton);
            tblMain.Controls.Add(flp, 1, tblMain.RowStyles.Count - 1);
        }

        private void HandleGameFileDirectory(TableLayoutPanel tblMain, TextBox txt)
        {
            DpiScale dpiScale = new DpiScale(CreateGraphics());
            m_gameFileDirectory = txt;
            m_gameFileDirectory.Width = dpiScale.ScaleIntX(TextBoxWidth);
            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            flp.Controls.Add(m_gameFileDirectory);

            Button browseButton = new Button
            {
                Text = "Browse..."
            };

            browseButton.Width = dpiScale.ScaleIntX(browseButton.Width);
            browseButton.Height = dpiScale.ScaleIntY(browseButton.Height);
            browseButton.Click += browseButton_Click;
            flp.Controls.Add(browseButton);
            tblMain.Controls.Add(flp, 1, tblMain.RowStyles.Count - 1);
        }

        void browseButton_Click(object sender, EventArgs e)
        {
            LauncherPath path = new LauncherPath(m_gameFileDirectory.Text);
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                SelectedPath = path.GetFullPath()
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                path = new LauncherPath(dialog.SelectedPath);
                m_gameFileDirectory.Text = path.GetPossiblyRelativePath();
            }
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            DirectoriesForm form = new DirectoriesForm();
            form.SetDirectories(Util.SplitString(m_screenshotDirectories.Text));
            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog(this) == DialogResult.OK)
                m_screenshotDirectories.Text = string.Join(";", form.GetDirectories());
        }

        private void PopulateDefaultSettings(IDataSourceAdapter adapter)
        {
            cmbSourcePorts.DataSource = adapter.GetSourcePorts();
            cmbIwad.DataSource = Util.GetIWadsDataSource(adapter);
            cmbSkill.DataSource = Util.GetSkills();
            cmbFileManagement.DataSource = Enum.GetValues(typeof(FileManagement));
            cmbViewType.DataSource = new [] { "Grid", "Tile Large", "Tile Small" };
            cmbTheme.DataSource = new [] { "Default", "Dark", "System" };

            cmbSourcePorts.SelectedValue = m_appConfig.GetTypedConfigValue(ConfigType.DefaultSourcePort, typeof(int));
            cmbIwad.SelectedValue = m_appConfig.GetTypedConfigValue(ConfigType.DefaultIWad, typeof(int));
            cmbSkill.SelectedItem = m_appConfig.GetTypedConfigValue(ConfigType.DefaultSkill, typeof(string));
            cmbFileManagement.SelectedIndex = (int)Enum.Parse(typeof(FileManagement), (string)m_appConfig.GetTypedConfigValue(ConfigType.FileManagement, typeof(string)));
            cmbViewType.SelectedIndex = (int)Enum.Parse(typeof(GameFileViewType), (string)m_appConfig.GetTypedConfigValue(ConfigType.GameFileViewType, typeof(string)));
            cmbTheme.SelectedIndex = (int)Enum.Parse(typeof(ColorThemeType), (string)m_appConfig.GetTypedConfigValue(ConfigType.ColorThemeType, typeof(string)));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (Tuple<IConfigurationData, object> config in m_configValues)
            {
                config.Item1.Value = GetValue(config.Item1, config.Item2);
                m_adapter.UpdateConfiguration(config.Item1);
            }

            HandleLaunchSettings();
            Close();
        }

        private void HandleLaunchSettings()
        { 
            string[] configNames = new string[]
            {  
                ConfigType.DefaultSourcePort.ToString("g"), 
                ConfigType.DefaultIWad.ToString("g"), 
                ConfigType.DefaultSkill.ToString("g"),
                ConfigType.FileManagement.ToString("g"),
                ConfigType.GameFileViewType.ToString("g"),
                ConfigType.VisibleViews.ToString("g"),
                ConfigType.ColorThemeType.ToString("g"),
            };
            string[] configValues = new string[]
            {
                cmbSourcePorts.SelectedItem == null ? null : ((ISourcePortData)cmbSourcePorts.SelectedItem).SourcePortID.ToString(),
                cmbIwad.SelectedItem == null ? null : ((IIWadData)cmbIwad.SelectedItem).IWadID.ToString(),
                cmbSkill.SelectedItem?.ToString(),
                cmbFileManagement.SelectedValue.ToString(),
                ((GameFileViewType)cmbViewType.SelectedIndex).ToString(),
                string.Join(";", chkListViews.CheckedItems.Cast<string>()),
                ((ColorThemeType)cmbTheme.SelectedIndex).ToString(),
            };

            IEnumerable<IConfigurationData> configuration = m_adapter.GetConfiguration().Where(x => configNames.Contains(x.Name));

            for (int i = 0; i < configNames.Length; i++)
            {
                string configName = configNames[i];
                string configValue = configValues[i];
                IConfigurationData config = configuration.FirstOrDefault(x => x.Name == configName);

                if (configValue != null)
                {
                    if (config == null)
                    {
                        config = CreateConfig(configName, configValue);
                        m_adapter.InsertConfiguration(config);
                    }
                    else
                    {
                        config.Value = configValue;
                        m_adapter.UpdateConfiguration(config);
                    }
                }
            }
        }

        private static IConfigurationData CreateConfig(string configName, string configValue)
        {
            return new ConfigurationData
            {
                Name = configName,
                Value = configValue,
                UserCanModify = false,
                AvailableValues = string.Empty
            };
        }

        private string GetValue(IConfigurationData config, object value)
        {
            if (!string.IsNullOrEmpty(config.AvailableValues) && value is ComboBox cmb && cmb.SelectedItem != null)
            {
                Tuple<string, string> item = cmb.SelectedItem as Tuple<string, string>;
                return item.Item2;
            }
            else if (value is TrackBar trk)
            {
                return trk.Value.ToString();
            }
            else if (value is TextBox txt)
            {
                return txt.Text;
            }

            return string.Empty;
        }

        private void CmbViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlViewRestart.Visible = GameFileViewFactory.IsBaseViewTypeChange(m_appConfig.GameFileViewType, (GameFileViewType)cmbViewType.SelectedIndex);         
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlViewRestart.Visible = true;
        }

        private void chkListViews_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            pnlViewRestart.Visible = true;
        }

        private static string AddSpaceBetweenWords(string item)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (char.IsUpper(item[i]) && i != 0)
                {
                    item = item.Insert(i, " ");
                    i++;
                }
            }

            return item;
        }
    }
}
