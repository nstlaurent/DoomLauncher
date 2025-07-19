using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace DoomLauncher.Handlers
{
    public class GameFileImageHandler
    {
        private static readonly int THUMBNAIL_SIZE = 300;
        private readonly IFileHandler m_fileHandler;
        private readonly bool m_deleteScreenshotsAfterImport;

        public GameFileImageHandler(IFileHandler fileHandler, bool deleteScreenshotsAfterImport = false)
        {
            m_fileHandler = fileHandler;
            m_deleteScreenshotsAfterImport = deleteScreenshotsAfterImport;
        }

        public string GetMainImageLarge(IGameFile gameFile)
        {
            return null;
        }

        public string GetMainImageSmall(IGameFile gameFile)
        {
            return null;
        }

        public List<string> GetMainImageAndScreenshots(IGameFile gameFile)
        {
            return null;
        }

        public IFileData InsertTitlePic(IGameFile gameFile, Image image)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            // There can only be one TitlePic
            m_fileHandler.DeleteFiles(gameFile, FileType.TitlePic);

            var titlePic = m_fileHandler.InsertFromMemory(gameFile, FileType.TitlePic, image, "png");

            if (titlePic != null)
            {
                CreateAndInsertThumbnail(gameFile, titlePic);
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

            if (screenshot != null) // && no thumbnails exist for that gameFile
            {
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
                    return m_fileHandler.InsertFromMemory(gameFile, FileType.Thumbnail, thumb, "png", file =>
                    {
                        file.DerivedFromFileID = parent.FileID;
                    });
                }
            }
        }
    }
}
