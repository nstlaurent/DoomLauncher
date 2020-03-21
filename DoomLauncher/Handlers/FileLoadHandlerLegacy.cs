using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Handlers
{
    public class FileLoadHandlerLegacy
    {
        private enum AddFilesType
        {
            SourcePort,
            IWAD
        }

        private List<IGameFile> m_iwadAdditionalFiles = new List<IGameFile>();
        private List<IGameFile> m_sourcePortAdditionalFiles = new List<IGameFile>();
        private List<IGameFile> m_saveAdditionalFiles = new List<IGameFile>();

        private List<IGameFile> m_currentFiles = new List<IGameFile>();
        private List<IGameFile> m_currentNewFiles = new List<IGameFile>();

        private readonly IDataSourceAdapter m_adapter;
        private readonly IGameFile m_gameFile;
        private IGameFile m_selectedIWad;
        private ISourcePortData m_selectedSourcePort;

        public FileLoadHandlerLegacy(IDataSourceAdapter adapter, IGameFile gameFile)
        {
            m_adapter = adapter;
            m_gameFile = gameFile;
            SetAdditionalFiles(Util.GetAdditionalFiles(m_adapter, (GameFile)gameFile));
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

        public void CalculateAdditionalFiles(IGameFile iwad, ISourcePortData sourcePort)
        {
            SetExtraAdditionalFilesFromSettings(iwad, sourcePort);

            IGameFile lastIwad = m_selectedIWad;
            ISourcePortData lastSourcePort = m_selectedSourcePort;
            if (lastIwad == null) lastIwad = iwad;
            if (lastSourcePort == null) lastSourcePort = sourcePort;

            List<IGameFile> gameFiles = m_currentFiles;
            List<IGameFile> originalList = gameFiles.ToList();
            List<IGameFile> newTypeFiles = GetAdditionalFiles(iwad, sourcePort);
            List<IGameFile> oldTypeFiles = GetAdditionalFiles(lastIwad, lastSourcePort);
            gameFiles.RemoveAll(x => oldTypeFiles.Contains(x));

            gameFiles.AddRange(newTypeFiles);

            gameFiles = SortByOriginal(gameFiles, originalList);
            m_currentFiles = gameFiles.Distinct().ToList();
            m_currentNewFiles = gameFiles.Except(originalList).ToList();

            m_selectedIWad = iwad;
            m_selectedSourcePort = sourcePort;
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
            }
        }

        private List<IGameFile> GetAdditionalFiles(IGameFile gameIwad, ISourcePortData sourcePort)
        {
            IEnumerable<IGameFile> exclude = new IGameFile[] { m_gameFile };
            return GetAdditionalFiles(AddFilesType.IWAD, gameIwad, sourcePort)
                .Union(GetAdditionalFiles(AddFilesType.SourcePort, gameIwad, sourcePort))
                .Except(exclude).ToList();
        }

        private List<IGameFile> GetAdditionalFiles(AddFilesType type, IGameFile gameIwad, ISourcePortData sourcePort)
        {
            switch (type)
            {
                case AddFilesType.IWAD:
                    if (gameIwad != null)
                        return Util.GetAdditionalFiles(m_adapter, (GameFile)gameIwad);
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
