using System;
using System.IO;
using System.Linq;

namespace DoomLauncher
{


    public class IWadInfo
    {
        private enum IWadType
        {
            FreeDoom1,
            FreeDoom2,
            Doom1,
            Doom,
            Doom2,
            Plutonia,
            TNT,
            FreeDM,
            Chex,
            Chex3,
            Hacx,
            Heretic1,
            Heretic,
            Hexen,
            Strife0,
            Strife1,
            Doom64
        }

        public static readonly IWadInfo FreeDoom1 = new IWadInfo(IWadType.FreeDoom1, "FREEDOOM1", "Freedoom: Phase 1", string.Empty);
        public static readonly IWadInfo FreeDoom2 = new IWadInfo(IWadType.FreeDoom2, "FREEDOOM2", "Freedoom: Phase 2", string.Empty);
        public static readonly IWadInfo Doom1 = new IWadInfo(IWadType.Doom1, "DOOM1", "Doom Shareware", "doom.png", Doom);
        public static readonly IWadInfo Doom = new IWadInfo(IWadType.Doom, "DOOM", "The Ultimate Doom", "doom.png", FreeDoom1);
        public static readonly IWadInfo Doom2 = new IWadInfo(IWadType.Doom2, "DOOM2", "Doom II: Hell on Earth", "doom2.png", FreeDoom2);
        public static readonly IWadInfo Plutonia = new IWadInfo(IWadType.Plutonia, "PLUTONIA", "Final Doom: The Plutonia Experiment", "plutonia.png");
        public static readonly IWadInfo TNT = new IWadInfo(IWadType.TNT, "TNT", "Final Doom: TNT: Evilution", "tnt.png");
        public static readonly IWadInfo FreeDM = new IWadInfo(IWadType.FreeDM, "FREEDM", "FreeDM", string.Empty);
        public static readonly IWadInfo Chex = new IWadInfo(IWadType.Chex, "CHEX", "Chex Quest", "chexquest.png");
        public static readonly IWadInfo Chex3 = new IWadInfo(IWadType.Chex3, "CHEX3", "Chex Quest 3", "chexquest3.png");
        public static readonly IWadInfo Hacx = new IWadInfo(IWadType.Hacx, "HACX", "Hacx: Twitch 'n Kill", "hacx.png");
        public static readonly IWadInfo Heretic1 = new IWadInfo(IWadType.Heretic1, "HERETIC1", "Heretic Shareware", "heretic.png", Heretic);
        public static readonly IWadInfo Heretic = new IWadInfo(IWadType.Heretic, "HERETIC", "Heretic: Shadow of the Serpent Riders", "heretic.png");
        public static readonly IWadInfo Hexen = new IWadInfo(IWadType.Hexen, "HEXEN", "Hexen: Beyond Heretic", "hexen.png");
        public static readonly IWadInfo Strife0 = new IWadInfo(IWadType.Strife0, "STRIFE0", "Strife Demo", "strife.png", Strife1);
        public static readonly IWadInfo Strife1 = new IWadInfo(IWadType.Strife1, "STRIFE1", "Strife: Quest for the Sigil", "strife.png");
        public static readonly IWadInfo Doom64 = new IWadInfo(IWadType.Doom64, "DOOM64", "Doom 64", "doom64.png");


        private static readonly IWadInfo[] All = new IWadInfo[]
        {
            Doom1, Doom,Doom2, Plutonia, TNT, FreeDoom1, FreeDoom2, FreeDM, Chex,
            Chex3, Hacx, Heretic1, Heretic, Hexen, Strife0, Strife1, Doom64
        };

        public string GameName { get; }
        public string Title { get; }
        public string TileImage { get; }

        public string FileName { get; }

        public IWadInfo BackupGame { get; }

        private readonly IWadType _iwadType;

        private IWadInfo(IWadType iWadType, string gameName, string title, string tileImage, IWadInfo backupGame = null)
        {
            _iwadType = iWadType;
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
            All.FirstOrDefault(iwadInfo => iwadInfo.GameName == gameName) != null;

        public static IWadInfo FromGameName(string gameName) =>
            All.FirstOrDefault(iwadInfo => iwadInfo.GameName.Equals(gameName));

        public static IWadInfo FromFileName(string fileName) =>
            FromGameName(GetGameName(fileName));

        private static string GetGameName(string fileName) =>
            Path.GetFileNameWithoutExtension(fileName).ToUpper();

        public override string ToString() =>
            GameName;

        public override int GetHashCode() =>
            _iwadType.GetHashCode();

        public override bool Equals(object o)
        {
            if (!(o is IWadInfo other))
                return false;

            return _iwadType == other._iwadType;
        }
    }
}
