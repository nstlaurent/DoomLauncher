using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DoomLauncher
{
    public class CNDoomStatsReader : StatFileScanner, IStatisticsReader
    {
        public event NewStatisticsEventHandler NewStastics;

        private readonly List<IStatsData> m_statistics = new List<IStatsData>();

        public CNDoomStatsReader(IGameFile gameFile, string stdoutFile) : base(stdoutFile)
        {
            GameFile = gameFile;
        }

        public IGameFile GameFile { get; set; }

        public string LaunchParameter { get { return "-printstats"; } }
        public bool ReadOnClose { get { return true; } }

        public void Start()
        {
            //nothing to do on start
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

        private static ParseItem[] s_regexItems = new ParseItem[]
        {
            new ParseItem(@"\w+", string.Empty, "MapName"),
            new ParseItem(@"\d+:\d+.\d+", string.Empty, "LevelTime"),
            new ParseItem(@":\d+", ":", "KillCount"),
            new ParseItem(@"/\d+", "/", "TotalKills"),
            new ParseItem(@":\d+", ":", "ItemCount"),
            new ParseItem(@"/\d+", "/", "TotalItems"),
            new ParseItem(@":\d+", ":", "SecretCount"),
            new ParseItem(@"/\d+", "/", "TotalSecrets")
        };

        private void ReadStatistics()
        {
            try
            {
                string statRegex = @"#+\w+#+Time:\d+:\d+.\d+Kills:\d+/\d+#+Items:\d+/\d+Secrets:\d+/\d+";
                string text = File.ReadAllText(StatFile);
                text = text.Replace(" ", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty);
                MatchCollection matches = Regex.Matches(text, statRegex, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    IStatsData stats = null;

                    if (match.Success)
                        stats = ParseLine(match.Value);

                    if (stats != null)
                    {
                        stats.RecordTime = DateTime.Now;
                        stats.GameFileID = GameFile.GameFileID.Value;

                        if (!m_statistics.Contains(stats))
                        {
                            m_statistics.Add(stats);

                            if (NewStastics != null)
                                NewStastics(this, new NewStatisticsEventArgs(stats));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_errors.Add(string.Format("Unexpected exception: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));
            }
        }

        private IStatsData ParseLine(string line)
        {
            StatsData ret = new StatsData();

            foreach (ParseItem item in s_regexItems)
            {
                Match match = Regex.Match(line, item.RegexInput);

                if (match.Success)
                {
                    line = ReplaceFirst(line, match.Value);
                    SetStatProperty(ret, item, match.Value);
                }
                else
                {
                    m_errors.Add(string.Format("Failed to parse {0} from levelstat file.", item.DataSourceProperty));
                }
            }

            return ret;
        }

        private static string ReplaceFirst(string text, string oldValue)
        {
            int idx = text.IndexOf(oldValue);

            if (idx != -1)
                return string.Concat(text.Substring(0, idx), text.Substring(idx + oldValue.Length));

            return text;
        }
    }
}
