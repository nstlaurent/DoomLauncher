using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class IdGamesTabViewCtrl : OptionsTabViewCtrl
    {
        private bool m_working;

        public IdGamesTabViewCtrl(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields, GameFileViewFactory factory)
            : base(key, title, adapter, selectFields, factory)
        {
            InitializeComponent();
        }

        public override void SetGameFiles()
        {
            SetGameFiles(null);
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            if (searchFields != null && searchFields.Any(x => x.SearchText.Length < 3))
            {
                MessageBox.Show(this, "The search query must be at least three characters.", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!m_working)
                {
                    m_working = true;
                    IdGamesDataSource = null;
                    base.SetDisplayText("Searching...");

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += UpdateIdGamesView_Worker;
                    worker.RunWorkerCompleted += UpdateIdGamesViewCompleted;
                    worker.RunWorkerAsync(searchFields);
                }
            }
        }

        private void UpdateIdGamesView_Worker(object sender, DoWorkEventArgs e)
        {
            try
            {
                IEnumerable<GameFileSearchField> searchFields = e.Argument as IEnumerable<GameFileSearchField>;

                if (searchFields == null || !searchFields.Any())
                {
                    IdGamesDataSource = Adapter.GetGameFiles();
                }
                else
                {
                    IEnumerable<IGameFile> ret = new IGameFile[0];

                    foreach (GameFileSearchField sf in searchFields)
                        ret = ret.Union(Adapter.GetGameFiles(new GameFileGetOptions(sf)));

                    IdGamesDataSource = ret;
                }
            }
            catch
            {
                IdGamesDataSource = new IGameFile[0];
            }
        }

        private void UpdateIdGamesViewCompleted(object sender, EventArgs e)
        {
            m_working = false;

            if (IdGamesDataSource != null)
            {
                base.SetDataSource(IdGamesDataSource);
            }
            else
            {
                base.SetDisplayText("Error retrieving data from id Games");
                MessageBox.Show(this, "There was an error retrieving data from id Games.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public override bool IsLocal { get { return false; } }
        public override bool IsEditAllowed { get { return false; } }
        public override bool IsDeleteAllowed { get { return false; } }
        public override bool IsSearchAllowed { get { return true; } }
        public override bool IsPlayAllowed { get { return false; } }
        public override bool IsAutoSearchAllowed { get { return false; } }

        public IEnumerable<IGameFile> IdGamesDataSource { get; set; }
        protected override bool FilterIWads { get { return false; } }
    }
}
