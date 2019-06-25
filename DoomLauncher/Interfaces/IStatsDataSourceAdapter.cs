using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface IStatsDataSourceAdapter
    {
        IEnumerable<IStatsData> GetStats();
        IEnumerable<IStatsData> GetStats(int gameFileID);
        void InsertStats(IStatsData stats);
        void UpdateStats(IStatsData stats);
        void DeleteStatsByFile(int gameFileID);
        void DeleteStats(int statID);
        void DeleteStats(ISourcePortData sourcePort);
    }
}
