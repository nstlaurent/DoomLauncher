using DoomLauncher.Interfaces;
using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using DoomLauncher.DataSources;

namespace DoomLauncher.Forms
{
    public enum PlayRandomType
    {
        [Description("Any")]
        Any,
        [Description("Unplayed")]
        Unplayed,
        [Description("Unrated")]
        Unrated,
        [Description("Current Tab")]
        CurrentTab,
    }

    public partial class PlayRandomForm : Form
    {
        public IGameFile GeneratedGameFile { get; private set; }
        public string GeneratedMap { get; private set; }
        public PlayRandomType SelectedType => (PlayRandomType)cmbType.SelectedIndex;

        private ITabView m_tabView;

        public PlayRandomForm()
        {
            InitializeComponent();

            lblText.Text = string.Empty;

            List<string> items = new List<string>();
            foreach (var item in Enum.GetValues(typeof(PlayRandomType)))
                items.Add(((PlayRandomType)item).GetDescription());

            cmbType.DataSource = items;

            Stylizer.Stylize(this, DesignMode, StylizerOptions.RemoveTitleBar);
        }

        public void Initialize(ITabView tabView)
        {
            m_tabView = tabView;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GeneratedRandom();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (GeneratedGameFile != null)
                return;

            GeneratedRandom();
        }

        private void GeneratedRandom()
        {
            IEnumerable<IGameFile> gameFiles = GetGameFilesBySelectedType();
            if (!gameFiles.Any())
            {
                lblText.Text = "No files found.";
                return;
            }

            Random rand = new Random(DateTime.Now.Millisecond);
            int index = rand.Next() % gameFiles.Count();
            GeneratedGameFile = gameFiles.ElementAt(index);

            GameFileGetOptions options = new GameFileGetOptions();
            options.SearchField = new GameFileSearchField(GameFileFieldType.GameFileID, GeneratedGameFile.GameFileID.ToString());
            GeneratedGameFile = DataCache.Instance.DataSourceAdapter.GetGameFiles(options).FirstOrDefault();

            if (GeneratedGameFile == null)
            {
                lblText.Text = $"Current tab ({m_tabView.Key}) is not supported in this context.";
                return;
            }

            if (chkRandomMap.Checked && GetRandomMap(rand, GeneratedGameFile, out string map))
            {
                GeneratedMap = map;
                lblText.Text = $"{GeneratedGameFile.FileNameNoPath} - {GeneratedMap}";
                return;
            }

            lblText.Text = $"{GeneratedGameFile.FileNameNoPath}";
        }

        private IEnumerable<IGameFile> GetGameFilesBySelectedType()
        {
            switch (SelectedType)
            {
                case PlayRandomType.Any:
                    return GetGameFiles(null);

                case PlayRandomType.Unplayed:
                    return GetGameFiles(GameFileFieldType.LastPlayed);

                case PlayRandomType.Unrated:
                    return GetGameFiles(GameFileFieldType.Rating);

                case PlayRandomType.CurrentTab:
                    return m_tabView.GameFileViewControl.DataSource;

                default:
                    break;
            }

            return Array.Empty<IGameFile>();
        }

        private IEnumerable<IGameFile> GetGameFiles(GameFileFieldType? field)
        {
            IDataSourceAdapter adapter = DataCache.Instance.DataSourceAdapter;
            IGameFileGetOptions options = new GameFileGetOptions(new GameFileFieldType[] { GameFileFieldType.GameFileID, GameFileFieldType.LastPlayed, GameFileFieldType.Rating });
            IEnumerable<IGameFile> gameFiles = adapter.GetGameFiles(options);

            if (field != null)
            {
                if (field.Value == GameFileFieldType.LastPlayed)
                    gameFiles = gameFiles.Where(x => !x.LastPlayed.HasValue);
                else if (field.Value == GameFileFieldType.Rating)
                    gameFiles = gameFiles.Where(x => !x.Rating.HasValue);
            }

            return gameFiles;
        }

        private bool GetRandomMap(Random rand, IGameFile gameFile, out string map)
        {
            map = string.Empty;
            string[] maps = GameFile.GetMaps(gameFile);
            if (maps.Length == 0)
                return false;

            map = maps[rand.Next() % maps.Length];
            return true;
        }

    }
}
