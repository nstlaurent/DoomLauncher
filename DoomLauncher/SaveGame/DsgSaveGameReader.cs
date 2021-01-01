using DoomLauncher.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DoomLauncher.SaveGame
{
    public class DsgSaveGameReader : ISaveGameReader
    {
        private readonly string m_file;

        public DsgSaveGameReader(string file)
        {
            m_file = file;
        }

        public string GetName()
        {
            try
            {
                using (var stream = File.OpenRead(m_file))
                {
                    byte[] data = new byte[24];
                    int read = stream.Read(data, 0, data.Length);

                    if (read != -1)
                    {
                        return Util.GetTrimmedNulTerminatedString(data);
                    }
                }
            }
            catch
            {
                //continue to return the filename if we fail for any reason
            }

            return Path.GetFileName(m_file);
        }

        public ISaveGameFile GetInfoFromFile(IIWadData iWadData, ISourcePortData sourcePort)
        {
            using (var stream = File.OpenRead(m_file))
            {
                int offset = 0x18;
                string saveName = GetName();
                stream.Seek(offset, SeekOrigin.Begin);
                var versionData = new byte[16];
                var hasReadVersionData = stream.Read(versionData, 0, versionData.Length);
                string version = null;
                if (hasReadVersionData != -1)
                {
                    version = Util.GetTrimmedNulTerminatedString(versionData);
                }
                offset += 16;
                var gameSkill = stream.ReadByte();
                var gameEpisode = stream.ReadByte();
                var gameMap = stream.ReadByte();

                //Ignoring the "PlayerInGame" array, at least for now.
                offset = 0x2f;
                stream.Seek(offset, SeekOrigin.Begin);
                byte[] levelTimeTics = new byte[4];
                stream.Read(levelTimeTics, 0, 3);
               
                for (int i = 3; i >= 1; i--)
                {
                    levelTimeTics[i] = levelTimeTics[i - 1];
                }
                // Since the DSG format saves the number of tics as a little-endian, three byte number, we need to reverse the array to get the correct number of tics.
                levelTimeTics[0] = 0;
                Array.Reverse(levelTimeTics);

                int levelTimeTicsCount = BitConverter.ToInt32(levelTimeTics, 0);
                TimeSpan levelTime = TimeSpan.FromSeconds(levelTimeTicsCount / 35);

                offset = 0x52;
                stream.Seek(offset, SeekOrigin.Begin);
                var playerHealth = stream.ReadByte();
                var armorPoints = stream.ReadByte();
                var armorType = stream.ReadByte();
                var lastSaveDate = File.GetLastWriteTime(m_file);


                ISaveGameFile saveGame = new SaveGameFile()
                {
                    MapName = null,
                    MapTitle = null,
                    Picture = null,
                    PlayerHealth = playerHealth,
                    PlayerArmor = armorPoints,
                    Timestamp = lastSaveDate,
                    MapTime = levelTime,
                    GameTime = null,
                    IWadData = iWadData,
                    SourcePort = sourcePort,
                    SaveName = saveName,
                    Version = version,
                    SkillLevel = gameSkill,
                    GameEpisode = gameEpisode,
                    ArmorType = armorType,
                    GameMap = gameMap
                };

                return saveGame;
            }
        }
    }
}
