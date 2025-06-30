using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System;
using System.IO;

namespace DoomLauncher.Handlers
{
    // Atomically keep the various files attached to the GameFile and the database in sync
    public class FileHandler : IFileHandler
    {
        private readonly IDataSourceAdapter m_database;
        private readonly IDirectoriesConfiguration m_config;

        public FileHandler(IDataSourceAdapter database, IDirectoriesConfiguration config)
        {
            m_database = database;
            m_config = config;
        }

        public IFileData InsertFileFromMemory(IGameFile gameFile, FileType fileType, MemoryStream fileStream, string extension, ISourcePortData sourcePort = null)
        {
            if (gameFile == null)
                throw new ArgumentNullException("GameFile can't be null");

            if (!gameFile.GameFileID.HasValue)
                throw new ArgumentException("GameFile must be persistent");

            string fileName = GetUniqueFileName(extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);

            IFileData createdFile;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                    fileStream.WriteTo(fs);

                createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, sourcePort);
            }
            catch (Exception e)
            {
                createdFile = null;
                File.Delete(path);
            }
            return createdFile;
        }

        private string GetUniqueFileName(string extension) => 
            $"{Guid.NewGuid()}.{extension}";

        private IFileData InsertDatabaseRecord(IGameFile gameFile, FileType fileType, string fileName, ISourcePortData sourcePort)
        {
            IFileData fileData = new FileData
            {
                FileName = fileName,
                GameFileID = gameFile.GameFileID.Value,
                SourcePortID = sourcePort?.SourcePortID ?? -1,
                FileTypeID = fileType,
                FileOrder = 0
            };

            m_database.InsertFile(fileData);
            m_database.IncrementFileOrder(gameFile, FileType.Screenshot);

            return fileData;
        }

        public IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string file, ISourcePortData sourcePort = null)
        {
            //FileInfo fi = new FileInfo(file);
            //string fileName = Guid.NewGuid().ToString() + fi.Extension;
            //fi.CopyTo(Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), fileName));
            return null;

        }

        public IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string[] files, ISourcePortData sourcePort = null)
        {
            return null;
        }

        public bool DeleteFile(IFileData file)
        {
            return false;
        }
    }

    public interface IFileException { }

    public class IFileInsertException : IFileException { }
}
