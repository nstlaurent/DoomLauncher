using System.Windows.Forms;

namespace DoomLauncher
{
    public class GameFileViewFactory
    {
        public bool IsUsingColumnView => DefaultType == GameFileViewType.GridView;
        public bool IsUsingTileView => DefaultType != GameFileViewType.GridView;
        public GameFileViewType DefaultType { get; private set; }

        private static readonly Padding TileMargin = new Padding(8, 8, 8, 8);

        private readonly ToolTipDisplayHandler m_toolTipDisplayHandler;

        public GameFileViewFactory(MainForm form, GameFileViewType defaultType)
        {
            DefaultType = defaultType;
            m_toolTipDisplayHandler = new ToolTipDisplayHandler(form);
        }

        public static bool IsBaseViewTypeChange(GameFileViewType type, GameFileViewType other)
        {
            if (type == GameFileViewType.GridView && other != GameFileViewType.GridView)
                return true;
            if (other == GameFileViewType.GridView && type != GameFileViewType.GridView)
                return true;

            return false;
        }

        public void UpdateDefaultType(GameFileViewType defaultType)
        {
            DefaultType = defaultType;
        }

        public static ColumnField[] DefaultColumnTextFields
        {
            get
            {
                return new ColumnField[]
                {
                    new ColumnField("FileNameNoPath", "File"),
                    new ColumnField("LastDirectory", "Directory"),
                    new ColumnField("Title", "Title"),
                    new ColumnField("Author", "Author"),
                    new ColumnField("ReleaseDate", "Release Date"),
                    new ColumnField("MapCount", "Maps"),
                    new ColumnField("Comments", "Comments"),
                    new ColumnField("Rating", "Rating"),
                    new ColumnField("Downloaded", "Downloaded"),
                    new ColumnField("LastPlayed", "Last Played"),
                };
            }
        }

        public IGameFileView CreateGameFileView()
        {
            IGameFileView view;

            switch (DefaultType)
            {
                case GameFileViewType.GridView:
                    view = new GameFileViewControl();
                    break;
                case GameFileViewType.TileView:
                case GameFileViewType.TileViewCondensed:
                    view = new GameFileTileViewControl();
                    break;
                default:
                    view = new GameFileViewControl();
                    break;
            }

            m_toolTipDisplayHandler.RegisterView(view);
            return view;
        }

        public GameFileTileBase CreateTile()
        {
            if (DefaultType == GameFileViewType.TileView)
                return new GameFileTileExpanded { Margin = TileMargin };
            else if (DefaultType == GameFileViewType.TileViewCondensed)
                return new GameFileTile { Margin = TileMargin };

            return null;
        }

        public static IGameFileView CreateGameFileViewGrid()
        {
            return new GameFileViewControl();
        }
    }
}
