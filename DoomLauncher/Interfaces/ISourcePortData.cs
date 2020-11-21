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
        LauncherPath AltSaveDirectory { get; set; }
        string GetFullExecutablePath();
    }
}
