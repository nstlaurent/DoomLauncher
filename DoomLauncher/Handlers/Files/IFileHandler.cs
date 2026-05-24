using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DoomLauncher.Handlers
{
    public interface IFileHandler
    {
        List<IFileData> GetFiles(IGameFile gameFile, params FileType[] fileTypes);
        List<IFileData> GetFilesTrimmed(IEnumerable<IGameFile> gameFiles, params FileType[] fileTypes);

        string GetFullFileName(FileType fileType, string fileNameNoPath);

        // Inserts a FileData under the GameFile, linked to the given in-memory image saved to disk. 
        IFileData InsertAndSave(IGameFile gameFile, FileType fileType, Image image, string extension, Action<IFileData> editBeforeSave);

        // Inserts a FileData under the GameFile, linked to a managed version of the given file on disk, copied from an arbitrary file.
        IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string originalFile, Action<IFileData> editBeforeSave);

        // Inserts a FileData under the GameFile, linked to a managed version of the given file on disk, deleting the original.
        IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string originalFile, Action<IFileData> editBeforeSave);

        // Inserts a FileData under the GameFile, linked to an unmanaged external file on disk.
        IFileData InsertAndRefer(IGameFile gameFile, FileType fileType, string originalFile, Action<IFileData> editBeforeSave);

        void UpdateFromOriginal(string originalDir, IFileData localFile, Action<IFileData> editBeforeSave);

        void DeleteFile(IFileData localFile);

        void DeleteFiles(IGameFile gameFile, FileType? fileType = null);
    }

    public static class IFileHandlerExtensions
    {
        public static IFileData InsertAndSave(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, Image image, string extension) =>
            fileHandler.InsertAndSave(gameFile, fileType, image, extension, x => { });

        public static IFileData InsertAndCopy(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string originalFile) =>
            fileHandler.InsertAndCopy(gameFile, fileType, originalFile, x => { });

        public static IFileData InsertAndMove(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string originalFile) =>
            fileHandler.InsertAndMove(gameFile, fileType, originalFile, x => { });

        public static IFileData InsertAndRefer(this IFileHandler fileHandler, IGameFile gameFile, FileType fileType, string originalFile) =>
            fileHandler.InsertAndRefer(gameFile, fileType, originalFile, x => { });

        public static void UpdateFromOriginal(this IFileHandler fileHandler, string originalDir, IFileData localFile) =>
            fileHandler.UpdateFromOriginal(originalDir, localFile, x => { });
    }
}
