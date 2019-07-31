using System.Linq;

namespace DoomLauncher
{
    public class GameFileSearchField
    {
        private static GameFileFieldType[] s_dateTimeFields = new[]
        {
            GameFileFieldType.ReleaseDate,
            GameFileFieldType.Downloaded,
            GameFileFieldType.LastPlayed
        };

        public GameFileSearchField(GameFileFieldType type, string search)
        {
            SearchFieldType = type;
            SearchText = search;
            SearchOp = GameFileSearchOp.Equal;
        }

        public GameFileSearchField(GameFileFieldType type, GameFileSearchOp op, string search)
        {
            SearchFieldType = type;
            SearchText = search;
            SearchOp = op;
        }

        public static bool IsDateTimeField(GameFileFieldType field)
        {
            return s_dateTimeFields.ToList().Contains(field);
        }

        public GameFileSearchOp SearchOp
        {
            get;
            set;
        }

        public GameFileFieldType SearchFieldType
        {
            get;
            set;
        }

        public string SearchText
        {
            get;
            set;
        }
    }
}
