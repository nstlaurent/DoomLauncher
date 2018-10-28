using DoomLauncher.Interfaces;
using DoomLauncher.TextFileParsers;
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
    public class SyncLibraryHandler
    {
        public event EventHandler SyncFileChange;
        public event EventHandler GameFileDataNeeded;

        private readonly List<InvalidFile> m_invalidFiles = new List<InvalidFile>();

        public SyncLibraryHandler(IGameFileDataSourceAdapter dbDataSource, IGameFileDataSourceAdapter syncDataSource,
            LauncherPath gameFileDirectory, LauncherPath tempDirectory, string[] dateParseFormats)
        {
            DbDataSource = dbDataSource;
            SyncDataSource = syncDataSource;
            GameFileDirectory = gameFileDirectory;
            TempDirectory = tempDirectory;
            DateParseFormats = dateParseFormats;

            SyncFileCurrent = SyncFileCount = 0;
        }

        public void Execute(string[] zipFiles)
        {
            m_invalidFiles.Clear();

            SyncFileCount = zipFiles.Length;
            SyncFileCurrent = 0;

            foreach (string fileName in zipFiles)
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
                        using (ZipArchive za = ZipFile.OpenRead(Path.Combine(GameFileDirectory.GetFullPath(), file.FileName)))
                        {
                            FillTextFileInfo(file, za);
                            FillMapInfo(file, za);
                        }
                    }
                    catch(IOException)
                    {
                        file.Map = string.Empty;
                        m_invalidFiles.Add(new InvalidFile(fileName, "File is in use"));
                    }
                    catch (InvalidDataException)
                    {
                        file.Map = string.Empty;
                        m_invalidFiles.Add(new InvalidFile(fileName, "Zip archive invalid or contained an improper pk3"));
                    }
                    catch (Exception ex)
                    {
                        file.Map = string.Empty;
                        m_invalidFiles.Add(new InvalidFile(fileName, CreateExceptionMsg(ex)));
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

        private void FillMapInfo(IGameFile file, ZipArchive za)
        {
            file.Map = GetMaps(za);
            if (!string.IsNullOrEmpty(file.Map))
                file.MapCount = file.Map.Count(x => x == ',') + 1;
        }

        private void FillTextFileInfo(IGameFile gameFile, ZipArchive za)
        {
            bool bParsedTxt = false;

            foreach (ZipArchiveEntry zae in za.Entries)
            {
                if (Path.GetExtension(zae.FullName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
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
                        gameFile.Title = gameFile.FileName;

                    bParsedTxt = !string.IsNullOrEmpty(gameFile.Title) || !string.IsNullOrEmpty(gameFile.Author) || !string.IsNullOrEmpty(gameFile.Description);
                }

                if (bParsedTxt)
                    break;
            }
        }

        private static string CreateExceptionMsg(Exception ex)
        {
            return string.Concat("Unexpected exception - ", ex.Message, ex.StackTrace);
        }

        private IEnumerable<ZipArchiveEntry> GetEntriesByExtension(ZipArchive za, string ext)
        {
            return za.Entries.Where(x => x.Name.Contains('.') && Path.GetExtension(x.Name).Equals(ext, StringComparison.OrdinalIgnoreCase));
        }

        private string GetMaps(ZipArchive za)
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
                using (var extractedZip = ZipFile.OpenRead(extractedFile))
                {
                    sb.Append(GetMaps(extractedZip));
                }
            }

            return sb.ToString();
        }

        private string MapStringFromMapInfo(ZipArchiveEntry zae)
        {
            string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), zae);

            if (File.Exists(extractedFile))
            {
                string mapinfo = File.ReadAllText(extractedFile);
                File.Delete(extractedFile);
                return GetMapStringFromMapInfo(mapinfo);
            }

            return string.Empty;
        }

        private static string GetMapStringFromMapInfo(string mapinfo)
        {
            Regex mapRegex = new Regex(@"\s*map\s+\w+");
            MatchCollection matches = mapRegex.Matches(mapinfo);

            return string.Join(", ", matches.Cast<Match>().Select(x => x.Value.Trim().Substring(3).Trim()));
        }

        private string MapStringFromGameFile(ZipArchive za)
        {
            List<string> maps = new List<string>();

            IEnumerable<ZipArchiveEntry> wadEntries = GetEntriesByExtension(za, ".wad");

            foreach (ZipArchiveEntry zae in wadEntries)
            {
                string extractFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), zae);
                string mapString = Util.GetMapStringFromWad(extractFile);
                if (!string.IsNullOrEmpty(mapString))
                    maps.Add(mapString);
            }

            return string.Join(", ", maps.ToArray());
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

        public string[] DateParseFormats
        {
            get;
            set;
        }
    }
}
