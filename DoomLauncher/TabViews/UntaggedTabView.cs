using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher
{
    class UntaggedTabView : LocalTabViewCtrl
    {
        public UntaggedTabView(object key, string title, IGameFileDataSourceAdapter adapter, GameFileFieldType[] selectFields, ITagMapLookup lookup, GameFileViewFactory factory)
            : base(key, title, adapter, selectFields, lookup, factory)
        {

        }

        public override void SetGameFiles()
        {
            SetDataSource(Adapter.GetUntaggedGameFiles());
        }

        public override void SetGameFiles(IEnumerable<GameFileSearchField> searchFields)
        {
            IEnumerable<IGameFile> items = new IGameFile[0];
            var untaggedFiles = Adapter.GetUntaggedGameFiles();

            foreach (GameFileSearchField sf in searchFields)
            {
                var search = Adapter.GetGameFiles(new GameFileGetOptions(m_selectFields, sf));
                items = items.Union(untaggedFiles.Intersect(search));
            }

            SetDataSource(items);
        }
    }
}
