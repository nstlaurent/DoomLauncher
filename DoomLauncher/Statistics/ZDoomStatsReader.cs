using DoomLauncher.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

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
        private NewFileDetector m_detector;
        private Timer m_checkTimer;
        private readonly List<IStatsData> m_statistics;
        private readonly List<string> m_errors = new List<string>();

        public ZDoomStatsReader(IGameFile gameFile, string directory, IEnumerable<IStatsData> existingStats)
        {
            m_dir = directory;
            m_detector = new NewFileDetector(new string[] { ".zds" }, directory, true);
            m_statistics = existingStats.ToList();
            GameFile = gameFile;
        }

        public IGameFile GameFile { get; set; }

        public static bool Supported(ISourcePort sourcePort)
        {
            string exe = sourcePort.Executable.ToLower();

            if (exe.Contains("zdoom.exe"))
                return true;
            if (exe.Contains("zandronum.exe"))
                return true;

            return false;
        }

        public static IStatisticsReader CreateDefault(IGameFile gameFile, string directory, IEnumerable<IStatsData> existingStats)
        {
            return new ZDoomStatsReader(gameFile, directory, existingStats);
        }

        public string LaunchParameter { get { return string.Empty; } }
        public bool ReadOnClose { get { return false; } }

        public void Start()
        {
            if (m_checkTimer == null)
            {
                m_detector.StartDetection();
                m_checkTimer = new Timer(1000);
                m_checkTimer.Elapsed += m_checkTimer_Elapsed;
                m_checkTimer.Start();
            }
        }

        public void Stop()
        {
            if (m_checkTimer != null)
            {
                m_checkTimer.Stop();
            }
        }

        public void ReadNow()
        {
            throw new NotSupportedException();
        }

        public string[] Errors
        {
            get { return m_errors.ToArray(); }
        }

        void m_checkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_checkTimer.Stop();

            string[] files = m_detector.GetNewFiles().Union(m_detector.GetModifiedFiles()).ToArray();

            if (files.Length > 0)
            {
                Array.ForEach(files, x => HandleSaveFile(x));

                m_detector = new NewFileDetector(new string[] { ".zds" }, m_dir, true);
                m_detector.StartDetection();
            }

            m_checkTimer.Start();
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
            lock (m_detector)
            {
                //if we fail to read json (new save format) attempt to read binary (old save format)
                if (!ParseJson(file))
                    ParseBinary(file);
            }
        }

        private bool ParseJson(string file)
        {
            bool success = false;
            try
            {
                using (ZipArchive za = ZipFile.OpenRead(file))
                {
                    var entry = za.Entries.FirstOrDefault(x => x.Name.Equals("globals.json"));

                    if (entry != null)
                    {
                        List<StatsData> stats = null;
                        using (var stream = new StreamReader(entry.Open()))
                            stats = ParseJsonStats(stream);

                        if (stats != null)
                        {
                            foreach (var stat in stats)
                                HandleStatsData(stat);
                            success = true;
                        }
                    }
                }
            }
            catch
            {
                //nothing to do
            }

            return success;
        }

        private static List<StatsData> ParseJsonStats(StreamReader stream)
        {
            List<StatsData> stats = new List<StatsData>();
            JObject obj = JsonConvert.DeserializeObject(stream.ReadToEnd()) as JObject;

            var statsource = obj.GetValue("statistics") as JObject;

            if (statsource != null)
            {
                var levels = statsource.GetValue("levels");

                if (levels != null)
                {
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
                        stats.Add(CreateJsonStatsDataSource(totalkills, kills, totalitems, items, totalsecrets, secrets, time, name));
                    }
                }
            }

            return stats;
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
            UInt32 leveltime, string name)
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
                MapName = name
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
