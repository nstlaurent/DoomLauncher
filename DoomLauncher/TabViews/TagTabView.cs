using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher
{
    public partial class TagTabView : BasicTabViewCtrl
    {
        private IDataSourceAdapter m_tagAdapter;

        public TagTabView(object key, string title, IDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagData tag, GameFileViewFactory factory)
            : this(key, title, adapter, selectFields, tag, factory.CreateGameFileView())
        {
            m_factory = factory;
        }

        public TagTabView(object key, string title, IDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagData tag, IGameFileView view)
            : base(key, title, adapter, selectFields, view)
        {
            InitializeComponent();
            TagDataSource = tag;
            m_tagAdapter = adapter;
        }

        public override object Clone()
        {
            TagTabView view = new TagTabView(m_key, Title, m_tagAdapter, m_selectFields, TagDataSource, GameFileViewFactory.CreateGameFileViewGrid());
            SetBaseCloneProperties(view);
            return view;
        }

        

        public override void SetGameFiles()
        {
            GameFileGetOptions options = new GameFileGetOptions();
            options.SelectFields = m_selectFields;
            SetDataSource(m_tagAdapter.GetGameFiles(options, TagDataSource));
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            IEnumerable<IGameFile> items = new IGameFile[0];

            foreach (GameFileSearchField sf in searchFields)
            {
                items = items.Union(m_tagAdapter.GetGameFiles(new GameFileGetOptions(m_selectFields, sf), TagDataSource));
            }

            SetDataSource(items);
        }

        public ITagData TagDataSource
        {
            get;
            set;
        }
    }
}
