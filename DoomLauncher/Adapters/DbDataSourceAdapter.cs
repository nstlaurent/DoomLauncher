using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DoomLauncher
{
    public class DbDataSourceAdapter : IDataSourceAdapter
    {
        public static readonly string DatabaseFileName = "DoomLauncher.sqlite";
        public static readonly string InitDatabaseFileName = "DoomLauncher_.sqlite";

        private static string[] s_opLookup = new string[] { "= ", "<>", "<", ">", "like" };

        public DbDataSourceAdapter(IDatabaseAdapter dbAdapter, string connectionString)
            : this(dbAdapter, connectionString, false)
        {

        }

        public DbDataSourceAdapter(IDatabaseAdapter dbAdapter, string connectionString, bool outOfDateDatabase)
        {
            DbAdapter = dbAdapter;
            ConnectionString = connectionString;
            m_outOfDateDatabase = outOfDateDatabase;

            DataAccess = new DataAccess(dbAdapter, connectionString);
        }

        public static IDataSourceAdapter CreateAdapter() => CreateAdapter(false);

        public static IDataSourceAdapter CreateAdapter(bool outOfDateDatabase)
        {
            string databaseFile = Path.Combine(LauncherPath.GetDataDirectory(), DatabaseFileName);      
            return new DbDataSourceAdapter(new SqliteDatabaseAdapter(), CreateConnectionString(databaseFile), outOfDateDatabase);
        }

        public static string CreateConnectionString(string dataSource)
        {
            return string.Format(@"Data Source={0}", dataSource);
        }

        public int GetGameFilesCount()
        {
            DataTable dt = DataAccess.ExecuteSelect("select count(*) from GameFiles").Tables[0];
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public IEnumerable<IGameFile> GetGameFiles()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from GameFiles").Tables[0];
            return Util.TableToStructure(dt, typeof(GameFile)).Cast<IGameFile>();
        }

        public IEnumerable<IGameFile> GetGameFiles(ITagData tag)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select GameFiles.* from GameFiles join TagMapping on TagMapping.FileID = GameFiles.GameFileID where TagID = {0}", 
                tag.TagID)).Tables[0];
            return Util.TableToStructure(dt, typeof(GameFile)).Cast<IGameFile>();
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options)
        {
            return GetGameFiles(options, null);
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options, ITagData tag)
        {
            DataTable dt;
            string selectColumns = "GameFiles.*";
            string join = string.Empty;
            string where = string.Empty;

            if (tag != null)
            {
                join = "join TagMapping on TagMapping.FileID = GameFiles.GameFileID";
                where = string.Format("TagMapping.TagID = {0}", tag.TagID);
            }

            if (options.SelectFields != null)
                selectColumns = GetSelectFieldString(options.SelectFields);

            if (options.SearchField != null)
            {
                string op = s_opLookup[(int)options.SearchField.SearchOp];

                if (op == "like")
                    options.SearchField.SearchText = string.Format("{0}{1}{0}", "%", options.SearchField.SearchText);

                string searchCol = options.SearchField.SearchFieldType.ToString("g");
                string searchParam = "@search";

                if (DataAccess.DbAdapter is SqliteDatabaseAdapter && GameFileSearchField.IsDateTimeField(options.SearchField.SearchFieldType)) //sqlite datetime comparison hack
                {
                    searchParam = string.Format("Datetime('{0}')", DateTime.Parse(options.SearchField.SearchText).ToString("yyyy-MM-dd"));
                }

                if (where != string.Empty) 
                    where = string.Format("and {0}", where);

                string query = string.Format("select {2} from GameFiles {5} where {0} {1} {3} {4} {6}",
                        searchCol, op, selectColumns, searchParam, GetLimitOrderString(options), join, where);
                dt = DataAccess.ExecuteSelect(query, new DbParameter[] { DataAccess.DbAdapter.CreateParameter("search", options.SearchField.SearchText) }).Tables[0];
            }
            else
            {
                if (where != string.Empty)
                    where = string.Format("where {0}", where);

                string query = string.Format("select {0} from GameFiles {2} {3} {1}", selectColumns, GetLimitOrderString(options), join, where);
                dt = DataAccess.ExecuteSelect(query).Tables[0];
            }

            return Util.TableToStructure(dt, typeof(GameFile)).Cast<IGameFile>();
        }

        public IEnumerable<IGameFile> GetUntaggedGameFiles()
        {
            DataTable dt = DataAccess.ExecuteSelect("select GameFiles.* from GameFiles left join TagMapping on GameFiles.GameFileID = TagMapping.FileID where FileID is null").Tables[0];
            return Util.TableToStructure(dt, typeof(GameFile)).Cast<IGameFile>();
        }

        private static string GetLimitOrderString(IGameFileGetOptions options)
        {
            string ret = string.Empty;

            if (options.OrderBy.HasValue && options.OrderField.HasValue)
                ret += string.Format("order by {0} {1}", options.OrderField.Value.ToString("g"), 
                    options.OrderBy.Value.ToString("g"));

            if (options.Limit.HasValue)
                ret += string.Format(" limit {0}", options.Limit.Value);

            return ret;
        }

        private string GetSelectFieldString(GameFileFieldType[] selectFields)
        {
            StringBuilder sb = new StringBuilder();

            foreach (GameFileFieldType field in selectFields)
            {
                sb.Append(field.ToString("g"));
                sb.Append(',');
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public IEnumerable<string> GetGameFileNames()
        {
            DataTable dt = DataAccess.ExecuteSelect("select FileName from GameFiles").Tables[0];
            List<string> ret = new List<string>(dt.Rows.Count);

            foreach (DataRow dr in dt.Rows)
                ret.Add((string)dr[0]);

            return ret;
        }

        public IGameFile GetGameFile(string fileName)
        {
            List<DbParameter> parameters = new List<DbParameter>
            { 
                DataAccess.DbAdapter.CreateParameter("FileName", fileName),
                DataAccess.DbAdapter.CreateParameter("FileNamePath", '%' + Path.DirectorySeparatorChar + fileName),
            };

            DataTable dt = DataAccess.ExecuteSelect("select * from GameFiles where Filename = @FileName COLLATE NOCASE or Filename like @FileNamePath COLLATE NOCASE", parameters).Tables[0];

            if (dt.Rows.Count > 0)
                return Util.TableToStructure(dt, typeof(GameFile)).Cast<GameFile>().ToList()[0];
            else
                return null;
        }

        public IEnumerable<IGameFile> GetGameFileIWads()
        {
            DataTable dt = DataAccess.ExecuteSelect("select GameFiles.* from GameFiles join IWads on IWads.GameFileID = GameFiles.GameFileID").Tables[0];
            return Util.TableToStructure(dt, typeof(GameFile)).Cast<GameFile>();
        }

        public void InsertGameFile(IGameFile gameFile)
        {
            string insert = InsertStatement("GameFiles", gameFile, new string[] { "GameFileID", "FileSizeBytes", "GameProfileID", "Name", "FullFileName" }, out List<DbParameter> parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateGameFile(IGameFile gameFile)
        {
            UpdateGameFile(gameFile, null);
        }

        public void UpdateGameFile(IGameFile gameFile, GameFileFieldType[] updateFields)
        {
            StringBuilder query = new StringBuilder("update GameFiles set ");

            if (updateFields != null && updateFields.Length > 0)
            {
                foreach(GameFileFieldType field in updateFields)
                {
                    if (field != GameFileFieldType.MD5 && field != GameFileFieldType.GameFileID)
                    {
                        query.Append(field.ToString());
                        query.Append(" = @");
                        query.Append(field.ToString());
                        query.Append(",");
                    }
                }

                query.Remove(query.Length - 1, 1);
                query.Append(" where GameFileID = @gameFileID");
            }
            else
            {
                query = new StringBuilder(@"update GameFiles set Title = @Title, Author = @Author, ReleaseDate = @ReleaseDate,
                    Description = @Description, Map = @Map, SourcePortID = @SourcePortID,
                    Thumbnail = @Thumbnail, Comments = @Comments, Rating = @Rating,
                    IWadID = @IWadID, LastPlayed = @LastPlayed, Downloaded = @Downloaded, 
                    SettingsMap = @SettingsMap, SettingsSkill = @SettingsSkill, SettingsExtraParams = @SettingsExtraParams, SettingsFiles = @SettingsFiles,
                    SettingsFilesSourcePort = @SettingsFilesSourcePort, SettingsFilesIWAD = @SettingsFilesIWAD,
                    SettingsSpecificFiles = @SettingsSpecificFiles, SettingsStat = @SettingsStat, SettingsLoadLatestSave = @SettingsLoadLatestSave, 
                    FileName = @FileName, MapCount = @MapCount, 
                    MinutesPlayed = @MinutesPlayed, SettingsGameProfileID = @SettingsGameProfileID, SettingsSaved = @SettingsSaved
                    where GameFileID = @gameFileID");
            }

            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("Title", gameFile.Title ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Author", gameFile.Author ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("ReleaseDate", gameFile.ReleaseDate ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Description", gameFile.Description ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Map", gameFile.Map ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SourcePortID", gameFile.SourcePortID ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Thumbnail", gameFile.Thumbnail ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Comments", gameFile.Comments ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Rating", gameFile.Rating.HasValue ? gameFile.Rating : (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("IWadID", gameFile.IWadID.HasValue ? gameFile.IWadID : (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("GameFileID", gameFile.GameFileID.Value),
                DataAccess.DbAdapter.CreateParameter("LastPlayed", gameFile.LastPlayed.HasValue ? gameFile.LastPlayed : (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("Downloaded", gameFile.Downloaded.HasValue ? gameFile.Downloaded : (object)DBNull.Value),

                DataAccess.DbAdapter.CreateParameter("SettingsMap", gameFile.SettingsMap ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsSkill", gameFile.SettingsSkill ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsExtraParams", gameFile.SettingsExtraParams ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsFiles", gameFile.SettingsFiles ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsFilesSourcePort", gameFile.SettingsFilesSourcePort ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsFilesIWAD", gameFile.SettingsFilesIWAD ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsSpecificFiles", gameFile.SettingsSpecificFiles ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsStat", gameFile.SettingsStat),
                DataAccess.DbAdapter.CreateParameter("SettingsLoadLatestSave", gameFile.SettingsLoadLatestSave),
                DataAccess.DbAdapter.CreateParameter("SettingsSaved", gameFile.SettingsSaved),
                DataAccess.DbAdapter.CreateParameter("SettingsGameProfileID", gameFile.SettingsGameProfileID ?? (object)DBNull.Value),

                DataAccess.DbAdapter.CreateParameter("MapCount", !gameFile.MapCount.HasValue ? (object)DBNull.Value : gameFile.MapCount),

                DataAccess.DbAdapter.CreateParameter("MinutesPlayed", gameFile.MinutesPlayed),

                DataAccess.DbAdapter.CreateParameter("FileName", gameFile.FileName)
            };

            if (m_outOfDateDatabase)
            {
                DataTable dt = GetTableColumns("GameFiles");
                query = RemoveUnknownColumnsFromQuery(dt, query);
                parameters = RemoveUnknownColumnsFromParameters(dt, parameters);
            }

            DataAccess.ExecuteNonQuery(query.ToString(), parameters);
        }

        public void UpdateGameFiles(GameFileFieldType ftWhere, GameFileFieldType ftSet, object fWhere, object fSet)
        {
            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("set1", fSet ?? DBNull.Value ),
                DataAccess.DbAdapter.CreateParameter("where1", fWhere ?? DBNull.Value )
            };

            DataAccess.ExecuteNonQuery(string.Format(@"update GameFiles set {0} = @set1 where {1} = @where1", ftWhere.ToString("g"), ftSet.ToString("g")), parameters);
        }

        public void DeleteGameFile(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
            {
                DataAccess.ExecuteNonQuery(string.Format("delete from GameFiles where GameFileID = {0}", gameFile.GameFileID));
            }
        }

        public IEnumerable<ISourcePortData> GetSourcePorts(bool loadArchived = false) =>
            GetSourcePorts(SourcePortLaunchType.SourcePort, loadArchived);

        public IEnumerable<ISourcePortData> GetUtilities(bool loadArchived = false) =>
            GetSourcePorts(SourcePortLaunchType.Utility, loadArchived);

        private IEnumerable<ISourcePortData> GetSourcePorts(SourcePortLaunchType type, bool loadArchived)
        {
            int sqlArchive = loadArchived ? 1 : 0;
            DataTable dt;

            try
            {
                dt = DataAccess.ExecuteSelect($"select * from SourcePorts where LaunchType = {(int)type} and Archived = {sqlArchive} order by Name collate nocase").Tables[0];
            }
            catch
            {
                // This is for updates before Archived column existed...
                dt = DataAccess.ExecuteSelect($"select * from SourcePorts where LaunchType = {(int)type}").Tables[0];
            }

            List<ISourcePortData> sourcePorts = new List<ISourcePortData>();

            foreach (DataRow dr in dt.Rows)
                sourcePorts.Add(CreateSourcePortDataSource(dt, dr));

            return sourcePorts;
        }

        private static ISourcePortData CreateSourcePortDataSource(DataTable dt, DataRow dr)
        {
            SourcePortData sourcePort = new SourcePortData
            {
                Directory = new LauncherPath((string)dr["Directory"]),
                Executable = (string)dr["Executable"],
                Name = (string)dr["Name"],
                SourcePortID = Convert.ToInt32(dr["SourcePortID"]),
                SupportedExtensions = (string)CheckDBNull(dr["SupportedExtensions"], string.Empty),
                LaunchType = (SourcePortLaunchType)Convert.ToInt32(dr["LaunchType"]),
                FileOption = (string)CheckDBNull(dr["FileOption"], string.Empty),
                ExtraParameters = (string)CheckDBNull(dr["ExtraParameters"], string.Empty),
                AltSaveDirectory = new LauncherPath((string)CheckDBNull(dr["AltSaveDirectory"], string.Empty)),
            };

            if (dt.Columns.Contains("SettingsFiles"))
                sourcePort.SettingsFiles = (string)CheckDBNull(dr["SettingsFiles"], string.Empty);
            if (dt.Columns.Contains("Archived"))
                sourcePort.Archived = Convert.ToInt32(dr["Archived"]) != 0;

            return sourcePort;
        }

        private static object CheckDBNull(object obj, object defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;
            else
                return obj;
        }

        public ISourcePortData GetSourcePort(int sourcePortID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from SourcePorts where SourcePortID = {0}", sourcePortID)).Tables[0];

            if (dt.Rows.Count > 0)
                return CreateSourcePortDataSource(dt, dt.Rows[0]);

            return null;
        }

        public void InsertSourcePort(ISourcePortData sourcePort)
        {
            string insert = @"insert into SourcePorts (Name,Executable,SupportedExtensions,Directory,SettingsFiles,LaunchType,FileOption,ExtraParameters,AltSaveDirectory,Archived) 
                values(@Name,@Executable,@SupportedExtensions,@Directory,@SettingsFiles,@LaunchType,@FileOption,@ExtraParameters,@AltSaveDirectory,@Archived)";

            DataAccess.ExecuteNonQuery(insert, GetSourcePortParams(sourcePort));
        }

        public void UpdateSourcePort(ISourcePortData sourcePort)
        {
            string query = @"update SourcePorts set 
            Name = @Name, Executable = @Executable, SupportedExtensions = @SupportedExtensions,
            Directory = @Directory, SettingsFiles = @SettingsFiles, LaunchType = @LaunchType, FileOption = @FileOption, ExtraParameters = @ExtraParameters,
            AltSaveDirectory = @AltSaveDirectory, Archived = @Archived
            where SourcePortID = @sourcePortID";

            DataAccess.ExecuteNonQuery(query, GetSourcePortParams(sourcePort));
        }

        private List<DbParameter> GetSourcePortParams(ISourcePortData sourcePort)
        {
            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("Name", sourcePort.Name ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("Executable", sourcePort.Executable ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("SupportedExtensions", sourcePort.SupportedExtensions ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("Directory", sourcePort.Directory == null ? string.Empty : sourcePort.Directory.GetPossiblyRelativePath()),
                DataAccess.DbAdapter.CreateParameter("SettingsFiles", sourcePort.SettingsFiles ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("SourcePortID", sourcePort.SourcePortID),
                DataAccess.DbAdapter.CreateParameter("LaunchType", sourcePort.LaunchType),
                DataAccess.DbAdapter.CreateParameter("FileOption", sourcePort.FileOption ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("ExtraParameters", sourcePort.ExtraParameters ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("AltSaveDirectory", sourcePort.AltSaveDirectory == null ? string.Empty : sourcePort.AltSaveDirectory.GetPossiblyRelativePath()),
                DataAccess.DbAdapter.CreateParameter("Archived", sourcePort.Archived)
            };

            return parameters;
        }

        public void DeleteSourcePort(ISourcePortData sourcePort)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from SourcePorts where SourcePortID = {0}", sourcePort.SourcePortID));
        }

        public IEnumerable<IIWadData> GetIWads()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from IWads order by Name collate nocase").Tables[0];
            return Util.TableToStructure(dt, typeof(IWadData)).Cast<IWadData>().ToList();
        }

        public IIWadData GetIWad(int gameFileID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from IWads where GameFileID = {0} order by Name collate nocase", gameFileID)).Tables[0];
            return Util.TableToStructure(dt, typeof(IWadData)).Cast<IWadData>().FirstOrDefault();
        }

        public void InsertIWad(IIWadData iwad)
        {
            string insert = InsertStatement("IWads", iwad, new string[] { "IWadID" }, out List<DbParameter> parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateIWad(IIWadData iwad)
        {
            string update = "update IWads set FileName = @FileName, Name = @Name, GameFileID = @GameFileID where IWadID = @IWadID";
            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("IWadID", iwad.IWadID),
                DataAccess.DbAdapter.CreateParameter("FileName", iwad.FileName),
                DataAccess.DbAdapter.CreateParameter("Name", iwad.Name),
                DataAccess.DbAdapter.CreateParameter("GameFileID", iwad.GameFileID.HasValue ? iwad.GameFileID : (object)DBNull.Value)
            };

            DataAccess.ExecuteNonQuery(update, parameters);
        }

        public void DeleteIWad(IIWadData iwad)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from IWads where IWadID = {0}", iwad.IWadID));
        }

        public IEnumerable<IFileData> GetFiles()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from Files").Tables[0];
            return Util.TableToStructure(dt, typeof(FileData)).Cast<FileData>().ToList();
        }

        public IEnumerable<IFileData> GetFiles(IGameFile gameFile)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from Files where GameFileID = {0} order by FileOrder, FileID", gameFile.GameFileID.Value)).Tables[0];
            return Util.TableToStructure(dt, typeof(FileData)).Cast<FileData>().ToList();
        }

        public IEnumerable<IFileData> GetFiles(IGameFile gameFile, FileType fileTypeID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from Files where GameFileID = {0} and FileTypeID = {1} order by FileOrder, FileID", gameFile.GameFileID.Value, (int)fileTypeID)).Tables[0];
            return Util.TableToStructure(dt, typeof(FileData)).Cast<FileData>().ToList();
        }

        public IEnumerable<IFileData> GetFiles(FileType fileTypeID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from Files where FileTypeID = {0} order by GameFileID, FileOrder desc", (int)fileTypeID)).Tables[0];
            return Util.TableToStructure(dt, typeof(FileData)).Cast<FileData>().ToList();
        }

        public void UpdateFile(IFileData file)
        {
            string query = @"update Files set 
            SourcePortID = @SourcePortID, Description = @Description, FileOrder = @FileOrder, DateCreated = @DateCreated
            where FileID = @FileID";

            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("SourcePortID", file.SourcePortID),
                DataAccess.DbAdapter.CreateParameter("Description", file.Description),
                DataAccess.DbAdapter.CreateParameter("FileID", file.FileID),
                DataAccess.DbAdapter.CreateParameter("FileOrder", file.FileOrder),
                DataAccess.DbAdapter.CreateParameter("DateCreated", file.DateCreated)
            };

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public void UpdateFiles(int sourcePortID_Where, int? sourcePortID_Set)
        {
            DataAccess.ExecuteNonQuery(string.Format("update Files set SourcePortID = @id where SourcePortID = {0}", sourcePortID_Where), 
                new DbParameter[] { DataAccess.DbAdapter.CreateParameter("id", sourcePortID_Set ?? (object)DBNull.Value) });
        }

        public void InsertFile(IFileData file)
        {
            string insert = InsertStatement("Files", file, new string[] { "FileID" }, out List<DbParameter> parameters);

            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void DeleteFile(IFileData file)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Files where FileID = {0}", file.FileID));
        }

        public void DeleteFile(IGameFile file)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Files where GameFileID = {0}", file.GameFileID.Value));
        }

        public void DeleteFiles(ISourcePortData sourcePort, FileType fileTypeID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Files where SourcePortID = {0} and FileTypeID = {1}", sourcePort.SourcePortID, (int)fileTypeID));
        }

        public IEnumerable<IConfigurationData> GetConfiguration()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from Configuration").Tables[0];
            return Util.TableToStructure(dt, typeof(ConfigurationData)).Cast<ConfigurationData>().ToList();
        }

        public void InsertConfiguration(IConfigurationData config)
        {
            string insert = InsertStatement("Configuration", config, new string[] { "ConfigID" }, out List<DbParameter> parameters);

            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateConfiguration(IConfigurationData config)
        {
            string query = @"update Configuration set 
            Name = @Name, Value = @Value, AvailableValues = @AvailableValues
            where ConfigID = @ConfigID";

            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("Name", config.Name ?? string.Empty ),
                DataAccess.DbAdapter.CreateParameter("Value", config.Value ?? string.Empty ),
                DataAccess.DbAdapter.CreateParameter("AvailableValues", config.AvailableValues == null ? string.Empty : config.AvailableValues),
                DataAccess.DbAdapter.CreateParameter("ConfigID", config.ConfigID)
            };

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public IEnumerable<ITagData> GetTags()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from Tags").Tables[0];
            return Util.TableToStructure(dt, typeof(TagData)).Cast<TagData>().ToList();
        }

        public void InsertTag(ITagData tag)
        {
            string insert = InsertStatement("Tags", tag, new string[] { "TagID" }, out List<DbParameter> parameters);

            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateTag(ITagData tag)
        {
            string query = @"update Tags set 
            Name = @Name, HasTab = @HasTab, HasColor = @HasColor, Color = @Color, ExcludeFromOtherTabs = @ExcludeFromOtherTabs, Favorite = @Favorite
            where TagID = @TagID";

            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("Name", tag.Name ?? string.Empty),
                DataAccess.DbAdapter.CreateParameter("HasTab", tag.HasTab),
                DataAccess.DbAdapter.CreateParameter("HasColor", tag.HasColor),
                DataAccess.DbAdapter.CreateParameter("Color", tag.Color.HasValue ? tag.Color : (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("TagID", tag.TagID),
                DataAccess.DbAdapter.CreateParameter("ExcludeFromOtherTabs", tag.ExcludeFromOtherTabs),
                DataAccess.DbAdapter.CreateParameter("Favorite", tag.Favorite)
            };

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public void DeleteTag(ITagData tag)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Tags where TagID = {0}", tag.TagID));
        }

        public IEnumerable<ITagMapping> GetTagMappings()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from TagMapping").Tables[0];
            return Util.TableToStructure(dt, typeof(TagMapping)).Cast<TagMapping>().ToList();
        }

        public IEnumerable<ITagMapping> GetTagMappings(int gameFileID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from TagMapping where FileID = {0}", gameFileID)).Tables[0];
            return Util.TableToStructure(dt, typeof(TagMapping)).Cast<TagMapping>().ToList();
        }

        public void InsertTagMapping(ITagMapping tag)
        {
            string insert = InsertStatement("TagMapping", tag, out List<DbParameter> parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void DeleteTagMapping(ITagMapping tag)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from TagMapping where TagID = {0} and FileID = {1}", tag.TagID, tag.FileID));
        }

        public void DeleteTagMapping(int tagID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from TagMapping where TagID = {0}", tagID));
        }

        public IEnumerable<IStatsData> GetStats()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from Stats").Tables[0];
            return Util.TableToStructure(dt, typeof(StatsData)).Cast<StatsData>().ToList();
        }

        public IEnumerable<IStatsData> GetStats(int gameFileID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from Stats where GameFileID = {0}", gameFileID)).Tables[0];
            return Util.TableToStructure(dt, typeof(StatsData)).Cast<StatsData>().ToList();
        }

        public IEnumerable<IStatsData> GetStats(IEnumerable<IGameFile> gameFiles)
        {
            if (!gameFiles.Any())
                return Array.Empty<IStatsData>();

            var gameFileIds = gameFiles.Select(x => x.GameFileID);
            DataTable dt = DataAccess.ExecuteSelect($"select * from Stats where GameFileID in ({string.Join(",", gameFileIds)})").Tables[0];
            return Util.TableToStructure(dt, typeof(StatsData)).Cast<StatsData>().ToList();
        }

        public void InsertStats(IStatsData stats)
        {
            string insert = InsertStatement("Stats", stats, new string[] { "StatID", "SaveFile" }, out List<DbParameter> parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateStats(IStatsData stats)
        {
            string query = @"update Stats set SourcePortID = @SourcePortID where StatID = @StatID";

            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("SourcePortID", stats.SourcePortID),
                DataAccess.DbAdapter.CreateParameter("StatID", stats.StatID)
            };

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public void DeleteStatsByFile(int gameFileID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Stats where GameFileID = {0}", gameFileID));
        }

        public void DeleteStats(int statID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Stats where StatID = {0}", statID));
        }

        public void DeleteStats(ISourcePortData sourcePort)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Stats where SourcePortID = {0}", sourcePort.SourcePortID));
        }

        public IEnumerable<IGameProfile> GetGameProfiles(int gameFileID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from GameProfiles where GameFileID = {0}", gameFileID)).Tables[0];
            return Util.TableToStructure(dt, typeof(GameProfile)).Cast<IGameProfile>();
        }

        public void InsertGameProfile(IGameProfile gameProfile)
        {
            string insert = InsertStatement("GameProfiles", gameProfile, new string[] { "GameProfileID" }, out List <DbParameter> parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateGameProfile(IGameProfile gameProfile)
        {
            string query = @"update GameProfiles set Name = @Name, SourcePortID = @SourcePortID, IWadID = @IWadID,
                    SettingsMap = @SettingsMap, SettingsSkill = @SettingsSkill, SettingsExtraParams = @SettingsExtraParams, SettingsFiles = @SettingsFiles,
                    SettingsFilesSourcePort = @SettingsFilesSourcePort, SettingsFilesIWAD = @SettingsFilesIWAD,
                    SettingsSpecificFiles = @SettingsSpecificFiles, SettingsStat = @SettingsStat, SettingsLoadLatestSave =@SettingsLoadLatestSave, SettingsSaved = @SettingsSaved
                    where GameProfileID = @gameProfileID";

            List<DbParameter> parameters = new List<DbParameter>
            {
                DataAccess.DbAdapter.CreateParameter("Name", gameProfile.Name ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SourcePortID", gameProfile.SourcePortID ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("IWadID", gameProfile.IWadID ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsMap", gameProfile.SettingsMap ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsSkill", gameProfile.SettingsSkill ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsExtraParams", gameProfile.SettingsExtraParams ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsFiles", gameProfile.SettingsFiles ?? (object)DBNull.Value ),
                DataAccess.DbAdapter.CreateParameter("SettingsFilesSourcePort", gameProfile.SettingsFilesSourcePort ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsFilesIWAD", gameProfile.SettingsFilesIWAD ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsSpecificFiles", gameProfile.SettingsSpecificFiles ?? (object)DBNull.Value),
                DataAccess.DbAdapter.CreateParameter("SettingsLoadLatestSave", gameProfile.SettingsLoadLatestSave),
                DataAccess.DbAdapter.CreateParameter("SettingsStat", gameProfile.SettingsStat),
                DataAccess.DbAdapter.CreateParameter("SettingsSaved", gameProfile.SettingsSaved),
                DataAccess.DbAdapter.CreateParameter("GameProfileID", gameProfile.GameProfileID),
            };

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public void DeleteGameProfile(int gameProfileID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from GameProfiles where GameProfileID = {0}", gameProfileID));
        }

        public IEnumerable<CleanupFile> GetCleanupFiles()
        {
            return Util.TableToStructure(DataAccess.ExecuteSelect("select * from CleanupFiles").Tables[0], typeof(CleanupFile)).Cast<CleanupFile>();
        }

        public void InsertCleanupFile(CleanupFile file)
        {
            string insert = InsertStatement("CleanupFiles", file, new string[] { "CleanupFileID" }, out List<DbParameter> parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void DeleteCleanupFile(CleanupFile file)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from CleanupFiles where CleanupFileID = {0}", file.CleanupFileID));
        }

        private string InsertStatement(string tableName, object obj, out List<DbParameter> parameters)
        {
            return InsertStatement(tableName, obj, new string[] { }, out parameters);
        }

        private string InsertStatement(string tableName, object obj, string[] exclude, out List<DbParameter> parameters)
        {
            StringBuilder sb = new StringBuilder("insert into ");
            sb.Append(tableName);
            sb.Append(" (");

            parameters = new List<DbParameter>();
            PropertyInfo[] properties = obj.GetType().GetProperties().Where(p => p.GetSetMethod() != null && p.GetGetMethod() != null &&
                                             !exclude.Contains(p.Name)).ToArray();

            foreach(PropertyInfo pi in properties)
            {
                sb.Append(pi.Name);
                sb.Append(',');
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(") values(");

            foreach(PropertyInfo pi in properties)
            {
                sb.Append("@");
                sb.Append(pi.Name);
                sb.Append(',');

                object value = pi.GetValue(obj);

                if (value == null)
                    value = DBNull.Value;

                parameters.Add(DataAccess.DbAdapter.CreateParameter(pi.Name, value));
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            return sb.ToString();
        }

        private static List<DbParameter> RemoveUnknownColumnsFromParameters(DataTable dt, List<DbParameter> parameters)
        {
            List<DbParameter> badParameters = new List<DbParameter>();

            foreach (var parameter in parameters)
            {
                if (!ColumnExists(dt, parameter.ParameterName))
                    badParameters.Add(parameter);
            }

            return parameters.Except(badParameters).ToList();
        }

        private static StringBuilder RemoveUnknownColumnsFromQuery(DataTable dt, StringBuilder query)
        {
            Regex regex = new Regex(@"@\S+");
            var matches = regex.Matches(query.ToString());

            foreach (Match match in matches)
            {
                string columnName = match.Value.Replace("@", string.Empty).Replace(",", string.Empty);
                if (!ColumnExists(dt, columnName))
                {
                    string replace = $"{columnName} = @{columnName}";
                    if (match.Value.EndsWith(","))
                        replace += ",";
                    query.Replace(replace, string.Empty);
                }
            }

            Regex whereFix = new Regex(@",\s+where");
            var whereMatch = whereFix.Match(query.ToString());
            if (whereMatch.Success)
                query.Replace(whereMatch.Value, " where");

            return query;
        }

        private static bool ColumnExists(DataTable dt, string columnName)
        {
            return dt.Select($"name = '{columnName}'").Any();
        }

        private DataTable GetTableColumns(string tableName)
        {
            return DataAccess.ExecuteSelect($"pragma table_info({tableName});").Tables[0];
        }

        private DataAccess DataAccess { get; set; }
        public IDatabaseAdapter DbAdapter { get; private set; }
        public string ConnectionString { get; private set; }

        private readonly bool m_outOfDateDatabase;
    }
}
