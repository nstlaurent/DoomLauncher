using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace DoomLauncher
{
    public class BoomStatsReader : StatFileScanner, IStatisticsReader
    {
        public event NewStatisticsEventHandler NewStastics;

        private readonly List<IStatsData> m_statistics = new List<IStatsData>();

        public BoomStatsReader(IGameFile gameFile, string statFile) : base(statFile)
        {
            GameFile = gameFile;
        }

        public IGameFile GameFile { get; set; }

        public string LaunchParameter { get { return "-levelstat"; } }
        public bool ReadOnClose { get { return true; } }

        public void Start()
        {
            try
            {
                FileInfo fi = new FileInfo(StatFile);
                if (fi.Exists)
                    fi.Delete();
            }
            catch
            {
                //failed, nothing to do
            }
        }

        public void Stop()
        {
            //nothing to do on stop
        }

        public void ReadNow()
        {
            ReadStatistics();
        }

        public string[] Errors
        {
            get { return m_errors.ToArray(); }
        }

        private void ReadStatistics()
        {
            try
            {
                IEnumerable<string> lines = from line in File.ReadLines(StatFile)
                                            where !string.IsNullOrEmpty(line)
                                            select line;

                foreach (string line in lines)
                {
                    IStatsData stats = ParseLine(line.Trim());
                    stats.RecordTime = DateTime.Now;
                    stats.GameFileID = GameFile.GameFileID.Value;

                    if (!m_statistics.Contains(stats))
                    {
                        m_statistics.Add(stats);
                        NewStastics?.Invoke(this, new NewStatisticsEventArgs(stats));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                //m_errors.Add(string.Format("The file {0} was not found and could not be parsed.", ex.FileName));
            }
        }

        private static ParseItem[] s_regexItems = new ParseItem[]
        {
            new ParseItem(@"^\S+-", "-", "MapName"),
            new ParseItem(@"-\S+:\S+\(", "-(", "LevelTime"),
            new ParseItem(@"K:\d+/", "K:/", "KillCount"),
            new ParseItem(@"/\d+I:", "I:/", "TotalKills"),
            new ParseItem(@"I:\d+/", "I:/", "ItemCount"),
            new ParseItem(@"/\d+S:", "S:/", "TotalItems"),
            new ParseItem(@"S:\d+/", "S:/", "SecretCount"),
            new ParseItem(@"/\d+$", "/", "TotalSecrets")
        };

        private IStatsData ParseLine(string line)
        {
            line = line.Replace(" ", string.Empty);
            StatsData ret = new StatsData();

            foreach (ParseItem item in s_regexItems)
            {
                Match match = Regex.Match(line, item.RegexInput);

                if (match.Success)
                    SetStatProperty(ret, item, match.Value);
                else
                    m_errors.Add(string.Format("Failed to parse {0} from levelstat file.", item.DataSourceProperty));
            }

            return ret;
        }
    }
}
