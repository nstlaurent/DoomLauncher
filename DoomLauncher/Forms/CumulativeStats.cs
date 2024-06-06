using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class CumulativeStats : Form
    {
        public CumulativeStats()
        {
            InitializeComponent();
            tblMain.CellPaint += TblMain_CellPaint;
            Stylizer.Stylize(this, DesignMode, StylizerOptions.SetupTitleBar);
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

        private void TblMain_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            Point pt = e.CellBounds.Location;
            Pen pen = new Pen(ColorTheme.Current.GridBorder);
            e.Graphics.DrawLine(pen, pt.X, pt.Y, pt.X, pt.Y + e.CellBounds.Height);
            e.Graphics.DrawLine(pen, pt.X + e.CellBounds.Width - 1, pt.Y, e.CellBounds.Width - 1, pt.Y + e.CellBounds.Height);
            e.Graphics.DrawLine(pen, pt.X, pt.Y + e.CellBounds.Height - 1, pt.X + e.CellBounds.Width, pt.Y + e.CellBounds.Height - 1);
        }
    }
}
