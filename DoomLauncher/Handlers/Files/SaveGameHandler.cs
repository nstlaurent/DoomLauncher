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

        public void UpdateSaveGameFromSourcePort(ISourcePortData sourcePort, IFileData saveGame)
        {
            string sourcePortSavePath = sourcePort.GetReadSavePath().GetFullPath();

            m_fileHandler.UpdateFromOriginal(sourcePortSavePath, saveGame, f =>
            {
                f.Description = GetSaveGameName(sourcePort, f.FullFileName);
            });
        }

        public void DeleteSaveGame(string originalFile, IFileData[] localFiles)
        {
            FileInfo originalFileInfo = new FileInfo(originalFile);
            IFileData matchingLocalFile = localFiles.FirstOrDefault(x => x.OriginalFileName == originalFileInfo.Name);
            if (matchingLocalFile != null)
                m_fileHandler.DeleteFile(matchingLocalFile);
        }

        public void CopySaveGameToSourcePort(ISourcePortData sourcePort, IFileData localFile)
        {
            LauncherPath sourcePortReadPath = sourcePort.GetReadSavePath();
            string originalFileName = sourcePortReadPath.GetFullPath(localFile.OriginalFileName);
            FileInfo localFileInfo = new FileInfo(localFile.FullFileName);

            try
            {
                if (localFileInfo.Exists)
                {
                    DirectoryInfo di = new DirectoryInfo(sourcePortReadPath.GetFullPath());
                    if (!di.Exists)
                        di.Create();

                    localFileInfo.CopyTo(originalFileName, true);
                }
            }
            catch
            {
                //failed, nothing to do
            }
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
    }
}
