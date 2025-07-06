using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher
{
    class DemoHandler
    {
        public DemoHandler(IDataSourceAdapter adapter, LauncherPath demoDirectory)
        {
            DataAdapter = adapter;
            DemoDirectory = demoDirectory;
        }

        public IEnumerable<IFileData> HandleNewDemo(ISourcePortData sourcePort, IGameFile gameFile, string demoFile, string descriptionText)
        {
            List<IFileData> ret = new List<IFileData>();
            FileInfo fi = new FileInfo(demoFile);

            if (gameFile.GameFileID.HasValue && fi.Exists)
            {
                fi.CopyTo(Path.Combine(DemoDirectory.GetFullPath(), fi.Name));

                FileData file = new FileData();
                file.FileName = fi.Name;
                file.GameFileID = gameFile.GameFileID.Value;
                file.SourcePortID = sourcePort.SourcePortID;
                file.FileTypeID = FileType.Demo;
                file.Description = descriptionText;

                DataAdapter.InsertFile(file);
            }

            return ret;
        }

        public IDataSourceAdapter DataAdapter { get; set; }
        public LauncherPath DemoDirectory { get; set; }
    }
}
