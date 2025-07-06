using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.IO;


namespace DoomLauncher.Handlers
{
    public interface IFileHandler
    {
        List<IFileData> GetFiles(IGameFile gameFile, FileType fileType);

        FileInfo GetFileInfo(FileType fileType, string fileName);

        IFileData InsertFromMemory(IGameFile gameFile, FileType fileType, MemoryStream fileStream, string extension, ISourcePortData sourcePort = null);

        IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string file, ISourcePortData sourcePort = null);

        IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string file, ISourcePortData sourcePort = null);

        void DeleteFile(IFileData file);

        void DeleteFiles(IGameFile gameFile, FileType fileType);
    }
}
