using DoomLauncher.Config;
using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class ThumbnailManager
    {
        private static readonly string DEFAULT_IMAGE_NAME = "doomlaunchertile";

        public static List<IGameFile> IWads = new List<IGameFile>();
        public static readonly Dictionary<int, IFileData> IWadTileImages = new Dictionary<int, IFileData>();

        private readonly IDataSourceAdapter m_database;
        private readonly IDirectoriesConfiguration m_config;

        public static ThumbnailManager Instance => 
            new ThumbnailManager(DataCache.Instance.DataSourceAdapter, DataCache.Instance.AppConfiguration);

        public ThumbnailManager(IDataSourceAdapter database, IDirectoriesConfiguration config) 
        {
            m_database = database;
            m_config = config;
        }

        public static void SetIWads(List<IGameFile> iwads)
        {
            IWads = iwads;
            IWadTileImages.Clear();

            foreach (var iwad in iwads)
            {
                if (!iwad.IWadID.HasValue || !IWadInfo.TryGetIWadInfo(iwad.FileName, out var iwadInfo) || 
                    string.IsNullOrEmpty(iwadInfo.TileImage) || !File.Exists(iwadInfo.TileImage))
                    continue;

                IWadTileImages[iwad.IWadID.Value] = new FileData()
                {
                    GameFileID = iwad.GameFileID.Value,
                    FileName = iwadInfo.TileImage,
                    FileTypeID = FileType.TileImage
                };  
            }
        }

        // Should we delete the old one? Followed by GetOrCreateThumbnail
        public void UpdateThumbnail(IGameFile gameFile)
        {
            bool delete = false;
            var titlePic = m_database.GetFiles(gameFile, FileType.TitlePic).FirstOrDefault();
            var thumbnail = m_database.GetFiles(gameFile, FileType.Thumbnail).FirstOrDefault();

            // All screenshots for this game file were deleted
            if (thumbnail != null && titlePic == null)
                delete = true;

            // The first screenshot was changed
            if (thumbnail != null && titlePic != null && thumbnail.DerivedFromFileID != titlePic.FileID)
                delete = true;

            if (delete)
            {
                string file = m_config.ThumbnailDirectory.GetFullPath(thumbnail.FileName);
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (IOException)
                {
                    // File is in use, insert to delete on next startup
                    m_database.InsertCleanupFile(new CleanupFile() { FileName = file });
                }

                m_database.DeleteFile(thumbnail);
            }

            GetOrCreateThumbnail(gameFile);
        }

        public static bool IsTileImage(IFileData fileData)
        {
            if (fileData.FileTypeID == FileType.TileImage)
                return true;

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileData.FileName);
            return IWadInfo.ALL.Any(info => info.GameName == fileNameWithoutExt) || DEFAULT_IMAGE_NAME == fileNameWithoutExt;
        }

        public string GetThumbnailImagePath(IFileData thumbnail)
        {
            if (IsTileImage(thumbnail))
                return m_config.TileImageDirectory.GetFullPath(thumbnail.FileName);
            else
                return m_config.GetFileDirectory(thumbnail.FileTypeID).GetFullPath(thumbnail.FileName);
        }

        // Returns or creates a new thumbnail and inserts into database if it doesn't exist
        // Will search screenshots and thumbnails if provided, otherwise will check from database
        public IFileData GetOrCreateThumbnail(IGameFile gameFile, 
            IEnumerable<IFileData> screenshots = null, 
            IEnumerable<IFileData> thumbnails = null, // These optional parameters are useless, it is always selecting all screenshots/thumbnails from DB just before calling this method
            bool checkIWad = true) 
        {
            // Populate thumbnails parameter if not provided
            if (thumbnails == null)
                thumbnails = m_database.GetFiles(gameFile, FileType.Thumbnail);

            var thumbnail = thumbnails.FirstOrDefault(x => x.GameFileID == gameFile.GameFileID.Value);

            // If we've already got one, we're done
            if (thumbnail != null)
                return thumbnail;

            // Populate screenshots parameter if not provided
            if (screenshots == null)
            {
                var combined = new List<IFileData>();
                combined.AddRange(m_database.GetFiles(gameFile, FileType.TitlePic));
                combined.AddRange(m_database.GetFiles(gameFile, FileType.Screenshot));
                screenshots = combined;
            }

            // If we've got a TitlePic or screenshot, make a thumbnail out of that and save to DB
            var screenshot = screenshots.FirstOrDefault(x => x.GameFileID == gameFile.GameFileID.Value);
            if (screenshot != null)
            {
                // Make a squishy thumbnail version of the screenshot and save to disk
                if (!TryCreateThumbnail(screenshot, out var thumbnailFile))
                    return null;

                FileData fileData = new FileData()
                {
                    GameFileID = gameFile.GameFileID.Value,
                    FileName = thumbnailFile,
                    FileTypeID = FileType.Thumbnail,
                    DerivedFromFileID = screenshot.FileID.Value
                };

                m_database.InsertFile(fileData);
                return fileData;
            }

            // No titlepic, but check the iwad
            if (checkIWad && gameFile.IWadID.HasValue)
            {
                // Fail if no iwads
                var iwads = m_database.GetIWads();
                var iwad = iwads.FirstOrDefault(w => w.IWadID == gameFile.IWadID.Value);
                if (iwad == null)
                    return null;

                // This is not an IWAD, so we'll use the iwad's TileImage
                if (IWadTileImages.TryGetValue(gameFile.IWadID.Value, out var fileData))
                    return fileData;          
            }

            return null;
        }

        private bool TryCreateThumbnail(IFileData titlePicOrScreenshot, out string filename)
        {
            filename = string.Empty;
            try
            {
                string file = this.GetThumbnailImagePath(titlePicOrScreenshot);
                if (!File.Exists(file))
                    return false;

                const int ThumbnailSize = 300;
                using (Image image = Image.FromFile(file))
                {
                    using (Image thumb = image.FixedSize(ThumbnailSize, GameFileTile.GetImageHeight(ThumbnailSize), Color.Black))
                    {
                        filename = Guid.NewGuid().ToString() + ".png";
                        thumb.Save(m_config.ThumbnailDirectory.GetFullPath(filename), ImageFormat.Png);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
