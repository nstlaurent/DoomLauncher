using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class DirectoryDataSourceAdapter : IGameFileDataSourceAdapter, IIWadDataSourceAdapter
    {
        public DirectoryDataSourceAdapter(LauncherPath gameFileDirectory)
        {
            GameFileDirectory = gameFileDirectory;
        }

        public int GetGameFilesCount()
        {
            DirectoryInfo dir = new DirectoryInfo(GameFileDirectory.GetFullPath());
            return dir.GetFiles().Length;
        }

        public IEnumerable<string> GetGameFileNames()
        {
            return Directory.GetFiles(GameFileDirectory.GetFullPath()).Select(x => Path.GetFileName(x));
        }

        public IGameFile GetGameFile(string fileName)
        {
            if (Util.IsDirectory(fileName))
                return CreateGameFileFromDirectory(fileName);

            FileInfo fi = new FileInfo(Path.Combine(GameFileDirectory.GetFullPath(), fileName));
            if (fi.Exists)
                return CreateGameFile(fi);

            return null;
        }

        public IEnumerable<IGameFile> GetGameFiles()
        {
            return GetGameFiles(null);
        }

        public IEnumerable<IGameFile> GetUntaggedGameFiles()
        {
            throw new NotSupportedException();
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options)
        {
            int limit = -1;

            if (options != null && options.Limit.HasValue)
                limit =  options.Limit.Value;

            List<IGameFile> ret = new List<IGameFile>();
            DirectoryInfo dir = new DirectoryInfo(GameFileDirectory.GetFullPath());
            int counter = 0;

            foreach (FileInfo fi in dir.GetFiles())
            {
                counter++;
                ret.Add(CreateGameFile(fi));

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

        private IGameFile CreateGameFile(FileInfo fi)
        {
            return new GameFile()
            {
                FileName = fi.Name
            };
        }

        private IGameFile CreateGameFileFromDirectory(string dir)
        {
            return new GameFile()
            {
                FileName = dir
            };
        }
    }
}
