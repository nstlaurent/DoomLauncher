using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface ISourcePortData
    {
        int SourcePortID { get; set; }
        string Name { get; set; }
        string Executable { get; set; }
        string SupportedExtensions { get; set; }
        LauncherPath Directory { get; set; }
        string SettingsFiles { get; set; }
        SourcePortLaunchType LaunchType { get; set; }
        string FileOption { get; set; }
        string ExtraParameters { get; set; }
        string GetFullExecutablePath();
    }
}
