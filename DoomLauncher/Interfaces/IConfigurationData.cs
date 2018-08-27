using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface IConfigurationData
    {
        int ConfigID { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        string AvailableValues { get; set; }
        bool UserCanModify { get; set; }
    }
}
