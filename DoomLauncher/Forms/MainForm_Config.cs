using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private void HandleFormClosing()
        {
            if (DataSourceAdapter != null)
            {
                IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();

                if (WindowState != FormWindowState.Minimized) //too many problems when the form is minimized, not supported
                {
                    UpdateConfig(config, AppConfiguration.SplitTopBottomName, splitTopBottom.SplitterDistance.ToString());
                    UpdateConfig(config, AppConfiguration.SplitLeftRightName, splitLeftRight.SplitterDistance.ToString());
                    UpdateConfig(config, AppConfiguration.SplitTagSelectName, splitTagSelect.SplitterDistance.ToString());

                    UpdateConfig(config, AppConfiguration.AppWidthName, Size.Width.ToString());
                    UpdateConfig(config, AppConfiguration.AppHeightName, Size.Height.ToString());
                    UpdateConfig(config, AppConfiguration.AppXName, Location.X.ToString());
                    UpdateConfig(config, AppConfiguration.AppYName, Location.Y.ToString());
                    UpdateConfig(config, AppConfiguration.WindowStateName, WindowState.ToString());
                }

                UpdateConfig(config, AppConfiguration.ColumnConfigName, BuildColumnConfig());
                UpdateConfig(config, ConfigType.AutoSearch.ToString("g"), chkAutoSearch.Checked.ToString());
                UpdateConfig(config, AppConfiguration.ItemsPerPageName, AppConfiguration.ItemsPerPage.ToString());
                UpdateConfig(config, AppConfiguration.LastSelectedTabIndexName, tabControl.SelectedIndex.ToString());
                UpdateConfig(config, AppConfiguration.TagSelectPinnedName, m_tagSelectControl.Pinned.ToString());
            }
        }

        private void UpdateColumnConfig()
        {
            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            UpdateConfig(config, AppConfiguration.ColumnConfigName, BuildColumnConfig());
        }

        private void UpdateConfig(IEnumerable<IConfigurationData> config, string name, string value)
        {
            IConfigurationData configFind = config.FirstOrDefault(x => x.Name == name);

            if (configFind == null)
            {
                DataSourceAdapter.InsertConfiguration(new ConfigurationData
                {
                    Name = name,
                    Value = value,
                    UserCanModify = false
                });
            }
            else
            {
                configFind.Value = value;
                DataSourceAdapter.UpdateConfiguration(configFind);
            }
        }

        private string BuildColumnConfig()
        {
            List<ColumnConfig> columnViewConfig = new List<ColumnConfig>();
            List<ColumnConfig> tileViewConfig = new List<ColumnConfig>();
            HashSet<string> tileViewKeys = new HashSet<string>();

            foreach (ITabView tab in m_tabHandler.TabViews)
            {
                if (tab.GameFileViewControl is IGameFileColumnView)
                {
                    columnViewConfig.AddRange(tab.GetColumnConfig());
                }
                else
                {
                    tileViewConfig.AddRange(tab.GetColumnConfig());
                    tileViewKeys.Add(tab.Key.ToString());
                }
            }

            // Only select columns from views that are tile views
            List<ColumnConfig> config = DataCache.Instance.GetColumnConfig().Where(x => tileViewKeys.Contains(x.Parent)).ToList();
            // Tile views use IGameFileSortableView which only stores the single column that is sorted, so clear all sorting on all columns
            config.ForEach(x => x.Sort = SortOrder.None);

            foreach (ColumnConfig viewColumn in tileViewConfig)
            {
                ColumnConfig existingColumn = config.FirstOrDefault(x => x.Parent == viewColumn.Parent && x.Column == viewColumn.Column);
                if (existingColumn != null)
                    existingColumn.Sort = viewColumn.Sort;
                else
                    config.Add(viewColumn);
            }

            config.AddRange(columnViewConfig);

            return SerializeColumnConfig(config);
        }

        private string SerializeColumnConfig(List<ColumnConfig> config)
        {
            try
            {
                StringWriter text = new StringWriter();
                XmlSerializer xml = new XmlSerializer(typeof(ColumnConfig[]));
                xml.Serialize(text, config.ToArray());
                return text.ToString();
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }

            return string.Empty;
        }
    }
}
