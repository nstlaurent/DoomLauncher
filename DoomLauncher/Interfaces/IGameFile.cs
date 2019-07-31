using System;

namespace DoomLauncher.Interfaces
{
    public interface IGameFile
    {
        int? GameFileID { get; set; }
        string FileName { get; set; }
        string Title { get; set; }
        string Author { get; set; }
        DateTime? ReleaseDate { get; set; }
        string Description { get; set; }
        int? IWadID { get; set; }
        int? SourcePortID { get; set; }
        string Thumbnail { get; set; }
        string Comments { get; set; }
        string Map { get; set; }
        int? MapCount { get; set; }
        double? Rating { get; set; }
        DateTime? LastPlayed { get; set; }
        DateTime? Downloaded { get; set; }

        string SettingsMap { get; set; }
        string SettingsSkill { get; set; }
        string SettingsExtraParams { get; set; }
        string SettingsFiles { get; set; }
        string SettingsFilesSourcePort { get; set; }
        string SettingsFilesIWAD { get; set; }
        string SettingsSpecificFiles { get; set; }
        bool SettingsStat { get; set; }

        int MinutesPlayed { get; set; }
        int FileSizeBytes { get; set; }
    }
}
