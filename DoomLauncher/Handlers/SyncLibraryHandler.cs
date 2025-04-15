using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WadReader;
using DoomLauncher.Handlers.Sync;
using DoomLauncher.Archive;

namespace DoomLauncher
{
    public class SyncLibraryHandler
    {
        // For looking inside WAD files
        private static readonly string[] MapInfoNames = new string[] { "mapinfo", "zmapinfo" };
        private static readonly string[] MapInfoSubNames = new string[] { "mapinfo.", "zmapinfo." };

        public event EventHandler SyncFileChange; // Notify progress, file by file
        public event EventHandler GameFileDataNeeded; // Ask for data to be filled in from the currently downloaded file


        public IGameFileDataSourceAdapter DbDataSource { get; set; } // Internal
        public LauncherPath TempDirectory { get; set; }  //  Internal
        public string[] DateParseFormats // Internal
        {
            get;
            set;
        }
        public IGameFile CurrentGameFile { get; set; } // Input, Output

        public int SyncFileCurrent { get; private set; } // Output, used by Progress Bar in MainForm_Sync
        public int SyncFileCount { get; private set; } // Output, used by Progress Bar in MainForm_Sync
        public string CurrentSyncFileName { get; private set; } // Output

        private readonly FileManagement m_fileManagement;
        private readonly Palette m_palette;

        private IGameFileDataSourceAdapter SyncDataSource { get; set; }
        private LauncherPath GameFileDirectory { get; set; }


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

            SyncFileCurrent = SyncFileCount = 0;
        }

        public SyncResult Execute(string[] files)
        {
            // Handle multiple files, such that a UI process can track progress
            SyncFileCount = files.Length;
            SyncFileCurrent = 0;

            SyncResult result = SyncResult.EMPTY;

            foreach (string fileName in files)
            {
                if (SyncFileChange != null)
                {
                    CurrentSyncFileName = fileName;
                    SyncFileChange(this, EventArgs.Empty);
                }

                result += SyncFile(fileName);

                SyncFileCurrent++;
            }

            return result;
        }

        public SyncResult SyncFile(string fileName)
        {
            SyncResult resultSoFar = SyncResult.EMPTY;

            IGameFile fileToUpdate = SyncDataSource.GetGameFile(fileName);
            IGameFile existing = DbDataSource.GetGameFile(fileName);

            // If we've already got a copy in the DB, modify that one
            if (existing != null)
                fileToUpdate = existing;

            if (fileToUpdate == null)
            {
                resultSoFar += SyncResult.InvalidFile(fileName, "Not found");

                // Delete the file from managed storage
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

                return resultSoFar;
            }

            // Surely we can bring this inside GameFile (No - m_fileManagement is supplied externally)
            if (m_fileManagement == FileManagement.Unmanaged)
                fileToUpdate.FileName = LauncherPath.GetRelativePath(fileName);

            CurrentGameFile = fileToUpdate; // Handpass the file to the GameFileDataNeeded handler code
            GameFileDataNeeded?.Invoke(this, EventArgs.Empty); // "IF THIS IS THE CURRENTLY DOWNLOADED FILE, FILL IN DETAILS FROM IT"
            fileToUpdate.Downloaded = existing == null ? DateTime.Now : existing.Downloaded;

            try
            {
                using (IArchiveReader reader = new RecursiveArchiveReader(CreateArchiveReader(fileToUpdate), CreateReaderForRecursiveEntry))
                {
                    resultSoFar += PopulateGameFileFromArchive(fileToUpdate, reader);
                }
            }
            catch (IOException)
            {
                fileToUpdate.Map = string.Empty;
                resultSoFar += SyncResult.InvalidFile(fileName, "File is in use/Not found");
            }
            catch (InvalidDataException)
            {
                fileToUpdate.Map = string.Empty;
                resultSoFar += SyncResult.InvalidFile(fileName, "Zip archive invalid or contained an improper pk3");
            }
            catch (Exception ex)
            {
                fileToUpdate.Map = string.Empty;
                resultSoFar += SyncResult.InvalidFile(fileName, CreateExceptionMsg(ex));
            }

            // Upsert to database
            if (existing == null)
            {
                DbDataSource.InsertGameFile(fileToUpdate);

                IGameFile gameFile = DbDataSource.GetGameFile(fileToUpdate.FileName);
                if (gameFile != null)
                    resultSoFar += SyncResult.AddedGameFile(gameFile);
            }
            else
            {
                DbDataSource.UpdateGameFile(fileToUpdate, Util.DefaultGameFileUpdateFields);
                resultSoFar += SyncResult.UpdatedGameFile(fileToUpdate);
            }

            return resultSoFar;
        }

        private IArchiveReader CreateArchiveReader(IGameFile file)
        {
            if (m_fileManagement == FileManagement.Unmanaged)
                return ArchiveReader.Create(new LauncherPath(file.FileName).GetFullPath());

            return ArchiveReader.Create(Path.Combine(GameFileDirectory.GetFullPath(), file.FileName));
        }

        private static string CreateExceptionMsg(Exception ex)
        {
            return string.Concat("Unexpected exception - ", ex.Message, ex.StackTrace);
        }

        private SyncResult PopulateGameFileFromArchive(IGameFile gameFile, IArchiveReader reader)
        {
            // Look in MAPINFO lumps for map string
            var entryList = reader.Entries.ToList();
            var mapInfoEntries = reader.Entries.Where(IsEntryMapInfo).ToArray();
            string[] mapInfoData = GetArchiveEntryData(mapInfoEntries);

            var gameFileFragments = new List<IGameFileFragment>
            {
                new TextFileGameFileFragment(DateParseFormats),
                new TitlePicFileFragment(m_palette),
                new Doom64GameFileFragment(),
                new MapStringGameFileFragment(TempDirectory)
            };

            var syncResults = gameFileFragments.Select(frag => frag.ApplyToGameFile(gameFile, reader, mapInfoData));
            return syncResults.Aggregate(SyncResult.EMPTY, (a, b) => a + b);
        }

        private IArchiveReader CreateReaderForRecursiveEntry(IArchiveEntry entry)
        {
            if (IsRecursiveEntry(entry))
            {
                string extractedFile = Util.ExtractTempFile(TempDirectory.GetFullPath(), entry);
                return ArchiveReader.Create(extractedFile);
            }
            else
            {
                return null;
            }
        }

        private bool IsRecursiveEntry(IArchiveEntry entry)
        {
            List<string> recursiveExtensions = new List<string>(Util.GetReadablePkExtensions()).Append(".wad").ToList();
            return entry.Name.Contains('.') && recursiveExtensions.Exists(ext => ext.Equals(Path.GetExtension(entry.Name), StringComparison.OrdinalIgnoreCase));
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
    }
}
