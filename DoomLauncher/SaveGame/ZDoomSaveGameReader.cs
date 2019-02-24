using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.SaveGame
{
    public class ZDoomSaveGameReader : ISaveGameReader
    {
        private readonly string m_file;

        public ZDoomSaveGameReader(string file)
        {
            m_file = file;
        }

        public string GetName()
        {
            string name = GetNameFromJson(m_file);

            if (name == null)
                name = GetNameFromBinary(m_file);

            if (name == null)
                return Path.GetFileName(m_file);

            return name;
        }

        private static string GetNameFromBinary(string file)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(file));
            long startPos = Util.ReadAfter(ms, Encoding.UTF8.GetBytes("tEXtTitle"));

            if (startPos != -1)
            {
                ms.Position = ++startPos;
                long endPos = Util.ReadAfter(ms, new byte[] { 0, 0, 0 });

                if (endPos != -1)
                {
                    int totalLength = (int)(endPos - startPos - 7);
                    if (totalLength > 0)
                    {
                        if (totalLength > 24)
                            totalLength = 24;
                        ms.Position = startPos;
                        byte[] strData = new byte[totalLength];
                        ms.Read(strData, 0, totalLength);
                        return Encoding.UTF8.GetString(strData);
                    }
                }
            }

            return null;
        }

        private static string GetNameFromJson(string file)
        {
            try
            {
                using (ZipArchive za = ZipFile.OpenRead(file))
                {
                    var entry = za.Entries.FirstOrDefault(x => x.Name.Equals("info.json"));

                    if (entry != null)
                    {
                        using (var stream = new StreamReader(entry.Open()))
                        {
                            JObject obj = JsonConvert.DeserializeObject(stream.ReadToEnd()) as JObject;
                            var saveName = obj.Values().FirstOrDefault(x => x.Path == "Title");
                            if (saveName != null)
                                return saveName.ToString();
                        }
                    }
                }
            }
            catch
            {
                //continue to return the filename if we fail for any reason
            }

            return null;
        }
    }
}
