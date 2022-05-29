using System.Collections.Generic;

namespace DoomLauncher.Interfaces
{
    public interface IStatsDataSourceAdapter
    {
        IEnumerable<IStatsData> GetStats();
        IEnumerable<IStatsData> GetStats(int gameFileID);
        IEnumerable<IStatsData> GetStats(IEnumerable<IGameFile> gameFiles);
        void InsertStats(IStatsData stats);
        void UpdateStats(IStatsData stats);
        void DeleteStatsByFile(int gameFileID);
        void DeleteStats(int statID);
        void DeleteStats(ISourcePortData sourcePort);
    }
}
