using System.Collections.Generic;
using System.Linq;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public partial class OptionsTabViewCtrl : LocalTabViewCtrl
    {
        public OptionsTabViewCtrl(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields, GameFileViewFactory factory) 
            : base(key, title, adapter, selectFields, null, factory)
        {
            InitializeComponent();
        }

        public OptionsTabViewCtrl(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagMapLookup lookup, GameFileViewFactory factory)
            : base(key, title, adapter, selectFields, lookup, factory)
        {
            InitializeComponent();
        }

        public GameFileGetOptions Options { get; set; }

        public override void SetGameFiles()
        {
            if (Options != null)
                SetGameFiles(null);
            else
                base.SetGameFiles();
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            if (Options != null)
            {
                GameFileGetOptions options = new GameFileGetOptions();
                options.Limit = 25;
                options.OrderBy = OrderType.Desc;
                options.OrderField = GameFileFieldType.Downloaded;
                options.SelectFields = base.m_selectFields;

                if (searchFields != null && searchFields.Any())
                {
                    IEnumerable<IGameFile> items = new IGameFile[0];

                    foreach (GameFileSearchField sf in searchFields)
                    {
                        options.SearchField = sf;
                        items = items.Union(Adapter.GetGameFiles(options));
                    }

                    base.SetDataSource(items);
                }
                else
                {
                    base.SetDataSource(Adapter.GetGameFiles(options));
                }
            }
            else
            {
                base.SetGameFiles(searchFields);
            }
        }
    }
}
