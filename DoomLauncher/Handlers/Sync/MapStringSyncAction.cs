using DoomLauncher.Archive;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace DoomLauncher.Handlers.Sync
{
    public class MapStringSyncAction : ISyncAction
    {
        private readonly LauncherPath m_tempDirectory;
        private static readonly Regex MapRegex = new Regex(@"\s*map\s+\w+", RegexOptions.IgnoreCase);
        private static readonly Regex IncludeRegex = new Regex(@"\s*include\s+(\S+)", RegexOptions.IgnoreCase);

        public MapStringSyncAction(LauncherPath tempDirectory)
        {
            m_tempDirectory = tempDirectory;
        }

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            var maps = GetMaps(reader, mapInfoData);
            var mapString = string.Join(", ", maps.ToArray());

            gameFile.Map = mapString;
            if (!string.IsNullOrEmpty(gameFile.Map))
                gameFile.MapCount = gameFile.Map.Count(x => x == ',') + 1;

            return SyncResult.EMPTY;
        }   

        private List<string> GetMaps(IArchiveReader reader, string[] mapInfoData)
        {
            List<string> maps = new List<string>();
            if (mapInfoData.Length > 0)
            {
                foreach (var mapInfo in mapInfoData)
                    maps.AddRange(MapStringFromMapInfo(reader, mapInfo));
            }
            else
            {
                maps.AddRange(MapStringFromGameFileWads(reader));
            }
            return maps.Distinct().ToList();
        }

        private List<string> MapStringFromMapInfo(IArchiveReader reader, string mapInfoData)
        {
            return MapStringFromMapInfo(reader, mapInfoData, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        private List<string> MapStringFromMapInfo(IArchiveReader reader, string mapInfoData, ISet<string> includedFiles)
        {
            List<string> maps = new List<string>();
            maps.AddRange(ParseMapInfoInclude(reader, mapInfoData, includedFiles));
            maps.AddRange(GetMapStringFromMapInfo(mapInfoData));

            return maps;
        }

        private List<string> ParseMapInfoInclude(IArchiveReader reader, string mapinfo, ISet<string> includedFiles)
        {
            List<string> maps = new List<string>();
            var includeFiles = new HashSet<string>(includedFiles); // copy the set to avoid modifying the original

            MatchCollection matches = IncludeRegex.Matches(mapinfo);
            foreach (Match match in matches)
            {
                if (match.Groups.Count < 2)
                    continue;

                string file = match.Groups[1].Value.Trim();
                if (includeFiles.Contains(file))
                    continue;

                includeFiles.Add(file);

                var entry = reader.Entries.FirstOrDefault(x => x.FullName.Equals(file, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                    continue;

                string[] entryData = GetArchiveEntryData(entry);
                if (entryData.Length > 0)
                    maps.AddRange(MapStringFromMapInfo(reader, entryData[0], includeFiles)); // Recurse
            }

            return maps;
        }

        private List<string> GetMapStringFromMapInfo(string mapinfo)
        {
            MatchCollection matches = MapRegex.Matches(mapinfo);
            return matches.Cast<Match>().Select(x => x.Value.Trim().Substring(3).Trim()).ToList();
        }

        private List<string> MapStringFromGameFileWads(IArchiveReader reader) =>
            GetIWadFilenames(reader).SelectMany(Util.GetMapStringFromWad).ToList();

        private List<string> GetIWadFilenames(IArchiveReader reader)
        {
            if (reader is WadArchiveReader wadArchive)
            {
                return new List<string>() { wadArchive.Filename };
            }
            else if (reader is RecursiveArchiveReader recursiveReader && recursiveReader.Root is WadArchiveReader wadArchive2)
            {
                return new List<string>() { wadArchive2.Filename };
            }
            else
            {
                IEnumerable<IArchiveEntry> wadEntries = GetEntriesByExtension(reader, ".wad");
                return wadEntries.Select(entry => Util.ExtractTempFile(m_tempDirectory.GetFullPath(), entry)).ToList();
            }
        }

        private static string[] GetArchiveEntryData(params IArchiveEntry[] entries)
        {
            string[] data = new string[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                try
                {
                    data[i] = Encoding.UTF8.GetString(entries[i].ReadEntry());
                }
                catch
                {
                    data[i] = string.Empty;
                }
            }

            return data;
        }

        private static IEnumerable<IArchiveEntry> GetEntriesByExtension(IArchiveReader reader, string ext)
        {
            return reader.Entries.Where(x => x.Name.Contains('.') && Path.GetExtension(x.Name).Equals(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
