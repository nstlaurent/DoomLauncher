using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public interface IStatsData
    {
        int KillCount { get; set; }
        int TotalKills { get; set; }
        int SecretCount { get; set; }
        int TotalSecrets { get; set; }
        float LevelTime { get; set; }
        int ItemCount { get; set; }
        int TotalItems { get; set; }
        string MapName { get; set; }
        DateTime RecordTime { get; set; }

        string FormattedKills { get; }
        string FormattedSecrets { get; }
        string FormattedItems { get; }
        string FormattedTime { get; }

        int GameFileID { get; set; }
        int SourcePortID { get; set; }
        int StatID { get; set; }
    }
}
