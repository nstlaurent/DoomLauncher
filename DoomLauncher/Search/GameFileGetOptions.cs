namespace DoomLauncher
{
    public class GameFileGetOptions : IGameFileGetOptions
    {
        public GameFileGetOptions()
        {
        }

        public GameFileGetOptions(GameFileSearchField sf)
        {
            SearchField = sf;
        }

        public GameFileGetOptions(GameFileFieldType[] selectFields)
        {
            SelectFields = selectFields;
        }

        public GameFileGetOptions(GameFileFieldType[] selectFields, GameFileSearchField searchField)
        {
            SelectFields = selectFields;
            SearchField = searchField;
        }

        public GameFileGetOptions(int limit)
        {
            Limit = limit;
        }

        public GameFileFieldType[] SelectFields { get; set; }
        public GameFileSearchField SearchField { get; set; }
        public GameFileFieldType? OrderField { get; set; }
        public OrderType? OrderBy { get; set; }
        public int? Limit { get; set; }
    }
}
