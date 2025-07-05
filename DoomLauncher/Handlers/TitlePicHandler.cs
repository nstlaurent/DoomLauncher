using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DoomLauncher
{
    public class TitlePicHandler
    {
        private readonly IFileHandler m_fileHandler;

        public TitlePicHandler(IFileHandler fileHandler)
        {
            m_fileHandler = fileHandler;
        }

        public IFileData InsertTitlePic(IGameFile gameFile, Image image)
        {
            if (gameFile == null || !gameFile.GameFileID.HasValue)
                return null;

            // There can only be one TitlePic
            m_fileHandler.DeleteFiles(gameFile, FileType.TitlePic);

            using (var imageStream = new MemoryStream())
            {
                image.Save(imageStream, ImageFormat.Png);

                var fileData = m_fileHandler.InsertFileFromMemory(gameFile, FileType.TitlePic, imageStream, "png");

                if (fileData != null)
                    ThumbnailManager.UpdateThumbnail(gameFile);

                return fileData;
            }
        }
    }
}
