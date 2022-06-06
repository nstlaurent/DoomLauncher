using DoomLauncher.DataSources;
using DoomLauncher.Handlers;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoomLauncher
{
    public class VersionHandler
    {
        private readonly IDataSourceAdapter m_adapter;
        private readonly AppConfiguration m_appConfig;

        public event EventHandler UpdateProgress;
        public event EventHandler UpdateComplete;
        public int ProgressPercent { get; private set; }

        public VersionHandler(DataAccess access, IDataSourceAdapter adapter, AppConfiguration appConfig)
        {
            DataAccess = access;
            m_adapter = adapter;
            m_appConfig = appConfig;
        }

        public bool UpdateRequired()
        {
            AppVersion version = GetVersion();
            var latestVersion = Enum.GetValues(typeof(AppVersion)).Cast<AppVersion>().Last();

            if (version == AppVersion.Unknown || version < latestVersion)
                return true;

            return false;
        }

        private void ExecuteUpdate(Action update, AppVersion version)
        {
            if (GetVersion() < version || version == AppVersion.Unknown)
            {
                update();

                if (version != AppVersion.Unknown)
                    WriteVersion(version);
            }
        }

        public void HandleVersionUpdate()
        {
            if (UpdateRequired())
            {
                ExecuteUpdate(Pre_0_9_2, AppVersion.Unknown);
                ExecuteUpdate(Pre_1_0_0, AppVersion.Version_1_0_0);
                ExecuteUpdate(Pre_1_1_0, AppVersion.Version_1_1_0);
                ExecuteUpdate(Pre_2_1_0, AppVersion.Version_2_1_0);
                ExecuteUpdate(Pre_2_2_0, AppVersion.Version_2_2_0);
                ExecuteUpdate(Pre_2_2_1, AppVersion.Version_2_2_1);
                ExecuteUpdate(Pre_2_3_0, AppVersion.Version_2_3_0);
                ExecuteUpdate(Pre_2_4_0, AppVersion.Version_2_4_0);
                ExecuteUpdate(Pre_2_4_1, AppVersion.Version_2_4_1);
                ExecuteUpdate(Pre_2_6_0, AppVersion.Version_2_6_0);
                ExecuteUpdate(Pre_2_6_3, AppVersion.Version_2_6_3);
                ExecuteUpdate(Pre_2_6_3_1, AppVersion.Version_2_6_3_1);
                ExecuteUpdate(Pre_2_6_3_2, AppVersion.Version_2_6_3_2);
                ExecuteUpdate(Pre_2_6_4_1, AppVersion.Version_2_6_4_1);
                ExecuteUpdate(Pre_2_6_4_1_Update, AppVersion.Version_2_6_4_1_Update1);
                ExecuteUpdate(Pre_2_7_0_0, AppVersion.Version_2_7_0_0);
                ExecuteUpdate(Pre_2_8_0_0, AppVersion.Version_2_8_0_0);
                ExecuteUpdate(Pre_2_8_0_0_1, AppVersion.Version_2_8_0_0_1);
                ExecuteUpdate(Pre_Version_3_1_0, AppVersion.Version_3_1_0);
                ExecuteUpdate(Pre_Version_3_2_0, AppVersion.Version_3_2_0);
                ExecuteUpdate(Pre_Version_3_2_0_Update1, AppVersion.Version_3_2_0_Update1);
                ExecuteUpdate(Pre_Version_3_2_0_Update2, AppVersion.Version_3_2_0_Update2);
                ExecuteUpdate(Pre_Version_3_2_0_Update3, AppVersion.Version_3_2_0_Update3);
                ExecuteUpdate(Pre_Version_3_3_0, AppVersion.Version_3_3_0);
                ExecuteUpdate(Pre_Version_3_3_1, AppVersion.Version_3_3_1);
                ExecuteUpdate(Pre_Version_3_3_2, AppVersion.Version_3_3_2);
                ExecuteUpdate(Pre_Version_3_4_0, AppVersion.Version_3_4_0);
                ExecuteUpdate(Pre_Version_3_4_0_Update1, AppVersion.Version_3_4_0_Update1);
                ExecuteUpdate(Pre_Version_3_5_2, AppVersion.Version_3_5_2);
                ExecuteUpdate(Pre_Version_3_5_2_Update1, AppVersion.Version_3_5_2_Update1);
            }
        }

        private AppVersion GetVersion()
        {
            IConfigurationData config = m_adapter.GetConfiguration().FirstOrDefault(x => x.Name == "Version");

            if (config != null)
                return (AppVersion)Convert.ToInt32(config.Value);

            return AppVersion.Unknown;
        }

        private void WriteVersion(AppVersion version)
        {
            IConfigurationData config = m_adapter.GetConfiguration().FirstOrDefault(x => x.Name == "Version");

            if (config != null)
            {
                config.Value = Convert.ToInt32(version).ToString();
                m_adapter.UpdateConfiguration(config);
            }
            else
            {
                ConfigurationData newConfig = new ConfigurationData();
                newConfig.Name = "Version";
                newConfig.UserCanModify = false;
                newConfig.Value = Convert.ToInt32(version).ToString();
                m_adapter.InsertConfiguration(newConfig);
            }
        }

        public void Pre_1_0_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];

            if (!dt.Select("name = 'SettingsMap'").Any())
            {
                string query = @"alter table GameFiles add column 'SettingsMap' TEXT;
                alter table GameFiles add column 'SettingsSkill' TEXT;
                alter table GameFiles add column 'SettingsExtraParams' TEXT;
                alter table GameFiles add column 'SettingsFiles' TEXT;";

                DataAccess.ExecuteNonQuery(query);
            }

            dt = DataAccess.ExecuteSelect("select * from Configuration where Name = 'ColumnConfig'").Tables[0];

            if (dt.Rows.Count == 0)
            {
                string query = @"insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('SplitTopBottom', '475', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('SplitLeftRight', '680', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('AppWidth', '1024', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('AppHeight', '768', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('AppX', '0', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('AppY', '0', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('WindowState', 'Normal', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('ColumnConfig', '', '', 0);";
                DataAccess.ExecuteNonQuery(query);
            }
        }

        public void Pre_0_9_2()
        {
            DataTable dt = DataAccess.ExecuteSelect("select name from sqlite_master where type='table' and name='Configuration';").Tables[0];

            if (dt.Rows.Count == 0)
            {
                string query = @"CREATE TABLE 'Configuration' (
	                'ConfigID'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                'Name'	TEXT NOT NULL,
	                'Value'	TEXT NOT NULL,
	                'AvailableValues'	TEXT NOT NULL,
	                'UserCanModify'	INTEGER);";

                DataAccess.ExecuteNonQuery(query);

                query = string.Format(@"insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('IdGamesUrl', 'http://www.doomworld.com/idgames/', '', 1);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('ApiPage', 'api/api.php', '', 1);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('MirrorUrl', 'ftp://mancubus.net/pub/idgames/', 'Germany;ftp://ftp.fu-berlin.de/pc/games/idgames/;Idaho;ftp://mirrors.syringanetworks.net/idgames/;Greece;ftp://ftp.ntua.gr/pub/vendors/idgames/;Texas;ftp://mancubus.net/pub/idgames/;New York;http://youfailit.net/pub/idgames/;Florida;http://www.gamers.org/pub/idgames/;', 1);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('ScreenshotCaptureDirectories', '', '', 1);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('DateParseFormats', 'dd/M/yy;dd/MM/yyyy;dd MMMM yyyy;', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('CleanTemp', 'true', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('GameFileDirectory', '{0}', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('ScreenshotDirectory', '{1}', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('TempDirectory', '{2}', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('GameWadDirectory', '{3}', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('DemoDirectory', '{4}', '', 0);
                insert into Configuration (Name, Value, AvailableValues, UserCanModify) values('SaveGameDirectory', '{5}', '', 0);",
                ConfigurationManager.AppSettings["GameFileDirectory"], ConfigurationManager.AppSettings["ScreenshotDirectory"], ConfigurationManager.AppSettings["TempDirectory"],
                ConfigurationManager.AppSettings["GameWadDirectory"], ConfigurationManager.AppSettings["DemoDirectory"], ConfigurationManager.AppSettings["GameFileDirectory"] + "SaveGames\\");

                DataAccess.ExecuteNonQuery(query);

                DirectoryInfo di = new DirectoryInfo(Path.Combine(LauncherPath.GetDataDirectory(), ConfigurationManager.AppSettings["GameFileDirectory"], "SaveGames"));
                if (!di.Exists)
                    di.Create();
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(Files);").Tables[0];

            if (!dt.Select("name = 'OriginalFileName'").Any())
            {
                string query = @"alter table Files add column 'OriginalFileName' TEXT;
                alter table Files add column 'OriginalFilePath' TEXT;";

                DataAccess.ExecuteNonQuery(query);
            }
        }

        public void Pre_1_1_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("select name from sqlite_master where type='table' and name='Tags';").Tables[0];
            //tag table update
            if (dt.Rows.Count == 0)
            {
                string query = @"CREATE TABLE 'Tags' (
	            'TagID'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	            'Name'	TEXT NOT NULL,
	            'HasTab'	INTEGER NOT NULL);";
                DataAccess.ExecuteSelect(query);

                query = @"CREATE TABLE 'TagMapping' (
	            'FileID'	INTEGER NOT NULL,
	            'TagID'	INTEGER NOT NULL,
	            PRIMARY KEY(FileID,TagID));";
                DataAccess.ExecuteSelect(query);
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(Tags);").Tables[0];

            if (!dt.Select("name = 'Color'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table Tags add column 'HasColor' int;");
                DataAccess.ExecuteNonQuery("alter table Tags add column 'Color' int;");
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];
            //GameFile map count update
            if (!dt.Select("name = 'MapCount'").Any())
            {
                CreateDatabaseBackup();

                DataAccess.ExecuteNonQuery("alter table GameFiles add column 'MapCount' int;");

                GameFileGetOptions options = new GameFileGetOptions();
                options.SelectFields = new GameFileFieldType[] { GameFileFieldType.GameFileID, GameFileFieldType.Map };
                IEnumerable<IGameFile> gameFiles = m_adapter.GetGameFiles(options);

                GameFileFieldType[] updateFields = new GameFileFieldType[] { GameFileFieldType.MapCount };
                float total = gameFiles.Count();
                int count = 0;

                foreach (IGameFile gameFile in gameFiles)
                {
                    if (UpdateProgress != null)
                    {
                        ProgressPercent = Convert.ToInt32(count / total * 100);
                        UpdateProgress(this, new EventArgs());
                    }

                    if (gameFile.Map != null)
                    {
                        gameFile.MapCount = gameFile.Map.Count(x => x == ',') + 1;
                        m_adapter.UpdateGameFile(gameFile, updateFields);
                    }

                    count++;
                }
            }

            if (!dt.Select("name = 'SettingsSpecificFiles'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table GameFiles add column 'SettingsSpecificFiles' TEXT;");
            }

            WriteVersion(AppVersion.Version_1_1_0);

            UpdateComplete?.Invoke(this, new EventArgs());
        }

        [Conditional("RELEASE")]
        private static void CreateDatabaseBackup()
        {
            FileInfo fi = new FileInfo(DbDataSourceAdapter.DatabaseFileName);
            fi.CopyTo(string.Format("{0}_{1}.sqlite.bak", DbDataSourceAdapter.DatabaseFileName, Guid.NewGuid().ToString()));
        }

        private void Pre_2_1_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(Files);").Tables[0];

            if (!dt.Select("name = 'FileOrder'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table Files add column 'FileOrder' int;");
                DataAccess.ExecuteNonQuery("update Files set FileOrder = 2");
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];

            if (!dt.Select("name = 'MinutesPlayed'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table GameFiles add column 'MinutesPlayed' int;");
                DataAccess.ExecuteNonQuery("update GameFiles set MinutesPlayed = 0");
            }
        }

        private void Pre_2_2_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("select name from sqlite_master where type='table' and name='Stats';").Tables[0];

            if (dt.Rows.Count == 0)
            {
                DataAccess.ExecuteNonQuery("update Configuration set UserCanModify = 1 where Name = 'GameFileDirectory'");

                string query = @"CREATE TABLE 'Stats' (
	            'StatID'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	            'GameFileID'	INTEGER NOT NULL,
                'KillCount'	INTEGER NOT NULL,
                'TotalKills'	INTEGER NOT NULL,
                'SecretCount'	INTEGER NOT NULL,
                'TotalSecrets'	INTEGER NOT NULL,
                'LevelTime'	REAL NOT NULL,
                'ItemCount'	INTEGER NOT NULL,
                'TotalItems'	INTEGER NOT NULL,
                'SourcePortID'	INTEGER NOT NULL,
                'MapName'	TEXT NOT NULL,
                'RecordTime'	TEXT NOT NULL);";

                DataAccess.ExecuteNonQuery(query);
            }
        }

        private void Pre_2_2_1()
        {
            IEnumerable<ISourcePortData> sourcePorts = m_adapter.GetSourcePorts();

            foreach (ISourcePortData sourcePort in sourcePorts)
            {
                if (sourcePort.SupportedExtensions.Contains(".pk3"))
                    sourcePort.SupportedExtensions = sourcePort.SupportedExtensions.Replace(".pk3", ".pk3,.pk7");

                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(DataAccess.DbAdapter.CreateParameter("ext", sourcePort.SupportedExtensions));
                parameters.Add(DataAccess.DbAdapter.CreateParameter("SourcePortID", sourcePort.SourcePortID));
                DataAccess.ExecuteNonQuery("update SourcePorts set SupportedExtensions = @ext where SourcePortID = @SourcePortID", parameters);
            }
        }

        private void Pre_2_3_0()
        {
            IEnumerable<IStatsData> stats = m_adapter.GetStats();
            HashSet<IStatsData> statSet = new HashSet<IStatsData>();

            foreach (IStatsData stat in stats)
            {
                if (!statSet.Contains(stat))
                    statSet.Add(stat);
                else
                    m_adapter.DeleteStats(stat.StatID);
            }
        }

        private void Pre_2_4_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(IWads);").Tables[0];

            if (!dt.Select("name = 'GameFileID'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table IWads add column 'GameFileID' int;");

                IEnumerable<IIWadData> iwads = m_adapter.GetIWads();
                IEnumerable<IGameFile> gameFiles = m_adapter.GetGameFiles();

                foreach (IIWadData iwad in iwads)
                {
                    IGameFile find = gameFiles.FirstOrDefault(x => x.FileName.ToLower() == iwad.FileName.ToLower().Replace(".wad", ".zip"));
                    if (find != null)
                    {
                        iwad.GameFileID = find.GameFileID;
                        m_adapter.UpdateIWad(iwad);
                    }
                }
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(SourcePorts);").Tables[0];

            if (!dt.Select("name = 'SettingsFiles'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table SourcePorts add column 'SettingsFiles' TEXT;");
            }

            //There was a bug in previous versions that would set MapCount to 1 when there were no maps found
            DataAccess.ExecuteNonQuery("update GameFiles set MapCount = null where Map is null or length(Map) = 0");
        }

        private void Pre_2_4_1()
        {
            string[] saveExts = new string[] { "*.zds", "*.dsg", "*.esg" };
            foreach (string ext in saveExts)
            {
                string[] files = Directory.GetFiles(m_appConfig.DemoDirectory.GetFullPath(), ext);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    FileInfo fiTo = new FileInfo(Path.Combine(m_appConfig.SaveGameDirectory.GetFullPath(), fi.Name));
                    if (fiTo.Exists)
                        fiTo.Delete();
                    fi.MoveTo(fi.FullName);
                }
            }
        }

        private void Pre_2_6_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(SourcePorts);").Tables[0];

            if (!dt.Select("name = 'LaunchType'").Any())
                DataAccess.ExecuteNonQuery("alter table SourcePorts add column 'LaunchType' TEXT;");
            if (!dt.Select("name = 'FileOption'").Any())
                DataAccess.ExecuteNonQuery("alter table SourcePorts add column 'FileOption' TEXT;");

            DataAccess.ExecuteNonQuery(string.Format("update SourcePorts set LaunchType = {0}", (int)SourcePortLaunchType.SourcePort));
            DataAccess.ExecuteNonQuery("update SourcePorts set FileOption = '-file'");

            dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];

            if (!dt.Select("name = 'SettingsStat'").Any())
            {
                DataAccess.ExecuteNonQuery("alter table GameFiles add column 'SettingsStat' INTEGER;");
                DataAccess.ExecuteNonQuery("update GameFiles set SettingsStat = 1");
            }
        }

        private void Pre_2_6_3()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(SourcePorts);").Tables[0];

            if (!dt.Select("name = 'ExtraParameters'").Any())
                DataAccess.ExecuteNonQuery("alter table SourcePorts add column 'ExtraParameters' TEXT;");
        }

        private void Pre_2_6_3_1()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];

            if (!dt.Select("name = 'SettingsFilesSourcePort'").Any())
                DataAccess.ExecuteNonQuery("alter table GameFiles add column 'SettingsFilesSourcePort' TEXT;");
            if (!dt.Select("name = 'SettingsFilesIWAD'").Any())
                DataAccess.ExecuteNonQuery("alter table GameFiles add column 'SettingsFilesIWAD' TEXT;");

            var gameFiles = m_adapter.GetGameFiles();
            var ports = m_adapter.GetSourcePorts().ToDictionary(x => x.SourcePortID, x => x);
            var iwads = m_adapter.GetIWads();
            var gameFileIwads = m_adapter.GetGameFileIWads().ToDictionary(x => iwads.First(y => y.GameFileID == x.GameFileID.Value).IWadID, x => x);

            foreach (var gameFile in gameFiles)
            {
                if (!string.IsNullOrEmpty(gameFile.SettingsFiles))
                {
                    var files = Util.GetAdditionalFiles(m_adapter, (GameFile)gameFile).Select(x => x.FileName);
                    FileLoadHandlerLegacy filehandler = new FileLoadHandlerLegacy(m_adapter, gameFile);
                    filehandler.CalculateAdditionalFiles(GetDictionaryData<IGameFile>(gameFile.IWadID, gameFileIwads),
                        GetDictionaryData<ISourcePortData>(gameFile.SourcePortID, ports));

                    var sourcePortFiles = filehandler.GetSourcePortFiles().Select(x => x.FileName).Where(x => files.Contains(x));
                    var iwadFiles = filehandler.GetIWadFiles().Select(x => x.FileName).Where(x => files.Contains(x)).Except(sourcePortFiles);

                    gameFile.SettingsFilesSourcePort = string.Join(";", sourcePortFiles.ToArray());
                    gameFile.SettingsFilesIWAD = string.Join(";", iwadFiles.ToArray());

                    m_adapter.UpdateGameFile(gameFile);
                }
            }
        }

        private void Pre_2_6_3_2()
        {
            DataAccess.ExecuteNonQuery("update SourcePorts set FileOption = '-file' where LaunchType = 0");
        }

        private void Pre_2_6_4_1()
        {
            ConfigurationData config = new ConfigurationData()
            {
                Name = "ScreenshotPreviewSize",
                Value = "0",
                UserCanModify = true
            };

            m_adapter.InsertConfiguration(config);
        }

        private void Pre_2_6_4_1_Update()
        {
            var sourcePorts = m_adapter.GetSourcePorts();
            var stats = m_adapter.GetStats();

            foreach (var stat in stats)
            {
                if (!sourcePorts.Any(x => x.SourcePortID == stat.SourcePortID))
                {
                    stat.SourcePortID = -1;
                    m_adapter.UpdateStats(stat);
                }
            }
        }

        private void Pre_2_7_0_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("select name from sqlite_master where type='table' and name='GameProfiles';").Tables[0];

            if (dt.Rows.Count == 0)
            {
                string query = @"CREATE TABLE 'GameProfiles' (
	                    'GameProfileID'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        'GameFileID'	INTEGER NOT NULL,
                        'SourcePortID'	INTEGER NOT NULL,
                        'IWadID'	INTEGER NOT NULL,
                        'Name'	TEXT NOT NULL,
	                    'SettingsMap'	TEXT,
	                    'SettingsSkill'	TEXT,
	                    'SettingsExtraParams'	TEXT,
	                    'SettingsFiles'	TEXT,
	                    'SettingsFilesSourcePort'	TEXT,
	                    'SettingsFilesIWAD'	TEXT,
	                    'SettingsSpecificFiles'	TEXT,
                        'SettingsStat'	INTEGER,
                        'SettingsSaved' INTEGER);";

                DataAccess.ExecuteNonQuery(query);
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];

            if (!dt.Select("name = 'SettingsGameProfileID'").Any())
                DataAccess.ExecuteNonQuery(@"alter table GameFiles add column 'SettingsGameProfileID' INTEGER;");

            if (!dt.Select("name = 'SettingsSaved'").Any())
            {
                DataAccess.ExecuteNonQuery(@"alter table GameFiles add column 'SettingsSaved' INTEGER;");
                DataAccess.ExecuteNonQuery("update GameFiles set SettingsSaved = 1 where SourcePortID is not null");
                DataAccess.ExecuteNonQuery("update GameFiles set SettingsSaved = 0 where SourcePortID is null");
            }
        }

        private void Pre_2_8_0_0()
        {
            ConfigurationData config = new ConfigurationData()
            {
                Name = "FileManagement",
                Value = FileManagement.Managed.ToString(),
                UserCanModify = false,
            };

            m_adapter.InsertConfiguration(config);
        }

        private void Pre_2_8_0_0_1()
        {
            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "GameFileViewType",
                Value = GameFileViewType.TileView.ToString(),
                UserCanModify = false,
            });

            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "ItemsPerPage",
                Value = "30",
                UserCanModify = false,
            });
        }

        private void Pre_Version_3_1_0()
        {
            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "DeleteScreenshotsAfterImport",
                Value = "false",
                UserCanModify = true,
                AvailableValues = "Yes;true;No;false"
            });
        }

        private void Pre_Version_3_2_0()
        {
            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "LastSelectedTabIndex",
                Value = "0",
                UserCanModify = false,
            });

            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(Tags);").Tables[0];

            if (!dt.Select("name = 'ExcludeFromOtherTabs'").Any())
            {
                DataAccess.ExecuteNonQuery(@"alter table Tags add column 'ExcludeFromOtherTabs' INTEGER;");
                DataAccess.ExecuteNonQuery("update Tags set ExcludeFromOtherTabs = 0");
            }
        }

        private void Pre_Version_3_2_0_Update1()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(Tags);").Tables[0];

            if (!dt.Select("name = 'Favorite'").Any())
            {
                DataAccess.ExecuteNonQuery(@"alter table Tags add column 'Favorite' INTEGER;");
                DataAccess.ExecuteNonQuery("update Tags set Favorite = 0");
            }

            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "SplitTagSelect",
                Value = "300",
                UserCanModify = false,
            });

            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "TagSelectPinned",
                Value = "False",
                UserCanModify = false,
            });

            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "ShowTabHeaders",
                Value = "true",
                UserCanModify = true,
                AvailableValues = "Yes;true;No;false"
            });
        }

        private void Pre_Version_3_2_0_Update2()
        {
            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "CopySaveFiles",
                Value = "true",
                UserCanModify = true,
                AvailableValues = "Yes;true;No;false"
            });

            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(SourcePorts);").Tables[0];

            if (!dt.Select("name = 'AltSaveDirectory'").Any())
                DataAccess.ExecuteNonQuery(@"alter table SourcePorts add column 'AltSaveDirectory' TEXT;");
        }

        private void Pre_Version_3_2_0_Update3()
        {
            IEnumerable<ISourcePortData> sourcePorts = m_adapter.GetSourcePorts();

            foreach (ISourcePortData sourcePort in sourcePorts)
            {
                if (!sourcePort.SupportedExtensions.Contains(".pk3"))
                    continue;
                
                sourcePort.SupportedExtensions = sourcePort.SupportedExtensions.Replace(".pk3", ".pk3,.ipk3");

                List<DbParameter> parameters = new List<DbParameter>
                {
                    DataAccess.DbAdapter.CreateParameter("ext", sourcePort.SupportedExtensions),
                    DataAccess.DbAdapter.CreateParameter("SourcePortID", sourcePort.SourcePortID)
                };
                DataAccess.ExecuteNonQuery("update SourcePorts set SupportedExtensions = @ext where SourcePortID = @SourcePortID", parameters);
            }
        }

        private void Pre_Version_3_3_0()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(GameFiles);").Tables[0];

            if (!dt.Select("name = 'SettingsLoadLatestSave'").Any())
            {
                DataAccess.ExecuteNonQuery(@"alter table GameFiles add column 'SettingsLoadLatestSave' INTEGER;");
                DataAccess.ExecuteNonQuery("update GameFiles set SettingsLoadLatestSave = 0");
            }

            dt = DataAccess.ExecuteSelect("pragma table_info(GameProfiles);").Tables[0];

            if (!dt.Select("name = 'SettingsLoadLatestSave'").Any())
            {
                DataAccess.ExecuteNonQuery(@"alter table GameProfiles add column 'SettingsLoadLatestSave' INTEGER;");
                DataAccess.ExecuteNonQuery("update GameProfiles set SettingsLoadLatestSave = 0");
            }
        }

        private void Pre_Version_3_3_1()
        {
            var sourcePorts = m_adapter.GetSourcePorts();
            foreach (var sourcePort in sourcePorts)
            {
                if (sourcePort.SupportedExtensions.Contains(".pke"))
                    continue;

                sourcePort.SupportedExtensions += ",.pke";
                m_adapter.UpdateSourcePort(sourcePort);
            }
        }

        private void Pre_Version_3_3_2()
        {
            DataTable dt = DataAccess.ExecuteSelect("select name from sqlite_master where type='table' and name='CleanupFiles';").Tables[0];

            if (dt.Rows.Count == 0)
            {
                string query = @"CREATE TABLE 'CleanupFiles' (
                    'CleanupFileID'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                'FileName' TEXT NOT NULL);";

                DataAccess.ExecuteNonQuery(query);
            }

            string urls = "Germany;https://www.quaddicted.com/files/idgames/;Idaho;ftp://mirrors.syringanetworks.net/idgames/;New York;https://youfailit.net/pub/idgames/;Sweden;https://ftpmirror1.infania.net/pub/idgames/;Virginia;http://www.gamers.org/pub/idgames/;";
            UpdateMirrorUrls(urls, "New York");
        }

        private void Pre_Version_3_4_0()
        {
            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "AllowMultiplePlaySessions",
                Value = "false",
                UserCanModify = true,
                AvailableValues = "Yes;true;No;false"
            });

            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "AutomaticallyPullTitlpic",
                Value = "true",
                UserCanModify = true,
                AvailableValues = "Yes;true;No;false"
            });
        }

        private void Pre_Version_3_4_0_Update1()
        {
            var columnConfig = DataCache.Instance.GetColumnConfig().ToList();
            var columnGroups = columnConfig.GroupBy(x => x.Parent);
            List<ColumnConfig> newColumns = new List<ColumnConfig>(columnConfig.Count * 2);

            // Insert LastDirectory after FileNameNoPath
            foreach (var set in columnGroups)
            {
                if (set.Any(x => x.Column.Equals("LastDirectory", StringComparison.OrdinalIgnoreCase)))
                    continue;  

                List<ColumnConfig> sortColumns = set.ToList();
                for (int i = 0; i < sortColumns.Count; i++)
                {
                    var col = sortColumns[i];
                    newColumns.Add(col);
                    if (!col.Column.Equals("FileNameNoPath", StringComparison.OrdinalIgnoreCase))
                        continue;

                    newColumns.Add(new ColumnConfig()
                    {
                        Parent = set.Key,
                        Column = "LastDirectory",
                        Width = 100,
                        Sort = SortOrder.None
                    });
                }

                newColumns.AddRange(sortColumns);
            }

            string data = DataCache.SerializeColumnConfig(newColumns);
            DataCache.Instance.UpdateConfig(DataCache.Instance.DataSourceAdapter.GetConfiguration(), AppConfiguration.ColumnConfigName, data);
        }

        private void Pre_Version_3_5_2()
        {
            m_adapter.InsertConfiguration(new ConfigurationData()
            {
                Name = "VisibleViews",
                Value = string.Join(";", TabKeys.KeyNames.Except(new string[] { TabKeys.LocalKey })),
                UserCanModify = false,
            });
        }

        private void Pre_Version_3_5_2_Update1()
        {
            DataTable dt = DataAccess.ExecuteSelect("pragma table_info(Stats);").Tables[0];
            if (!dt.Select("name = 'Skill'").Any())
                DataAccess.ExecuteNonQuery(@"alter table Stats add column 'Skill' INTEGER;");
        }

        private void UpdateMirrorUrls(string urls, string defaultUrl)
        { 
            var configuration = m_adapter.GetConfiguration().FirstOrDefault(x => x.Name.Equals("MirrorUrl"));
            if (configuration == null)
                return;

            var newUrls = GetConfigItems(urls);
            var oldUrls = GetConfigItems(configuration.AvailableValues);

            if (!GetConfigKey(configuration.Value, oldUrls, out string urlKey) || !newUrls.Any(x => x.Item1.Equals(urlKey)))
                urlKey = defaultUrl;

            if (GetConfigValue(urlKey, newUrls, out string value))
                configuration.Value = value;
            configuration.AvailableValues = urls;
            m_adapter.UpdateConfiguration(configuration);
        }

        private static List<Tuple<string,string>> GetConfigItems(string text)
        {    
            List<Tuple<string, string>> configItems = new List<Tuple<string, string>>();
            var items = text.Split(new char[] { ';' });

            for (int i = 0; i < items.Length - 1; i+=2)
                configItems.Add(new Tuple<string, string>(items[i], items[i + 1]));

            return configItems;
        }

        private static bool GetConfigKey(string data, List<Tuple<string, string>> items, out string key)
        {
            key = string.Empty;
            var item = items.FirstOrDefault(x => x.Item2.Equals(data));
            if (item == null)
                return false;
            key = item.Item1;
            return true;
        }

        private static bool GetConfigValue(string data, List<Tuple<string, string>> items, out string value)
        {
            value = string.Empty;
            var item = items.FirstOrDefault(x => x.Item1.Equals(data));
            if (item == null)
                return false;
            value = item.Item2;
            return true;
        }

        private static T GetDictionaryData<T>(int? id, Dictionary<int, T> values)
        {
            if (id.HasValue && values.ContainsKey(id.Value))
                return values[id.Value];
            return default(T);
        }

        public DataAccess DataAccess { get; set; }
    }
}