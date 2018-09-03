using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.DataSources
{
    public class GameFile : IGameFile
    {
        public GameFile()
        {
            FileName = Title = Author = Description = Thumbnail = Comments = Map = SettingsMap = SettingsSkill = SettingsExtraParams = SettingsFiles
                = SettingsSpecificFiles = string.Empty;
            SettingsStat = true;
        }

        public int? GameFileID { get; set; }
        public virtual string FileName { get; set; }
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

        public string SettingsMap { get; set; }
        public string SettingsSkill { get; set; }
        public string SettingsExtraParams { get; set; }
        public string SettingsFiles { get; set; }
        public string SettingsFilesSourcePort { get; set; }
        public string SettingsFilesIWAD { get; set; }
        public string SettingsSpecificFiles { get; set; }
        public bool SettingsStat { get; set; }

        public int MinutesPlayed { get; set; }
        public virtual int FileSizeBytes { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is IGameFile)
            {
                return ((IGameFile)obj).FileName == FileName;
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (FileName != null)
                return FileName.GetHashCode();

            return 0;
        }
    }
}
