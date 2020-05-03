using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                UpdateConfig(config, "ColumnConfig", BuildColumnConfig());
                UpdateConfig(config, ConfigType.AutoSearch.ToString("g"), chkAutoSearch.Checked.ToString());
            }
        }

        private void UpdateColumnConfig()
        {
            IEnumerable<IConfigurationData> config = DataSourceAdapter.GetConfiguration();
            UpdateConfig(config, "ColumnConfig", BuildColumnConfig());
        }

        private void UpdateConfig(IEnumerable<IConfigurationData> config, string name, string value)
        {
            IConfigurationData configFind = config.Where(x => x.Name == name).FirstOrDefault();

            if (configFind == null)
            {
                configFind = new ConfigurationData();
                configFind.Name = name;
                configFind.Value = value;
                configFind.UserCanModify = false;
                DataSourceAdapter.InsertConfiguration(configFind);
            }
            else
            {
                configFind.Value = value;
                DataSourceAdapter.UpdateConfiguration(configFind);
            }
        }

        private string BuildColumnConfig()
        {
            if (m_tabHandler != null)
            {
                List<ColumnConfig> config = new List<ColumnConfig>();

                foreach (ITabView tab in m_tabHandler.TabViews)
                    config.AddRange(tab.GetColumnConfig());

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
            }

            return string.Empty;
        }

        private ColumnConfig[] GetColumnConfig()
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ColumnConfig[]));
                StringReader text = new StringReader(AppConfiguration.ColumnConfig);
                ColumnConfig[] ret = xml.Deserialize(text) as ColumnConfig[];

                // Previous revisions of Doom Launcher use filename, most recent version uses filenamenopath - fix it here
                var columnsToFix = ret.Where(x => x.Column.Equals("filename", StringComparison.OrdinalIgnoreCase));
                foreach(var colFix in columnsToFix)
                    colFix.Column = "filenamenopath";

                if (ret != null)
                    return ret;
            }
            catch { }

            return new ColumnConfig[] { };
        }
    }
}
