using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        public List<IFileData> GetFiles(IGameFile gameFile, params FileType[] fileTypes)
        {
            // TODO this could be one database query, maybe
            List<IFileData> files = new List<IFileData>();
            foreach (var fileType in fileTypes)
            {
                files.AddRange(m_database.GetFiles(gameFile, fileType));
            }

            // FullFileName is a derived field that needs to be set after retrieval
            files.ForEach(file => 
                file.FullFileName = GetFullFileName(file.FileTypeID, file.FileName));

            return files;
        }

        public List<IFileData> GetFilesTrimmed(IEnumerable<IGameFile> gameFiles, params FileType[] fileTypes)
        {
            var files = m_database.GetFilesTrimmed(gameFiles, fileTypes).ToList();
            foreach (var file in files)
                file.FullFileName = GetFullFileName(file.FileTypeID, file.FileName);
            return files;
        }

        public string GetFullFileName(FileType fileType, string fileNameNoPath) => 
            m_config.GetFileDirectory(fileType).GetFullPath(fileNameNoPath);

        public IFileData InsertAndSave(IGameFile gameFile, FileType fileType, Image image, string extension, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            string fileName = GetUniqueFileName(extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);

            IFileData createdFile;
            try
            {
                using (var imageStream = new MemoryStream())
                {
                    image.Save(imageStream, ImageFormat.Png);

                    using (FileStream fs = new FileStream(path, FileMode.Create))
                        imageStream.WriteTo(fs);
                }

                createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, null, editBeforeSave);
            }
            catch (Exception)
            {
                createdFile = null;
                File.Delete(path);
            }
            return createdFile;
        }

        public IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string originalFile, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            FileInfo originalFileInfo = new FileInfo(originalFile);
            string fileName = GetUniqueFileName(originalFileInfo.Extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);
            
            IFileData createdFile = null;
            if (originalFileInfo.Exists)
            {
                try
                {
                    originalFileInfo.CopyTo(path);
                    createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, originalFile, editBeforeSave);
                }
                catch (Exception)
                {
                    
                    if (File.Exists(path))
                        File.Delete(path);
                }
            }
            return createdFile;
        }

        public IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string originalFile, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            FileInfo originalFileInfo = new FileInfo(originalFile);
            string fileName = GetUniqueFileName(originalFileInfo.Extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);

            IFileData createdFile = null;
            if (originalFileInfo.Exists)
            {
                try
                {
                    originalFileInfo.MoveTo(path);
                    createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, originalFile, editBeforeSave);
                }
                catch (Exception)
                {
                    if (File.Exists(path) && File.Exists(originalFile))
                        File.Delete(path);
                }
            }
            return createdFile;
        }

        public IFileData InsertAndRefer(IGameFile gameFile, FileType fileType, string originalFile, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue || !fileType.IsFixedContent())
                return null;

            FileInfo originalFileInfo = new FileInfo(originalFile);
            string fileName = Path.GetFileName(originalFile);

            IFileData createdFile = null;
            
            // Although we're not doing anything to the file, it needs to exist in the place we expect it.
            if (originalFileInfo.Exists && File.Exists(GetFullFileName(fileType, fileName)))
            {
                createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, originalFile, editBeforeSave);
            }
            return createdFile;
        }

        public void UpdateFromOriginal(string originalDir, IFileData localFile, Action<IFileData> editBeforeSave)
        {
            var originalFileInfo = new FileInfo(Path.Combine(originalDir, localFile.OriginalFileName));
            if (originalFileInfo.Exists && localFile.DateCreated < originalFileInfo.LastWriteTime)
            {
                try
                {
                    originalFileInfo.CopyTo(m_config.GetFileDirectory(localFile.FileTypeID).GetFullPath(localFile.FileName), true);
                }
                catch
                {
                    // failed, nothing to do
                }
                localFile.DateCreated = originalFileInfo.LastWriteTime;
                editBeforeSave(localFile);
                m_database.UpdateFile(localFile);
            }
        }

        public void DeleteFile(IFileData file)
        {
            // Can't delete a remote file...
            if (file.IsUrl)
                return;

            if (!file.FileTypeID.IsFixedContent())
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
            }

            m_database.DeleteFile(file);

            var derivedFiles = m_database.GetDerivedFiles(file).ToList();
            foreach (var derivedFile in derivedFiles)
                DeleteFile(derivedFile);
        }

        public void DeleteFiles(IGameFile gameFile, FileType? fileType = null)
        {
            var filesToDelete = fileType.HasValue ? m_database.GetFiles(gameFile, fileType.Value) : m_database.GetFiles(gameFile);
            foreach (var file in filesToDelete)
                DeleteFile(file);
        }

        private string GetUniqueFileName(string extension)
        {
            if (!extension.StartsWith("."))
                extension = $".{extension}";

            return $"{Guid.NewGuid()}{extension}";
        }

        private IFileData InsertDatabaseRecord(IGameFile gameFile, FileType fileType, string fileName, string originalFile, Action<IFileData> editBeforeSave)
        {
            var originalFileInfo = originalFile != null ? new FileInfo(originalFile) : null;
            IFileData fileData = new FileData
            {
                FileName = fileName,
                FullFileName = GetFullFileName(fileType, fileName), // Not stored, this is for the return value
                OriginalFileName = originalFileInfo?.Name,
                GameFileID = gameFile.GameFileID.Value,
                FileTypeID = fileType,
                FileOrder = 0,
                DateCreated = DateTime.Now
            };

            editBeforeSave(fileData);
            m_database.InsertFile(fileData);

            return fileData;
        }
    }
}
