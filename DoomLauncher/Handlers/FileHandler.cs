using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.IO;
using System.Linq;

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
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            string fileName = GetUniqueFileName(extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);

            IFileData createdFile;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                    fileStream.WriteTo(fs);

                createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, sourcePort);
            }
            catch (Exception)
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

            return fileData;
        }

        public IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string file, ISourcePortData sourcePort = null)
        {
            //FileInfo fi = new FileInfo(file);
            //string fileName = Guid.NewGuid().ToString() + fi.Extension;
            //fi.CopyTo(Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), fileName));
            return null;

        }

        public IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string files, ISourcePortData sourcePort = null)
        {
            return null;
        }

        public void DeleteFile(IFileData file)
        {
            string path = m_config.GetFileDirectory(file.FileTypeID).GetFullPath(file.FileName);
            try
            {
                FileInfo fi = new FileInfo(path);

                if (fi.Exists)
                    fi.Delete();
            }
            catch (IOException)
            {
                // File is in use, insert to delete on next startup
                m_database.InsertCleanupFile(new CleanupFile() { FileName = path });
            }

            m_database.DeleteFile(file);
        }

        public void DeleteAttachedFiles(IGameFile gameFile, FileType fileType)
        {
            var filesToDelete = m_database.GetFiles(gameFile, fileType).ToList();
            filesToDelete.ForEach(DeleteFile);
        }
    }
}
