using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DoomLauncher
{
    class IdGamesDataAdapater : IGameFileDataSourceAdapter
    {
        static string[] s_queryLookup = new string[] { 
            "action=search&type=filename&query={0}", //Filename,
            "action=search&type=title&query={0}", //Title,
            "action=search&type=author&query={0}", //Author,
            "action=search&type=descrption&query={0}", //Description,
            string.Empty, //ReleaseDate,
            "action=get&id={0}" }; //GameFileID,

        public IdGamesDataAdapater(string url, string apiPage, string mirrorUrl)
        {
            Url = url;
            ApiPage = apiPage;
            MirrorUrl = mirrorUrl;
        }

        public int GetGameFilesCount()
        {
            return 0;
        }

        public IEnumerable<IGameFile> GetGameFiles()
        {
            return GetGameFiles(null);
        }

        public IEnumerable<IGameFile> GetUntaggedGameFiles()
        {
            throw new NotSupportedException();
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options)
        {
            if (options == null)
            {
                int limit = options != null && options.Limit.HasValue ? options.Limit.Value : 25;
                return GetFiles(string.Format("action=latestfiles&limit={0}", limit), "file");
            }
            else
            {
                if (string.IsNullOrEmpty(s_queryLookup[(int)options.SearchField.SearchFieldType]))
                    return new List<IGameFile>();

                return GetFiles(string.Format(s_queryLookup[(int)options.SearchField.SearchFieldType], options.SearchField.SearchText),
                    options.SearchField.SearchFieldType == GameFileFieldType.GameFileID ? "content" : "file");
            }
        }

        public IEnumerable<IGameFile> GetGameFileIWads()
        {
            throw new NotSupportedException();
        }

        public IEnumerable<string> GetGameFileNames()
        {
            throw new NotSupportedException();
        }

        public IGameFile GetGameFile(string fileName)
        {
            GameFileSearchField sf = new GameFileSearchField(GameFileFieldType.Filename, fileName);
            return GetGameFiles(new GameFileGetOptions(sf)).FirstOrDefault();
        }

        public IEnumerable<IGameFile> GetGameFilesByName(string fileName)
        {
            GameFileSearchField sf = new GameFileSearchField(GameFileFieldType.Filename, fileName);
            return GetGameFiles(new GameFileGetOptions(sf));
        }

        public void InsertGameFile(IGameFile gameFile)
        {
            throw new NotSupportedException();
        }

        public void UpdateGameFile(IGameFile gameFile)
        {
            throw new NotSupportedException();
        }

        public void UpdateGameFile(IGameFile gameFile, GameFileFieldType[] updateFields)
        {
            throw new NotSupportedException();
        }

        public void DeleteGameFile(IGameFile gameFile)
        {
            throw new NotSupportedException();
        }

        public string Url { get; set; }
        public string ApiPage { get; set; }
        public string MirrorUrl  { get; set; }

        private IEnumerable<IGameFile> GetFiles(string query, string itemName)
        {
            WebRequest request = WebRequest.Create(string.Format(Url + ApiPage + "?" + query));
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string xmlResponse = reader.ReadToEnd();

            reader.Close();
            response.Close();

            StringReader xmlReader = new StringReader(xmlResponse);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);
            xmlReader.Dispose();

            if (ds.Tables.Contains("warning") && ds.Tables["warning"].Rows[0]["type"].ToString() == "No Results")
            {
                return new List<IGameFile>();
            }

            IEnumerable<IdGamesGameFile> files = Util.TableToStructure(ds.Tables[itemName], typeof(IdGamesGameFile)).Cast<IdGamesGameFile>();

            foreach (IdGamesGameFile file in files)
            {
                file.Description = file.Description.Replace("<br>", "\n");
            }

            return files.Cast<IGameFile>();
        }
    }
}
