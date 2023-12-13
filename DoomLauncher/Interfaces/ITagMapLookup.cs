using System;
using System.Collections.Generic;

namespace DoomLauncher.Interfaces
{
    public interface ITagMapLookup
    {
        event EventHandler<ITagData[]> TagMappingChanged;
        void Refresh(ITagData[] tags);
        void RemoveGameFile(IGameFile gameFile);
        ITagData[] GetTags(IGameFile gameFile);
        IEnumerable<ITagData> GetEnumerableTags(IGameFile gameFile);
    }
}
