using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DoomLauncher
{
    public class ZDoomStatsReader : IStatisticsReader
    {
        public event NewStatisticsEventHandler NewStastics;

        struct LevelCount
        {
            public UInt32 levelcount;
        }

        struct LevelStats //item stats excluded for binary (old save format) compatibility
        {
            public UInt32 totalkills;
            public UInt32 killcount;
            public UInt32 totalsecrets;
            public UInt32 secretcount;
            public UInt32 leveltime;
        }

        private readonly string m_dir;
        private List<NewFileDetector> m_detectors = new List<NewFileDetector>();
        private readonly List<IStatsData> m_statistics;
        private readonly List<string> m_errors = new List<string>();

        public ZDoomStatsReader(IGameFile gameFile, string directory, IEnumerable<IStatsData> existingStats)
        {
            m_dir = directory;
            m_statistics = existingStats.ToList();
            GameFile = gameFile;
        }

        public IGameFile GameFile { get; set; }

        public string LaunchParameter { get { return string.Empty; } }
        public bool ReadOnClose { get { return true; } }

        public void Start()
        {
            m_detectors.Clear();
            string[] zdsExtensions = new string[] { ".zds" };
            m_detectors.Add(new NewFileDetector(zdsExtensions, m_dir, true));

            string userDir = ZDoomSourcePort.UserSaveGameDirectory;
            if (Directory.Exists(userDir))
                m_detectors.Add(new NewFileDetector(zdsExtensions, userDir, true));
            
            m_detectors.ForEach(x => x.StartDetection());
        }

        public void Stop()
        {
            // Nothing to do
        }

        public void ReadNow()
        {
            if (m_detectors.Count == 0)
                return;

            List<string> files = new List<string>();
            foreach (var detector in m_detectors)
            {
                files.AddRange(detector.GetNewFiles());
                files.AddRange(detector.GetModifiedFiles());
            }

            if (files.Count > 0)
                files.ForEach(x => HandleSaveFile(x));
        }

        public string[] Errors
        {
            get { return m_errors.ToArray(); }
        }

        private void HandleSaveFile(string file)
        {
            try
            {
                try
                {
                    ReadSaveFile(file);
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(200);
                    ReadSaveFile(file); //try one more time...
                }
            }
            catch (FileNotFoundException ex)
            {
                m_errors.Add(string.Format("The file {0} was not found and could not be parsed.", ex.FileName));
            }
            catch (Exception ex)
            {
                m_errors.Add(string.Format("An unexpected error occurred with {0}: {1} {2} {3}",
                    file, ex.Message, Environment.NewLine, ex.StackTrace));
            }
        }

        private void ReadSaveFile(string file)
        {
            lock (m_detectors)
            {
                //if we fail to read json (new save format) attempt to read binary (old save format)
                if (!ParseJson(file))
                    ParseBinary(file);
            }
        }

        private bool ParseJson(string file)
        {
            List<StatsData> stats = null;
            bool success = false;
            try
            {
                using (ZipArchive za = ZipFile.OpenRead(file))
                {
                    var entry = za.Entries.FirstOrDefault(x => x.Name.Equals("globals.json"));
                    if (entry != null)
                    {
                        using (var stream = new StreamReader(entry.Open()))
                            stats = ParseJsonStats(stream);
                    }
                }
            }
            catch
            {
                //nothing to do
            }

            if (stats != null)
            {
                foreach (var stat in stats)
                    HandleStatsData(stat);
                success = true;
            }

            return success;
        }

        private static List<StatsData> ParseJsonStats(StreamReader stream)
        {
            List<StatsData> stats = new List<StatsData>();
            JObject obj = JsonConvert.DeserializeObject(stream.ReadToEnd()) as JObject;

            var statsource = obj.GetValue("statistics") as JObject;
            if (statsource == null)
                return stats;
                
            var levels = statsource.GetValue("levels");
            if (levels == null)
                return stats;

            int? skill = GetSkill(obj);            
            foreach (JObject level in levels.OfType<JObject>())
            {
                uint totalkills = Convert.ToUInt32(level.GetValue("totalkills"));
                uint kills = Convert.ToUInt32(level.GetValue("killcount"));

                //item stats are only included in json (new save format), so they're converted to UInt32 here
                uint totalitems = Convert.ToUInt32(level.GetValue("totalitems"));
                uint items = Convert.ToUInt32(level.GetValue("itemcount"));

                uint totalsecrets = Convert.ToUInt32(level.GetValue("totalsecrets"));
                uint secrets = Convert.ToUInt32(level.GetValue("secretcount"));

                uint time = Convert.ToUInt32(level.GetValue("leveltime"));
                string name = level.GetValue("levelname").ToString();
                stats.Add(CreateJsonStatsDataSource(totalkills, kills, totalitems, items, totalsecrets, secrets, time, name, skill));
            }            

            return stats;
        }

        private static int? GetSkill(JObject obj)
        {
            var cvars = obj.GetValue("importantcvars");
            if (cvars != null)
                return GetSKillByImportCvars(cvars);

            if (obj.GetValue("servercvars") is JObject objectCvars)
                return GetSkillByServerCvars(objectCvars);

            return null;
        }

        private static int? GetSkillByServerCvars(JObject cvars)
        {
            var skill = cvars.GetValue("skill");
            if (skill == null)
                return null;

            if (int.TryParse(skill.ToString(), out int skillValue))
                return skillValue + 1;

            return null;
        }

        private static int? GetSKillByImportCvars(JToken cvars)
        {
            string[] stringCvars = cvars.ToString().Split(new char[] { '\\' });
            int index = Array.FindIndex(stringCvars, x => x.Equals("skill", StringComparison.OrdinalIgnoreCase));
            if (index == -1 || index + 1 >= stringCvars.Length)
                return null;

            if (int.TryParse(stringCvars[index + 1], out int parsedSkill))
                return parsedSkill + 1;

            return null;
        }

        private void ParseBinary(string file)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(file));
            long position = Util.ReadAfter(ms, Encoding.UTF8.GetBytes("sTat"));

            if (position != -1)
                ReadStatistics(ms);
            else
                m_errors.Add(string.Format("Unable to find statistics in the save file {0}.", file));
        }

        private void ReadStatistics(MemoryStream ms)
        {
            LevelCount count = ReadStuctureFromFile<LevelCount>(ms);
            count = CheckLevelCount(count);
            int stringLength = ReadCount(ms);
            ms.Position += stringLength - 1; //This part MAY contain the start episode map, just skip it

            for (int i = 0; i < count.levelcount; i++)
            {
                LevelStats stats = ReadStuctureFromFile<LevelStats>(ms);
                stats = CheckStats(stats);

                ms.Position += 1; //skip NEW_NAME (27)
                stringLength = ReadCount(ms) - 1;
                byte[] levelNameBytes = new byte[stringLength];
                ms.Read(levelNameBytes, 0, levelNameBytes.Length);

                string levelName = Encoding.ASCII.GetString(levelNameBytes).ToLower();
                StatsData statsData = CreateBinaryStatsDataSource(stats.totalkills, stats.killcount, stats.totalsecrets, stats.secretcount, stats.leveltime, levelName);
                HandleStatsData(statsData);
            }
        }

        private void HandleStatsData(StatsData statsData)
        {
            statsData.GameFileID = GameFile.GameFileID.Value;

            if (!m_statistics.Contains(statsData))
            {
                m_statistics.Add(statsData);
                NewStastics?.Invoke(this, new NewStatisticsEventArgs(statsData));
            }
        }

        private static int s_endianCheck = 0x0000FFFF; //if bytes are set on the other side, then it must be stored backwards

        private LevelCount CheckLevelCount(LevelCount count)
        {
            if (count.levelcount > s_endianCheck)
                count.levelcount = ReverseBytes(count.levelcount);

            return count;
        }

        private LevelStats CheckStats(LevelStats stats)
        {
            if (stats.killcount > s_endianCheck)
                stats.killcount = ReverseBytes(stats.killcount);
            if (stats.leveltime > s_endianCheck)
                stats.leveltime = ReverseBytes(stats.leveltime);
            if (stats.secretcount > s_endianCheck)
                stats.secretcount = ReverseBytes(stats.secretcount);
            if (stats.totalkills > s_endianCheck)
                stats.totalkills = ReverseBytes(stats.totalkills);
            if (stats.totalsecrets > s_endianCheck)
                stats.totalsecrets = ReverseBytes(stats.totalsecrets);
            return stats;
        }

        //separate json and binary to avoid stat overflow with how binary (old save format) is read
        private static StatsData CreateJsonStatsDataSource(UInt32 totalkills, UInt32 killcount, UInt32 totalitems, UInt32 itemcount, UInt32 totalsecrets, UInt32 secretcount,
            UInt32 leveltime, string name, int? skill)
        {
            float calctime = Convert.ToSingle(leveltime) / 35.0f;
            StatsData stats = new StatsData
            {
                RecordTime = DateTime.Now,
                TotalKills = (int)totalkills,
                KillCount = (int)killcount,
                TotalItems = (int)totalitems,
                ItemCount = (int)itemcount,
                TotalSecrets = (int)totalsecrets,
                SecretCount = (int)secretcount,
                LevelTime = calctime,
                MapName = name,
                Skill = skill
            };

            return stats;
        }

        private static StatsData CreateBinaryStatsDataSource(UInt32 totalkills, UInt32 killcount, UInt32 totalsecrets, UInt32 secretcount, UInt32 leveltime, string name)
        {
            float calctime = Convert.ToSingle(leveltime) / 35.0f;
            StatsData stats = new StatsData
            {
                RecordTime = DateTime.Now,
                TotalKills = (int)totalkills,
                KillCount = (int)killcount,
                TotalSecrets = (int)totalsecrets,
                SecretCount = (int)secretcount,
                LevelTime = calctime,
                MapName = name
            };

            return stats;
        }

        private static int ReadCount(MemoryStream ms)
        {
            byte[] len = new byte[1];
            int count = 0;
            int ofs = 0;

            do
            {
                ms.Read(len, 0, 1);
                count |= (len[0] & 0x7f) << ofs;
                ofs += 7;
            } while ((len[0] & 0x80) != 0);

            return count;
        }

        private static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        private static T ReadStuctureFromFile<T>(MemoryStream ms)
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
            ms.Read(bytes, 0, bytes.Length);

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T obj = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return obj;
        }
    }
}
