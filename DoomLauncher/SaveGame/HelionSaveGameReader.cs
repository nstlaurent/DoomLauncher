using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DoomLauncher.SaveGame
{
    public class HelionSaveGameReader : ISaveGameReader
    {
        private readonly string m_file;

        public HelionSaveGameReader(string file)
        {
            m_file = file;
        }

        public string GetName()
        {
            try
            {
                string name = TryGetSaveName(m_file);
                if (!string.IsNullOrEmpty(name))
                    return name;

                return Path.GetFileName(m_file);
            }
            catch(Exception)
            {
                return Path.GetFileName(m_file);
            }
        }

        private static string TryGetSaveName(string fileName)
        {
            ZipArchiveReader zipArchive = new ZipArchiveReader(fileName);
            var entry = zipArchive.Entries.FirstOrDefault(x => x.Name.Equals("save.json", StringComparison.OrdinalIgnoreCase));

            if (entry == null)
                return null;

            JObject data = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(entry.ReadEntry())) as JObject;
            JToken textData = data.GetValue("Text");
            if (textData == null)
                return null;

            return textData.ToString();
        }
    }
}
