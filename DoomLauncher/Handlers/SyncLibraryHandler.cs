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
        public event EventHandler SyncFileChange;
        public event EventHandler GameFileDataNeeded;

        public List<IGameFile> AddedGameFiles => m_addedFiles;
        public List<IGameFile> UpdatedGameFiles => m_updatedFiles;
        public List<IGameFile> FailedTitlePicFiles => m_failedTitlepics;

        private readonly List<IGameFile> m_addedFiles = new List<IGameFile>();
        private readonly List<IGameFile> m_updatedFiles = new List<IGameFile>();
        private readonly List<InvalidFile> m_invalidFiles = new List<InvalidFile>();
        private readonly HashSet<string> m_includeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly FileManagement m_fileManagement;
        private readonly Palette m_palette;
        private readonly Dictionary<IGameFile, Image> m_titlepics = new Dictionary<IGameFile, Image>();
        private readonly List<IGameFile> m_failedTitlepics = new List<IGameFile>();
        private readonly bool m_pullTitlepic;

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

        public void Execute(string[] zipFiles)
        {
            ClearData();

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

                if (m_fileManagement == FileManagement.Unmanaged)
                    file.FileName = fileName;

                if (file != null)
                {
                    CurrentGameFile = file;
                    GameFileDataNeeded?.Invoke(this, EventArgs.Empty);
                    file.Downloaded = existing == null ? DateTime.Now : existing.Downloaded;

                    try
                    {
                        using (IArchiveReader reader = ArchiveReader.Create(Path.Combine(GameFileDirectory.GetFullPath(), file.FileName)))
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

        private void AddTitlepic(IGameFile file, IArchiveReader reader)
        {
            if (!m_pullTitlepic || m_titlepics.ContainsKey(file) || !DoomImageUtil.FindTitlepic(reader, out IArchiveEntry entry))
                return;

            Palette palette = m_palette;
            if (DoomImageUtil.FindPalette(reader, out IArchiveEntry paletteEntry))
                palette = Palette.From(paletteEntry.ReadEntry());

            if (!DoomImageUtil.ConvertToImage(entry.ReadEntry(), palette, out Image image))
            {
                if (!m_failedTitlepics.Contains(file))
                    m_failedTitlepics.Add(file);
                return;
            }

            m_titlepics[file] = image;
        }

        private void AddLatestGameFile(string filename, List<IGameFile> gameFiles)
        {
            IGameFile gameFile = DbDataSource.GetGameFile(filename);
            if (gameFile != null)
                gameFiles.Add(gameFile);
        }

        private void PopulateGameFileData(IGameFile file, IArchiveReader reader)
        {
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

            var txtEntries = GetEntriesByExtension(reader, ".txt").Where(x => x.Name.Equals("mapinfo.txt", StringComparison.InvariantCultureIgnoreCase));
            if (txtEntries.Any())
                AppendMapSet(sb, MapStringFromMapInfo(reader, txtEntries.First()));

            AddTitlepic(gameFile, reader);
            AppendMapSet(sb, MapStringFromGameFile(reader));

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

        private string MapStringFromMapInfo(IArchiveReader reader, IArchiveEntry entry)
        {
            StringBuilder sb = new StringBuilder();
            string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);

            if (File.Exists(extractedFile))
            {
                string mapinfo = File.ReadAllText(extractedFile);
                AppendMapSet(sb, ParseMapInfoInclude(reader, mapinfo));
                if (entry.ExtractRequired)
                    File.Delete(extractedFile);
                AppendMapSet(sb, GetMapStringFromMapInfo(mapinfo));
            }

            return sb.ToString();
        }

        private string ParseMapInfoInclude(IArchiveReader reader, string mapinfo)
        {
            StringBuilder sb = new StringBuilder();
            Regex mapRegex = new Regex(@"\s*include\s+(\S+)");
            MatchCollection matches = mapRegex.Matches(mapinfo);
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

                AppendMapSet(sb, MapStringFromMapInfo(reader, entry));
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

        private static string GetMapStringFromMapInfo(string mapinfo)
        {
            Regex mapRegex = new Regex(@"\s*map\s+\w+");
            MatchCollection matches = mapRegex.Matches(mapinfo);

            return string.Join(", ", matches.Cast<Match>().Select(x => x.Value.Trim().Substring(3).Trim()));
        }

        private string MapStringFromGameFile(IArchiveReader reader)
        {
            List<string> maps = new List<string>();

            IEnumerable<IArchiveEntry> wadEntries = GetEntriesByExtension(reader, ".wad");

            foreach (IArchiveEntry entry in wadEntries)
            {
                string extractFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);
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
