using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    class DemoHandler
    {
        public DemoHandler(IDataSourceAdapter adapter, LauncherPath demoDirectory)
        {
            DataAdapter = adapter;
            DemoDirectory = demoDirectory;
        }

        public IEnumerable<IFileData> HandleNewDemo(ISourcePort sourcePort, IGameFile gameFile, string demoFile, string descriptionText)
        {
            List<IFileData> ret = new List<IFileData>();
            FileInfo fi = new FileInfo(demoFile);

            if (gameFile.GameFileID.HasValue && fi.Exists)
            {
                fi.CopyTo(Path.Combine(DemoDirectory.GetFullPath(), fi.Name));

                FileData file = new FileData
                {
                    FileName = fi.Name,
                    GameFileID = gameFile.GameFileID.Value,
                    SourcePortID = sourcePort.SourcePortID,
                    FileTypeID = FileType.Demo,
                    Description = descriptionText
                };

                DataAdapter.InsertFile(file);
            }

            return ret;
        }

        public IDataSourceAdapter DataAdapter { get; set; }
        public LauncherPath DemoDirectory { get; set; }
    }
}
