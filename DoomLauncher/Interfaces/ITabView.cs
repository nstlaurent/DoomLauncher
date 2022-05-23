using System;
using System.Collections.Generic;

namespace DoomLauncher.Interfaces
{
    public interface ITabView : ICloneable
    {
        event EventHandler<GameFileListEventArgs> DataSourceChanging;
        event EventHandler<GameFileListEventArgs> DataSourceChanged;

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
        string Title { get; set; }
        object Key { get; }
        IGameFileDataSourceAdapter Adapter { get; set; }
        IGameFileView GameFileViewControl { get; }
    }
}
