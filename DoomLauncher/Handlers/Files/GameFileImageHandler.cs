using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace DoomLauncher.Handlers
{
    public class GameFileImageHandler
    {
        private static readonly int THUMBNAIL_SIZE = 300;
        private readonly IFileHandler m_fileHandler;

        public GameFileImageHandler(IFileHandler fileHandler)
        {
            m_fileHandler = fileHandler;
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
