using System;

namespace DoomLauncher.Interfaces
{
    public interface ITagMapLookup
    {
        event EventHandler<ITagData[]> TagMappingChanged;
        void Refresh(ITagData[] tags);
        ITagData[] GetTags(IGameFile gameFile);
    }
}
