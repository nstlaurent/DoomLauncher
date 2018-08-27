using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface ITabView : ICloneable
    {
        void SetGameFiles();
        void SetGameFiles(IEnumerable<GameFileSearchField> searchFields);
        void SetGameFilesData(IEnumerable<IGameFile> gameFiles);
        void UpdateDataSourceFile(IGameFile gameFile);
        List<ColumnConfig> GetColumnConfig();
        void SetColumnConfig(ColumnField[] columnTextFields, ColumnConfig[] colConfig);
        bool IsLocal { get; }
        bool IsEditAllowed { get; }
        bool IsDeleteAllowed { get; }
        bool IsSearchAllowed { get; }
        bool IsPlayAllowed { get; }
        bool IsAutoSearchAllowed { get; }
        string Title { get; }
        object Key { get; }
        IGameFileDataSourceAdapter Adapter { get; set; }
        GameFileViewControl GameFileViewControl { get; }
    }
}
