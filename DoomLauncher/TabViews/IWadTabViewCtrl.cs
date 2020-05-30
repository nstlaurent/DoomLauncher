using System;
using System.Collections.Generic;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public partial class IWadTabViewCtrl : LocalTabViewCtrl
    {
        private readonly IDataSourceAdapter m_dsAdapter;

        public IWadTabViewCtrl(object key, string title, IDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagMapLookup lookup, GameFileViewFactory factory)
            : base(key, title, adapter, selectFields, lookup, factory)
        {
            InitializeComponent();
            m_dsAdapter = adapter;
        }

        public override void SetGameFiles()
        {
            base.SetDataSource(m_dsAdapter.GetGameFileIWads());
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            throw new NotSupportedException();
        }

        public override bool IsSearchAllowed { get { return false; } }
        protected override bool FilterIWads { get { return false; } }
    }
}
