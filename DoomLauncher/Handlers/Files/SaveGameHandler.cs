using DoomLauncher.Config;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using DoomLauncher.SaveGame;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class SaveGameHandler
    {
        private readonly IFileHandler m_fileHandler;

        public SaveGameHandler(IDataSourceAdapter database, IDirectoriesConfiguration config)
        {
            m_fileHandler = new FileHandler(database, config);
            DataSourceAdapter = database;
            SaveGameDirectory = config.SaveGameDirectory;
        }

        public IFileData InsertSaveGame(ISourcePortData sourcePort, IGameFile gameFile, string file)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            return m_fileHandler.InsertAndCopy(gameFile, FileType.SaveGame, file, f =>
            {
                f.Description = GetSaveGameName(sourcePort, file);
                f.SourcePortID = sourcePort.SourcePortID;
            });
        }

        private static string GetSaveGameName(ISourcePortData sourcePort, string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            ISaveGameReader reader = sourcePort.GetFlavor().CreateSaveGameReader(fileInfo);

            if (reader != null)
                return reader.GetName();
            else 
                return fileInfo.Name;
        }

        public void HandleUpdateSaveGames(ISourcePortData sourcePort, IGameFile gameFile, IFileData file)
        {
            FileInfo fi = new FileInfo(sourcePort.GetReadSavePath().GetFullPath(file.OriginalFileName));

            if (fi.Exists && file.DateCreated != fi.LastWriteTime)
            {
                try
                {
                    fi.CopyTo(SaveGameDirectory.GetFullPath(file.FileName), true);
                }
                catch
                {
                    //failed, nothing to do
                }

                //check to see if the save name changed
                string saveName = GetSaveGameName(sourcePort, fi.FullName);
                if (saveName != file.Description)
                    file.Description = saveName;

                file.DateCreated = fi.LastWriteTime;
                DataSourceAdapter.UpdateFile(file);
            }
        }

        public void HandleDeleteSaveGames(string[] deletedFiles, IFileData[] previousFiles)
        {
            foreach (var file in deletedFiles)
            {
                FileInfo fi = new FileInfo(file);
                IFileData saveFile = previousFiles.FirstOrDefault(x => x.OriginalFileName == fi.Name);
                if (saveFile != null)
                    DataSourceAdapter.DeleteFile(saveFile);
            }
        }

        public void CopySaveGamesToSourcePort(ISourcePortData sourcePort, IFileData[] files)
        {
            foreach (IFileData file in files)
            {
                string savePath = sourcePort.GetReadSavePath().GetFullPath();
                string fileName = Path.Combine(sourcePort.GetReadSavePath().GetFullPath(), file.OriginalFileName);
                FileInfo fiFrom = new FileInfo(Path.Combine(SaveGameDirectory.GetFullPath(), file.FileName));

                try
                {
                    if (fiFrom.Exists)
                    {
                        DirectoryInfo di = new DirectoryInfo(savePath);

                        if (!di.Exists)
                            di.Create();

                        fiFrom.CopyTo(fileName, true);
                    }
                }
                catch
                {
                    //failed, nothing to do
                }
            }
        }

        public LauncherPath SaveGameDirectory { get; set; }
        public IDataSourceAdapter DataSourceAdapter { get; set; }
    }
}
