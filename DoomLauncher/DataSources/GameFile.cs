using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System;
using System.IO;

namespace DoomLauncher.DataSources
{
    public class GameFile : IGameFile, IGameProfile, ICloneable
    {
        public const string DefaultProfileName = "Default Profile";
        public static string[] GetMaps(IGameFile gameFile) => gameFile.Map.Split(new [] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);

        private string m_lastDirectory;

        public GameFile()
        {
            FileName = Title = Author = Description = Thumbnail = Comments = Map = SettingsMap = SettingsSkill = SettingsExtraParams = SettingsFiles
                = SettingsSpecificFiles = string.Empty;
            Name = DefaultProfileName;
            SettingsStat = true;
            GameProfileID = -1;
        }

        public int? GameFileID { get; set; }
        public string FullFileName { get; set; }
        public virtual string FileName { get; set; }
        public string FileNameBase => Path.GetFileNameWithoutExtension(FileName);
        public string FileNameNoPath => Path.GetFileName(FileName);
        public virtual string LastDirectory => GetLastDirectory(FileName);
        public virtual string Title { get; set; }
        public virtual string Author { get; set; }
        public virtual DateTime? ReleaseDate { get; set; }
        public virtual string Description { get; set; }
        public int? IWadID { get; set; }
        public int? SourcePortID { get; set; }
        public string Thumbnail { get; set; }
        public string Comments { get; set; }
        public string Map { get; set; }
        public int? MapCount { get; set; }
        public virtual double? Rating { get; set; }
        public DateTime? LastPlayed { get; set; }
        public DateTime? Downloaded { get; set; }

        public int GameProfileID { get; set; }
        public string Name { get; set; }
        public string SettingsMap { get; set; }
        public string SettingsSkill { get; set; }
        public string SettingsExtraParams { get; set; }
        public string SettingsFiles { get; set; }
        public string SettingsFilesSourcePort { get; set; }
        public string SettingsFilesIWAD { get; set; }
        public string SettingsSpecificFiles { get; set; }
        public bool SettingsStat { get; set; }
        public bool SettingsLoadLatestSave { get; set; }
        public bool SettingsSaved { get; set; }
        public bool SettingsExtraParamsOnly { get; set; }
        public int? SettingsGameProfileID { get; set; }

        public bool IsGlobal => false;

        public int MinutesPlayed { get; set; }
        public virtual int FileSizeBytes { get; set; }

        public bool IsDoom64 { get; set; }

        public bool IsUnmanaged() => IsUnmanaged(FileName);

        public static bool IsUnmanaged(string filename) => 
            Path.IsPathRooted(filename) || (PathExtensions.IsPartiallyQualified(filename) && HasPathSeparator(filename));

        private static bool HasPathSeparator(string filename)
        {
            for (int i = 0; i < filename.Length; i++)
            {
                if (filename[i] == Path.DirectorySeparatorChar || filename[i] == Path.AltDirectorySeparatorChar)
                    return true;
            }

            return false;
        }

        public bool IsDirectory()
        {
            if (!IsUnmanaged())
                return false;

            return Util.IsDirectory(FileName);
        }

        public IArchiveReader OpenGameFile(LauncherPath gameFileDirectory)
        {
            string file;
            if (IsUnmanaged())
                file = new LauncherPath(FileName).GetFullPath();
            else
                file = Path.Combine(gameFileDirectory.GetFullPath(), FileName);

            // If the unmanaged file is a pk3 then ArchiveReader.Create will read it as a zip and try to unpack
            // Return FileArchiveReader instead so the pk3 will be added as a file
            // Zip extensions are ignored in this case since Doom Launcher's base functionality revovles around reading zip contents
            // SpecificFilesForm will also read zip files explicitly to allow user to select files in the archive
            if (!IsDirectory() && IsUnmanaged() && !ArchiveUtil.ShouldReadPackagedArchive(FileName))
                return new FileArchiveReader(file);

            return ArchiveReader.Create(file);
        }

        public bool ArchiveExists(LauncherPath gameFileDirectory)
        {
            if (IsDirectory())
            {
                return Directory.Exists(FileName);
            }
            else if (IsUnmanaged())
            {
                var launcherPath = new LauncherPath(FileName);
                FileInfo fi = new FileInfo(launcherPath.GetFullPath());
                return fi.Exists;
            }
            else 
            {
                FileInfo fi = new FileInfo(Path.Combine(gameFileDirectory.GetFullPath(), FileName));
                return fi.Exists;
            }
        }

        public object Clone()
        {
            GameFile gameFile = new GameFile();
            var properties = gameFile.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetSetMethod() != null)
                    prop.SetValue(gameFile, prop.GetValue(this));
            }

            return gameFile;
        }

        public override bool Equals(object obj)
        {
            if (obj is IGameFile gameFile)
                return gameFile.FileName == FileName;

            return false;
        }

        public override int GetHashCode()
        {
            if (FileName != null)
                return FileName.GetHashCode();

            return 0;
        }

        public override string ToString()
        {
            if (FileName == null)
                return string.Empty;
            return FileName;
        }

        private string GetLastDirectory(string path)
        {
            if (m_lastDirectory != null)
                return m_lastDirectory;

            if (!IsUnmanaged())
            {
                m_lastDirectory = string.Empty;
                return m_lastDirectory;
            }

            string dir = Path.GetDirectoryName(FileName);
            m_lastDirectory = new DirectoryInfo(dir).Name;
            return m_lastDirectory;
        }
    }
}
