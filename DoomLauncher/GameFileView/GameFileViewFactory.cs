namespace DoomLauncher
{
    public class GameFileViewFactory
    {
        public bool IsUsingColumnView => false;

        private readonly GameFileViewType m_defaultType;
        private readonly ToolTipDisplayHandler m_toolTipDisplayHandler = new ToolTipDisplayHandler();

        public static ColumnField[] DefaultColumnTextFields
        {
            get
            {
                return new ColumnField[]
                {
                    new ColumnField("FileNameNoPath", "File"),
                    new ColumnField("Title", "Title"),
                    new ColumnField("Author", "Author"),
                    new ColumnField("ReleaseDate", "Release Date"),
                    new ColumnField("MapCount", "Maps"),
                    new ColumnField("Comments", "Comments"),
                    new ColumnField("Rating", "Rating"),
                    new ColumnField("Downloaded", "Downloaded"),
                    new ColumnField("LastPlayed", "Last Played")
                };
            }
        }

        public GameFileViewFactory(GameFileViewType defaultType)
        {
            m_defaultType = defaultType;
        }

        public IGameFileView CreateGameFileView()
        {
            IGameFileView view;

            switch (m_defaultType)
            {
                case GameFileViewType.GridView:
                    view = new GameFileViewControl();
                    break;
                case GameFileViewType.TileView:
                    view = new GameFileTileViewControl();
                    break;
                default:
                    view = new GameFileViewControl();
                    break;
            }

            m_toolTipDisplayHandler.RegisterView(view);
            return view;
        }

        public IGameFileView CreateGameFileViewGrid()
        {
            return new GameFileViewControl();
        }
    }
}
