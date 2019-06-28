using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DoomLauncher.Interfaces;

namespace DoomLauncher.Statistics
{
    public abstract class MultiLineStatReader : StatFileScanner, IStatisticsReader
    {
        public event NewStatisticsEventHandler NewStastics;

        private readonly List<IStatsData> m_statistics = new List<IStatsData>();
        private readonly ParseItem[] m_parseItems;
        private readonly string m_statRegex;

        protected MultiLineStatReader(IGameFile gameFile, string statFile, string statRegex, ParseItem[] parseItems) : base(statFile)
        {
            GameFile = gameFile;
            m_parseItems = parseItems;
            m_statRegex = statRegex;
            ErrorOnFail = false;
            RemoveNewLines = true;
        }

        public IGameFile GameFile { get; set; }
        public bool ReadOnClose { get { return true; } }
        public bool ErrorOnFail { get; set; }
        public bool RemoveNewLines { get; set; }
        public abstract string LaunchParameter { get; }

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
                string text = File.ReadAllText(StatFile);
                text = text.Replace(" ", string.Empty).Replace("\t", string.Empty);

                if (RemoveNewLines)
                    text = text.Replace("\r\n", string.Empty).Replace("\n", string.Empty);
                else
                    text = text.Replace("\r\n", "\n");

                MatchCollection matches = Regex.Matches(text, m_statRegex, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    IStatsData stats = null;

                    if (match.Success)
                        stats = ParseLine(match.Value);

                    if (stats != null)
                    {
                        stats.RecordTime = DateTime.Now;
                        stats.GameFileID = GameFile.GameFileID.Value;

                        //Revived monsters adds to kill count
                        if (stats.KillCount > stats.TotalKills)
                            stats.KillCount = stats.TotalKills;

                        if (!m_statistics.Contains(stats))
                        {
                            m_statistics.Add(stats);
                            NewStastics?.Invoke(this, new NewStatisticsEventArgs(stats));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ErrorOnFail)
                    m_errors.Add(string.Format("Unexpected exception: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));
            }
        }

        private IStatsData ParseLine(string line)
        {
            StatsData ret = new StatsData();

            foreach (ParseItem item in m_parseItems)
            {
                Match match = Regex.Match(line, item.RegexInput);

                if (match.Success)
                {
                    line = ReplaceFirst(line, match.Value);
                    if (item.DataSourceProperty != null)
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
