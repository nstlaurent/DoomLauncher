using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface ISettingsDataSource
    {
        int GameFileID { get; set; }
        int SettingsID { get; set; }
        string SettingsName { get; set; }
        string SettingsMap { get; set; }
        string SettingsSkill { get; set; }
        string SettingsExtraParams { get; set; }
        string SettingsFiles { get; set; }
        string SettingsSpecificFiles { get; set; }
    }
}
