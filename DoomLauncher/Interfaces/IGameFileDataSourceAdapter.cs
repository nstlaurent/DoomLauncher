using System.Collections.Generic;

namespace DoomLauncher.Interfaces
{
    public interface IGameFileDataSourceAdapter
    {
        int GetGameFilesCount();
        IEnumerable<IGameFile> GetGameFiles();
        IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options);
        IEnumerable<IGameFile> GetGameFileIWads();
        IEnumerable<IGameFile> GetUntaggedGameFiles();
        IEnumerable<string> GetGameFileNames();
        IGameFile GetGameFile(string fileName);
        void InsertGameFile(IGameFile gameFile);
        void UpdateGameFile(IGameFile gameFile);
        void UpdateGameFile(IGameFile gameFile, GameFileFieldType[] updateFields);
        void DeleteGameFile(IGameFile gameFile);
    }
}
