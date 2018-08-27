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

        struct LevelStats
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
        private readonly List<IStatsData> m_statistics = new List<IStatsData>();
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
            catch(FileNotFoundException ex)
            {
                m_errors.Add(string.Format("The file {0} was not found and could not be parsed.", ex.FileName));
            }
            catch(Exception ex)
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
                            foreach(var stat in stats)
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
                    foreach (JObject level in levels)
                    {
                        uint totalkills = Convert.ToUInt32(level.GetValue("totalkills"));
                        uint kills = Convert.ToUInt32(level.GetValue("killcount"));

                        uint totalsecrets = Convert.ToUInt32(level.GetValue("totalsecrets"));
                        uint secrets = Convert.ToUInt32(level.GetValue("secretcount"));

                        uint totalitems = Convert.ToUInt32(level.GetValue("totalitems"));
                        uint items = Convert.ToUInt32(level.GetValue("itemcount"));

                        uint time = Convert.ToUInt32(level.GetValue("leveltime"));
                        string name = level.GetValue("levelname").ToString();
                        stats.Add(CreateStatsDataSource(totalkills, kills, totalsecrets, secrets, time, name));
                    }
                }
            }

            return stats;
        }

        private void ParseBinary(string file)
        {
            bool success = false;
            string magicID = "sTat";
            byte[] check = new byte[magicID.Length];
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(file));
            int position = 0;

            while (position + magicID.Length < ms.Length)
            {
                ms.Read(check, 0, check.Length);
                string text = Encoding.ASCII.GetString(check);

                if (text.Equals(magicID))
                {
                    ReadStatistics(ms);
                    success = true;
                    break;
                }

                ms.Position = ++position;
            }

            if (!success)
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
                StatsData statsData = CreateStatsDataSource(stats.totalkills, stats.killcount, stats.totalsecrets, stats.secretcount, stats.leveltime, levelName);
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

        private static int s_endianCheck = 0x0000FFFF;//if bytes are set on the other side, then it must be stored backwards

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

        private static StatsData CreateStatsDataSource(UInt32 totalkills, UInt32 killcount, UInt32 totalsecrets, UInt32 secretcount, UInt32 leveltime, string name)
        {
            float calctime = Convert.ToSingle(leveltime) / 35.0f;
            StatsData stats = new StatsData();
            stats.RecordTime = DateTime.Now;
            stats.TotalKills = (int)totalkills;
            stats.KillCount = (int)killcount;
            stats.TotalSecrets = (int)totalsecrets;
            stats.SecretCount = (int)secretcount;
            stats.LevelTime = calctime;
            stats.MapName = name;

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
