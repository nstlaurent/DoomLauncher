using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public partial class StatsControl : UserControl
    {
        public StatsControl()
        {
            InitializeComponent();
            pbMaps.Image = DoomLauncher.Properties.Resources.map;
            pbItems.Image = DoomLauncher.Properties.Resources.bon2b;
            pbKills.Image = DoomLauncher.Properties.Resources.kill;
            pbSecrets.Image = DoomLauncher.Properties.Resources.secret;
        }

        public void SetStatistics(IGameFile gameFile, IEnumerable<IStatsData> stats)
        {
            SetStatistics(new IGameFile[] { gameFile }, stats);
        }

        public void SetStatistics(IEnumerable<IGameFile> gameFiles, IEnumerable<IStatsData> stats)
        {
            StatsData statTotal = new StatsData();

            foreach (IStatsData stat in stats)
            {
                statTotal.KillCount += stat.KillCount;
                statTotal.TotalKills += stat.TotalKills;
                statTotal.SecretCount += stat.SecretCount;
                statTotal.TotalSecrets += stat.TotalSecrets;
                statTotal.ItemCount += stat.ItemCount;
                statTotal.TotalItems += stat.TotalItems;
                statTotal.LevelTime += stat.LevelTime;
            }

            int maps = 0, totalMaps = 0;

            foreach(var gameFile in gameFiles)
            {
                if (gameFile.MapCount.HasValue)
                {
                    var gameFileStats = stats.Where(x => x.GameFileID == gameFile.GameFileID.Value);
                    int mapCount = gameFileStats.Select(x => x.MapName).Distinct().Count();
                    if (mapCount > gameFile.MapCount.Value)
                        mapCount = gameFile.MapCount.Value;
                    maps += mapCount;
                    totalMaps += gameFile.MapCount.Value;
                }
            }

            ctrlStatsMaps.SetStats(maps, totalMaps, string.Format("{0}/{1}", maps, totalMaps));
            ctrlStatsKills.SetStats(statTotal.KillCount, statTotal.TotalKills, statTotal.FormattedKills);
            ctrlStatsSecrets.SetStats(statTotal.SecretCount, statTotal.TotalSecrets, statTotal.FormattedSecrets);
            ctrlStatsItems.SetStats(statTotal.ItemCount, statTotal.TotalItems, statTotal.FormattedItems);
        }
    }
}
