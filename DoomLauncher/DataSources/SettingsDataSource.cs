using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    class SettingsDataSource : ISettingsDataSource
    {
        public int GameFileID { get; set; }
        public int SettingsID { get; set; }
        public string SettingsName { get; set; }
        public string SettingsMap { get; set; }
        public string SettingsSkill { get; set; }
        public string SettingsExtraParams { get; set; }
        public string SettingsFiles { get; set; }
        public string SettingsSpecificFiles { get; set; }
    }
}
