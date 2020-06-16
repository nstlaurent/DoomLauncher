using Newtonsoft.Json;
using System.Collections.Generic;

namespace DoomLauncher.DataSources
{
    class WadArchiveGameFile : GameFile
    {
        public override string FileName
        {
            get
            {
                if (filenames != null && filenames.Length > 0)
                    return filenames[0];
                return null;
            }
            set
            {
                if (filenames == null)
                    filenames = new string[1];
                filenames[0] = value;
            }
        }
        [JsonProperty("extra")]
        public override string Title { get; set; }
        [JsonProperty("size")]
        public override int FileSizeBytes { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("port")]
        public string port { get; set; }
        [JsonProperty("links")]
        public string[] links { get; set; }
        [JsonProperty("filenames")]
        public string[] filenames { get; set; }
        [JsonProperty("screenshots")]
        public Dictionary<string, string> screenshots { get; set; }
    }

    class ScreenshotItem
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
    }
}
