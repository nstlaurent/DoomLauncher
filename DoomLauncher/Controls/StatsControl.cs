using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DoomLauncher
{
    public partial class StatsControl : UserControl
    {
        private IStatsData m_stats;

        public StatsControl()
        {
            InitializeComponent();

            pbItems.Image = DoomLauncher.Properties.Resources.bon2b;
            pbKills.Image = DoomLauncher.Properties.Resources.kill;
            pbSecrets.Image = DoomLauncher.Properties.Resources.secret;

            lblItems.Text = lblKills.Text = lblSecrets.Text = string.Empty;
            lblItems.Visible = lblKills.Visible = lblSecrets.Visible = false;
        }

        public void SetStatistics(IEnumerable<IStatsData> stats)
        {
            StatsData statTotal = new StatsData();

            foreach (IStatsData stat in stats)
            {
                statTotal.KillCount += stat.KillCount;
                statTotal.TotalKills += stat.TotalKills;
                statTotal.SecretCount += stat.SecretCount;
                statTotal.TotalSecrets += stat.TotalSecrets;
                statTotal.ItemCount += stat.ItemCount;
                statTotal.TotalItems += stat.TotalItems;
                statTotal.LevelTime += stat.LevelTime;
            }

            m_stats = statTotal;
        }

        private void tblStats_Paint(object sender, PaintEventArgs e)
        {
            if (m_stats != null)
            {
                DrawProgress(GetProgressDrawPoint(lblKills), m_stats.KillCount, m_stats.TotalKills, m_stats.FormattedKills);
                DrawProgress(GetProgressDrawPoint(lblSecrets), m_stats.SecretCount, m_stats.TotalSecrets, m_stats.FormattedSecrets);
                DrawProgress(GetProgressDrawPoint(lblItems), m_stats.ItemCount, m_stats.TotalItems, m_stats.FormattedItems);
            }
        }

        private Point GetProgressDrawPoint(Control ctrl)
        {
            Point pt = ctrl.Location;
            pt.Offset(-4, -4);
            return pt;
        }

        private void DrawProgress(Point pt, int count, int total, string text)
        {
            int width = 160;
            int height = 23 - 3;

            Graphics g = tblStats.CreateGraphics();
            Pen pen = new Pen(Color.Black, 1.0f);
            Rectangle rect = new Rectangle(pt, new Size(width, height));

            g.DrawRectangle(pen, rect);

            double percent = 0; 
            if (total > 0)
                percent = count / (double)total;
            width = (int)((width-1) * percent);

            pt.Offset(1, 1);
            rect = new Rectangle(pt, new Size(rect.Width - 1, rect.Height - 1));
            Brush bgBrush = new LinearGradientBrush(rect, Color.DarkGray, Color.LightGray, 90.0f);
            Rectangle percentRect = new Rectangle(rect.Location, new Size(width, rect.Height));
            Brush brush = GetPercentBrush(rect, percent);

            g.FillRectangle(bgBrush, rect);
            if (percent > 0)
            {
                g.FillRectangle(brush, percentRect);
                pt.Offset(-1, -1);
                g.DrawRectangle(GetPrecentPen(percent), new Rectangle(pt, new Size(percentRect.Width + 1, percentRect.Height + 1)));
            }

            Brush fontBrush = new SolidBrush(Color.Black);
            g.DrawString(text, new Font(FontFamily.GenericSansSerif, 10.0f), fontBrush, new PointF(pt.X + 3, pt.Y + 3));
        }

        private static Brush GetPercentBrush(Rectangle rect, double percent)
        {
            if (percent >= 1.0)
                return new LinearGradientBrush(rect, Color.LightGreen, Color.Green, LinearGradientMode.ForwardDiagonal);
            else
                return new LinearGradientBrush(rect, Color.LightBlue, Color.Blue, LinearGradientMode.ForwardDiagonal);
        }

        private static Pen GetPrecentPen(double percent)
        {
            if (percent >= 1.0)
                return new Pen(Color.Green);
            else
                return new Pen(Color.Blue);
        }
    }
}
