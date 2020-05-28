using DoomLauncher.Interfaces;
using System.Collections.Generic;

namespace DoomLauncher
{
    public class GameFileListEventArgs
    {
        public IEnumerable<IGameFile> GameFiles { get; set; }

        public GameFileListEventArgs(IEnumerable<IGameFile> gameFiles)
        {
            GameFiles = gameFiles;
        }
    }
}
