using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class StatsControl : UserControl
    {
        public StatsControl()
        {
            InitializeComponent();
            DpiScale dpiScale = new DpiScale(CreateGraphics());

            SetImage(pbMaps, Properties.Resources.map, 0.7f * dpiScale.DpiScaleY);
            SetImage(pbKills, Properties.Resources.kill, 0.7f * dpiScale.DpiScaleY);
            SetImage(pbItems, Properties.Resources.bon2b, 0.8f * dpiScale.DpiScaleY);
            SetImage(pbSecrets, Properties.Resources.secret, 0.8f * dpiScale.DpiScaleY);

            SetStatBarDpi(ctrlStatsMaps, dpiScale);
            SetStatBarDpi(ctrlStatsKills, dpiScale);
            SetStatBarDpi(ctrlStatsSecrets, dpiScale);
            SetStatBarDpi(ctrlStatsItems, dpiScale);
        }

        private void SetStatBarDpi(StatBar statBar, DpiScale dpiScale)
        {
            statBar.MaximumSize = new Size(statBar.MaximumSize.Width, dpiScale.ScaleIntY(statBar.MaximumSize.Height));
            statBar.SetMaxHeight(statBar.MaximumSize.Height);
            statBar.Size = MaximumSize;
        }

        private void SetImage(PictureBox pb, Bitmap bmp, float scale)
        {
            pb.Width = Convert.ToInt32(bmp.Width * scale);
            pb.Height = Convert.ToInt32(bmp.Height * scale);
            pb.Image = bmp;
        }

        public void SetStatistics(IGameFile gameFile, IEnumerable<IStatsData> stats)
        {
            SetStatistics(new IGameFile[] { gameFile }, stats);
        }

        public void SetStatistics(IEnumerable<IGameFile> gameFiles, IEnumerable<IStatsData> stats)
        {
            StatsData statTotal = new StatsData();
            int maps = 0, totalMaps = 0;
            var groupedStats = stats.OrderByDescending(x => x.RecordTime).GroupBy(x => x.GameFileID);

            foreach (var group in groupedStats)
            {
                var gameFile = gameFiles.FirstOrDefault(x => x.GameFileID.Value == group.Key);

                if (gameFile != null)
                {
                    var mapStats = group.GroupBy(x => x.MapName).Select(x => x.First());

                    if (gameFile.MapCount.HasValue)
                    {
                        int mapCount = mapStats.Count();
                        if (mapCount > gameFile.MapCount.Value)
                            mapCount = gameFile.MapCount.Value;
                        maps += mapCount;
                        totalMaps += gameFile.MapCount.Value;
                    }

                    foreach (var stat in mapStats)
                    {
                        statTotal.KillCount += stat.KillCount;
                        statTotal.TotalKills += stat.TotalKills;
                        statTotal.SecretCount += stat.SecretCount;
                        statTotal.TotalSecrets += stat.TotalSecrets;
                        statTotal.ItemCount += stat.ItemCount;
                        statTotal.TotalItems += stat.TotalItems;
                        statTotal.LevelTime += stat.LevelTime;
                    }
                }
            }

            ctrlStatsMaps.SetStats(maps, totalMaps, string.Format("{0} / {1}", maps, totalMaps));
            ctrlStatsKills.SetStats(statTotal.KillCount, statTotal.TotalKills, statTotal.FormattedKills);
            ctrlStatsSecrets.SetStats(statTotal.SecretCount, statTotal.TotalSecrets, statTotal.FormattedSecrets);
            ctrlStatsItems.SetStats(statTotal.ItemCount, statTotal.TotalItems, statTotal.FormattedItems);
        }
    }
}
