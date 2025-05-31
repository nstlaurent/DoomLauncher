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
        private string m_errorMessage;

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
            if (m_working)
                return;
            
            m_working = true;
            IdGamesDataSource = null;
            SetDisplayText("Searching...");

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += UpdateIdGamesView_Worker;
            worker.RunWorkerCompleted += UpdateIdGamesViewCompleted;
            worker.RunWorkerAsync(searchFields);
        }

        private void UpdateIdGamesView_Worker(object sender, DoWorkEventArgs e)
        {
            m_errorMessage = string.Empty;
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
            catch (IdGamesBadResponseException ex)
            {
                m_errorMessage = ex.Message;
                IdGamesDataSource = null;
            }
            catch (Exception ex)
            {
                m_errorMessage = $"Failed to query id games: {ex.Message}";
                IdGamesDataSource = null;
            }
        }

        private void UpdateIdGamesViewCompleted(object sender, EventArgs e)
        {
            m_working = false;

            if (m_errorMessage.Length > 0)
            {
                SetDisplayText(m_errorMessage);
                return;
            }

            if (IdGamesDataSource != null)
            {
                SetDataSource(IdGamesDataSource);
            }
            else
            {
                SetDisplayText("Error retrieving data from id Games");
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
