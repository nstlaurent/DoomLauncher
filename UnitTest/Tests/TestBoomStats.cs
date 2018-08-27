using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DoomLauncher;
using DoomLauncher.DataSources;
using System.Collections.Generic;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestBoomStats
    {
        private readonly List<NewStatisticsEventArgs> m_args = new List<NewStatisticsEventArgs>();

        [TestMethod]
        public void TestBoomStatFile()
        {
            string stats = @"MAP01 - 0:04.23 (0:04)  K: 12/123  I: 5/33  S: 0/1 
MAP02 - 0:03.83 (0:07)  K: 23/234  I: 7/28  S: 1/2 ";

            System.IO.File.WriteAllText("statfile.txt", stats);

            BoomStatsReader statsReader = new BoomStatsReader(new GameFile() { GameFileID = 1 }, "statfile.txt");
            statsReader.NewStastics += StatsReader_NewStastics;
            statsReader.ReadNow();

            Assert.AreEqual(2, m_args.Count);

            Assert.AreEqual("MAP01", m_args[0].Statistics.MapName);
            Assert.AreEqual(12, m_args[0].Statistics.KillCount);
            Assert.AreEqual(123, m_args[0].Statistics.TotalKills);

            Assert.AreEqual(5, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(33, m_args[0].Statistics.TotalItems);

            Assert.AreEqual(0, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(1, m_args[0].Statistics.TotalSecrets);

            Assert.AreEqual(4.23f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("MAP02", m_args[1].Statistics.MapName);
            Assert.AreEqual(23, m_args[1].Statistics.KillCount);
            Assert.AreEqual(234, m_args[1].Statistics.TotalKills);

            Assert.AreEqual(7, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(28, m_args[1].Statistics.TotalItems);

            Assert.AreEqual(1, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(2, m_args[1].Statistics.TotalSecrets);

            Assert.AreEqual(3.83f, m_args[1].Statistics.LevelTime);

            //Add another level, should one be one new level read
            stats = @"MAP01 - 0:04.23 (0:04)  K: 12/123  I: 5/33  S: 0/1 
MAP02 - 0:03.83 (0:07)  K: 23/234  I: 7/28  S: 1/2 
MAP03 - 0:04.83 (0:07)  K: 123/1234  I: 22/50  S: 2/2";

            System.IO.File.WriteAllText("statfile.txt", stats);
            statsReader.ReadNow();

            Assert.AreEqual(3, m_args.Count);

            Assert.AreEqual("MAP03", m_args[2].Statistics.MapName);
            Assert.AreEqual(123, m_args[2].Statistics.KillCount);
            Assert.AreEqual(1234, m_args[2].Statistics.TotalKills);

            Assert.AreEqual(22, m_args[2].Statistics.ItemCount);
            Assert.AreEqual(50, m_args[2].Statistics.TotalItems);

            Assert.AreEqual(2, m_args[2].Statistics.SecretCount);
            Assert.AreEqual(2, m_args[2].Statistics.TotalSecrets);

            Assert.AreEqual(4.83f, m_args[2].Statistics.LevelTime);
        }

        private void StatsReader_NewStastics(object sender, NewStatisticsEventArgs e)
        {
            m_args.Add(e);
        }
    }
}
