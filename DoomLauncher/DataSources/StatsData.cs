using System;
using System.Globalization;

namespace DoomLauncher
{
    public class StatsData : IStatsData
    {
        public int KillCount { get; set; }
        public int TotalKills { get; set; }
        public int SecretCount { get; set; }
        public int TotalSecrets { get; set; }
        public float LevelTime { get; set; }
        public int ItemCount { get; set; }
        public int TotalItems { get; set; }
        public string MapName { get; set; }
        public DateTime RecordTime { get; set; }

        public string FormattedKills { get { return string.Format("{0} / {1}", KillCount.ToString("N0", CultureInfo.InvariantCulture), TotalKills.ToString("N0", CultureInfo.InvariantCulture)); } }
        public string FormattedSecrets { get { return string.Format("{0} / {1}", SecretCount.ToString("N0", CultureInfo.InvariantCulture), TotalSecrets.ToString("N0", CultureInfo.InvariantCulture)); } }
        public string FormattedItems { get { return string.Format("{0} / {1}", ItemCount.ToString("N0", CultureInfo.InvariantCulture), TotalItems.ToString("N0", CultureInfo.InvariantCulture)); } }
        public string FormattedTime
        {
            get
            {
                TimeSpan ts = TimeSpan.FromSeconds(LevelTime);
                return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}ms",
                                ts.Hours,
                                ts.Minutes,
                                ts.Seconds,
                                ts.Milliseconds);
            }
        }

        public int GameFileID { get; set; }
        public int SourcePortID { get; set; }
        public int StatID { get; set; }

        public string SaveFile { get; set; }

        public override bool Equals(object obj)
        {
            IStatsData stats = obj as IStatsData;

            if (stats != null)
            {
                return GameFileID == stats.GameFileID && KillCount == stats.KillCount && TotalKills == stats.TotalKills &&
                    SecretCount == stats.SecretCount && TotalSecrets == stats.TotalSecrets &&
                    ItemCount == stats.ItemCount && TotalItems == stats.TotalItems &&
                    LevelTime == stats.LevelTime &&
                    MapName.ToLower() == stats.MapName.ToLower();
            }

            return false;
        }

        public override int GetHashCode()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", GameFileID, KillCount, TotalKills, SecretCount, TotalSecrets, 
                ItemCount, TotalItems, LevelTime, MapName.ToLower()).GetHashCode();
        }
    }
}
