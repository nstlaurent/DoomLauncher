using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DoomLauncher.Handlers
{
    public delegate IIWadData GetIWad(int iwadID);

    public class GameFileImageHandler
    {
        public static readonly string DEFAULT_TILE_IMAGE = "DoomLauncherTile.png";
        private static readonly int THUMBNAIL_SIZE = 300;

        private readonly IFileHandler m_fileHandler;
        private readonly GetIWad m_getIWad;
        private readonly bool m_deleteScreenshotsAfterImport;

        public GameFileImageHandler(IFileHandler fileHandler, GetIWad getIwad, bool deleteScreenshotsAfterImport = false)
        {
            m_fileHandler = fileHandler;
            m_getIWad = getIwad;
            m_deleteScreenshotsAfterImport = deleteScreenshotsAfterImport;
        }

        public IFileData GetMainImageLarge(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
            {
                IFileData bestImage = m_fileHandler.GetFiles(gameFile, FileType.TitlePic, FileType.Screenshot, FileType.TileImage).FirstOrDefault();
                return bestImage ?? CreateAndInsertTileImage(gameFile);
            }
            else
            {
                return GetInMemoryDefaultImage();
            }
        }

        public IFileData GetMainImageSmall(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
            {
                IFileData bestImage = m_fileHandler.GetFiles(gameFile, FileType.Thumbnail, FileType.TileImage).FirstOrDefault();
                return bestImage ?? CreateAndInsertTileImage(gameFile);
            }
            else
            {
                return GetInMemoryDefaultImage();
            }
        }

        public List<IFileData> GetMainImageAndScreenshots(IGameFile gameFile)
        {
            var mainImage = GetMainImageLarge(gameFile);
            var screenshots = GetScreenshots(gameFile).Where(scr => scr.FileID != mainImage.FileID);
            var list = new List<IFileData>() { mainImage };
            list.AddRange(screenshots);
            return list;
        }

        public List<IFileData> GetScreenshots(IGameFile gameFile) => 
            m_fileHandler.GetFiles(gameFile, FileType.Screenshot).ToList();

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

        public void UpdateImages(IGameFile gameFile)
        {
            var mainImage = m_fileHandler.GetFiles(gameFile, FileType.TitlePic, FileType.Screenshot).FirstOrDefault();
            if (mainImage != null)
            {
                m_fileHandler.DeleteFiles(gameFile, FileType.TileImage);
                m_fileHandler.DeleteFiles(gameFile, FileType.Thumbnail);
                CreateAndInsertThumbnail(gameFile, mainImage);
            }
            else
            {
                CreateAndInsertTileImage(gameFile);
            }
        }

        private IFileData CreateAndInsertThumbnail(IGameFile gameFile, IFileData parent)
        {
            var parentFile = m_fileHandler.GetFullFileName(parent.FileTypeID, parent.FileName);
            using (Image image = Image.FromFile(parentFile))
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

        private IFileData GetInMemoryDefaultImage() =>
            new FileData()
            {
                FileName = DEFAULT_TILE_IMAGE,
                FullFileName = m_fileHandler.GetFullFileName(FileType.TileImage, DEFAULT_TILE_IMAGE)
            };

        private IFileData CreateAndInsertTileImage(IGameFile gameFile)
        {
            // Can only have one tile image
            m_fileHandler.DeleteFiles(gameFile, FileType.TileImage);

            string fileNameNoPath = null;
            if (gameFile.IWadID != null)
            {
                var iwad = m_getIWad(gameFile.IWadID.Value);
                fileNameNoPath = iwad?.Info?.TileImage;
            }

            if (fileNameNoPath == null)
                fileNameNoPath = gameFile.IntendedGame?.TileImage ?? DEFAULT_TILE_IMAGE;

            string fileName = m_fileHandler.GetFullFileName(FileType.TileImage, fileNameNoPath);
            return m_fileHandler.InsertAndRefer(gameFile, FileType.TileImage, fileName);
        }
    }
}
