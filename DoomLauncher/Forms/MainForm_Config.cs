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
                    UpdateConfig(config, "SplitTopBottom", splitTopBottom.SplitterDistance.ToString());
                    UpdateConfig(config, "SplitLeftRight", splitLeftRight.SplitterDistance.ToString());

                    UpdateConfig(config, "AppWidth", Size.Width.ToString());
                    UpdateConfig(config, "AppHeight", Size.Height.ToString());
                    UpdateConfig(config, "AppX", Location.X.ToString());
                    UpdateConfig(config, "AppY", Location.Y.ToString());
                    UpdateConfig(config, "WindowState", WindowState.ToString());
                }

                if (GameFileViewFactory.IsUsingColumnView)
                    UpdateConfig(config, "ColumnConfig", BuildColumnConfig());
                else
                    UpdateConfig(config, "ColumnConfig", BuildTileColumnConfig());

                UpdateConfig(config, ConfigType.AutoSearch.ToString("g"), chkAutoSearch.Checked.ToString());
                UpdateConfig(config, "ItemsPerPage", AppConfiguration.ItemsPerPage.ToString());
            }
        }

        private void UpdateColumnConfig()
        {
            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            UpdateConfig(config, "ColumnConfig", BuildColumnConfig());
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

        private string BuildTileColumnConfig()
        {
            if (m_tabHandler == null || GameFileViewFactory.IsUsingColumnView)
                return string.Empty;

            List<ColumnConfig> viewConfig = new List<ColumnConfig>();

            foreach (ITabView tab in m_tabHandler.TabViews)
                viewConfig.AddRange(tab.GetColumnConfig());

            List<ColumnConfig> config = DataCache.Instance.GetColumnConfig().ToList();
            // Tile views use IGameFileSortableView which only stores the single column that is sorted, so clear all sorting on all columns
            config.ForEach(x => x.Sort = SortOrder.None);

            foreach (ColumnConfig viewColumn in viewConfig)
            {
                ColumnConfig existingColumn = config.FirstOrDefault(x => x.Parent == viewColumn.Parent && x.Column == viewColumn.Column);
                if (existingColumn != null)
                    existingColumn.Sort = viewColumn.Sort;
                else
                    config.Add(viewColumn);
            }

            return SerializeColumnConfig(config);
        }

        private string BuildColumnConfig()
        {
            if (m_tabHandler == null || !GameFileViewFactory.IsUsingColumnView)
                return string.Empty;

            List<ColumnConfig> config = new List<ColumnConfig>();

            foreach (ITabView tab in m_tabHandler.TabViews)
                config.AddRange(tab.GetColumnConfig());

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
