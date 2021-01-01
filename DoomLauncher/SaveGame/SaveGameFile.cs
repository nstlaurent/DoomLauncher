using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.SaveGame
{
    public class SaveGameFile : ISaveGameFile
    {
        public string MapName { get; set; }
        public string MapTitle { get; set; }
        public Stream Picture { get; set; }
        public int PlayerHealth { get; set; }
        public int PlayerArmor { get; set; }
        public DateTime? Timestamp { get; set; }
        public TimeSpan MapTime { get; set; }
        public TimeSpan? GameTime { get; set; }
        public IIWadData IWadData { get; set; }
        public ISourcePortData SourcePort { get; set; }
        public string SaveName { get; set; }
        public string Version { get; set; }
        public int SkillLevel { get; set; }
        public int GameEpisode { get; set; }
        public int ArmorType { get; set; }
        public int GameMap { get; set; }

        public string GetArmorTypeName()
        {
            return ArmorTypes[ArmorType];
        }

        public string GetSkillLevelName()
        {
            return SkillLevels[SkillLevel];
        }

        private static string[] ArmorTypes = {"None", "Green", "Blue"};
        private static string[] SkillLevels = { "I'm too young to die!", "Not too rough", "Hurt me plenty", "Ultra Violence", "Nightmare!" };
    }
}
