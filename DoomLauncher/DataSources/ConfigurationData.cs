using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    class ConfigurationData : IConfigurationData
    {
        public ConfigurationData()
        {
            AvailableValues = string.Empty;
            Value = string.Empty;
        }

        public int ConfigID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string AvailableValues { get; set; }
        public bool UserCanModify { get; set; }
    }
}
