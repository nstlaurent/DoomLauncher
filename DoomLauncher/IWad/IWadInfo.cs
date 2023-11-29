using System.IO;

namespace DoomLauncher
{
    class IWadInfo
    {
        public readonly string Title;
        public readonly string TileImage;
        public readonly bool HasMetadata;

        public IWadInfo(string title, string tileImage, bool hasMeta = false)
        {
            Title = title;
            TileImage = Path.Combine("TileImages", tileImage);
            HasMetadata = hasMeta;
        }

        public static bool TryGetIWadInfo(string fileName, out IWadInfo iwadInfo)
        {
            iwadInfo = GetIWadInfo(fileName);
            return iwadInfo != null;
        }

        public static IWadInfo GetIWadInfo(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName).ToUpper();

            switch (name)
            {
                case "DOOM1":
                    return new IWadInfo("Doom Shareware", "doom.png");
                case "DOOM":
                    return new IWadInfo("The Ultimate Doom", "doom.png");
                case "DOOM2":
                    return new IWadInfo("Doom II: Hell on Earth", "doom2.png");
                case "PLUTONIA":
                    return new IWadInfo("Final Doom: The Plutonia Experiment", "plutonia.png");
                case "TNT":
                    return new IWadInfo("Final Doom: TNT: Evilution", "tnt.png");
                case "FREEDOOM1":
                    return new IWadInfo("Freedoom: Phase 1", string.Empty);
                case "FREEDOOM2":
                    return new IWadInfo("Freedoom: Phase 2", string.Empty);
                case "FREEDM":
                    return new IWadInfo("FreeDM", string.Empty);
                case "CHEX":
                    return new IWadInfo("Chex Quest", "chexquest.png");
                case "CHEX3":
                    return new IWadInfo("Chex Quest 3", "chexquest3.png");
                case "HACX":
                    return new IWadInfo("Hacx: Twitch 'n Kill", "hacx.png");
                case "HERETIC1":
                    return new IWadInfo("Heretic Shareware", "heretic.png");
                case "HERETIC":
                    return new IWadInfo("Heretic: Shadow of the Serpent Riders", "heretic.png");
                case "HEXEN":
                    return new IWadInfo("Hexen: Beyond Heretic", "hexen.png");
                case "STRIFE0":
                    return new IWadInfo("Strife Demo", "strife.png");
                case "STRIFE1":
                    return new IWadInfo("Strife: Quest for the Sigil", "strife.png");
                default:
                    break;
            }

            return null;
        }  
    }
}
