using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DoomLauncher.Handlers.Sync
{
    public class SyncResult
    {
        public static readonly SyncResult EMPTY = 
            new SyncResult(new List<IGameFile>(), new List<IGameFile>(), 
                new List<InvalidFile>(), new Dictionary<IGameFile, Image>(), new List<IGameFile>());  

        public List<IGameFile> AddedGameFiles { get; }
        public List<IGameFile> UpdatedGameFiles { get; }
        public List<InvalidFile> InvalidFiles { get; }
        public Dictionary<IGameFile, Image> TitlePics { get; }
        public List<IGameFile> FailedTitlePicFiles { get; }

        public bool Failed => InvalidFiles.Count > 0;

        public List<IGameFile> AddedOrUpdatedFiles
        {
            get => AddedGameFiles.Union(UpdatedGameFiles).ToList();
        }

        private SyncResult(List<IGameFile> addedGameFiles, List<IGameFile> updatedGameFiles, List<InvalidFile> invalidFiles,
            Dictionary<IGameFile, Image> titlePics, List<IGameFile> failedTitlePicFiles)
        {
            AddedGameFiles = new List<IGameFile>(addedGameFiles);
            UpdatedGameFiles = new List<IGameFile>(updatedGameFiles);
            InvalidFiles = new List<InvalidFile>(invalidFiles);
            TitlePics = titlePics;
            FailedTitlePicFiles = new List<IGameFile>(failedTitlePicFiles);
        }

        public static SyncResult operator +(SyncResult a, SyncResult b) => a.Combine(b);

        public SyncResult Combine(SyncResult other)
        {
            return new SyncResult(
                addedGameFiles: CombineLists(AddedGameFiles, other.AddedGameFiles),
                updatedGameFiles: CombineLists(UpdatedGameFiles, other.UpdatedGameFiles),
                invalidFiles: CombineLists(InvalidFiles, other.InvalidFiles),
                titlePics: CombineDictionariesKeepLatest(TitlePics, other.TitlePics),
                failedTitlePicFiles: CombineLists(FailedTitlePicFiles, other.FailedTitlePicFiles));
        }

        public bool GetTitlePic(IGameFile gameFile, out Image image) =>
            TitlePics.TryGetValue(gameFile, out image);


        public static SyncResult AddedGameFile(IGameFile gameFile)
        {
            return new SyncResult(
                addedGameFiles: new List<IGameFile>() { gameFile },
                updatedGameFiles: new List<IGameFile>(),
                invalidFiles: new List<InvalidFile>(),
                titlePics: new Dictionary<IGameFile, Image>(),
                failedTitlePicFiles: new List<IGameFile>());
        }

        public static SyncResult UpdatedGameFile(IGameFile gameFile)
        {
            return new SyncResult(
                addedGameFiles: new List<IGameFile>(),
                updatedGameFiles: new List<IGameFile>() { gameFile },
                invalidFiles: new List<InvalidFile>(),
                titlePics: new Dictionary<IGameFile, Image>(),
                failedTitlePicFiles: new List<IGameFile>());
        }

        public static SyncResult InvalidFile(string filename, string reason)
        {
            return new SyncResult(
                addedGameFiles: new List<IGameFile>(),
                updatedGameFiles: new List<IGameFile>(),
                invalidFiles: new List<InvalidFile>() { new InvalidFile(filename, reason) },
                titlePics: new Dictionary<IGameFile, Image>(),
                failedTitlePicFiles: new List<IGameFile>());
        }

        public static SyncResult TitlePic(IGameFile gameFile, Image image)
        {
            return new SyncResult(
                addedGameFiles: new List<IGameFile>(),
                updatedGameFiles: new List<IGameFile>() ,
                invalidFiles: new List<InvalidFile>(),
                titlePics: new Dictionary<IGameFile, Image>() { { gameFile, image } },
                failedTitlePicFiles: new List<IGameFile>());
        }

        public static SyncResult FailedTitlePicFile(IGameFile gameFile)
        {
            return new SyncResult(
                addedGameFiles: new List<IGameFile>(),
                updatedGameFiles: new List<IGameFile>(),
                invalidFiles: new List<InvalidFile>(),
                titlePics: new Dictionary<IGameFile, Image>(),
                failedTitlePicFiles: new List<IGameFile>() { gameFile });
        }

        private static List<A> CombineLists<A>(List<A> ourList, List<A> otherList)
        {
            var newList = new List<A>(ourList.Count + otherList.Count);
            newList.AddRange(ourList);
            newList.AddRange(otherList);
            return newList.Distinct().ToList();
        }

        // Keep the existing key/values
        private static Dictionary<K, V> CombineDictionariesKeepLatest<K, V>(Dictionary<K, V> ourDict, Dictionary<K, V> otherDict)
        {
            var combinedDictionary = new Dictionary<K, V>(otherDict);
            foreach (var key in ourDict.Keys)
            {
                if (!combinedDictionary.ContainsKey(key))
                    combinedDictionary[key] = ourDict[key];
            }
            return combinedDictionary;
        }
    }
}
