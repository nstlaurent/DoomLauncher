using DoomLauncher.Interfaces;

namespace DoomLauncher.DataSources
{
    public class GameProfile : IGameProfile
    {
        public GameProfile()
        {
            SettingsMap = SettingsSkill = SettingsExtraParams = SettingsFiles
                = SettingsSpecificFiles = string.Empty;
            SettingsStat = SettingsSaved = true;
        }

        public GameProfile(int gameFileID, string name)
            : this()
        {
            GameFileID = gameFileID;
            Name = name;
        }

        public static void ApplyDefaultsToProfile(IGameProfile gameProfile, AppConfiguration appConfig)
        {
            if (!gameProfile.SourcePortID.HasValue)
                gameProfile.SourcePortID = (int)appConfig.GetTypedConfigValue(ConfigType.DefaultSourcePort, typeof(int));
            if (!gameProfile.IWadID.HasValue)
                gameProfile.IWadID = (int)appConfig.GetTypedConfigValue(ConfigType.DefaultIWad, typeof(int));
            if (string.IsNullOrEmpty(gameProfile.SettingsSkill))
                gameProfile.SettingsSkill = (string)appConfig.GetTypedConfigValue(ConfigType.DefaultSkill, typeof(string));
        }

        public int GameProfileID { get; set; }
        public int? GameFileID { get; set; }
        public int? IWadID { get; set; }
        public int? SourcePortID { get; set; }
        public string Name { get; set; }
        public string SettingsMap { get; set; }
        public string SettingsSkill { get; set; }
        public string SettingsExtraParams { get; set; }
        public string SettingsFiles { get; set; }
        public string SettingsFilesSourcePort { get; set; }
        public string SettingsFilesIWAD { get; set; }
        public string SettingsSpecificFiles { get; set; }
        public bool SettingsStat { get; set; }
        public bool SettingsSaved { get; set; }
    }
}
