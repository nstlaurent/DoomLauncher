using DoomLauncher.Interfaces;
using DoomLauncher.SaveGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    class SaveGameHandler
    {
        public SaveGameHandler(IDataSourceAdapter adapter, LauncherPath savegameDirectory)
        {
            DataSourceAdapter = adapter;
            SaveGameDirectory = savegameDirectory;
        }

        public IEnumerable<IFileData> HandleNewSaveGames(ISourcePort sourcePort, IGameFile gameFile, string[] files)
        {
            List<IFileData> ret = new List<IFileData>();

            if (gameFile != null && gameFile.GameFileID.HasValue)
            {
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        string fileName = Guid.NewGuid().ToString() + fi.Extension;
                        fi.CopyTo(Path.Combine(SaveGameDirectory.GetFullPath(), fileName));

                        FileData fileData = new FileData
                        {
                            Description = GetSaveGameName(fi),
                            OriginalFileName = fi.Name,
                            FileName = fileName,
                            GameFileID = gameFile.GameFileID.Value,
                            SourcePortID = sourcePort.SourcePortID,
                            FileTypeID = FileType.SaveGame
                        };

                        DataSourceAdapter.InsertFile(fileData);
                        ret.Add(fileData);
                    }
                    catch
                    {
                        //failed, nothing to do
                    }
                }
            }

            return ret;
        }

        private static string GetSaveGameName(FileInfo fi)
        {
            ISaveGameReader reader = CreateSaveGameReader(fi);

            if (reader != null)
                return reader.GetName();

            return fi.Name;
        }

        private static ISaveGameReader CreateSaveGameReader(FileInfo fi)
        {
            if (fi.Extension.Equals(".zds", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ZDoomSaveGameReader(fi.FullName);
            }
            else if (fi.Extension.Equals(".dsg", StringComparison.InvariantCultureIgnoreCase))
            {
                return new DsgSaveGameReader(fi.FullName);

            }

            return null;
        }

        public void HandleUpdateSaveGames(ISourcePort sourcePort, IGameFile gameFile, IFileData[] files)
        {
            foreach(IFileData file in files)
            {
                FileInfo fi = new FileInfo(Path.Combine(sourcePort.Directory.GetFullPath(), file.OriginalFileName));

                if (fi.Exists)
                {
                    try
                    {
                        fi.CopyTo(Path.Combine(SaveGameDirectory.GetFullPath(), file.FileName), true);
                    }
                    catch
                    {
                        //failed, nothing to do
                    }

                    //check to see if the save name changed
                    string saveName = GetSaveGameName(fi);
                    if (saveName != file.Description)
                    {
                        file.Description = saveName;
                        DataSourceAdapter.UpdateFile(file);
                    }
                }
            }
        }

        public void CopySaveGamesToSourcePort(ISourcePort sourcePort, IFileData[] files)
        {
            files = files.Where(x => x.SourcePortID == sourcePort.SourcePortID).ToArray();

            foreach (IFileData file in files)
            {
                string fileName = Path.Combine(sourcePort.Directory.GetFullPath(), file.OriginalFileName);
                FileInfo fiFrom = new FileInfo(Path.Combine(SaveGameDirectory.GetFullPath(), file.FileName));
                try
                {
                    if (fiFrom.Exists)
                    {
                        DirectoryInfo di = new DirectoryInfo(Path.Combine(sourcePort.Directory.GetFullPath(), file.OriginalFilePath));

                        if (!di.Exists)
                            di.Create();

                        fiFrom.CopyTo(fileName);
                    }
                }
                catch
                {
                    //failed, nothing to do
                }
            }
        }

        public LauncherPath SaveGameDirectory { get; set; }
        public IDataSourceAdapter DataSourceAdapter { get; set; }
    }
}
