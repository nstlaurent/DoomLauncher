using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using DoomLauncher.TextFileParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class DirectoryDataSourceAdapter : IGameFileDataSourceAdapter, IIWadDataSourceAdapter
    {
        public DirectoryDataSourceAdapter(LauncherPath gameFileDirectory, string[] dateParseFormats)
        {
            GameFileDirectory = gameFileDirectory;
            DateParseFormats = dateParseFormats.ToArray();
        }

        public int GetGameFilesCount()
        {
            DirectoryInfo dir = new DirectoryInfo(GameFileDirectory.GetFullPath());
            return dir.GetFiles().Length;
        }

        public IEnumerable<string> GetGameFileNames()
        {
            return Directory.GetFiles(GameFileDirectory.GetFullPath());
        }

        public IGameFile GetGameFile(string fileName)
        {
            FileInfo fi = new FileInfo(Path.Combine(GameFileDirectory.GetFullPath(), fileName));

            if (fi.Exists)
                return FromZipFile(fi);

            return null;
        }

        public IEnumerable<IGameFile> GetGameFiles()
        {
            return GetGameFiles(null);
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options)
        {
            int limit = options.Limit.HasValue ? options.Limit.Value : -1;

            List<IGameFile> ret = new List<IGameFile>();
            DirectoryInfo dir = new DirectoryInfo(GameFileDirectory.GetFullPath());
            int counter = 0;

            foreach (FileInfo fi in dir.GetFiles())
            {
                counter++;
                ret.Add(FromZipFile(fi));

                if (limit > -1 && counter == limit)
                    break;
            }

            return ret;
        }

        public IEnumerable<IGameFile> GetGameFileIWads()
        {
            throw new NotSupportedException();
        }

        public void DeleteGameFile(IGameFile gameFile)
        {
            HandleDelete(GameFileDirectory.GetFullPath(), gameFile.FileName);
        }

        private static void HandleDelete(string directory, string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                FileInfo fi = new FileInfo(Path.Combine(directory, filename));

                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
        }

        public IEnumerable<IIWadData> GetIWads()
        {
            throw new NotSupportedException();
        }

        public IIWadData GetIWad(int gameFileID)
        {
            throw new NotSupportedException();
        }

        public void InsertIWad(IIWadData iwad)
        {
            throw new NotSupportedException();
        }

        public void UpdateIWad(IIWadData iwad)
        {
            throw new NotSupportedException();
        }

        public void DeleteIWad(IIWadData iwad)
        {
            throw new NotSupportedException();
        }

        private IGameFile FromZipFile(FileInfo fi)
        {
            IGameFile gameFile = new GameFile();
            gameFile.FileName = fi.Name;
            ZipArchive za = null;

            try
            {
                za = ZipFile.OpenRead(fi.FullName);
            }
            catch //probably corrupt, delete?
            {
                return null;
            }

            bool bParsedTxt = false;

            try
            {
                foreach (ZipArchiveEntry zae in za.Entries)
                {
                    FileInfo zipFile = new FileInfo(zae.FullName);

                    if (zipFile.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        byte[] buffer = new byte[zae.Length];
                        zae.Open().Read(buffer, 0, Convert.ToInt32(zae.Length));

                        IdGamesTextFileParser parser = new IdGamesTextFileParser(DateParseFormats);
                        parser.Parse(UnicodeEncoding.UTF7.GetString(buffer));

                        gameFile.Title = parser.Title;
                        gameFile.Author = parser.Author;
                        gameFile.ReleaseDate = parser.ReleaseDate;
                        gameFile.Description = parser.Description;

                        if (string.IsNullOrEmpty(gameFile.Title))
                            gameFile.Title = fi.Name;

                        bParsedTxt = !string.IsNullOrEmpty(gameFile.Title) || !string.IsNullOrEmpty(gameFile.Author) || !string.IsNullOrEmpty(gameFile.Description);
                    }

                    if (bParsedTxt)
                        break;
                }
            }
            catch(InvalidDataException)
            {
                gameFile = null;
            }

            return gameFile;
        }

        public void InsertGameFile(IGameFile gameFile)
        {
            throw new NotSupportedException();
        }

        public void UpdateGameFile(IGameFile gameFile)
        {
            throw new NotSupportedException();
        }

        public void UpdateGameFile(IGameFile gameFile, GameFileFieldType[] updateFields)
        {
            throw new NotSupportedException();
        }

        public LauncherPath GameFileDirectory
        {
            get;
            set;
        }

        public string[] DateParseFormats
        {
            get;
            set;
        }
    }
}
