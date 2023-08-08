using DoomLauncher.Interfaces;
using System;
using System.IO;

namespace DoomLauncher.DataSources
{
    public class GameFile : IGameFile, IGameProfile, ICloneable
    {
        public static string[] GetMaps(IGameFile gameFile) => gameFile.Map.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);

        private string m_lastDirectory = null;

        public GameFile()
        {
            FileName = Title = Author = Description = Thumbnail = Comments = Map = SettingsMap = SettingsSkill = SettingsExtraParams = SettingsFiles
                = SettingsSpecificFiles = string.Empty;
            Name = "Default Profile";
            SettingsStat = true;
            GameProfileID = -1;
        }

        public int? GameFileID { get; set; }
        public string FullFileName { get; set; }
        public virtual string FileName { get; set; }
        public string FileNameNoPath => Path.GetFileName(FileName);
        public virtual string LastDirectory => GetLastDirectory(FileName);
        public virtual string Title { get; set; }
        public virtual string Author { get; set; }
        public virtual DateTime? ReleaseDate { get; set; }
        public virtual string Description { get; set; }
        public int? IWadID { get; set; }
        public int? SourcePortID { get; set; }
        public string Thumbnail { get; set; }
        public string Comments { get; set; }
        public string Map { get; set; }
        public int? MapCount { get; set; }
        public virtual double? Rating { get; set; }
        public DateTime? LastPlayed { get; set; }
        public DateTime? Downloaded { get; set; }

        public int GameProfileID { get; set; }
        public string Name { get; set; }
        public string SettingsMap { get; set; }
        public string SettingsSkill { get; set; }
        public string SettingsExtraParams { get; set; }
        public string SettingsFiles { get; set; }
        public string SettingsFilesSourcePort { get; set; }
        public string SettingsFilesIWAD { get; set; }
        public string SettingsSpecificFiles { get; set; }
        public bool SettingsStat { get; set; }
        public bool SettingsLoadLatestSave { get; set; }
        public bool SettingsSaved { get; set; }
        public bool SettingsExtraParamsOnly { get; set; }
        public int? SettingsGameProfileID { get; set; }
        public bool IsGlobal => false;

        public int MinutesPlayed { get; set; }
        public virtual int FileSizeBytes { get; set; }

        public bool IsUnmanaged() => Path.IsPathRooted(FileName);

        public bool IsDirectory()
        {
            if (!IsUnmanaged())
                return false;

            return Util.IsDirectory(FileName);
        }

        public object Clone()
        {
            GameFile gameFile = new GameFile();
            var properties = gameFile.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetSetMethod() != null)
                    prop.SetValue(gameFile, prop.GetValue(this));
            }

            return gameFile;
        }

        public override bool Equals(object obj)
        {
            if (obj is IGameFile gameFile)
                return gameFile.FileName == FileName;

            return false;
        }

        public override int GetHashCode()
        {
            if (FileName != null)
                return FileName.GetHashCode();

            return 0;
        }

        public override string ToString()
        {
            if (FileName == null)
                return string.Empty;
            return FileName;
        }

        private string GetLastDirectory(string path)
        {
            if (m_lastDirectory != null)
                return m_lastDirectory;

            if (!IsUnmanaged())
            {
                m_lastDirectory = string.Empty;
                return m_lastDirectory;
            }

            string dir = Path.GetDirectoryName(FileName);
            m_lastDirectory = new DirectoryInfo(dir).Name;
            return m_lastDirectory;
        }
    }
}
