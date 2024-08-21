using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher.Handlers
{
    public class FileLoadHandler
    {
        private enum AddFilesType
        {
            SourcePort,
            IWAD
        }

        private List<IGameFile> m_iwadAdditionalFiles;
        private List<IGameFile> m_sourcePortAdditionalFiles;
        private List<IGameFile> m_saveAdditionalFiles = new List<IGameFile>();

        private List<IGameFile> m_currentFiles = new List<IGameFile>();
        private List<IGameFile> m_currentNewFiles = new List<IGameFile>();

        private readonly IDataSourceAdapter m_adapter;
        private readonly IGameFile m_gameFile;
        private readonly IGameProfile m_gameProfile;
        private IGameFile m_selectedIWad;
        private ISourcePortData m_selectedSourcePort;

        public FileLoadHandler(IDataSourceAdapter adapter, IGameFile gameFile)
            : this (adapter, gameFile, (GameFile)gameFile)
        {
        }

        public FileLoadHandler(IDataSourceAdapter adapter, IGameFile gameFile, IGameProfile gameProfile)
        {
            m_adapter = adapter;
            m_gameFile = gameFile;
            m_gameProfile = gameProfile;

            // Load gameProfile's "SettingsFiles" GameFiles from the database,
            // add gameFile, and set them to m_currentFiles and m_saveCurrentFiles.
            SetAdditionalFiles(Util.GetAdditionalFiles(m_adapter, gameProfile));

            // If gameProfile has an associated IWAD, load the IWAD GameFile, load its "SettingsFiles" GameFiles,
            // removing those that are in its "SettingsFilesSourcePort" GameFiles.
            //
            // Otherwise load the gameProfile's "SettingsFilesIWAD" GameFiles.
            m_iwadAdditionalFiles = GetIWadFilesFromGameFile(gameProfile);

            // Load the "SettingsFilesSourcePort" GameFiles of gameProfile.
            m_sourcePortAdditionalFiles = Util.GetSourcePortAdditionalFiles(m_adapter, gameProfile);
        }

        private List<IGameFile> GetIWadFilesFromGameFile(IGameProfile gameFile)
        {
            if (gameFile.IWadID.HasValue)
            {
                var gameFileIwad = m_adapter.GetGameFileIWads().FirstOrDefault(x => x.IWadID == gameFile.IWadID.Value);
                if (gameFileIwad != null)
                    return GetAdditionalFiles(AddFilesType.IWAD, gameFileIwad, null);
            }

            return Util.GetIWadAdditionalFiles(m_adapter, gameFile);
        }

        public bool IsIWadFile(IGameFile gameFile)
        {
            return m_iwadAdditionalFiles.Contains(gameFile);
        }

        public bool IsSourcePortFile(IGameFile gameFile)
        {
            return m_sourcePortAdditionalFiles.Contains(gameFile);
        }

        public List<IGameFile> GetCurrentAdditionalFiles()
        {
            return m_currentFiles.ToList();
        }

        public List<IGameFile> GetCurrentAdditionalNewFiles()
        {
            return m_currentNewFiles.ToList();
        }

        public List<IGameFile> GetIWadFiles()
        {
            return m_iwadAdditionalFiles.ToList();
        }

        public List<IGameFile> GetSourcePortFiles()
        {
            return m_sourcePortAdditionalFiles.ToList();
        }

        // Reset m_currentFiles and m_saveAdditionalFiles back to the state they were in after construction.
        public void Reset()
        {
            SetAdditionalFiles(m_saveAdditionalFiles);
        }

        private void SetAdditionalFiles(IEnumerable<IGameFile> gameFiles)
        {
            //In pervious versions you were not able to control the order the current file. If it doesn't exist in the list add it.
            if (!gameFiles.Contains(m_gameFile))
            {
                List<IGameFile> setFiles = new List<IGameFile>();
                setFiles.Add(m_gameFile);
                setFiles.AddRange(gameFiles);
                gameFiles = setFiles.Distinct();
            }

            m_currentFiles = gameFiles.ToList();
            m_saveAdditionalFiles = gameFiles.ToList();
        }

        // Recalculate m_currentFiles, m_iwadPortAdditionalFiles and m_sourcePortAdditionalFiles
        // based on the given IWAD and SourcePort, also using the IWAD, SourcePort and results
        // from the previous CalculateAdditionalFiles() call.
        //
        // Also records the difference from the existing m_currentFiles, and records the two 
        // parameters for next time.
        public void CalculateAdditionalFiles(IGameFile iwad, ISourcePortData sourcePort)
        {
            // if iwad is provided, load the "SettingsFile" GameFiles from sourcePort into
            // m_sourcePortAdditionalFiles, and unless the iwad is m_gameFile,
            // load the iwad's "SettingsFile" GameFiles into m_iwadPortAdditionalFiles, subtracting the iwad's "SettingsFileSourcePort" GameFiles.
            // Exclude m_gameFile from both. 
            //
            // Otherwise clear m_iwadPortAdditionalFiles and m_sourcePortAdditionalFiles.
            SetExtraAdditionalFilesFromSettings(iwad, sourcePort);

            IGameFile lastIwad = m_selectedIWad;
            ISourcePortData lastSourcePort = m_selectedSourcePort;
            if (lastIwad == null)
                lastIwad = iwad;
            if (lastSourcePort == null)
                lastSourcePort = sourcePort;

            List<IGameFile> gameFiles = m_currentFiles;
            List<IGameFile> originalList = gameFiles.ToList();

            // Get the "SettingsFile" GameFiles from iwad, subtracting its
            // "SettingsFileSourcePort" GameFiles, adding the "SettingsFile" GameFiles
            // from sourcePort and removing m_gameFile
            List<IGameFile> newTypeFiles = GetAdditionalFiles(iwad, sourcePort);
            List<IGameFile> oldTypeFiles = GetAdditionalFiles(lastIwad, lastSourcePort);
            gameFiles.RemoveAll(x => oldTypeFiles.Contains(x));
            gameFiles.AddRange(newTypeFiles);

            // Any files that are still around from before should come first
            gameFiles = SortByOriginal(gameFiles, originalList);

            // If iwad has "SettingsFile" GameFiles that are not in "SettingsFileSourcePort", then  
            // make sure these come first
            gameFiles = CheckIwadFileOrder(gameFiles, iwad, sourcePort);

            // Remember the calculated game files, and which ones are new 
            m_currentFiles = gameFiles.Distinct().ToList();
            m_currentNewFiles = gameFiles.Except(originalList).ToList();

            // Remember the inputs for next time
            m_selectedIWad = iwad;
            m_selectedSourcePort = sourcePort;
        }

        private List<IGameFile> CheckIwadFileOrder(List<IGameFile> gameFiles, IGameFile iwad, ISourcePortData sourcePort)
        {
            var iwadAdditionalFiles = GetAdditionalFiles(AddFilesType.IWAD, iwad, sourcePort, removeIwad: false);

            if (iwadAdditionalFiles.Count == 0)
                return gameFiles;

            if (iwadAdditionalFiles[0].GameFileID == iwad.GameFileID)
                return gameFiles;

            gameFiles.RemoveAll(x => iwadAdditionalFiles.Contains(x));
            return GetAdditionalFiles(AddFilesType.IWAD, iwad, sourcePort).Union(gameFiles).ToList();
        }

        private void SetExtraAdditionalFilesFromSettings(IGameFile iwad, ISourcePortData sourcePort)
        {
            m_iwadAdditionalFiles.Clear();
            m_sourcePortAdditionalFiles.Clear();

            if (iwad != null)
            {
                if (!iwad.Equals(m_gameFile))
                    m_iwadAdditionalFiles = GetAdditionalFiles(AddFilesType.IWAD, iwad, sourcePort);
                m_sourcePortAdditionalFiles = GetAdditionalFiles(AddFilesType.SourcePort, iwad, sourcePort);

                m_iwadAdditionalFiles.Remove(m_gameFile);
                m_sourcePortAdditionalFiles.Remove(m_gameFile);
            }
        }

        private List<IGameFile> GetAdditionalFiles(IGameFile gameIwad, ISourcePortData sourcePort)
        {
            var iwadExclude = Util.GetSourcePortAdditionalFiles(m_adapter, (GameFile)gameIwad);
            return GetAdditionalFiles(AddFilesType.IWAD, gameIwad, sourcePort).Except(iwadExclude)
                .Union(GetAdditionalFiles(AddFilesType.SourcePort, gameIwad, sourcePort))
                .Except(new IGameFile[] { m_gameFile }).ToList();
        }

        private List<IGameFile> GetAdditionalFiles(AddFilesType type, IGameFile gameIwad, ISourcePortData sourcePort,
            bool removeIwad = true)
        {
            switch (type)
            {
                case AddFilesType.IWAD:
                    if (gameIwad != null)
                    {
                        var ret = Util.GetAdditionalFiles(m_adapter, (GameFile)gameIwad).Except(Util.GetSourcePortAdditionalFiles(m_adapter, (GameFile)gameIwad)).ToList();
                        if (removeIwad)
                            ret.Remove(gameIwad);
                        return ret;
                    }
                    break;
                case AddFilesType.SourcePort:
                    if (sourcePort != null)
                        return Util.GetAdditionalFiles(m_adapter, sourcePort);
                    break;
            }
            return new List<IGameFile>();
        }

        private List<IGameFile> SortByOriginal(List<IGameFile> gameFiles, List<IGameFile> originalList)
        {
            List<IGameFile> sortedList = new List<IGameFile>();

            foreach (var gameFile in originalList)
            {
                if (gameFiles.Contains(gameFile))
                    sortedList.Add(gameFile);
            }

            sortedList.AddRange(gameFiles.Except(sortedList));
            return sortedList;
        }
    }
}
