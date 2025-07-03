using DoomLauncher.Interfaces;
using System.IO;


namespace DoomLauncher.Handlers
{
    public interface IFileHandler
    {
        IFileData InsertFileFromMemory(IGameFile gameFile, FileType fileType, MemoryStream fileStream, string extension, ISourcePortData sourcePort = null);
        
        void DeleteFile(IFileData file);

        void DeleteAttachedFiles(IGameFile gameFile, FileType fileType);
    }
}
