using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher
{
    public partial class TagTabView : BasicTabViewCtrl
    {
        private IDataSourceAdapter m_tagAdapter;

        public TagTabView(object key, string title, IDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagData tag)
            : base(key, title, adapter, selectFields)
        {
            InitializeComponent();
            TagDataSource = tag;
            m_tagAdapter = adapter;
        }

        public override object Clone()
        {
            TagTabView view = new TagTabView(m_key, m_title, m_tagAdapter, m_selectFields, TagDataSource);
            base.SetBaseCloneProperties(view);
            return view;
        }

        public override void SetGameFiles()
        {
            GameFileGetOptions options = new GameFileGetOptions();
            options.SelectFields = m_selectFields;
            base.SetDataSource(m_tagAdapter.GetGameFiles(options, TagDataSource));
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            IEnumerable<IGameFile> items = new IGameFile[0];

            foreach (GameFileSearchField sf in searchFields)
            {
                items = items.Union(m_tagAdapter.GetGameFiles(new GameFileGetOptions(m_selectFields, sf), TagDataSource));
            }

            base.SetDataSource(items);
        }

        public ITagData TagDataSource
        {
            get;
            private set;
        }
    }
}
