using DoomLauncher.Interfaces;
using DoomLauncher.TextFileParsers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WadReader;

namespace DoomLauncher
{
    public class SyncLibraryHandler
    {
        private static readonly string[] MapInfoNames = new string[] { "mapinfo", "zmapinfo" };
        private static readonly string[] MapInfoSubNames = new string[] { "mapinfo.", "zmapinfo." };
        private static readonly Regex TitlePageRegex = new Regex(@"titlepage\s*=\s*""([^""]*)""");
        private static readonly Regex IncludeRegex = new Regex(@"\s*include\s+(\S+)");
        private static readonly Regex MapRegex = new Regex(@"\s*map\s+\w+");

        public event EventHandler SyncFileChange;
        public event EventHandler GameFileDataNeeded;

        public List<IGameFile> AddedGameFiles => m_addedFiles;
        public List<IGameFile> UpdatedGameFiles => m_updatedFiles;
        public List<IGameFile> FailedTitlePicFiles => m_failedTitlepics;

        private readonly List<IGameFile> m_addedFiles = new List<IGameFile>();
        private readonly List<IGameFile> m_updatedFiles = new List<IGameFile>();
        private readonly List<InvalidFile> m_invalidFiles = new List<InvalidFile>();
        private readonly HashSet<string> m_includeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> m_mapSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly FileManagement m_fileManagement;
        private readonly Palette m_palette;
        private readonly Dictionary<IGameFile, Image> m_titlepics = new Dictionary<IGameFile, Image>();
        private readonly List<IGameFile> m_failedTitlepics = new List<IGameFile>();
        private readonly bool m_pullTitlepic;
        private bool m_readMapInfo;

        public SyncLibraryHandler(IGameFileDataSourceAdapter dbDataSource, IGameFileDataSourceAdapter syncDataSource,
            LauncherPath gameFileDirectory, LauncherPath tempDirectory, string[] dateParseFormats, FileManagement fileManagement,
            Palette palette, bool pullTitlepic)
        {
            DbDataSource = dbDataSource;
            SyncDataSource = syncDataSource;
            GameFileDirectory = gameFileDirectory;
            TempDirectory = tempDirectory;
            DateParseFormats = dateParseFormats;
            m_fileManagement = fileManagement;
            m_palette = palette;
            m_pullTitlepic = pullTitlepic;

            SyncFileCurrent = SyncFileCount = 0;
        }

        public void Execute(string[] files)
        {
            ClearData();

            SyncFileCount = files.Length;
            SyncFileCurrent = 0;

            foreach (string fileName in files)
            {
                if (SyncFileChange != null)
                {
                    CurrentSyncFileName = fileName;
                    SyncFileChange(this, EventArgs.Empty);
                }

                IGameFile file = SyncDataSource.GetGameFile(fileName);
                IGameFile existing = DbDataSource.GetGameFile(fileName);

                if (existing != null)
                    file = existing;

                if (file == null)
                {
                    m_invalidFiles.Add(new InvalidFile(fileName, "Not found"));

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

                    continue;
                }

                if (m_fileManagement == FileManagement.Unmanaged)
                    file.FileName = LauncherPath.GetRelativePath(fileName);

                CurrentGameFile = file;
                GameFileDataNeeded?.Invoke(this, EventArgs.Empty);
                file.Downloaded = existing == null ? DateTime.Now : existing.Downloaded;

                try
                {
                    using (IArchiveReader reader = CreateArchiveReader(file))
                    {
                        FillTextFileInfo(file, reader);
                        PopulateGameFileData(file, reader);
                    }
                }
                catch (IOException)
                {
                    file.Map = string.Empty;
                    m_invalidFiles.Add(new InvalidFile(fileName, "File is in use/Not found"));
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
                {
                    DbDataSource.InsertGameFile(file);
                    AddLatestGameFile(file.FileName, m_addedFiles);
                }
                else
                {
                    DbDataSource.UpdateGameFile(file, Util.DefaultGameFileUpdateFields);
                    m_updatedFiles.Add(file);
                }                

                SyncFileCurrent++;
            }
        }

        private IArchiveReader CreateArchiveReader(IGameFile file)
        {
            if (m_fileManagement == FileManagement.Unmanaged)
                return ArchiveReader.Create(new LauncherPath(file.FileName).GetFullPath());

            return ArchiveReader.Create(Path.Combine(GameFileDirectory.GetFullPath(), file.FileName));
        }

        private void ClearData()
        {
            m_addedFiles.Clear();
            m_updatedFiles.Clear();
            m_invalidFiles.Clear();
            m_includeFiles.Clear();
            m_titlepics.Clear();
            m_failedTitlepics.Clear();
        }

        public bool GetTitlePic(IGameFile gameFile, out Image image) =>
            m_titlepics.TryGetValue(gameFile, out image);

        private void AddTitlepic(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            if (!m_pullTitlepic || m_titlepics.ContainsKey(file))
                return;

            string titlepicName = DoomImageUtil.TitlepicName;
            if (GetTitlepicNameFromMapInfo(mapInfoData, out string newTitlepicName))
                titlepicName = newTitlepicName;

            if (!DoomImageUtil.GetEntry(reader, titlepicName, out IArchiveEntry entry))
                return;

            Palette palette = GetPaletteOrDefault(reader);
            if (!DoomImageUtil.ConvertToImage(entry.ReadEntry(), palette, out Image image))
            {
                if (!m_failedTitlepics.Contains(file))
                    m_failedTitlepics.Add(file);
                return;
            }

            m_titlepics[file] = image;
        }

        private Palette GetPaletteOrDefault(IArchiveReader reader)
        {
            if (!DoomImageUtil.FindPalette(reader, out IArchiveEntry paletteEntry))
                return m_palette;

            Palette palette = Palette.From(paletteEntry.ReadEntry());
            if (palette != null)
                return palette;

            return m_palette;
        }

        private bool GetTitlepicNameFromMapInfo(string[] mapInfoData, out string newTitlepicName)
        {            
            newTitlepicName = string.Empty;
            foreach (string data in mapInfoData)
            {
                Match match = TitlePageRegex.Match(data);
                if (!match.Success)
                    continue;

                newTitlepicName = match.Groups[1].Value;
                return true;

            }

            return false;
        }

        private void AddLatestGameFile(string filename, List<IGameFile> gameFiles)
        {
            IGameFile gameFile = DbDataSource.GetGameFile(filename);
            if (gameFile != null)
                gameFiles.Add(gameFile);
        }

        private void PopulateGameFileData(IGameFile file, IArchiveReader reader)
        {
            m_readMapInfo = false;
            m_mapSet.Clear();
            ReadArchiveEntries(file, reader, out string maps);
            file.Map = maps;
            if (!string.IsNullOrEmpty(file.Map))
                file.MapCount = file.Map.Count(x => x == ',') + 1;
        }

        private void FillTextFileInfo(IGameFile gameFile, IArchiveReader reader)
        {
            bool bParsedTxt = false;

            foreach (var entry in reader.Entries)
            {
                if (Path.GetExtension(entry.FullName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] buffer = new byte[entry.Length];
                    try
                    {
                        entry.Read(buffer, 0, Convert.ToInt32(entry.Length));
                    }
                    catch (Exception)
                    {
                        // Do not fail because we couldn't read a text file
                    }

                    IdGamesTextFileParser parser = new IdGamesTextFileParser(DateParseFormats);
                    parser.Parse(Encoding.UTF7.GetString(buffer));

                    bParsedTxt = !string.IsNullOrEmpty(parser.Title) || !string.IsNullOrEmpty(parser.Author) || !string.IsNullOrEmpty(parser.Description);

                    if (bParsedTxt)
                    {
                        gameFile.Title = parser.Title;
                        gameFile.Author = parser.Author;
                        gameFile.ReleaseDate = parser.ReleaseDate;
                        gameFile.Description = parser.Description;
                    }
                }

                if (bParsedTxt)
                    break;
            }

            if (string.IsNullOrEmpty(gameFile.Title))
                gameFile.Title = gameFile.FileNameNoPath;
        }

        private static string CreateExceptionMsg(Exception ex)
        {
            return string.Concat("Unexpected exception - ", ex.Message, ex.StackTrace);
        }

        private IEnumerable<IArchiveEntry> GetEntriesByExtension(IArchiveReader reader, string ext)
        {
            return reader.Entries.Where(x => x.Name.Contains('.') && Path.GetExtension(x.Name).Equals(ext, StringComparison.OrdinalIgnoreCase));
        }

        private void ReadArchiveEntries(IGameFile gameFile, IArchiveReader reader, out string maps)
        {
            maps = string.Empty;
            StringBuilder sb = new StringBuilder();

            var mapInfoEntries = reader.Entries.Where(x => IsEntryMapInfo(x)).ToArray();
            string[] mapInfoData = Array.Empty<string>();
            if (mapInfoEntries.Length > 0)
            {
                mapInfoData = GetArchiveEntryData(mapInfoEntries);
                m_readMapInfo = true;
                foreach (var mapInfo in mapInfoData)
                    AppendMapSet(sb, MapStringFromMapInfo(reader, mapInfo));
            }

            AddTitlepic(gameFile, reader, mapInfoData);
            // Only scan wad files if there is no MapInfo
            if (!m_readMapInfo)
                AppendMapSet(sb, MapStringFromGameFileWads(reader));

            IEnumerable<IArchiveEntry> pk3Entries = Util.GetEntriesByExtension(reader, Util.GetReadablePkExtensions());
            IEnumerable<IArchiveEntry> wadEntries = Util.GetEntriesByExtension(reader, new string[] { ".wad" });

            foreach (IArchiveEntry entry in pk3Entries.Union(wadEntries))
            {
                string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);
                using (var extractedZip = ArchiveReader.Create(extractedFile))
                {
                    ReadArchiveEntries(gameFile, extractedZip, out string extractedmaps);
                    AppendMapSet(sb, extractedmaps);
                }
            }

            maps = sb.ToString();
        }

        private string[] GetArchiveEntryData(params IArchiveEntry[] entries)
        {
            string[] data = new string[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                try
                {
                    data[i] = Encoding.UTF8.GetString(entries[i].ReadEntry());
                }
                catch
                {
                    data[i] = string.Empty;
                }
            }

            return data;
        }

        private static bool IsEntryMapInfo(IArchiveEntry entry)
        {
            try
            {
                string entryName = entry.GetNameWithoutExtension();
                foreach (string name in MapInfoNames)
                {
                    if (entryName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                foreach (string name in MapInfoSubNames)
                {
                    if (entryName.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private string MapStringFromMapInfo(IArchiveReader reader, string mapInfoData)
        {
            StringBuilder sb = new StringBuilder();
            AppendMapSet(sb, ParseMapInfoInclude(reader, mapInfoData));
            AppendMapSet(sb, GetMapStringFromMapInfo(mapInfoData));

            return sb.ToString();
        }

        private string ParseMapInfoInclude(IArchiveReader reader, string mapinfo)
        {
            StringBuilder sb = new StringBuilder();
            
            MatchCollection matches = IncludeRegex.Matches(mapinfo);
            foreach (Match match in matches)
            {
                if (match.Groups.Count < 2)
                    continue;

                string file = match.Groups[1].Value.Trim();
                if (m_includeFiles.Contains(file))
                    continue;

                m_includeFiles.Add(file);

                var entry = reader.Entries.FirstOrDefault(x => x.FullName.Equals(file, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                    continue;

                string[] entryData = GetArchiveEntryData(entry);
                if (entryData.Length > 0)
                    AppendMapSet(sb, MapStringFromMapInfo(reader, entryData[0]));
            }

            return sb.ToString();
        }

        private static void AppendMapSet(StringBuilder sb, string maps)
        {
            if (string.IsNullOrWhiteSpace(maps))
                return;

            if (sb.Length > 0)
                sb.Append(", ");
            sb.Append(maps);
        }

        private string GetMapStringFromMapInfo(string mapinfo)
        {            
            MatchCollection matches = MapRegex.Matches(mapinfo);

            List<string> maps = new List<string>();
            var mapMatches = matches.Cast<Match>().Select(x => x.Value.Trim().Substring(3).Trim());
            foreach (var mapMatch in mapMatches)
            {
                if (m_mapSet.Contains(mapMatch))
                    continue;

                m_mapSet.Add(mapMatch);
                maps.Add(mapMatch);
            }

            return string.Join(", ", maps);
        }

        private string MapStringFromGameFileWads(IArchiveReader reader)
        {
            List<string> maps = new List<string>();
            if (reader is WadArchiveReader wadArchive)
            {
                SetMapsFromWadFile(maps, wadArchive.Filename);
            }
            else
            {
                IEnumerable<IArchiveEntry> wadEntries = GetEntriesByExtension(reader, ".wad");
                foreach (IArchiveEntry entry in wadEntries)
                {
                    string extractFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);
                    SetMapsFromWadFile(maps, extractFile);
                }
            }

            return string.Join(", ", maps);
        }

        private void SetMapsFromWadFile(List<string> maps, string extractFile)
        {
            List<string> wadEntryMaps = Util.GetMapStringFromWad(extractFile);
            foreach (string map in wadEntryMaps)
            {
                if (m_mapSet.Contains(map))
                    continue;

                m_mapSet.Add(map);
                maps.Add(map);
            }
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
