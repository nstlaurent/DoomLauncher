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

        public List<IFileData> GetFiles(IGameFile gameFile, FileType fileType)
        {
            return m_database.GetFiles(gameFile, fileType).ToList();
        }

        public FileInfo GetFileInfo(FileType fileType, string fileName)
        {
            return new FileInfo(m_config.GetFileDirectory(fileType).GetFullPath(fileName));
        }

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

                createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, editBeforeSave);
            }
            catch (Exception)
            {
                createdFile = null;
                File.Delete(path);
            }
            return createdFile;
        }


        public IFileData InsertAndCopy(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            FileInfo fi = new FileInfo(file);
            string fileName = GetUniqueFileName(fi.Extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);
            
            IFileData createdFile = null;
            if (fi.Exists)
            {
                try
                {
                    fi.CopyTo(path);
                    createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, editBeforeSave);
                }
                catch (Exception)
                {
                    
                    if (File.Exists(path))
                        File.Delete(path);
                }
            }
            return createdFile;
        }

        public IFileData InsertAndMove(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            FileInfo fi = new FileInfo(file);
            string fileName = GetUniqueFileName(fi.Extension);
            string path = m_config.GetFileDirectory(fileType).GetFullPath(fileName);

            IFileData createdFile = null;
            if (fi.Exists)
            {
                try
                {
                    fi.MoveTo(path);
                    createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, editBeforeSave);
                }
                catch (Exception)
                {
                    if (File.Exists(path) && File.Exists(file))
                        File.Delete(path);
                }
            }
            return createdFile;
        }

        public IFileData InsertAndRefer(IGameFile gameFile, FileType fileType, string file, Action<IFileData> editBeforeSave)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            FileInfo fi = new FileInfo(file);
            string fileName = Path.GetFileName(file);

            IFileData createdFile = null;
            
            // While we're not doing anything to the file, it needs to exist in the place we expect it.
            if (fi.Exists && GetFileInfo(fileType, fileName).Exists)
            {
                createdFile = InsertDatabaseRecord(gameFile, fileType, fileName, editBeforeSave);
            }
            return createdFile;
        }

        public void DeleteFile(IFileData file)
        {
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
            derivedFiles.ForEach(DeleteFile);
        }

        public void DeleteFiles(IGameFile gameFile, FileType fileType)
        {
            var filesToDelete = m_database.GetFiles(gameFile, fileType).ToList();
            filesToDelete.ForEach(DeleteFile);
        }

        private string GetUniqueFileName(string extension) =>
            $"{Guid.NewGuid()}.{extension}";

        private IFileData InsertDatabaseRecord(IGameFile gameFile, FileType fileType, string fileName, Action<IFileData> editBeforeSave)
        {
            IFileData fileData = new FileData
            {
                FileName = fileName,
                GameFileID = gameFile.GameFileID.Value,
                FileTypeID = fileType,
                FileOrder = 0
            };

            editBeforeSave(fileData);
            m_database.InsertFile(fileData);

            return fileData;
        }
    }
}
