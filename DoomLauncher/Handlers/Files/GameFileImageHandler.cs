using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;


namespace DoomLauncher.Handlers
{
    public class GameFileImageHandler
    {
        private static readonly int THUMBNAIL_SIZE = 300;
        private readonly IFileHandler m_fileHandler;
        private readonly IDataSourceAdapter m_database;
        private readonly bool m_deleteScreenshotsAfterImport;

        public GameFileImageHandler(IFileHandler fileHandler, IDataSourceAdapter database = null, bool deleteScreenshotsAfterImport = false)
        {
            m_fileHandler = fileHandler;
            m_database = database;
            m_deleteScreenshotsAfterImport = deleteScreenshotsAfterImport;
        }

        public string GetMainImageLarge(IGameFile gameFile)
        {
            IFileData bestImage = m_fileHandler.GetFiles(gameFile, FileType.TitlePic, FileType.Screenshot, FileType.TileImage).FirstOrDefault();

            if (bestImage == null)
                bestImage = CreateAndInsertTileImage(gameFile);

            return GetFullFileName(bestImage);
        }

        public string GetMainImageSmall(IGameFile gameFile)
        {
            IFileData bestImage = m_fileHandler.GetFiles(gameFile, FileType.Thumbnail, FileType.TileImage).FirstOrDefault();

            if (bestImage == null)
                bestImage = CreateAndInsertTileImage(gameFile);

            return GetFullFileName(bestImage);
        }

        public List<string> GetMainImageAndScreenshots(IGameFile gameFile)
        {
            List<string> returnValue = new List<string>();
            var mainImage = GetMainImageLarge(gameFile);
            var screenshots = GetScreenshots(gameFile).Where(scr => scr != mainImage);
            var list = new List<string>() { mainImage };
            list.AddRange(screenshots);
            return list;
        }

        public List<string> GetScreenshots(IGameFile gameFile)
        {
            return m_fileHandler
                .GetFiles(gameFile, FileType.Screenshot)
                .Select(GetFullFileName)
                .ToList();
        }

        private string GetFullFileName(IFileData file) =>
            m_fileHandler.GetFileInfo(file.FileTypeID, file.FileName).FullName;

        public IFileData InsertTitlePic(IGameFile gameFile, Image image)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            // There can only be one TitlePic
            m_fileHandler.DeleteFiles(gameFile, FileType.TitlePic);

            var titlePic = m_fileHandler.InsertAndSave(gameFile, FileType.TitlePic, image, "png");

            if (titlePic != null)
            {
                CreateAndInsertThumbnail(gameFile, titlePic);

                // We have a proper titlepic/thumbnail, no need for stock images
                m_fileHandler.DeleteFiles(gameFile, FileType.TileImage);
            }

            return titlePic;
        }

        public IFileData InsertScreenshot(ISourcePortData sourcePort, IGameFile gameFile, string screenshotFile)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            IFileData screenshot;
            if (m_deleteScreenshotsAfterImport)
            {
                screenshot = m_fileHandler.InsertAndMove(gameFile, FileType.Screenshot, screenshotFile, file =>
                {
                    file.SourcePortID = sourcePort.SourcePortID;
                });
            }
            else
            {
                screenshot = m_fileHandler.InsertAndCopy(gameFile, FileType.Screenshot, screenshotFile, file =>
                {
                    file.SourcePortID = sourcePort.SourcePortID;
                });
            }
            
            if (screenshot != null )
            {
                // We have a proper screenshot image, no need for stock images
                m_fileHandler.DeleteFiles(gameFile, FileType.TileImage);

                // Screenshots are lower priority than TitlePics and earlier screenshots, so only 
                // create a thumbnail if it's missing.
                var existingThumbnails = m_fileHandler.GetFiles(gameFile, FileType.Thumbnail);
                if (existingThumbnails.Count == 0)
                    CreateAndInsertThumbnail(gameFile, screenshot);
            }

            return screenshot;
        }

        private IFileData CreateAndInsertThumbnail(IGameFile gameFile, IFileData parent)
        {
            var parentFile = m_fileHandler.GetFileInfo(parent.FileTypeID, parent.FileName);
            using (Image image = Image.FromFile(parentFile.FullName))
            {
                using (Image thumb = image.FixedSize(THUMBNAIL_SIZE, GameFileTile.GetImageHeight(THUMBNAIL_SIZE), Color.Black))
                {
                    return m_fileHandler.InsertAndSave(gameFile, FileType.Thumbnail, thumb, "png", file =>
                    {
                        file.DerivedFromFileID = parent.FileID;
                    });
                }
            }
        }

        private IFileData CreateAndInsertTileImage(IGameFile gameFile)
        {
            // Can only have one tile image
            m_fileHandler.DeleteFiles(gameFile, FileType.TileImage);

            string fileName = null;
            if (gameFile.IWadID != null)
                fileName = m_database.GetIWad(gameFile.IWadID.Value).Info?.FileName;

            if (fileName == null)
                fileName = gameFile.IntendedGame?.FileName ?? "DoomLauncherTile.png";

            return m_fileHandler.InsertAndRefer(gameFile, FileType.TileImage, fileName);
        }

    }
}
