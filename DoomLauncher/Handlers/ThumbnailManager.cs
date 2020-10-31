using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public static class ThumbnailManager
    {
        public static void UpdateThumbnail(IGameFile gameFile)
        {
            bool delete = false;
            var screenshot = DataCache.Instance.DataSourceAdapter.GetFiles(gameFile, FileType.Screenshot).FirstOrDefault();
            var thumbnail = DataCache.Instance.DataSourceAdapter.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            // All screenshots for this game file were deleted
            if (thumbnail != null && screenshot == null)
                delete = true;
            // The first screenshot was changed
            if (thumbnail != null && screenshot != null && thumbnail.SourcePortID != screenshot.FileID)
                delete = true;

            if (delete)
            {
                string file = Path.Combine(DataCache.Instance.AppConfiguration.ThumbnailDirectory.GetFullPath(), thumbnail.FileName);
                if (File.Exists(file))
                    File.Delete(file);

                DataCache.Instance.DataSourceAdapter.DeleteFile(thumbnail);
            }

            GetOrCreateThumbnail(gameFile);
        }

        // Returns or creates a new thumbnail and inserts into database if it doesn't exist
        // Will search screenshots and thumbnails if provided, otherwise will check from database
        public static IFileData GetOrCreateThumbnail(IGameFile gameFile, IEnumerable<IFileData> screenshots = null, IEnumerable<IFileData> thumbnails = null)
        {
            if (thumbnails == null)
                thumbnails = DataCache.Instance.DataSourceAdapter.GetFiles(gameFile, FileType.Thumbnail);

            var thumbnail = thumbnails.FirstOrDefault(x => x.GameFileID == gameFile.GameFileID.Value);

            if (thumbnail != null)
                return thumbnail;

            if (screenshots == null)
                screenshots = DataCache.Instance.DataSourceAdapter.GetFiles(gameFile, FileType.Screenshot);

            var screenshot = screenshots.FirstOrDefault(x => x.GameFileID == gameFile.GameFileID.Value);

            if (screenshot != null)
            {
                string thumbnailFile = CreateThumbnail(screenshot);
                if (thumbnailFile == null)
                    return null;

                // Store the FileID of the screenshot in SourcePortID so we can use it to keep track of screenshot re-ordering / deletes
                FileData fileData = new FileData()
                {
                    GameFileID = gameFile.GameFileID.Value,
                    FileName = thumbnailFile,
                    FileTypeID = FileType.Thumbnail,
                    SourcePortID = screenshot.FileID.Value
                };

                DataCache.Instance.DataSourceAdapter.InsertFile(fileData);
                return fileData;
            }

            return null;
        }

        private static string CreateThumbnail(IFileData screenshot)
        {
            string file = Path.Combine(DataCache.Instance.AppConfiguration.ScreenshotDirectory.GetFullPath(), screenshot.FileName);
            if (!File.Exists(file))
                return null;

            Image image = Image.FromFile(file);
            Image thumb = Util.FixedSize(image, GameFileTile.ImageWidth, GameFileTile.ImageHeight, Color.Black);
            string filename = Guid.NewGuid().ToString() + ".png";
            thumb.Save(Path.Combine(DataCache.Instance.AppConfiguration.ThumbnailDirectory.GetFullPath(), filename), ImageFormat.Png);
            return filename;
        }
    }
}
