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
zone memory: 029C0020, 2000000 allocated for zone
V_Init: allocate screens.
M_LoadDefaults: Load system defaults.
saving config in default.cfg
W_Init: Init WADfiles.
 adding C:\DoomLauncher\GameFiles\Temp\DOOM.WAD
===========================================================================
                           The Ultimate DOOM
===========================================================================
 Competition Doom is free software, covered by the GNU General Public
 License.  There is NO warranty; not even for MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE. You are welcome to change and distribute
 copies under certain conditions. See the source for more information.
===========================================================================
I_Init: Setting up machine state.
NET_Init: Init network subsystem.
M_Init: Init miscellaneous info.
R_Init: Init DOOM refresh daemon - ..........................
P_Init: Init Playloop state.
S_Init: Setting up sound.
D_CheckNetGame: Checking network game status.
startskill 2  deathmatch: 0  startmap: 1  startepisode: 1
player 1 of 1 (1 nodes)
Emulating the behavior of the 'Ultimate Doom' executable.
HU_Init: Setting up heads up display.
ST_Init: Init status bar.
I_InitGraphics: Windowboxed (1280x960 within 1920x1080)
I_InitStretchTables: Generating lookup tables....
I_InitGraphics: Windowboxed (1280x960 within 1920x1080)

### E1M1 ######################################
#                                             #
#   Time:  00:22.57       Kills:     3/6      #
#  Items:     8/37      Secrets:     1/3      #
#                                             #
################### Total time: 00:00:22.57 ###

### E1M2 ######################################
#                                             #
#   Time:  00:45.20       Kills:     4/41     #
#  Items:     6/42      Secrets:     3/6      #
#                                             #
################### Total time: 00:01:07.77 ###

### MAP11 #####################################
#                                             #
#   Time:  05:03.23       Kills:    82/71     #
#  Items:    19/21      Secrets:     2/3      #
#                                             #
################### Total time: 00:05:03.23 ###
";

            CNDoomStatsReader statsReader = new CNDoomStatsReader(new GameFile() { GameFileID = 1 }, "stdout.txt");
            statsReader.NewStastics += StatsReader_NewStastics;
            File.WriteAllText("stdout.txt", file);
            statsReader.ReadNow();

            Assert.AreEqual(3, m_args.Count);
            Assert.AreEqual(0, statsReader.Errors.Length);

            Assert.AreEqual("E1M1", m_args[0].Statistics.MapName);
            Assert.AreEqual(3, m_args[0].Statistics.KillCount);
            Assert.AreEqual(6, m_args[0].Statistics.TotalKills);

            Assert.AreEqual(8, m_args[0].Statistics.ItemCount);
            Assert.AreEqual(37, m_args[0].Statistics.TotalItems);

            Assert.AreEqual(1, m_args[0].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[0].Statistics.TotalSecrets);

            Assert.AreEqual(22.57f, m_args[0].Statistics.LevelTime);

            Assert.AreEqual("E1M2", m_args[1].Statistics.MapName);
            Assert.AreEqual(4, m_args[1].Statistics.KillCount);
            Assert.AreEqual(41, m_args[1].Statistics.TotalKills);

            Assert.AreEqual(6, m_args[1].Statistics.ItemCount);
            Assert.AreEqual(42, m_args[1].Statistics.TotalItems);

            Assert.AreEqual(3, m_args[1].Statistics.SecretCount);
            Assert.AreEqual(6, m_args[1].Statistics.TotalSecrets);

            Assert.AreEqual(45.20f, m_args[1].Statistics.LevelTime);

            //Kill count should not exceed total
            Assert.AreEqual("MAP11", m_args[2].Statistics.MapName);
            Assert.AreEqual(71, m_args[2].Statistics.KillCount);
            Assert.AreEqual(71, m_args[2].Statistics.TotalKills);

            Assert.AreEqual(19, m_args[2].Statistics.ItemCount);
            Assert.AreEqual(21, m_args[2].Statistics.TotalItems);

            Assert.AreEqual(2, m_args[2].Statistics.SecretCount);
            Assert.AreEqual(3, m_args[2].Statistics.TotalSecrets);

            Assert.AreEqual(303.23f, m_args[2].Statistics.LevelTime);
        }

        private void StatsReader_NewStastics(object sender, NewStatisticsEventArgs e)
        {
            m_args.Add(e);
        }
    }
}
