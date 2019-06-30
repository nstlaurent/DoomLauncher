using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher.Statistics;
using DoomLauncher.DataSources;
using DoomLauncher;
using System.Collections.Generic;
using System.IO;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestChocolateDoomStats
    {
        private readonly List<NewStatisticsEventArgs> m_args = new List<NewStatisticsEventArgs>();

        [TestMethod]
        public void TestChocolateDoomStatSingle()
        {
            string file = @"===========================================
E1M1 / MAP01
===========================================

Time: 0:22 (par: 0:30)

Player 1 (Green):
	Kills: 5 / 6 (83%)
	Items: 3 / 37 (8%)
	Secrets: 0 / 3 (0%)
";

            ChocolateDoomStatsReader statsReader = new ChocolateDoomStatsReader(new GameFile() { GameFileID = 1 }, "stats.txt");
            statsReader.NewStastics += statsReader_NewStastics;

            File.WriteAllText("stats.txt", file);
            statsReader.ReadNow();

            Assert.AreEqual(1, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("E1M1/MAP01", m_args[0].Statistics.MapName);
            Assert.AreEqual(5, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);
            Assert.AreEqual(3, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(37, m_args[0].Statistics.TotalItems);
            Assert.AreEqual(0, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);
            Assert.AreEqual(22.0f, m_args[0].Statistics.LevelTime);
        }

        [TestMethod]
        public void TestChocolateDoomStatFileMultiple()
        {
            string file = @"===========================================
E1M1
===========================================

Time: 0:20 (par: 0:30)

Player 1 (Green):
	Kills: 5 / 6 (83%)
	Items: 8 / 37 (21%)
	Secrets: 1 / 3 (0%)

===========================================
E1M2
===========================================

Time: 12:23 (par: 1:15)

Player 1 (Green):
	Kills: 13 / 41 (31%)
	Items: 12 / 42 (2%)
	Secrets: 3/ 6 (0%)


";

            ChocolateDoomStatsReader statsReader = new ChocolateDoomStatsReader(new GameFile() { GameFileID = 1 }, "stats.txt");
            statsReader.NewStastics += statsReader_NewStastics;

            File.WriteAllText("stats.txt", file);
            statsReader.ReadNow();

            Assert.AreEqual(2, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("E1M1", m_args[0].Statistics.MapName);
            Assert.AreEqual(5, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);
            Assert.AreEqual(8, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(37, m_args[0].Statistics.TotalItems);
            Assert.AreEqual(1, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);
            Assert.AreEqual(20.0f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("E1M2", m_args[1].Statistics.MapName);
            Assert.AreEqual(13, m_args[1].Statistics.KillCount);
            Assert.AreEqual(41, m_args[1].Statistics.TotalKills);
            Assert.AreEqual(12, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(42, m_args[1].Statistics.TotalItems);
            Assert.AreEqual(3, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(6, m_args[1].Statistics.TotalSecrets);
            Assert.AreEqual(743.0f, m_args[1].Statistics.LevelTime);
        }

        [TestMethod]
        public void TestChocolateDoomStatFileZero()
        {
            string file = @"===========================================
MAP30
===========================================

Time: 0:21 (par: 3:00)

Player 1 (Green):
	Kills: 0
	Items: 1 / 6 (16%)
	Secrets: 0
";

            ChocolateDoomStatsReader statsReader = new ChocolateDoomStatsReader(new GameFile() { GameFileID = 1 }, "stats.txt");
            statsReader.NewStastics += statsReader_NewStastics;

            File.WriteAllText("stats.txt", file);
            statsReader.ReadNow();

            Assert.AreEqual(1, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("MAP30", m_args[0].Statistics.MapName);
            Assert.AreEqual(0, m_args[0].Statistics.KillCount);
            Assert.AreEqual(0, m_args[0].Statistics.TotalKills);
            Assert.AreEqual(1, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalItems);
            Assert.AreEqual(0, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(0, m_args[0].Statistics.TotalSecrets);
            Assert.AreEqual(21.0f, m_args[0].Statistics.LevelTime);
        }

        private void statsReader_NewStastics(object sender, DoomLauncher.NewStatisticsEventArgs e)
        {
            m_args.Add(e);
        }
    }
}
