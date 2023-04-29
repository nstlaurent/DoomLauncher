using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class CumulativeStats : Form
    {
        public CumulativeStats()
        {
            InitializeComponent();

            Stylizer.Stylize(this, DesignMode, StylizerOptions.RemoveTitleBar);
            MaximizedBounds = Screen.GetWorkingArea(this);
        }

        public string Title
        {
            get { return titleBar.Title; }
            set { titleBar.Title = value; }
        }

        public void SetStatistics(IEnumerable<IGameFile> gameFiles, IEnumerable<IStatsData> stats)
        {
            lblNote.Text = $"Note: Search filters apply ({gameFiles.Count()} Files)";

            int statsMinutes = 0;
            int launchMinutes = 0;
            foreach (IStatsData stat in stats)
                statsMinutes += (int)(stat.LevelTime / 60.0);

            foreach (IGameFile gameFile in gameFiles)
                launchMinutes += gameFile.MinutesPlayed;

            lblTimeLaunched.Text = Util.GetTimePlayedString(launchMinutes);
            lblTimePlayed.Text = Util.GetTimePlayedString(statsMinutes);
            ctrlStats.SetStatistics(gameFiles, stats);
        }
    }
}
