using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DoomLauncher
{
    class WadArchiveDataAdapter : IDataSourceAdapter
    {
        private string m_urlMD5 = "http://www.wad-archive.com/api/latest/";
        private string m_urlFilename = "http://www.wad-archive.com/wadseeker/";

        public WadArchiveGameFile Test(string file)
        {
            return GetGameFileFromMD5(file);
        }

        private WadArchiveGameFile GetGameFileFromMD5(string filePath)
        {
            var md5 = MD5.Create();

            byte[] hash = null;

            using (var stream = File.OpenRead(filePath))
            {
                hash = md5.ComputeHash(stream);
            }

            string md5_string = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

            return GetGameFileRequest(m_urlMD5 + md5_string);
        }

        private WadArchiveGameFile GetGameFileByName(string name)
        {
            return GetGameFileRequest(m_urlFilename + name);
        }

        private WadArchiveGameFile GetGameFileRequest(string url)
        {
            WebRequest request = WebRequest.Create(string.Format(url));
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string data = reader.ReadToEnd();

            if (data != "[]")
            {
                if (data.EndsWith(",\"screenshots\":[]}"))
                    data = data.Replace(",\"screenshots\":[]", string.Empty);
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                return JsonConvert.DeserializeObject<WadArchiveGameFile>(data, settings);
            }

            return null;
        }

        public int GetGameFilesCount()
        {
            return 0;
        }

        public IEnumerable<IGameFile> GetGameFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGameFile> GetUntaggedGameFiles()
        {
            throw new NotSupportedException();
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options)
        {
            if (options.SearchField.SearchFieldType == GameFileFieldType.MD5)
            {
                return new WadArchiveGameFile[] { GetGameFileFromMD5(options.SearchField.SearchText) };
            }
            else
            {
                throw new NotSupportedException("Only GamefileFieldType.MD5 is supported.");
            }
        }

        public IEnumerable<IGameFile> GetGameFileIWads()
        {
            throw new NotSupportedException();
        }

        public IEnumerable<string> GetGameFileNames()
        {
            throw new NotImplementedException();
        }

        public IGameFile GetGameFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void InsertGameFile(IGameFile gameFile)
        {
            throw new NotImplementedException();
        }

        public void UpdateGameFile(IGameFile gameFile)
        {
            throw new NotImplementedException();
        }

        public void UpdateGameFile(IGameFile gameFile, GameFileFieldType[] updateFields)
        {
            throw new NotImplementedException();
        }

        public void DeleteGameFile(IGameFile gameFile)
        {
            throw new NotImplementedException();
        }

        public void UpdateGameFiles(GameFileFieldType ftWhere, GameFileFieldType ftSet, object fWhere, object fSet)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISourcePortData> GetSourcePorts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISourcePortData> GetUtilities()
        {
            throw new NotImplementedException();
        }


        public ISourcePortData GetSourcePort(int sourcePortID)
        {
            throw new NotImplementedException();
        }

        public void InsertSourcePort(ISourcePortData sourcePort)
        {
            throw new NotImplementedException();
        }

        public void UpdateSourcePort(ISourcePortData sourcePort)
        {
            throw new NotImplementedException();
        }

        public void DeleteSourcePort(ISourcePortData sourcePort)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileData> GetFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileData> GetFiles(IGameFile gameFile)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileData> GetFiles(IGameFile gameFile, FileType fileTypeID)
        {
            WadArchiveGameFile archiveGameFile = gameFile as WadArchiveGameFile;

            if (archiveGameFile != null)
            {
                List<WadArchiveFile> ret = new List<WadArchiveFile>();

                if (archiveGameFile.screenshots != null)
                {
                    foreach (var item in archiveGameFile.screenshots)
                    {
                        WadArchiveFile file = new WadArchiveFile();
                        file.FileName = item.Value;
                        ret.Add(file);
                    }
                }

                return ret;
            }
            else
            {
                throw new ArgumentException("Parameter gameFile must be of type WadArchiveGameFile");
            }
        }

        public void UpdateFiles(int sourcePortID_Where, int? sourcePortID_Set)
        {
            throw new NotImplementedException();
        }

        public void InsertFile(IFileData file)
        {
            throw new NotImplementedException();
        }

        public void UpdateFile(IFileData file)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(IFileData file)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(IGameFile file)
        {
            throw new NotImplementedException();
        }

        public void DeleteFiles(ISourcePortData sourcePort, FileType fileTypeID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGameFile> GetGameFiles(ITagData tag)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGameFile> GetGameFiles(IGameFileGetOptions options, ITagData tag)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIWadData> GetIWads()
        {
            throw new NotImplementedException();
        }

        public IIWadData GetIWad(int gameFileID)
        {
            throw new NotImplementedException();
        }

        public void InsertIWad(IIWadData iwad)
        {
            throw new NotImplementedException();
        }

        public void DeleteIWad(IIWadData iwad)
        {
            throw new NotImplementedException();
        }

        public void UpdateIWad(IIWadData iwad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IConfigurationData> GetConfiguration()
        {
            throw new NotImplementedException();
        }

        public void InsertConfiguration(IConfigurationData config)
        {
            throw new NotImplementedException();
        }

        public void UpdateConfiguration(IConfigurationData config)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITagData> GetTags()
        {
            throw new NotImplementedException();
        }

        public void InsertTag(ITagData tag)
        {
            throw new NotImplementedException();
        }

        public void UpdateTag(ITagData tag)
        {
            throw new NotImplementedException();
        }

        public void DeleteTag(ITagData tag)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITagMapping> GetTagMappings()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITagMapping> GetTagMappings(int gameFileID)
        {
            throw new NotImplementedException();
        }

        public void InsertTagMapping(ITagMapping tag)
        {
            throw new NotImplementedException();
        }

        public void DeleteTagMapping(ITagMapping tag)
        {
            throw new NotImplementedException();
        }

        public void DeleteTagMapping(int tagID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStatsData> GetStats()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStatsData> GetStats(int gameFileID)
        {
            throw new NotImplementedException();
        }

        public void InsertStats(IStatsData stats)
        {
            throw new NotImplementedException();
        }

        public void UpdateStats(IStatsData stats)
        {
            throw new NotImplementedException();
        }

        public void DeleteStatsByFile(int gameFileID)
        {
            throw new NotImplementedException();
        }

        public void DeleteStats(int statID)
        {
            throw new NotImplementedException();
        }

        public void DeleteStats(ISourcePortData sourcePort)
        {
            throw new NotImplementedException();
        }
    }
}
