namespace DoomLauncher.Interfaces
{
    public interface ITagMapLookup
    {
        void Refresh();
        ITagData[] GetTags(IGameFile gameFile);
    }
}
