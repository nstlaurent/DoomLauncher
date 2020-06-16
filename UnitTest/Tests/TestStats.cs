using DoomLauncher;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestStats
    {
        private List<IStatsData> CreateStatsData()
        {
            List<IStatsData> stats = new List<IStatsData>();
            for (int i = 0; i < 10; i++)
            {
                stats.Add(
                    new StatsData()
                    {
                        StatID = i + 1,
                        GameFileID = i,
                        ItemCount = i,
                        TotalItems = i,
                        KillCount = i,
                        TotalKills = i,
                        SecretCount = i,
                        TotalSecrets = i,
                        LevelTime = i,
                        SourcePortID = i,
                        SaveFile = i.ToString(),
                        MapName = i.ToString(),
                        RecordTime = new DateTime(2019, 1, 1, i, 0, 0),
                    }
                );
            }
            return stats;
        }

        [TestMethod]
        public void TestStatData()
        {
            TestInsert();
            TestGet();
            TestUpdate();
            TestDelete();
        }
        
        public void TestInsert()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var stats = CreateStatsData();
            foreach (var stat in stats)
                adapter.InsertStats(stat);

            Assert.AreEqual(stats.Count, adapter.GetStats().Count());
        }
        
        public void TestGet()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var testStats = CreateStatsData();
            var stats = adapter.GetStats();

            foreach (var testStat in testStats)
            {
                var stat = stats.First(x => x.StatID == testStat.StatID);
                Assert.AreEqual(testStat, stat);
            }
        }
        
        public void TestUpdate()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var stats = adapter.GetStats();

            var updateStat = stats.First();
            updateStat.SourcePortID = 1000;

            adapter.UpdateStats(updateStat);

            Assert.AreEqual(updateStat, adapter.GetStats().First(x => x.StatID == updateStat.StatID));
        }
        
        public void TestDelete()
        {
            IDataSourceAdapter adapter = TestUtil.CreateAdapter();
            var stats = adapter.GetStats();
            int count = stats.Count();

            var sourcePortStat = stats.First();
            adapter.DeleteStats(new SourcePortData() { SourcePortID = sourcePortStat.SourcePortID });
            count--;

            Assert.AreEqual(count, adapter.GetStats().Count());
            stats = adapter.GetStats();

            foreach(var stat in stats)
            {
                adapter.DeleteStats(stat.StatID);
                count--;
                Assert.AreEqual(count, adapter.GetStats().Count());
            }
        }
    }
}
