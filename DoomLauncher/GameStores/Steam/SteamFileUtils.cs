using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher.GameStores.Steam
{
    public static class SteamFileUtils
    {
        public static bool TryGetInstallDir(string filePath, out string installDir)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    dynamic fileObject = VdfConvert.Deserialize(File.ReadAllText(filePath));
                    installDir = fileObject.Value.installdir?.Value?.ToString();
                    return installDir != null;
                }
                catch (VdfException)
                {
                    // Invalid .vdf/.acf
                }
            }

            installDir = null;
            return false;
        }

        public static bool TryGetLibraryPaths(string filePath, out List<string> libraryPaths)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    dynamic fileObject = VdfConvert.Deserialize(File.ReadAllText(filePath));
                    libraryPaths = new List<string>();
                    foreach (var libraryFolder in fileObject.Value)
                    {
                        List<VToken> tokens = libraryFolder.Value.Children();
                        var properties = tokens.OfType<VProperty>();
                        var path = properties.FirstOrDefault(x => x.Key == "path")?.Value.Value<string>();
                        if (path != null)
                        {
                            libraryPaths.Add(path);
                        }
                    }
                    if (libraryPaths.Any())
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                    when (ex is VdfException || ex is InvalidCastException)
                {
                    // Invalid .vdf/.acf, or path has invalid value
                }
            }

            libraryPaths = null;
            return false;
        }
    }
}
