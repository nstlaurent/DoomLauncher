using DoomLauncher.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DoomLauncher
{
    class DataCache
    {
        public static readonly DataCache Instance = new DataCache();

        public IDataSourceAdapter DataSourceAdapter { get; private set; }
        public AppConfiguration AppConfiguration { get; private set; }
        public ITagMapLookup TagMapLookup { get; private set; }
        public ITagData[] Tags { get; private set; }
        public Image DefaultImage { get; private set; }

        public void Init(IDataSourceAdapter adapter)
        {
            DataSourceAdapter = adapter;
            AppConfiguration = new AppConfiguration(adapter);
            TagMapLookup = new TagMapLookup(adapter);
            DefaultImage = Image.FromFile(Path.Combine(LauncherPath.GetDataDirectory(), "TileImages", "DoomLauncherTile.png"));

            UpdateTags();
        }

        public void UpdateTags()
        {
            Tags = DataSourceAdapter.GetTags().OrderBy(x => x.Name).ToArray();
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

                return ret;
            }
            catch
            {
                return new ColumnConfig[] { };
            }
        }
    }
}
