using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher.GameStores
{
    public class GameStoreFiles
    {
        public List<string> InstalledIWads { get; }
        public List<string> InstalledPWads { get; }
        public string InstalledDoom64Exe { get; }

        public static readonly GameStoreFiles EMPTY = new GameStoreFiles(new List<string>(), new List<string>(), null);

        public GameStoreFiles(List<string> installedIWads, List<string> installedPWads, string installedDoom64Exe)
        {
            InstalledIWads = new List<string>(installedIWads);
            InstalledPWads = new List<string>(installedPWads);
            InstalledDoom64Exe = installedDoom64Exe;
        }

        public GameStoreFiles Combine(GameStoreFiles other) =>
            new GameStoreFiles(
                installedIWads: CombineListsIgnoreRepeats(InstalledIWads, other.InstalledIWads),
                installedPWads: CombineListsIgnoreRepeats(InstalledPWads, other.InstalledPWads),
                installedDoom64Exe: InstalledDoom64Exe ?? other.InstalledDoom64Exe);

        private static bool HasSameFileName(string file1, string file2) =>
            Path.GetFileName(file1).Equals(Path.GetFileName(file2));

        private static List<string> CombineListsIgnoreRepeats(List<string> existingItems, List<string> newItems)
        {
            var resultList = new List<string>(existingItems);
            foreach (var newItem in newItems)
            {
                if (!resultList.Exists(existingItem => HasSameFileName(newItem, existingItem)))
                    resultList.Add(newItem);
            }
            return resultList;
        }

        public override bool Equals(object obj)
        {
            if (obj is GameStoreFiles)
            {
                GameStoreFiles other = (GameStoreFiles)obj;
                return InstalledIWads.SequenceEqual(other.InstalledIWads) 
                    && InstalledPWads.SequenceEqual(other.InstalledPWads)
                    && (InstalledDoom64Exe != null && InstalledDoom64Exe.Equals(other.InstalledDoom64Exe)) || InstalledDoom64Exe == other.InstalledDoom64Exe;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (InstalledIWads, InstalledPWads, InstalledDoom64Exe).GetHashCode();
        }
    }
}
