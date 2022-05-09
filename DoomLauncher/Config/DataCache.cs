using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WadReader;

namespace DoomLauncher
{
    class DataCache
    {
        public static readonly DataCache Instance = new DataCache();

        public event EventHandler TagsChanged;

        public IDataSourceAdapter DataSourceAdapter { get; private set; }
        public AppConfiguration AppConfiguration { get; private set; }
        public ITagMapLookup TagMapLookup { get; private set; }
        public ITagData[] Tags { get; private set; }
        public ITagData[] PreviousTags { get; private set; }
        public Image DefaultImage { get; private set; }
        public Palette DefaultPalette { get; private set; }

        public void Init(IDataSourceAdapter adapter)
        {
            DataSourceAdapter = adapter;
            AppConfiguration = new AppConfiguration(adapter);
            TagMapLookup = new TagMapLookup(adapter);
            DefaultImage = Image.FromFile(Path.Combine(LauncherPath.GetDataDirectory(), "TileImages", "DoomLauncherTile.png"));
            DefaultPalette = Palette.From(Properties.Resources.DoomPalette);

            UpdateTags();
        }

        public void UpdateTags()
        {
            if (Tags != null)
                PreviousTags = Tags.ToArray();

            Tags = SortTags(DataSourceAdapter.GetTags()).ToArray();
            if (PreviousTags == null)
                PreviousTags = Tags;

            TagsChanged?.Invoke(this, EventArgs.Empty);
        }

        // Sorts tags by Favorite, then by Name
        public IEnumerable<ITagData> SortTags(IEnumerable<ITagData> tags) => tags.OrderByDescending(x => x.Favorite).ThenBy(x => x.Name);

        public void UpdateGameFileTags(IEnumerable<IGameFile> gameFiles, IEnumerable<ITagData> tags)
        {
            ITagData[] changedTagsAll = Array.Empty<ITagData>();

            foreach (IGameFile gameFile in gameFiles)
            {
                ITagData[] existingTags = Instance.TagMapLookup.GetTags(gameFile);

                var addTags = tags.Except(existingTags);
                var removeTags = existingTags.Except(tags);
                var updateFiles = new IGameFile[] { gameFile };

                foreach (var tag in addTags)
                    AddGameFileTag(updateFiles, tag, out _);

                foreach (var tag in removeTags)
                    RemoveGameFileTag(updateFiles, tag);

                changedTagsAll = changedTagsAll.Union(addTags.Union(removeTags)).ToArray();
            }

            TagMapLookup.Refresh(changedTagsAll);
        }

        public void AddGameFileTag(IEnumerable<IGameFile> gameFiles, ITagData tag, out List<IGameFile> alreadyTagged)
        {
            alreadyTagged = new List<IGameFile>();

            foreach (IGameFile gameFile in gameFiles)
            {
                TagMapping tagMapping = new TagMapping
                {
                    FileID = gameFile.GameFileID.Value,
                    TagID = tag.TagID
                };

                if (!DataSourceAdapter.GetTagMappings(tagMapping.FileID).Contains(tagMapping))
                    DataSourceAdapter.InsertTagMapping(tagMapping);
                else
                    alreadyTagged.Add(gameFile);
            }
        }

        public void RemoveGameFileTag(IEnumerable<IGameFile> gameFiles, ITagData tag)
        {
            TagMapping tagMapping = new TagMapping();
            foreach (IGameFile gameFile in gameFiles)
            {
                tagMapping.TagID = tag.TagID;
                tagMapping.FileID = gameFile.GameFileID.Value;
                DataSourceAdapter.DeleteTagMapping(tagMapping);
            }
        }

        public ColumnConfig[] GetColumnConfig()
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ColumnConfig[]));
                StringReader text = new StringReader(AppConfiguration.ColumnConfig);
                ColumnConfig[] ret = xml.Deserialize(text) as ColumnConfig[];

                // Previous revisions of Doom Launcher use filename, most recent version uses filenamenopath - fix it here
                var columnsToFix = ret.Where(x => x.Column.Equals("filename", StringComparison.OrdinalIgnoreCase));
                foreach (var colFix in columnsToFix)
                    colFix.Column = "filenamenopath";

                var idGamesReleaseDate = ret.FirstOrDefault(x => x.Parent.Equals(TabKeys.IdGamesKey) && x.Column.Equals("releasedate", StringComparison.OrdinalIgnoreCase));
                if (idGamesReleaseDate == null)
                    return InsertIdGamesReleaseDate(ret);

                return ret;
            }
            catch
            {
                return Array.Empty<ColumnConfig>();
            }
        }

        private static ColumnConfig[] InsertIdGamesReleaseDate(ColumnConfig[] ret)
        {
            var columnList = ret.ToList();
            var idGamesFirstCol = columnList.FirstOrDefault(x => x.Parent.Equals(TabKeys.IdGamesKey));
            if (idGamesFirstCol != null)
            {
                columnList.Insert(columnList.IndexOf(idGamesFirstCol) + 2, new ColumnConfig()
                {
                    Parent = TabKeys.IdGamesKey,
                    Column = "releasedate",
                    Width = 100
                });
            }

            return columnList.ToArray();
        }

        public static string SerializeColumnConfig(List<ColumnConfig> config)
        {
            StringWriter text = new StringWriter();
            XmlSerializer xml = new XmlSerializer(typeof(ColumnConfig[]));
            xml.Serialize(text, config.ToArray());
            return text.ToString();
        }

        public void UpdateConfig(IEnumerable<IConfigurationData> config, string name, string value)
        {
            IConfigurationData configFind = config.FirstOrDefault(x => x.Name == name);

            if (configFind == null)
            {
                DataSourceAdapter.InsertConfiguration(new ConfigurationData
                {
                    Name = name,
                    Value = value,
                    UserCanModify = false
                });
            }
            else
            {
                configFind.Value = value;
                DataSourceAdapter.UpdateConfiguration(configFind);
            }
        }

    }
}
