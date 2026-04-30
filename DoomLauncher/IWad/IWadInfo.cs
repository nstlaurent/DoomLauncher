using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class IWadInfo
    {
        public static readonly IWadInfo FreeDoom1 = new IWadInfo("FREEDOOM1", "Freedoom: Phase 1", string.Empty);
        public static readonly IWadInfo FreeDoom2 = new IWadInfo("FREEDOOM2", "Freedoom: Phase 2", string.Empty);
        public static readonly IWadInfo Doom1 = new IWadInfo("DOOM1", "Doom Shareware", "doom.png", Doom);
        public static readonly IWadInfo Doom = new IWadInfo("DOOM", "The Ultimate Doom", "doom.png", FreeDoom1);
        public static readonly IWadInfo Doom2 = new IWadInfo("DOOM2", "Doom II: Hell on Earth", "doom2.png", FreeDoom2);
        public static readonly IWadInfo Plutonia = new IWadInfo("PLUTONIA", "Final Doom: The Plutonia Experiment", "plutonia.png");
        public static readonly IWadInfo TNT = new IWadInfo("TNT", "Final Doom: TNT: Evilution", "tnt.png");
        public static readonly IWadInfo FreeDM = new IWadInfo("FREEDM", "FreeDM", string.Empty);
        public static readonly IWadInfo Chex = new IWadInfo("CHEX", "Chex Quest", "chexquest.png");
        public static readonly IWadInfo Chex3 = new IWadInfo("CHEX3", "Chex Quest 3", "chexquest3.png");
        public static readonly IWadInfo Hacx = new IWadInfo("HACX", "Hacx: Twitch 'n Kill", "hacx.png");
        public static readonly IWadInfo Heretic1 = new IWadInfo("HERETIC1", "Heretic Shareware", "heretic.png", Heretic);
        public static readonly IWadInfo Heretic = new IWadInfo("HERETIC", "Heretic: Shadow of the Serpent Riders", "heretic.png");
        public static readonly IWadInfo Hexen = new IWadInfo("HEXEN", "Hexen: Beyond Heretic", "hexen.png");
        public static readonly IWadInfo Strife0 = new IWadInfo("STRIFE0", "Strife Demo", "strife.png", Strife1);
        public static readonly IWadInfo Strife1 = new IWadInfo("STRIFE1", "Strife: Quest for the Sigil", "strife.png");
        public static readonly IWadInfo Doom64 = new IWadInfo("DOOM64", "Doom 64", "doom64.png");

        private static readonly IWadInfo[] ALL = new IWadInfo[]
        {
            Doom1, Doom,Doom2, Plutonia, TNT, FreeDoom1, FreeDoom2, FreeDM, Chex, 
            Chex3, Hacx, Heretic1, Heretic, Hexen, Strife0, Strife1, Doom64
        };

        public string GameName { get; }
        public string Title { get; }
        public string TileImage { get; }

        public string FileName { get; }

        public IWadInfo BackupGame { get; }

        private IWadInfo(string gameName, string title, string tileImage, IWadInfo backupGame = null)
        {
            GameName = gameName;
            Title = title;
            TileImage = Path.Combine(LauncherPath.GetDataDirectory(), "TileImages", tileImage);
            FileName = $"{GameName.ToLower()}.zip";
            BackupGame = backupGame;
        }

        public static bool TryGetIWadInfo(string fileName, out IWadInfo iwadInfo)
        {
            iwadInfo = FromFileName(fileName);
            return iwadInfo != null;
        }

        public static bool IsGameName(string gameName) =>
            ALL.FirstOrDefault(iwadInfo => iwadInfo.GameName == gameName) != null;

        public static IWadInfo FromGameName(string gameName) =>
            ALL.FirstOrDefault(iwadInfo => iwadInfo.GameName.Equals(gameName));

        public static IWadInfo FromFileName(string fileName) =>
            FromGameName(GetGameName(fileName));

        private static string GetGameName(string fileName) => 
            Path.GetFileNameWithoutExtension(fileName).ToUpper();

        public override string ToString() =>
            GameName;

        public override int GetHashCode() =>
            GameName.GetHashCode();

        public override bool Equals(object o) => 
            GameName.Equals((o as IWadInfo)?.GameName);
    }
}
