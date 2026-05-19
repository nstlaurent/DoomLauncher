using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DoomLauncher.GameStores.Steam
{
    public static class SteamFileUtils
    {
        public static bool TryLoadToObject<T>(string filePath, out T fileObject)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    fileObject = VdfConvert
                        .Deserialize(File.ReadAllText(filePath))
                        .ToJson()
                        .Value
                        .ToObject<T>();
                    return true;
                }
                catch (Exception ex)
                    when (ex is VdfException || ex is JsonSerializationException || ex is JsonReaderException)
                {
                    // Invalid .vdf/.acf, or not expected structure
                }
            }

            fileObject = default;
            return false;
        }
    }
}
