using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;


namespace DoomLauncher.Handlers
{
    public interface IFileHandler
    {
        List<IFileData> GetFiles(IGameFile gameFile, FileType fileType);

        FileInfo GetFileInfo(FileType fileType, string fileName);

        IFileData InsertFromMemory(IGameFile gameFile, FileType fileType, MemoryStream fileStream, string extension, Action<IFileData> editBeforeSave);

        IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave);

        IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave);

        void DeleteFile(IFileData file);

        void DeleteFiles(IGameFile gameFile, FileType fileType);
    }

    public static class IFileHandlerExtensions
    {
        public static IFileData InsertFromMemory(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, MemoryStream fileStream, string extension) =>
            fileHandler.InsertFromMemory(gameFile, fileType, fileStream, extension, x => { });

        public static IFileData InsertAndCopy(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string file) =>
            fileHandler.InsertAndCopy(gameFile, fileType, file, x => { });

        public static IFileData InsertAndMove(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string file) =>
            fileHandler.InsertAndMove(gameFile, fileType, file, x => { });
    }
}
