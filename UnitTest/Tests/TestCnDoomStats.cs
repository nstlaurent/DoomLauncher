using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoomLauncher;
using DoomLauncher.DataSources;
using System.IO;
using System.Collections.Generic;

namespace UnitTest.Tests
{
    [TestClass]
    public class TestCnDoomStats
    {
        private readonly List<NewStatisticsEventArgs> m_args = new List<NewStatisticsEventArgs>();
        [Ignore]
        [TestMethod]
        public void TestCnDoomStatFile()
        {
            string file = @"                       Competition Doom 2.0.3.2
Z_Init: Init zone memory allocation daemon. 
zone memory: 04D23020, 2000000 allocated for zone
V_Init: allocate screens.
M_LoadDefaults: Load system defaults.
saving config in default.cfg
W_Init: Init WADfiles.
 adding C:\DoomLauncher\GameFiles\Temp\DOOM.WAD
===========================================================================
                            DOOM Shareware
===========================================================================
 Competition Doom is free software, covered by the GNU General Public
 License.  There is NO warranty; not even for MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE. You are welcome to change and distribute
 copies under certain conditions. See the source for more information.
===========================================================================
I_Init: Setting up machine state.
OPL_Init: Using driver 'SDL'.
NET_Init: Init network subsystem.
M_Init: Init miscellaneous info.
R_Init: Init DOOM refresh daemon - ...................
P_Init: Init Playloop state.
S_Init: Setting up sound.
D_CheckNetGame: Checking network game status.
startskill 2  deathmatch: 0  startmap: 1  startepisode: 1
player 1 of 1 (1 nodes)
Emulating the behavior of the 'Doom 1.9' executable.
HU_Init: Setting up heads up display.
ST_Init: Init status bar.

### E1M1 ######################################
#                                             #
#   Time:  00:37.11       Kills:     5/6      #
#  Items:     6/37      Secrets:     1/3      #
#                                             #
################### Total time: 00:00:37.11 ###

### E1M2 ######################################
#                                             #
#   Time:  00:59.89       Kills:    13/41     #
#  Items:     1/42      Secrets:     0/6      #
#                                             #
################### Total time: 00:01:37.00 ###
";

            CNDoomStatsReader statsReader = new CNDoomStatsReader(new GameFile() { GameFileID = 1 }, "stdout.txt");
            statsReader.NewStastics += StatsReader_NewStastics;
            File.WriteAllText("stdout.txt", file);
            statsReader.ReadNow();

            Assert.AreEqual(2, m_args.Count);

            Assert.AreEqual("e1m1", m_args[0].Statistics.MapName.ToLower());
            Assert.AreEqual(5, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);

            Assert.AreEqual(6, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(37, m_args[0].Statistics.TotalItems);

            Assert.AreEqual(1, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);

            Assert.AreEqual(37.11f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("e1m2", m_args[1].Statistics.MapName.ToLower());
            Assert.AreEqual(13, m_args[1].Statistics.KillCount);
            Assert.AreEqual(41, m_args[1].Statistics.TotalKills);

            Assert.AreEqual(1, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(42, m_args[1].Statistics.TotalItems);

            Assert.AreEqual(0, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(6, m_args[1].Statistics.TotalSecrets);

            Assert.AreEqual(59.89f, m_args[1].Statistics.LevelTime);
        }

        private void StatsReader_NewStastics(object sender, NewStatisticsEventArgs e)
        {
            m_args.Add(e);
        }
    }
}
