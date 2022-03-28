using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DoomLauncher
{
    public static class ScreenshotHandler
    {
        public static bool InsertScreenshot(IGameFile gameFile, Image image, out IFileData fileData)
        {
            fileData = null;
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return false;

            try
            {
                string fileName = Guid.NewGuid().ToString() + ".png";
                string path = Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), fileName);
                image.Save(path);

                fileData = new FileData
                {
                    FileName = fileName,
                    GameFileID = gameFile.GameFileID.Value,
                    SourcePortID = -1,
                    FileTypeID = FileType.Screenshot
                };

                DataCache.Instance.DataSourceAdapter.InsertFile(fileData);
            }
            catch
            {
                //failed, nothing to do
            }

            return true;
        }

        public static IEnumerable<IFileData> HandleNewScreenshots(ISourcePortData sourcePort, IGameFile gameFile, string[] files)
        {
            List<IFileData> ret = new List<IFileData>();
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return ret;

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

            return ret;
        }
    }
}
