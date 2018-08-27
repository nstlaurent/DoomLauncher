using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class CumulativeStats : Form
    {
        public CumulativeStats()
        {
            InitializeComponent();
        }

        public void SetStatistics(IEnumerable<IStatsData> stats)
        {
            int minutes = 0;

            foreach (IStatsData stat in stats)
                minutes += (int)(stat.LevelTime / 60.0);

            lblTimePlayed.Text = Util.GetTimePlayedString(minutes);
            ctrlStats.SetStatistics(stats);
        }
    }
}
