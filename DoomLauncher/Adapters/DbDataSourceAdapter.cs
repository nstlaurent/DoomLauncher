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
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class DbDataSourceAdapter : IDataSourceAdapter
    {
        private static string[] s_opLookup = new string[] { "= ", "<>", "<", ">", "like" };

        public DbDataSourceAdapter(IDatabaseAdapter dbAdapter, string connectionString)
        {
            DbAdapter = dbAdapter;
            ConnectionString = connectionString;

            DataAccess = new DataAccess(dbAdapter, connectionString);
        }

        public static string GetDatabaseFileName()
        {
            return "DoomLauncher.sqlite";
        }

        public static IDataSourceAdapter CreateAdapter()
        {
            string dataSource = Path.Combine(Directory.GetCurrentDirectory(), GetDatabaseFileName());
            return new DbDataSourceAdapter(new SqliteDatabaseAdapter(), CreateConnectionString(dataSource));
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
            List<DbParameter> parameters = new List<DbParameter>();
            parameters.Add(DataAccess.DbAdapter.CreateParameter("FileName", fileName));
            DataTable dt = DataAccess.ExecuteSelect("select * from GameFiles where Filename = @FileName COLLATE NOCASE", parameters).Tables[0];

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
            List<DbParameter> parameters;
            string insert = InsertStatement("GameFiles", gameFile, new string[] { "GameFileID", "FileSizeBytes" }, out parameters);

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
                    SettingsSpecificFiles = @SettingsSpecificFiles, SettingsStat = @SettingsStat, FileName = @FileName, MapCount = @MapCount, 
                    MinutesPlayed = @MinutesPlayed
                    where GameFileID = @gameFileID");
            }

            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("Title", gameFile.Title == null ? (object)DBNull.Value : gameFile.Title));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Author", gameFile.Author == null ? (object)DBNull.Value : gameFile.Author));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("ReleaseDate", !gameFile.ReleaseDate.HasValue ? (object)DBNull.Value : gameFile.ReleaseDate.Value));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Description", gameFile.Description == null ? (object)DBNull.Value : gameFile.Description));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Map", gameFile.Map == null ? (object)DBNull.Value : gameFile.Map));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SourcePortID", !gameFile.SourcePortID.HasValue ? (object)DBNull.Value : gameFile.SourcePortID.Value));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Thumbnail", gameFile.Thumbnail == null ? (object)DBNull.Value : gameFile.Thumbnail));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Comments", gameFile.Comments == null ? (object)DBNull.Value : gameFile.Comments));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Rating", !gameFile.Rating.HasValue ? (object)DBNull.Value : gameFile.Rating));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("IWadID", !gameFile.IWadID.HasValue ? (object)DBNull.Value : gameFile.IWadID));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("GameFileID", gameFile.GameFileID.Value));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("LastPlayed", !gameFile.LastPlayed.HasValue ? (object)DBNull.Value : gameFile.LastPlayed));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Downloaded", !gameFile.Downloaded.HasValue ? (object)DBNull.Value : gameFile.Downloaded));

            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsMap", gameFile.SettingsMap == null ? (object)DBNull.Value : gameFile.SettingsMap));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsSkill", gameFile.SettingsSkill == null ? (object)DBNull.Value : gameFile.SettingsSkill));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsExtraParams", gameFile.SettingsExtraParams == null ? (object)DBNull.Value : gameFile.SettingsExtraParams));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsFiles", gameFile.SettingsFiles == null ? (object)DBNull.Value : gameFile.SettingsFiles));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsFilesSourcePort", gameFile.SettingsFilesSourcePort == null ? (object)DBNull.Value : gameFile.SettingsFilesSourcePort));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsFilesIWAD", gameFile.SettingsFilesIWAD == null ? (object)DBNull.Value : gameFile.SettingsFilesIWAD));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsSpecificFiles", gameFile.SettingsSpecificFiles == null ? (object)DBNull.Value : gameFile.SettingsSpecificFiles));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsStat", gameFile.SettingsStat));

            parameters.Add(DataAccess.DbAdapter.CreateParameter("MapCount", !gameFile.MapCount.HasValue ? (object)DBNull.Value : gameFile.MapCount));

            parameters.Add(DataAccess.DbAdapter.CreateParameter("MinutesPlayed", gameFile.MinutesPlayed));

            parameters.Add(DataAccess.DbAdapter.CreateParameter("FileName", gameFile.FileName));

            DataAccess.ExecuteNonQuery(query.ToString(), parameters);
        }

        public void UpdateGameFiles(GameFileFieldType ftWhere, GameFileFieldType ftSet, object fWhere, object fSet)
        {
            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("set1", fSet == null ? DBNull.Value : fSet));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("where1", fWhere == null ? DBNull.Value : fWhere));

            DataAccess.ExecuteNonQuery(string.Format(@"update GameFiles set {0} = @set1 where {1} = @where1", ftWhere.ToString("g"), ftSet.ToString("g")), parameters);
        }

        public void DeleteGameFile(IGameFile gameFile)
        {
            if (gameFile.GameFileID.HasValue)
            {
                DataAccess.ExecuteNonQuery(string.Format("delete from GameFiles where GameFileID = {0}", gameFile.GameFileID));
            }
        }

        public IEnumerable<ISourcePort> GetSourcePorts()
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from SourcePorts where LaunchType = {0} order by Name collate nocase", (int)SourcePortLaunchType.SourcePort)).Tables[0];

            List<ISourcePort> sourcePorts = new List<ISourcePort>();

            foreach(DataRow dr in dt.Rows)                       
                sourcePorts.Add(CreateSourcePortDataSource(dt, dr));

            return sourcePorts;
        }

        public IEnumerable<ISourcePort> GetUtilities()
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from SourcePorts where LaunchType = {0} order by Name collate nocase", (int)SourcePortLaunchType.Utility)).Tables[0];

            List<ISourcePort> sourcePorts = new List<ISourcePort>();

            foreach (DataRow dr in dt.Rows)
                sourcePorts.Add(CreateSourcePortDataSource(dt, dr));

            return sourcePorts;
        }

        private static ISourcePort CreateSourcePortDataSource(DataTable dt, DataRow dr)
        {
            SourcePort sourcePort = new SourcePort();
            sourcePort.Directory = new LauncherPath((string)dr["Directory"]);
            sourcePort.Executable = (string)dr["Executable"];
            sourcePort.Name = (string)dr["Name"];
            if (dt.Columns.Contains("SettingsFiles"))
                sourcePort.SettingsFiles = (string)CheckDBNull(dr["SettingsFiles"], string.Empty);
            sourcePort.SourcePortID = Convert.ToInt32(dr["SourcePortID"]);
            sourcePort.SupportedExtensions = (string)CheckDBNull(dr["SupportedExtensions"], string.Empty);
            sourcePort.LaunchType = (SourcePortLaunchType)Convert.ToInt32(dr["LaunchType"]);
            sourcePort.FileOption = (string)CheckDBNull(dr["FileOption"], string.Empty);
            sourcePort.ExtraParameters = (string)CheckDBNull(dr["ExtraParameters"], string.Empty);

            return sourcePort;
        }

        private static object CheckDBNull(object obj, object defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;
            else
                return obj;
        }

        public ISourcePort GetSourcePort(int sourcePortID)
        {
            DataTable dt = DataAccess.ExecuteSelect(string.Format("select * from SourcePorts where SourcePortID = {0}", sourcePortID)).Tables[0];

            if (dt.Rows.Count > 0)
                return CreateSourcePortDataSource(dt, dt.Rows[0]);

            return null;
        }

        public void InsertSourcePort(ISourcePort sourcePort)
        {
            string insert = @"insert into SourcePorts (Name,Executable,SupportedExtensions,Directory,SettingsFiles,LaunchType,FileOption,ExtraParameters) 
                values(@Name,@Executable,@SupportedExtensions,@Directory,@SettingsFiles,@LaunchType,@FileOption,@ExtraParameters)";

            DataAccess.ExecuteNonQuery(insert, GetSourcePortParams(sourcePort));
        }

        public void UpdateSourcePort(ISourcePort sourcePort)
        {
            string query = @"update SourcePorts set 
            Name = @Name, Executable = @Executable, SupportedExtensions = @SupportedExtensions,
            Directory = @Directory, SettingsFiles = @SettingsFiles, LaunchType = @LaunchType, FileOption = @FileOption, ExtraParameters = @ExtraParameters
            where SourcePortID = @sourcePortID";

            DataAccess.ExecuteNonQuery(query, GetSourcePortParams(sourcePort));
        }

        private List<DbParameter> GetSourcePortParams(ISourcePort sourcePort)
        {
            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("Name", sourcePort.Name == null ? string.Empty : sourcePort.Name));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Executable", sourcePort.Executable == null ? string.Empty : sourcePort.Executable));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SupportedExtensions", sourcePort.SupportedExtensions == null ? string.Empty : sourcePort.SupportedExtensions));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Directory", sourcePort.Directory == null ? string.Empty : sourcePort.Directory.GetPossiblyRelativePath()));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SettingsFiles", sourcePort.SettingsFiles == null ? string.Empty : sourcePort.SettingsFiles));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("SourcePortID", sourcePort.SourcePortID));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("LaunchType", sourcePort.LaunchType));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("FileOption", sourcePort.FileOption == null ? string.Empty : sourcePort.FileOption));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("ExtraParameters", sourcePort.ExtraParameters == null ? string.Empty : sourcePort.ExtraParameters));

            return parameters;
        }

        public void DeleteSourcePort(ISourcePort sourcePort)
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
            List<DbParameter> parameters;
            string insert = InsertStatement("IWads", iwad, new string[] { "IWadID" }, out parameters);

            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateIWad(IIWadData iwad)
        {
            string update = "update IWads set FileName = @FileName, Name = @Name, GameFileID = @GameFileID where IWadID = @IWadID";
            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("IWadID", iwad.IWadID));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("FileName", iwad.FileName));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Name", iwad.Name));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("GameFileID", iwad.GameFileID.HasValue ? iwad.GameFileID : (object)DBNull.Value));

            DataAccess.ExecuteNonQuery(update, parameters);
        }

        public void DeleteIWad(IIWadData iwad)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from IWads where IWadID = {0}", iwad.IWadID));
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

        public void UpdateFile(IFileData file)
        {
            string query = @"update Files set 
            SourcePortID = @SourcePortID, Description = @Description, FileOrder = @FileOrder
            where FileID = @FileID";

            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("SourcePortID", file.SourcePortID));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Description", file.Description));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("FileID", file.FileID));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("FileOrder", file.FileOrder));

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public void UpdateFiles(int sourcePortID_Where, int? sourcePortID_Set)
        {
            DataAccess.ExecuteNonQuery(string.Format("update Files set SourcePortID = @id where SourcePortID = {0}", sourcePortID_Where), 
                new DbParameter[] { DataAccess.DbAdapter.CreateParameter("id", sourcePortID_Set.HasValue? sourcePortID_Set.Value : (object)DBNull.Value) });
        }

        public void InsertFile(IFileData file)
        {
            List<DbParameter> parameters;
            string insert = InsertStatement("Files", file, new string[] { "FileID" }, out parameters);

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

        public IEnumerable<IConfigurationData> GetConfiguration()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from Configuration").Tables[0];
            return Util.TableToStructure(dt, typeof(ConfigurationData)).Cast<ConfigurationData>().ToList();
        }

        public void InsertConfiguration(IConfigurationData config)
        {
            List<DbParameter> parameters;
            string insert = InsertStatement("Configuration", config, new string[] { "ConfigID" }, out parameters);

            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateConfiguration(IConfigurationData config)
        {
            string query = @"update Configuration set 
            Name = @Name, Value = @Value, AvailableValues = @AvailableValues
            where ConfigID = @ConfigID";

            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("Name", config.Name == null ? string.Empty : config.Name));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Value", config.Value == null ? string.Empty : config.Value));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("AvailableValues", config.AvailableValues == null ? string.Empty : config.AvailableValues));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("ConfigID", config.ConfigID));

            DataAccess.ExecuteNonQuery(query, parameters);
        }

        public IEnumerable<ITagData> GetTags()
        {
            DataTable dt = DataAccess.ExecuteSelect("select * from Tags").Tables[0];
            return Util.TableToStructure(dt, typeof(TagData)).Cast<TagData>().ToList();
        }

        public void InsertTag(ITagData tag)
        {
            List<DbParameter> parameters;
            string insert = InsertStatement("Tags", tag, new string[] { "TagID" }, out parameters);

            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void UpdateTag(ITagData tag)
        {
            string query = @"update Tags set 
            Name = @Name, HasTab = @HasTab, HasColor = @HasColor, Color = @Color
            where TagID = @TagID";

            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add(DataAccess.DbAdapter.CreateParameter("Name", tag.Name == null ? string.Empty : tag.Name));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("HasTab", tag.HasTab));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("HasColor", tag.HasColor));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("Color", tag.Color.HasValue ? tag.Color : (object)DBNull.Value));
            parameters.Add(DataAccess.DbAdapter.CreateParameter("TagID", tag.TagID));

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
            List<DbParameter> parameters;
            string insert = InsertStatement("TagMapping", tag, out parameters);

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

        private string InsertStatement(string tableName, object obj, out List<DbParameter> parameters)
        {
            return InsertStatement(tableName, obj, new string[] { }, out parameters);
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

        public void InsertStats(IStatsData stats)
        {
            List<DbParameter> parameters;
            string insert = InsertStatement("Stats", stats, new string[] { "StatID", "SaveFile" }, out parameters);
            DataAccess.ExecuteNonQuery(insert, parameters);
        }

        public void DeleteStatsByFile(int gameFileID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Stats where GameFileID = {0}", gameFileID));
        }

        public void DeleteStats(int statID)
        {
            DataAccess.ExecuteNonQuery(string.Format("delete from Stats where StatID = {0}", statID));
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

        private DataAccess DataAccess { get; set; }
        public IDatabaseAdapter DbAdapter { get; private set; }
        public string ConnectionString { get; private set; }
    }
}
