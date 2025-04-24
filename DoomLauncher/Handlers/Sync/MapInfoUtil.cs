using System;
using System.Linq;
using System.Text;

namespace DoomLauncher.Handlers.Sync
{
    public static class MapInfoUtil
    {
        private static readonly string[] MapInfoNames = new string[] { "mapinfo", "zmapinfo" };
        private static readonly string[] MapInfoSubNames = new string[] { "mapinfo.", "zmapinfo." };

        public static string[] GetMapInfoData(IArchiveReader reader)
        {
            var mapInfoEntries = reader.Entries.Where(IsEntryMapInfo).ToArray();
            return GetArchiveEntryData(mapInfoEntries.ToArray());
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
        private static bool IsEntryMapInfo(IArchiveEntry entry)
        {
            try
            {
                string entryName = entry.GetNameWithoutExtension();
                foreach (string name in MapInfoNames)
                {
                    if (entryName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                foreach (string name in MapInfoSubNames)
                {
                    if (entryName.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
