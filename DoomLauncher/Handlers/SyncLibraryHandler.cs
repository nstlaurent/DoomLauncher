using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WadReader;

namespace DoomLauncher
{
    class SyncLibraryHandler
    {
        public event EventHandler SyncFileChange;
        public event EventHandler GameFileDataNeeded;

        private readonly List<InvalidFile> m_invalidFiles = new List<InvalidFile>();

        public SyncLibraryHandler(IGameFileDataSourceAdapter dbDataSource, IGameFileDataSourceAdapter syncDataSource,
            LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            DbDataSource = dbDataSource;
            SyncDataSource = syncDataSource;
            GameFileDirectory = gameFileDirectory;
            TempDirectory = tempDirectory;

            SyncFileCurrent = SyncFileCount = 0;
        }

        public void Execute(string[] syncFiles)
        {
            m_invalidFiles.Clear();
            IEnumerable<string> diff = null;

            if (syncFiles == null)
            {
                IEnumerable<string> dsFiles = SyncDataSource.GetGameFileNames();
                IEnumerable<string> dbFiles = DbDataSource.GetGameFileNames();
                diff = dsFiles.Except(dbFiles);
            }
            else
            {
                diff = syncFiles;
            }

            SyncFileCount = diff.Count();
            SyncFileCurrent = 0;

            foreach (string fileName in diff)
            {
                if (SyncFileChange != null)
                {
                    CurrentSyncFileName = fileName;
                    SyncFileChange(this, new EventArgs());
                }

                IGameFile file = SyncDataSource.GetGameFile(fileName);
                IGameFile existing = DbDataSource.GetGameFile(file.FileName);

                if (existing != null)
                    file = existing;

                if (file != null)
                {
                    CurrentGameFile = file;
                    GameFileDataNeeded?.Invoke(this, new EventArgs());
                    file.Downloaded = DateTime.Now;

                    try
                    {
                        file.Map = GetMaps(Path.Combine(GameFileDirectory.GetFullPath(), file.FileName));
                        if (!string.IsNullOrEmpty(file.Map))
                            file.MapCount = file.Map.Count(x => x == ',') + 1;
                    }
                    catch(IOException)
                    {
                        file.Map = string.Empty;
                        m_invalidFiles.Add(new InvalidFile(fileName, "File is in use"));
                    }
                    catch (Exception)
                    {
                        file.Map = string.Empty;
                        m_invalidFiles.Add(new InvalidFile(fileName, "Zip archive contained an improper pk3"));
                    }

                    if (existing == null)
                        DbDataSource.InsertGameFile(file);
                    else
                        DbDataSource.UpdateGameFile(file, Util.GetSyncGameFileUpdateFields());
                }
                else
                {
                    m_invalidFiles.Add(new InvalidFile(fileName, "Not a valid zip archive"));

                    try
                    {
                        FileInfo fileDelete = new FileInfo(Path.Combine(GameFileDirectory.GetFullPath(), fileName));
                        if (fileDelete.Exists)
                            fileDelete.Delete();
                    }
                    catch
                    {
                        //delete failed, just keep going
                    }
                }

                SyncFileCurrent++;
            }
        }

        private IEnumerable<ZipArchiveEntry> GetEntriesByExtension(ZipArchive za, string ext)
        {
            return za.Entries.Where(x => x.Name.Contains('.') && new FileInfo(x.Name).Extension.Equals(ext, StringComparison.OrdinalIgnoreCase));
        }

        private string GetMaps(string filename)
        {
            using (ZipArchive za = ZipFile.OpenRead(filename))
            {
                IEnumerable<ZipArchiveEntry> txtEntries = GetEntriesByExtension(za, ".txt").Where(x => x.Name.Equals("mapinfo.txt", StringComparison.InvariantCultureIgnoreCase));
                if (txtEntries.Any())
                    return MapStringFromMapInfo(txtEntries.First());

                StringBuilder sb = new StringBuilder();
                sb.Append(MapStringFromGameFile(za));

                IEnumerable<ZipArchiveEntry> pk3Entries = Util.GetEntriesByExtension(za, Util.GetPkExtenstions());

                foreach (ZipArchiveEntry zae in pk3Entries)
                {
                    string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), zae);
                    sb.Append(GetMaps(extractedFile));
                }

                return sb.ToString();
            }
        }

        private string MapStringFromMapInfo(ZipArchiveEntry zae)
        {
            string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), zae);
            StringBuilder sb = new StringBuilder();

            if (File.Exists(extractedFile))
            {
                Regex mapRegex = new Regex(@"\s*map\s+\w+");
                string text = File.ReadAllText(extractedFile);
                MatchCollection matches = mapRegex.Matches(text);

                foreach(Match match in matches)
                {
                    sb.Append(match.Value.Trim().Substring(3).Trim());
                    sb.Append(", ");
                }

                FileInfo deleteFile = new FileInfo(extractedFile);
                deleteFile.Delete();
            }

            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        private string MapStringFromGameFile(ZipArchive za)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<ZipArchiveEntry> wadEntries = GetEntriesByExtension(za, ".wad");

            foreach (ZipArchiveEntry zae in wadEntries)
            {
                string extractFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), zae);
                sb.Append(Util.GetMapStringFromWad(extractFile));
            }

            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public IGameFileDataSourceAdapter DbDataSource { get; set; }
        public IGameFileDataSourceAdapter SyncDataSource { get; set; }
        public LauncherPath GameFileDirectory { get; set; }
        public LauncherPath TempDirectory { get; set; }
        public IGameFile CurrentGameFile { get; set; }

        public InvalidFile[] InvalidFiles
        {
            get { return m_invalidFiles.ToArray(); }
        }

        public int SyncFileCurrent { get; private set; }
        public int SyncFileCount { get; private set; }
        public string CurrentSyncFileName { get; private set; }
    }
}
