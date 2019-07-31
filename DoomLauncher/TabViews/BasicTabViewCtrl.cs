using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DoomLauncher.Interfaces;
using DoomLauncher.DataSources;
using Equin.ApplicationFramework;
using System.Globalization;

namespace DoomLauncher
{
    public partial class BasicTabViewCtrl : UserControl, ITabView, ICloneable
    {
        protected string m_title;
        protected object m_key;
        protected GameFileFieldType[] m_selectFields;

        public BasicTabViewCtrl(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields)
        {
            InitializeComponent();
            m_key = key;
            m_title = title;
            Adapter = adapter;
            m_selectFields = selectFields.ToArray();
        }

        public virtual object Clone()
        {
            BasicTabViewCtrl view = new BasicTabViewCtrl(m_key, m_title, Adapter, m_selectFields);
            SetBaseCloneProperties(view);
            return view;
        }

        protected void SetBaseCloneProperties(ITabView view)
        {
            view.SetColumnConfig(GameFileViewControl.ColumnFields, GetColumnConfig().ToArray());
        }

        public List<ColumnConfig> GetColumnConfig()
        {
            List<ColumnConfig> config = new List<ColumnConfig>();

            foreach (string key in GameFileViewControl.GetColumnKeyOrder())
            {
                ColumnConfig col = new ColumnConfig(Title, key,
                    GameFileViewControl.GetColumnWidth(key), GameFileViewControl.GetColumnSort(key));
                config.Add(col);
            }

            return config;
        }

        public void SetColumnConfig(ColumnField[] columnTextFields, ColumnConfig[] colConfig)
        {
            GameFileViewControl.SuspendLayout();

            var sortedColumns = SortColumns(Title, columnTextFields, colConfig);
            GameFileViewControl.SetColumnFields(sortedColumns);
            SetSortedColumn(Title, GameFileViewControl, colConfig);
            SetColumnWidths(Title, GameFileViewControl, colConfig);

            GameFileViewControl.SetColumnFormat("ReleaseDate", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            GameFileViewControl.SetColumnFormat("Downloaded", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            GameFileViewControl.SetColumnFormat("LastPlayed", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);

            GameFileViewControl.ResumeLayout();
        }

        private static ColumnField[] SortColumns(string tab, ColumnField[] items, ColumnConfig[] config)
        {
            List<ColumnField> ret = new List<ColumnField>();
            //Pre 1.1.0 update check. Without this maps will be thrown on the end of the column list
            bool setMapLocation = config.Length > 0 && !config.Any(x => x.Column == "MapCount");

            foreach (ColumnConfig configItem in config)
            {
                var check = items.FirstOrDefault(x =>
                    configItem.Parent == tab && x.DataKey.Equals(configItem.Column, StringComparison.InvariantCultureIgnoreCase));

                if (check != null)
                    ret.Add(check);
            }
            //continue with pre 1.1.0 check (this can probably be removed at this point, though)
            if (setMapLocation && ret.Count > 4)
            {
                var maps = items.FirstOrDefault(x => x.Title.Equals("maps", StringComparison.InvariantCultureIgnoreCase));

                if (maps != null)
                {
                    ret.Remove(maps);
                    ret.Insert(4, maps);
                }
            }

            return ret.Union(items).ToArray();
        }

        private static void SetColumnWidths(string tab, GameFileViewControl ctrl, ColumnConfig[] config)
        {
            IEnumerable<ColumnConfig> configSet = config.Where(x => x.Parent == tab);
            foreach (ColumnConfig col in configSet)
            {
                ctrl.SetColumnWidth(col.Column, col.Width);
            }
        }

        private static void SetSortedColumn(string tab, GameFileViewControl gameFileViewControl, ColumnConfig[] colConfig)
        {
            ColumnConfig config = colConfig.FirstOrDefault(x => x.Parent == tab && x.Sort != SortOrder.None);

            if (config != null)
                gameFileViewControl.SetSortedColumn(config.Column, config.Sort);
        }

        public GameFileViewControl GameFileViewControl { get { return ctrlView; } }

        public object Key { get { return m_key; } }

        protected virtual bool FilterIWads { get { return true; } }

        public virtual void SetGameFiles()
        {
            GameFileGetOptions options = new GameFileGetOptions();
            options.SelectFields = m_selectFields;
            SetDataSource(Adapter.GetGameFiles(options));
        }

        public virtual void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            IEnumerable<IGameFile> items = new IGameFile[0];

            foreach (GameFileSearchField sf in searchFields)
            {
                items = items.Union(Adapter.GetGameFiles(new GameFileGetOptions(m_selectFields, sf)));
            }

            SetDataSource(items);
        }

        public void SetGameFilesData(IEnumerable<IGameFile> gameFiles)
        {
            SetDataSource(gameFiles);
        }

        public virtual void UpdateDataSourceFile(IGameFile gameFile)
        {
            if (ctrlView.DataSource != null)
            {
                foreach (ObjectView<GameFile> item in (BindingListView<GameFile>)ctrlView.DataSource)
                {
                    if (item.Object.Equals(gameFile))
                    {
                        IGameFile dsSet = item.Object as IGameFile;
                        Array.ForEach(dsSet.GetType().GetProperties(), x => x.SetValue(dsSet, x.GetValue(gameFile)));
                        ctrlView.Invalidate(true);
                        break;
                    }
                }
            }
        }

        protected void SetDisplayText(string text)
        {
            ctrlView.SetDisplayText(text);
        }

        protected void SetDataSource(IEnumerable<IGameFile> gameFiles)
        {
            if (FilterIWads && !(Adapter is IdGamesDataAdapater))
                gameFiles = gameFiles.Except(Adapter.GetGameFileIWads());

            if (!gameFiles.Any())
            {
                ctrlView.DataSource = null;
                ctrlView.SetDisplayText("No Results Found");
            }
            else
            {
                ctrlView.DataSource = new BindingListView<GameFile>(gameFiles.Cast<GameFile>().ToList());
            }
        }

        protected IGameFile FromDataBoundItem(object item)
        {
            return ((ObjectView<GameFile>)item).Object as IGameFile;
        }

        public virtual bool IsLocal { get { return true; } }
        public virtual bool IsEditAllowed { get { return true; } }
        public virtual bool IsDeleteAllowed { get { return true; } }
        public virtual bool IsSearchAllowed { get { return true; } }
        public virtual bool IsPlayAllowed { get { return true; } }
        public virtual bool IsAutoSearchAllowed { get { return true; } }
        public string Title { get { return m_title; } }
        public virtual IGameFileDataSourceAdapter Adapter { get; set; }
    }
}
