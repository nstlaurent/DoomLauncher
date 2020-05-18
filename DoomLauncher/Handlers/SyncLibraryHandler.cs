using DoomLauncher.Interfaces;
using DoomLauncher.TextFileParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoomLauncher
{
    public class SyncLibraryHandler
    {
        public event EventHandler SyncFileChange;
        public event EventHandler GameFileDataNeeded;

        private readonly List<InvalidFile> m_invalidFiles = new List<InvalidFile>();
        private readonly FileManagement m_fileManagement;

        public SyncLibraryHandler(IGameFileDataSourceAdapter dbDataSource, IGameFileDataSourceAdapter syncDataSource,
            LauncherPath gameFileDirectory, LauncherPath tempDirectory, string[] dateParseFormats, FileManagement fileManagement)
        {
            DbDataSource = dbDataSource;
            SyncDataSource = syncDataSource;
            GameFileDirectory = gameFileDirectory;
            TempDirectory = tempDirectory;
            DateParseFormats = dateParseFormats;
            m_fileManagement = fileManagement;

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

                if (m_fileManagement == FileManagement.Unmanaged)
                    file.FileName = fileName;

                if (file != null)
                {
                    CurrentGameFile = file;
                    GameFileDataNeeded?.Invoke(this, new EventArgs());
                    file.Downloaded = DateTime.Now;

                    try
                    {
                        using (IArchiveReader reader = ArchiveReader.Create(Path.Combine(GameFileDirectory.GetFullPath(), file.FileName)))
                        {
                            FillTextFileInfo(file, reader);
                            FillMapInfo(file, reader);
                        }
                    }
                    catch(IOException)
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

        private void FillMapInfo(IGameFile file, IArchiveReader reader)
        {
            file.Map = GetMaps(reader);
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

        private string GetMaps(IArchiveReader reader)
        {
            var txtEntries = GetEntriesByExtension(reader, ".txt").Where(x => x.Name.Equals("mapinfo.txt", StringComparison.InvariantCultureIgnoreCase));
            if (txtEntries.Any())
                return MapStringFromMapInfo(txtEntries.First());

            StringBuilder sb = new StringBuilder();
            sb.Append(MapStringFromGameFile(reader));

            IEnumerable<IArchiveEntry> pk3Entries = Util.GetEntriesByExtension(reader, Util.GetReadablePkExtensions());

            foreach (IArchiveEntry entry in pk3Entries)
            {
                string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);
                using (var extractedZip = ArchiveReader.Create(extractedFile))
                    sb.Append(GetMaps(extractedZip));
            }

            return sb.ToString();
        }

        private string MapStringFromMapInfo(IArchiveEntry entry)
        {
            string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);

            if (File.Exists(extractedFile))
            {
                string mapinfo = File.ReadAllText(extractedFile);
                if (entry.ExtractRequired)
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
