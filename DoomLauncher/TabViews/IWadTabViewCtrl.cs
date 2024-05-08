using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher
{
    public partial class IWadTabViewCtrl : LocalTabViewCtrl
    {
        private static readonly GameFileFieldType[] SearchFields = new[] { GameFileFieldType.GameFileID, GameFileFieldType.Filename };
        private readonly IDataSourceAdapter m_dsAdapter;

        public IWadTabViewCtrl(object key, string title, IDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagMapLookup lookup, GameFileViewFactory factory)
            : base(key, title, adapter, selectFields, lookup, factory)
        {
            InitializeComponent();
            m_dsAdapter = adapter;
        }

        public override void SetGameFiles()
        {
            SetDataSource(m_dsAdapter.GetGameFileIWads());
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            var gameFileIWads = m_dsAdapter.GetGameFileIWads();
            IEnumerable<IGameFile> items = Enumerable.Empty<IGameFile>();
            foreach (GameFileSearchField sf in searchFields)
                items = items.Union(Adapter.GetGameFiles(new GameFileGetOptions(SearchFields, sf)));

            gameFileIWads = gameFileIWads.Where(x => items.Any(y => x.GameFileID == y.GameFileID));
            SetDataSource(gameFileIWads);
        }

        public override bool IsSearchAllowed => true;
        protected override bool FilterIWads => false;
    }
}
