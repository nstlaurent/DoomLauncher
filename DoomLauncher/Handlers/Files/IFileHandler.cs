using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace DoomLauncher.Handlers
{
    public interface IFileHandler
    {
        List<IFileData> GetFiles(IGameFile gameFile, FileType fileType);

        FileInfo GetFileInfo(FileType fileType, string fileName);

        IFileData InsertFromMemory(IGameFile gameFile, FileType fileType, Image image, string extension, Action<IFileData> editBeforeSave);

        IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave);

        IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave);

        IFileData InsertAndRefer(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave);

        void DeleteFile(IFileData file);

        void DeleteFiles(IGameFile gameFile, FileType fileType);
    }

    public static class IFileHandlerExtensions
    {
        public static IFileData InsertFromMemory(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, Image image, string extension) =>
            fileHandler.InsertFromMemory(gameFile, fileType, image, extension, x => { });

        public static IFileData InsertAndCopy(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string file) =>
            fileHandler.InsertAndCopy(gameFile, fileType, file, x => { });

        public static IFileData InsertAndMove(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string file) =>
            fileHandler.InsertAndMove(gameFile, fileType, file, x => { });

        public static IFileData InsertAndRefer(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string file) =>
            fileHandler.InsertAndRefer(gameFile, fileType, file, x => { });
    }
}
