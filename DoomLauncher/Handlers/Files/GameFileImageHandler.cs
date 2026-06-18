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

        private readonly IFileHandler m_fileHandler;
        private readonly GetIWad m_getIWad;
        private readonly bool m_deleteScreenshotsAfterImport;

        private readonly Dictionary<IWadType, IFileData> m_IWadTileImages = new Dictionary<IWadType, IFileData>();
        private readonly Dictionary<int, IWadInfo> m_IWadIdToIWadInfo = new Dictionary<int, IWadInfo>();

        private readonly IFileData DefaultTile = new FileData()
        {
            FileName = DEFAULT_TILE_IMAGE,
            FileTypeID = FileType.TileImage,
            SourcePortID = 0
        };

        public GameFileImageHandler(IFileHandler fileHandler, GetIWad getIwad, bool deleteScreenshotsAfterImport = false)
        {
            m_fileHandler = fileHandler;
            m_getIWad = getIwad;
            m_deleteScreenshotsAfterImport = deleteScreenshotsAfterImport;

            DefaultTile.FullFileName = m_fileHandler.GetFullFileName(FileType.TileImage, DEFAULT_TILE_IMAGE);
        }

        public IFileData GetMainImageLarge(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
            {
                IFileData bestImage = GetBestMainImage(gameFile);
                return bestImage ?? CreateTileImage(gameFile);
            }
            else
            {
                return DefaultTile;
            }
        }

        public Dictionary<int, List<IFileData>> GetImageFiles(IEnumerable<IGameFile> gameFiles)
        {
            var lookup = m_fileHandler.GetFilesTrimmed(gameFiles, FileType.Thumbnail).GroupBy(x => x.GameFileID).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var gameFile in gameFiles)
            {
                if (!gameFile.GameFileID.HasValue)
                    continue;

                if (lookup.ContainsKey(gameFile.GameFileID.Value))
                    continue;

                lookup[gameFile.GameFileID.Value] = new List<IFileData>() { CreateTileImage(gameFile) };
            }

            return lookup;
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
            m_fileHandler.DeleteFiles(gameFile, FileType.Thumbnail);

            var mainImage = GetBestMainImage(gameFile);
            if (mainImage != null)
            {
                CreateAndInsertThumbnail(gameFile, mainImage);
            }
        }

        private IFileData GetBestMainImage(IGameFile gameFile)
        {
            var candidateImages = m_fileHandler.GetFiles(gameFile, FileType.TitlePic, FileType.Screenshot);
            IFileData bestImage = candidateImages.FirstOrDefault();

            // Override the title pic with a screenshot if one is marked as "IsMain"
            foreach (var image in candidateImages)
            {
                if (image.FileTypeID == FileType.Screenshot && image.IsMain)
                {
                    bestImage = image;
                    break;
                }
            }
            return bestImage;
        }

        private IFileData CreateAndInsertThumbnail(IGameFile gameFile, IFileData parent)
        {
            var parentFile = m_fileHandler.GetFullFileName(parent.FileTypeID, parent.FileName);
            using (Image image = Image.FromFile(parentFile))
            {
                const int Width = 300;

                // If the image is a titlepic then force to 1.2 stretching like the original game.
                if (parent.FileTypeID == FileType.TitlePic)
                {
                    var aspect = image.Width / (double)image.Height;
                    var newAspect = aspect / 1.2;

                    using (Image thumb = image.StretchTo(Width, (int)(Width / newAspect)))
                    {
                        return m_fileHandler.InsertAndSave(gameFile, FileType.Thumbnail, thumb, "png", file =>
                        {
                            file.DerivedFromFileID = parent.FileID;
                        });
                    }
                }

                // Default: conform to 16:9 aspect ratio. 
                using (Image thumb = image.FixedSize(Width, (int)(Width / (16.0 / 9.0)), Color.Black))
                {
                    return m_fileHandler.InsertAndSave(gameFile, FileType.Thumbnail, thumb, "png", file =>
                    {
                        file.DerivedFromFileID = parent.FileID;
                    });
                }
            }
        }

        private IFileData CreateTileImage(IGameFile gameFile)
        {
            string fileNameNoPath = null;
            if (gameFile.IWadID == null && gameFile.IntendedGame == null)
                return DefaultTile;

            var info = GetIWadInfo(gameFile);

            if (info == null)
                return DefaultTile;

            if (m_IWadTileImages.TryGetValue(info.IWadType, out var file))
                return file;

            if (fileNameNoPath == null)
                fileNameNoPath = info.TileImage ?? DEFAULT_TILE_IMAGE;

            string fullFileName = m_fileHandler.GetFullFileName(FileType.TileImage, fileNameNoPath);
            var fileData = new FileData()
            {
                FileID = (int)FileType.TileImage,
                FileName = fileNameNoPath,
                FullFileName = fullFileName,
            };

            m_IWadTileImages[info.IWadType] = fileData;
            return fileData;
        }

        private IWadInfo GetIWadInfo(IGameFile gameFile)
        {
            if (gameFile.IWadID.HasValue)
            {
                if (m_IWadIdToIWadInfo.TryGetValue(gameFile.IWadID.Value, out var info))
                    return info;

                var iwad = m_getIWad(gameFile.IWadID.Value);
                if (iwad == null)
                {
                    m_IWadIdToIWadInfo[gameFile.IWadID.Value] = null;
                    return null;
                }

                m_IWadIdToIWadInfo[gameFile.IWadID.Value] = iwad.Info;
                return iwad.Info;
            }

            return gameFile.IntendedGame;
        }
    }
}
