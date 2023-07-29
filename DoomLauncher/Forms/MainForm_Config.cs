using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private void HandleFormClosing()
        {
            if (DataSourceAdapter == null || !m_writeConfigOnClose)
                return;
            
            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            if (titleBar.WindowState != FormWindowState.Minimized)
            {
                // Too many problems when the form is minimized, not supported
                if (splitTopBottom.SplitterDistance > 0)
                    DataCache.Instance.UpdateConfig(config, AppConfiguration.SplitTopBottomName, GetSplitterPercent(splitTopBottom, Height).ToString(CultureInfo.InvariantCulture));
                if (splitLeftRight.SplitterDistance > 0)
                    DataCache.Instance.UpdateConfig(config, AppConfiguration.SplitLeftRightName, GetSplitterPercent(splitLeftRight, Width).ToString(CultureInfo.InvariantCulture));
                if (splitTagSelect.SplitterDistance > 0)
                    DataCache.Instance.UpdateConfig(config, AppConfiguration.SplitTagSelectName, GetSplitterPercent(splitTagSelect, Width).ToString(CultureInfo.InvariantCulture));

                DataCache.Instance.UpdateConfig(config, AppConfiguration.AppWidthName, Size.Width.ToString());
                DataCache.Instance.UpdateConfig(config, AppConfiguration.AppHeightName, Size.Height.ToString());
                DataCache.Instance.UpdateConfig(config, AppConfiguration.AppXName, Location.X.ToString());
                DataCache.Instance.UpdateConfig(config, AppConfiguration.AppYName, Location.Y.ToString());
                DataCache.Instance.UpdateConfig(config, AppConfiguration.WindowStateName, titleBar.WindowState.ToString());
            }

            DataCache.Instance.UpdateConfig(config, AppConfiguration.ColumnConfigName, BuildColumnConfig());
            DataCache.Instance.UpdateConfig(config, ConfigType.AutoSearch.ToString("g"), chkAutoSearch.Checked.ToString());
            DataCache.Instance.UpdateConfig(config, AppConfiguration.ItemsPerPageName, AppConfiguration.ItemsPerPage.ToString());
            DataCache.Instance.UpdateConfig(config, AppConfiguration.LastSelectedTabIndexName, tabControl.SelectedIndex.ToString());
            DataCache.Instance.UpdateConfig(config, AppConfiguration.TagSelectPinnedName, m_tagSelectControl.Pinned.ToString());
        }

        private static double GetSplitterPercent(SplitContainer splitter, int windowDimension)
        {
            if (windowDimension <= 0)
                return 0.1;

            return splitter.SplitterDistance / (double)windowDimension;
        }

        private void UpdateColumnConfig()
        {
            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            DataCache.Instance.UpdateConfig(config, AppConfiguration.ColumnConfigName, BuildColumnConfig());
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
                return DataCache.SerializeColumnConfig(config);
            }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
            }

            return string.Empty;
        }
    }
}
