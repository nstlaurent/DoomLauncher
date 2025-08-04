using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;

namespace DoomLauncher
{
    public class DemoHandler
    {
        private IFileHandler m_fileHandler;

        public DemoHandler(IFileHandler fileHandler)
        {
            m_fileHandler = fileHandler;
        }

        public IFileData InsertNewDemo(ISourcePortData sourcePort, IGameFile gameFile, string demoFile, string descriptionText)
        {
            return m_fileHandler.InsertAndCopy(gameFile, FileType.Demo, demoFile, file => 
            {
                file.SourcePortID = sourcePort.SourcePortID;
                file.Description = descriptionText;
            });
        }
    }
}
