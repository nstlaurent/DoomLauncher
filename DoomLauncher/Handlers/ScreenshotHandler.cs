using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher
{
    class ScreenshotHandler
    {
        public IEnumerable<IFileData> HandleNewScreenshots(ISourcePortData sourcePort, IGameFile gameFile, string[] files)
        {
            List<IFileData> ret = new List<IFileData>();

            if (gameFile != null && gameFile.GameFileID.HasValue)
            {
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        string fileName = Guid.NewGuid().ToString() + fi.Extension;
                        fi.CopyTo(Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), fileName));

                        FileData fileData = new FileData
                        {
                            FileName = fileName,
                            GameFileID = gameFile.GameFileID.Value,
                            SourcePortID = sourcePort.SourcePortID,
                            FileTypeID = FileType.Screenshot
                        };

                        DataCache.Instance.DataSourceAdapter.InsertFile(fileData);
                        ret.Add(fileData);

                        if (DataCache.Instance.AppConfiguration.DeleteScreenshotsAfterImport)
                            File.Delete(file);
                    }
                    catch
                    {
                        //failed, nothing to do
                    }
                }
            }

            return ret;
        }
    }
}
