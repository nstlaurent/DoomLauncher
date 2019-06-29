using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
using DoomLauncher.DataSources;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestZdoomStats
    {
        private readonly List<NewStatisticsEventArgs> m_args = new List<NewStatisticsEventArgs>();

        [TestMethod]
        public void TestZdoomBinary()
        {
            string save1 = "zdoomsave_v1.zds";
            TestUtil.DeleteResourceFile(save1);
            ZDoomStatsReader statsReader = CreateStatsReader();
            statsReader.NewStastics += StatsReader_NewStastics;
            statsReader.Start();

            TestUtil.CopyResourceFile(save1);

            WaitForEvents();
            statsReader.Stop();

            Assert.AreEqual(2, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("e1m1", m_args[0].Statistics.MapName.ToLower());
            Assert.AreEqual(5, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);

            Assert.AreEqual(0, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(0, m_args[0].Statistics.TotalItems);

            Assert.AreEqual(1, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);

            Assert.AreEqual(25.22857f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("e1m2", m_args[1].Statistics.MapName.ToLower());
            Assert.AreEqual(22, m_args[1].Statistics.KillCount);
            Assert.AreEqual(41, m_args[1].Statistics.TotalKills);

            Assert.AreEqual(0, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(0, m_args[1].Statistics.TotalItems);

            Assert.AreEqual(2, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(6, m_args[1].Statistics.TotalSecrets);

            Assert.AreEqual(79.77143f, m_args[1].Statistics.LevelTime);
        }

        [TestMethod]
        public void TestZdoomJson()
        {
            //this is the pre 3.5 json version that did not include items
            string save1 = "zdoomsave_v2.zds";
            TestUtil.DeleteResourceFile(save1);

            ZDoomStatsReader statsReader = CreateStatsReader();
            statsReader.NewStastics += StatsReader_NewStastics;
            statsReader.Start();

            TestUtil.CopyResourceFile(save1);

            WaitForEvents();
            statsReader.Stop();

            Assert.AreEqual(2, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("e1m1", m_args[0].Statistics.MapName.ToLower());
            Assert.AreEqual(5, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);

            Assert.AreEqual(0, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(0, m_args[0].Statistics.TotalItems);

            Assert.AreEqual(1, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);

            Assert.AreEqual(34.2857132f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("e1m2", m_args[1].Statistics.MapName.ToLower());
            Assert.AreEqual(25, m_args[1].Statistics.KillCount);
            Assert.AreEqual(41, m_args[1].Statistics.TotalKills);

            Assert.AreEqual(0, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(0, m_args[1].Statistics.TotalItems);

            Assert.AreEqual(2, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(6, m_args[1].Statistics.TotalSecrets);

            Assert.AreEqual(132.65715f, m_args[1].Statistics.LevelTime);

            //Give the reader the existing statistics, and let it re-read the save. It should not fire the events to prevent duplicates
            TestUtil.DeleteResourceFile(save1);
            statsReader = new ZDoomStatsReader(new GameFile() { GameFileID = 1 }, Directory.GetCurrentDirectory(), m_args.Select(x => x.Statistics).ToArray());
            m_args.Clear();
            statsReader.NewStastics += StatsReader_NewStastics;
            statsReader.Start();

            WaitForEvents();

            File.Copy(Path.Combine("Resources", save1), save1);
            Assert.AreEqual(0, m_args.Count);
        }

        [TestMethod]
        public void TestZdoomJson_3_5()
        {
            //This is to test the 3.5 save format that includes items
            string save1 = "zdoomsave_v3.zds";
            TestUtil.DeleteResourceFile(save1);

            ZDoomStatsReader statsReader = CreateStatsReader();
            statsReader.NewStastics += StatsReader_NewStastics;
            statsReader.Start();

            TestUtil.CopyResourceFile(save1);

            WaitForEvents();
            statsReader.Stop();

            Assert.AreEqual(2, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("e1m1", m_args[0].Statistics.MapName.ToLower());
            Assert.AreEqual(5, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);

            Assert.AreEqual(15, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(37, m_args[0].Statistics.TotalItems);

            Assert.AreEqual(1, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);

            Assert.AreEqual(40.2f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("e1m2", m_args[1].Statistics.MapName.ToLower());
            Assert.AreEqual(21, m_args[1].Statistics.KillCount);
            Assert.AreEqual(41, m_args[1].Statistics.TotalKills);

            Assert.AreEqual(8, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(42, m_args[1].Statistics.TotalItems);

            Assert.AreEqual(2, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(6, m_args[1].Statistics.TotalSecrets);

            Assert.AreEqual(83.37143f, m_args[1].Statistics.LevelTime);

            //Give the reader the existing statistics, and let it re-read the save. It should not fire the events to prevent duplicates
            TestUtil.DeleteResourceFile(save1);
            statsReader = new ZDoomStatsReader(new GameFile() { GameFileID = 1 }, Directory.GetCurrentDirectory(), m_args.Select(x => x.Statistics).ToArray());
            m_args.Clear();
            statsReader.NewStastics += StatsReader_NewStastics;
            statsReader.Start();

            WaitForEvents();

            File.Copy(Path.Combine("Resources", save1), save1);
            Assert.AreEqual(0, m_args.Count);
        }

        private void WaitForEvents()
        {
            DateTime start = DateTime.Now;

            while (DateTime.Now.Subtract(start).TotalMilliseconds < 2000 && m_args.Count == 0)
                System.Threading.Thread.Sleep(100);
        }

        private static ZDoomStatsReader CreateStatsReader()
        {
            return new ZDoomStatsReader(new GameFile() { GameFileID = 1 }, Directory.GetCurrentDirectory(), new IStatsData[] { });
        }

        private void StatsReader_NewStastics(object sender, NewStatisticsEventArgs e)
        {
            m_args.Add(e);
        }
    }
}
