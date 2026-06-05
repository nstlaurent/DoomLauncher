using System;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public enum IWadType
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

    public class IWadInfo
    {
        public static readonly IWadInfo FreeDoom1 = new IWadInfo(IWadType.FreeDoom1, "Freedoom: Phase 1", string.Empty);
        public static readonly IWadInfo FreeDoom2 = new IWadInfo(IWadType.FreeDoom2, "Freedoom: Phase 2", string.Empty);
        public static readonly IWadInfo Doom1 = new IWadInfo(IWadType.Doom1, "Doom Shareware", "doom.png", Doom);
        public static readonly IWadInfo Doom = new IWadInfo(IWadType.Doom, "The Ultimate Doom", "doom.png", FreeDoom1);
        public static readonly IWadInfo Doom2 = new IWadInfo(IWadType.Doom2, "Doom II: Hell on Earth", "doom2.png", FreeDoom2);
        public static readonly IWadInfo Plutonia = new IWadInfo(IWadType.Plutonia, "Final Doom: The Plutonia Experiment", "plutonia.png");
        public static readonly IWadInfo TNT = new IWadInfo(IWadType.TNT, "Final Doom: TNT: Evilution", "tnt.png");
        public static readonly IWadInfo FreeDM = new IWadInfo(IWadType.FreeDM, "FreeDM", string.Empty);
        public static readonly IWadInfo Chex = new IWadInfo(IWadType.Chex, "Chex Quest", "chexquest.png");
        public static readonly IWadInfo Chex3 = new IWadInfo(IWadType.Chex3, "Chex Quest 3", "chexquest3.png");
        public static readonly IWadInfo Hacx = new IWadInfo(IWadType.Hacx, "Hacx: Twitch 'n Kill", "hacx.png");
        public static readonly IWadInfo Heretic1 = new IWadInfo(IWadType.Heretic1, "Heretic Shareware", "heretic.png", Heretic);
        public static readonly IWadInfo Heretic = new IWadInfo(IWadType.Heretic, "Heretic: Shadow of the Serpent Riders", "heretic.png");
        public static readonly IWadInfo Hexen = new IWadInfo(IWadType.Hexen, "Hexen: Beyond Heretic", "hexen.png");
        public static readonly IWadInfo Strife0 = new IWadInfo(IWadType.Strife0, "Strife Demo", "strife.png", Strife1);
        public static readonly IWadInfo Strife1 = new IWadInfo(IWadType.Strife1, "Strife: Quest for the Sigil", "strife.png");
        public static readonly IWadInfo Doom64 = new IWadInfo(IWadType.Doom64, "Doom 64", "doom64.png");

        public static readonly IWadInfo[] All = new IWadInfo[]
        {
            Doom1, Doom,Doom2, Plutonia, TNT, FreeDoom1, FreeDoom2, FreeDM, Chex,
            Chex3, Hacx, Heretic1, Heretic, Hexen, Strife0, Strife1, Doom64
        };

        public IWadType IWadType { get; }

        public string GameName { get; private set; }

        public string Title { get; }
        public string TileImage { get; }

        public IWadInfo BackupGame { get; }

        private IWadInfo(IWadType iWadType, string title, string tileImage, IWadInfo backupGame = null)
        {
            IWadType = iWadType;
            Title = title;
            TileImage = Path.Combine(LauncherPath.GetDataDirectory(), "TileImages", tileImage);
            BackupGame = backupGame;
            GameName = IWadType.ToString();
        }

        public static bool TryGetIWadInfo(string fileName, out IWadInfo iwadInfo)
        {
            iwadInfo = FromFileName(fileName);
            return iwadInfo != null;
        }

        public static bool IsGameName(string gameName) =>
            All.FirstOrDefault(iwadInfo => iwadInfo.GameName.Equals(gameName, StringComparison.OrdinalIgnoreCase)) != null;

        public static IWadInfo FromGameName(string gameName) =>
            All.FirstOrDefault(iwadInfo => iwadInfo.GameName.Equals(gameName, StringComparison.OrdinalIgnoreCase));

        public static IWadInfo FromFileName(string fileName) =>
            FromGameName(GetGameName(fileName));

        private static string GetGameName(string fileName) =>
            Path.GetFileNameWithoutExtension(fileName).ToUpper();

        public override string ToString() =>
            GameName;

        public override int GetHashCode() =>
            IWadType.GetHashCode();

        public override bool Equals(object o)
        {
            if (!(o is IWadInfo other))
                return false;

            return IWadType == other.IWadType;
        }
    }
}
