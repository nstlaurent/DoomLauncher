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
