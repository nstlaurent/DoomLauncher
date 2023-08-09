namespace DoomLauncher.Interfaces
{
    public interface IGameProfile
    {
        int GameProfileID { get; set; }
        int? GameFileID { get; set; }
        int? IWadID { get; set; }
        int? SourcePortID { get; set; }
        string Name { get; set; }
        string SettingsMap { get; set; }
        string SettingsSkill { get; set; }
        string SettingsExtraParams { get; set; }
        string SettingsFiles { get; set; }
        string SettingsFilesSourcePort { get; set; }
        string SettingsFilesIWAD { get; set; }
        string SettingsSpecificFiles { get; set; }
        bool SettingsStat { get; set; }
        bool SettingsLoadLatestSave { get; set; }
        bool SettingsSaved { get; set; }
        bool SettingsExtraParamsOnly { get; set; }
        bool IsGlobal { get; }
    }
}
