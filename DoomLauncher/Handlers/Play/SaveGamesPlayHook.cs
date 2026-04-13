
using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher.Handlers.Play
{
    public class SaveGamesPlayHook : PlayHook
    {
        private List<IFileData> m_saveGames;
        private readonly SaveGameHandler m_saveGameHandler;
        private readonly IDirectoriesConfiguration m_config;
        private readonly List<INewFileDetector> m_saveFileDetectors;


        public SaveGamesPlayHook(SaveGameHandler saveGameHandler)
        {
            m_saveGameHandler = saveGameHandler;
        }

        public void BeforePlay(IGameFile gameFile, ISourcePortData sourcePort)
        {
            var fileHandler = new FileHandler(m_database, m_config);
            m_saveGames = fileHandler.GetFiles(gameFile, FileType.SaveGame).Where(x => x.SourcePortID == sourcePort.SourcePortID).ToArray();

            m_saveGames.ToList().ForEach(file => m_saveGameHandler.CopySaveGameToSourcePort(sourcePort, file));
        }

        public void AfterPlay(IGameFile gameFile, ISourcePortData sourcePort)
        {

            var newSaveGames = GetNewSaveGames(m_saveFileDetectors, m_saveGames).ToList();
            newSaveGames.ForEach(file => m_saveGameHandler.InsertSaveGame(sourcePort, gameFile, file));

            var updatedSaveGames = m_saveGames.ToList();
            updatedSaveGames.ForEach(file => m_saveGameHandler.UpdateSaveGameFromSourcePort(sourcePort, file));

            var deletedSaveGames = GetDeletedSaveGames(m_saveFileDetectors).ToList();
            deletedSaveGames.ForEach(file => m_saveGameHandler.DeleteSaveGame(file, m_saveGames));
        }
        private static string[] GetNewSaveGames(List<INewFileDetector> saveFileDetectors, List<IFileData> existingSaves)
        {
            IEnumerable<string> newFiles = new string[] { };
            saveFileDetectors.ForEach(x => newFiles = newFiles.Union(x.GetNewFiles()));

            IEnumerable<string> modifiedFiles = new string[] { };
            saveFileDetectors.ForEach(x => modifiedFiles = modifiedFiles.Union(x.GetModifiedFiles()));

            //modified files uses full path, m_saveGames does not. This section checks for modified files that were not part of the gamefile's save games
            //e.g save0.zds was a save game for this gamefile. User overwrites save1.zds for this gamefile. We now need to keep track of save1.zds as well.
            IEnumerable<string> saveFiles = existingSaves.Select(x => x.OriginalFileName);
            List<string> ret = newFiles.ToList();
            foreach (string modifiedFile in modifiedFiles)
            {
                FileInfo fi = new FileInfo(modifiedFile);
                if (!saveFiles.Contains(fi.Name) && !ret.Contains(fi.Name))
                    ret.Add(modifiedFile);
            }

            return ret.ToArray();
        }

        private static string[] GetDeletedSaveGames(List<INewFileDetector> saveFileDetectors)
        {
            IEnumerable<string> deletedFiles = new string[] { };
            saveFileDetectors.ForEach(x => deletedFiles = deletedFiles.Union(x.GetDeletedFiles()));
            return deletedFiles.ToArray();
        }

        private void CreateFileDetectors(ISourcePortData sourcePortData)
        {
            ISourcePortFlavor sourcePortFlavor = sourcePortData.GetFlavor();
            //CreateScreenshotDetectors(sourcePortData, sourcePortFlavor);
            CreateSaveGameDetectors(sourcePortData, sourcePortFlavor);
        }

        private void CreateSaveGameDetectors(ISourcePortData sourcePortData, ISourcePortFlavor sourcePort)
        {
            m_saveFileDetectors = CreateDefaultSaveGameDetectors();
            m_saveFileDetectors.Add(CreateSaveGameDetector(sourcePortData.GetReadSavePath().GetFullPath()));

            foreach (var saveDir in sourcePort.GetSaveGameDirectories())
            {
                if (!string.IsNullOrEmpty(saveDir) && Directory.Exists(saveDir))
                    m_saveFileDetectors.Add(CreateSaveGameDetector(saveDir));
            }

            Array.ForEach(m_saveFileDetectors.ToArray(), x => x.StartDetection());
        }

    }
}
