using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Archive;
using DoomLauncher.Config;

namespace DoomLauncher
{
    public class SyncLibraryHandler
    {
        public delegate void SyncProgressHandler(SyncProgressEvent e);
        public event SyncProgressHandler SyncFileChanged;

        public delegate void GameFileDataNeededHandler(GameFileDataNeededEvent e);
        public event GameFileDataNeededHandler GameFileDataNeeded;

        private readonly IGameFileDataSourceAdapter m_dbDataSource;
        private readonly IGameFileDataSourceAdapter m_syncDataSource;
        private readonly FileManagement m_fileManagement;
        private readonly List<ISyncAction> m_syncActions;
        private readonly IDirectoriesConfiguration m_directories;

        public SyncLibraryHandler(IGameFileDataSourceAdapter dbDataSource, IGameFileDataSourceAdapter syncDataSource,
            IDirectoriesConfiguration directories, FileManagement fileManagement, List<ISyncAction> syncActions)
        {
            m_dbDataSource = dbDataSource;
            m_syncDataSource = syncDataSource;
            m_syncActions = syncActions;
            m_fileManagement = fileManagement;
            m_directories = directories;
        }

        public SyncResult SyncManyFiles(string[] files)
        {
            int syncFileCurrent = 0;
            SyncResult result = SyncResult.EMPTY;

            foreach (string fileName in files)
            {
                SyncFileChanged?.Invoke(new SyncProgressEvent(fileName, syncFileCurrent, files.Length));
                result += SyncFile(fileName);
                syncFileCurrent++;
            }

            return result;
        }

        public SyncResult SyncFile(string fileName)
        {
            SyncResult resultSoFar = PrepareGameFileForUpdate(fileName, out var existingFile, out var fileToUpdate);
            if (resultSoFar.Failed)
                return resultSoFar;

            GameFileDataNeeded?.Invoke(new GameFileDataNeededEvent(fileToUpdate));

            try
            {
                using (IArchiveReader reader = new RecursiveArchiveReader(CreateRootArchiveReader(fileToUpdate), CreateBranchArchiveReader))
                {
                    resultSoFar += PopulateGameFileFromArchive(fileToUpdate, reader);
                }
            }
            catch (IOException)
            {
                fileToUpdate.Map = string.Empty;
                resultSoFar += SyncResult.InvalidFile(fileName, "File is in use/Not found");
            }
            catch (InvalidDataException)
            {
                fileToUpdate.Map = string.Empty;
                resultSoFar += SyncResult.InvalidFile(fileName, "Zip archive invalid or contained an improper pk3");
            }
            catch (Exception ex)
            {
                fileToUpdate.Map = string.Empty;
                var errorMsg = string.Concat("Unexpected exception - ", ex.Message, ex.StackTrace);
                resultSoFar += SyncResult.InvalidFile(fileName, errorMsg);
            }

            resultSoFar += Upsert(existingFile, fileToUpdate);

            return resultSoFar;
        }

        private SyncResult PrepareGameFileForUpdate(string fileName, out IGameFile existingFile, out IGameFile fileToUpdate)
        {
            var resultSoFar = SyncResult.EMPTY;

            fileToUpdate = m_syncDataSource.GetGameFile(fileName);
            existingFile = m_dbDataSource.GetGameFile(fileName);

            // If we've already got a copy in the DB, modify that one
            if (existingFile != null)
                fileToUpdate = existingFile;

            if (fileToUpdate == null)
            {
                // Delete the file from managed storage
                try
                {
                    FileInfo fileDelete = new FileInfo(Path.Combine(m_directories.GameFileDirectory.GetFullPath(), fileName));
                    if (fileDelete.Exists)
                        fileDelete.Delete();
                }
                catch
                {
                    //delete failed, just keep going
                }
                return SyncResult.InvalidFile(fileName, "Not found");
            }

            if (m_fileManagement == FileManagement.Unmanaged)
                fileToUpdate.FileName = LauncherPath.GetRelativePath(fileName);

            fileToUpdate.Downloaded = existingFile == null ? DateTime.Now : existingFile.Downloaded;

            return SyncResult.EMPTY;
        }

        private SyncResult Upsert(IGameFile existingFile, IGameFile fileToUpdate)
        {
            if (existingFile == null)
            {
                m_dbDataSource.InsertGameFile(fileToUpdate);

                IGameFile gameFile = m_dbDataSource.GetGameFile(fileToUpdate.FileName);
                return (gameFile != null) ? SyncResult.AddedGameFile(gameFile) : SyncResult.EMPTY;
            }
            else
            {
                m_dbDataSource.UpdateGameFile(fileToUpdate, Util.DefaultGameFileUpdateFields);
                return SyncResult.UpdatedGameFile(fileToUpdate);
            }
        }

        private SyncResult PopulateGameFileFromArchive(IGameFile gameFile, IArchiveReader reader)
        {
            string[] mapInfoData = MapInfoUtil.GetMapInfoData(reader);
            var syncResults = m_syncActions.Select(frag => frag.ApplyToGameFile(gameFile, reader, mapInfoData));
            return syncResults.Aggregate(SyncResult.EMPTY, (a, b) => a + b);
        }

        private IArchiveReader CreateRootArchiveReader(IGameFile file)
        {
            if (m_fileManagement == FileManagement.Unmanaged)
                return ArchiveReader.Create(new LauncherPath(file.FileName).GetFullPath());

            return ArchiveReader.Create(Path.Combine(m_directories.GameFileDirectory.GetFullPath(), file.FileName));
        }

        private IArchiveReader CreateBranchArchiveReader(IArchiveEntry entry)
        {
            if (IsRecursiveEntry(entry))
            {
                string extractedFile = Util.ExtractTempFile(m_directories.TempDirectory.GetFullPath(), entry);
                return ArchiveReader.Create(extractedFile);
            }
            else
            {
                return null;
            }
        }

        private bool IsRecursiveEntry(IArchiveEntry entry)
        {
            List<string> recursiveExtensions = new List<string>(Util.GetReadablePkExtensions()).Append(".wad").ToList();
            return entry.Name.Contains('.') && recursiveExtensions.Exists(ext => ext.Equals(Path.GetExtension(entry.Name), StringComparison.OrdinalIgnoreCase));
        }

        public class SyncProgressEvent
        {
            public string CurrentSyncFileName { get; }
            public int SyncFileCurrent { get; }
            public int SyncFileCount { get; }

            public SyncProgressEvent(string currentSyncFileName, int syncFileCurrent, int syncFileCount)
            {
                CurrentSyncFileName = currentSyncFileName;
                SyncFileCurrent = syncFileCurrent;
                SyncFileCount = syncFileCount;
            }
        }

        public class GameFileDataNeededEvent
        {
            public IGameFile CurrentGameFile { get; }

            public GameFileDataNeededEvent(IGameFile currentGameFile)
            {
                CurrentGameFile = currentGameFile;
            }
        }
    }
}
