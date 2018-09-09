using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using System.Globalization;
using System.Reflection;
using Equin.ApplicationFramework;

namespace DoomLauncher
{
    public partial class StatisticsView : UserControl, IFileAssociationView
    {
        private ContextMenuStrip m_menu;

        public StatisticsView()
        {
            InitializeComponent();

            dgvMain.RowHeadersVisible = false;
            dgvMain.AutoGenerateColumns = false;
            dgvMain.ShowCellToolTips = false;
            dgvMain.DefaultCellStyle.SelectionBackColor = Color.Gray;

            SetColumnFields(new Tuple<string, string>[]
            {
                new Tuple<string, string>("MapName", "Map"),
                new Tuple<string, string>("FormattedKills", "Kills"),
                new Tuple<string, string>("FormattedSecrets", "Secrets"),
                new Tuple<string, string>("FormattedItems", "Items"),
                new Tuple<string, string>("FormattedTime", "Time"),
                new Tuple<string, string>("RecordTime", "Date"),
                new Tuple<string, string>("SourcePort", "SourcePort"),
            });

            dgvMain.Columns[dgvMain.Columns.Count - 2].DefaultCellStyle.Format = string.Format("{0} {1}",
                CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
        }

        public void SetContextMenu(ContextMenuStrip menu)
        {
            m_menu = menu;
        }

        public void SetData(IGameFile gameFile)
        {
            if (gameFile != null && gameFile.GameFileID.HasValue)
            {
                IEnumerable<IStatsData> stats = DataSourceAdapter.GetStats(gameFile.GameFileID.Value);
                dgvMain.DataSource = new BindingListView<StatsBind>(GetStatsBind(stats));
                dgvMain.ContextMenuStrip = m_menu;
            }
            else
            {
                dgvMain.DataSource = null;
            }
        }

        public void ClearData()
        {
            dgvMain.DataSource = null;
        }

        private List<StatsBind> GetStatsBind(IEnumerable<IStatsData> stats)
        {
            List<ISourcePort> sourcePorts = Util.GetSourcePortsData(DataSourceAdapter);

            IEnumerable<StatsBind> statsBind = from stat in stats
                                               join sp in sourcePorts on stat.SourcePortID equals sp.SourcePortID
                                               select new StatsBind(stat.MapName, stat.FormattedKills, stat.FormattedSecrets,
                                               stat.FormattedItems, TimeSpan.FromSeconds(stat.LevelTime), stat.RecordTime, sp.Name, stat);
            return statsBind.ToList();
        }

        public bool Delete()
        {
            if (dgvMain.SelectedRows.Count > 0 &&
                MessageBox.Show(this, "Delete selected statistic(s)?", "Confirm", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                foreach (DataGridViewRow row in dgvMain.SelectedRows)
                {
                    IStatsData stats = GetStatsFromGridRow(row);
                    DataSourceAdapter.DeleteStats(stats.StatID);
                }
                return true;
            }

            return false;
        }

        public void CopyToClipboard()
        {
            Clipboard.SetDataObject(dgvMain.GetClipboardContent(), true);
        }

        public void CopyAllToClipboard()
        {
            dgvMain.SelectAll();
            Clipboard.SetDataObject(dgvMain.GetClipboardContent(), true);
        }

        private IStatsData GetStatsFromGridRow(DataGridViewRow row)
        {
            StatsBind bind = ((ObjectView<StatsBind>)row.DataBoundItem).Object as StatsBind;
            return bind.StatsData;
        }

        public bool New() { throw new NotSupportedException(); }
        public bool Edit() { throw new NotSupportedException(); }
        public void View() { throw new NotSupportedException(); }
        public bool MoveFileOrderUp() { throw new NotSupportedException(); }
        public bool MoveFileOrderDown() { throw new NotSupportedException(); }
        public bool SetFileOrderFirst() { throw new NotSupportedException(); }

        public IDataSourceAdapter DataSourceAdapter { get; set; }

        private void SetColumnFields(IEnumerable<Tuple<string, string>> columnFields)
        {
            if (columnFields.Any())
            {
                foreach (Tuple<string, string> item in columnFields)
                {
                    DataGridViewColumn col = new DataGridViewTextBoxColumn
                    {
                        HeaderText = item.Item2,
                        Name = item.Item1,
                        DataPropertyName = item.Item1
                    };
                    dgvMain.Columns.Add(col);
                }

                dgvMain.Columns[dgvMain.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        public IGameFile GameFile { get; set; }

        public bool DeleteAllowed { get { return true; } }
        public bool CopyAllowed { get { return false; } }
        public bool NewAllowed { get { return false; } }
        public bool EditAllowed { get { return false; } }
        public bool ViewAllowed { get { return false; } }
        public bool ChangeOrderAllowed { get { return false; } }

        private class StatsBind
        {
            public StatsBind(string map, string kills, string secrets, string items, TimeSpan time, DateTime recordtime, string sourceport, IStatsData statsdatasource)
            {
                MapName = map;
                FormattedKills = kills;
                FormattedSecrets = secrets;
                FormattedItems = items;
                FormattedTime = time;
                RecordTime = recordtime;
                SourcePort = sourceport;
                StatsData = statsdatasource;
            }

            public string MapName { get; set; }
            public string FormattedKills { get; set; }
            public string FormattedSecrets { get; set; }
            public string FormattedItems { get; set; }
            public TimeSpan FormattedTime { get; set; }
            public DateTime RecordTime { get; set; }
            public string SourcePort  { get; set; }
            public IStatsData StatsData { get; set; }
        }
    }
}
