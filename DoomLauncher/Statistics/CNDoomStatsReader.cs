using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public static bool Supported(ISourcePort sourcePort)
        {
            string exe = sourcePort.Executable.ToLower();
            return exe.Contains("cndoom.exe");
        }

        public static CNDoomStatsReader CreateDefault(IGameFile gameFile, string directory)
        {
            return new CNDoomStatsReader(gameFile, Path.Combine(directory, "stdout.txt"));
        }

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

        public void Test()
        {
            ReadStatistics();
        }

        private static readonly ParseItem[] s_regexItems = new ParseItem[]
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
                text = text.Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
                MatchCollection matches = Regex.Matches(text, string.Format(statRegex), RegexOptions.Singleline);

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

                            NewStastics?.Invoke(this, new NewStatisticsEventArgs(stats));
                        }
                    }
                }

                //if (matches.Count == 0)
                //    m_errors.Add(string.Format("The file {0} did not contain any statistics information.", StatFile));
            }
            catch(FileNotFoundException)
            {
                //m_errors.Add(string.Format("The file {0} was not found and could not be parsed.", ex.FileName));
            }
            catch(Exception ex)
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
