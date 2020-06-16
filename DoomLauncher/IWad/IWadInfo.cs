using System.IO;

namespace DoomLauncher
{
    class IWadInfo
    {
        public readonly string Title;

        public IWadInfo(string title)
        {
            Title = title;
        }

        public static IWadInfo GetIWadInfo(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName).ToUpper();

            switch (name)
            {
                case "DOOM1":
                    return new IWadInfo("Doom Shareware");
                case "DOOM":
                    return new IWadInfo("The Ultimate Doom");
                case "DOOM2":
                    return new IWadInfo("Doom II: Hell on Earth");
                case "PLUTONIA":
                    return new IWadInfo("Final Doom: The Plutonia Experiment");
                case "TNT":
                    return new IWadInfo("Final Doom: TNT: Evilution");
                case "FREEDOOM1":
                    return new IWadInfo("Freedoom: Phase 1");
                case "FREEDOOM2":
                    return new IWadInfo("Freedoom: Phase 2");
                case "FREEDM":
                    return new IWadInfo("FreeDM");
                case "CHEX":
                    return new IWadInfo("Chex Quest");
                case "CHEX3":
                    return new IWadInfo("Chex Quest 3");
                case "HACX":
                    return new IWadInfo("Hacx: Twitch 'n Kill");
                case "HERETIC1":
                    return new IWadInfo("Heretic Shareware");
                case "HERETIC":
                    return new IWadInfo("Heretic: Shadow of the Serpent Riders");
                case "HEXEN":
                    return new IWadInfo("Hexen: Beyond Heretic");
                case "STRIFE0":
                    return new IWadInfo("Strife Demo");
                case "STRIFE1":
                    return new IWadInfo("Strife: Quest for the Sigil");
                default:
                    break;
            }

            return null;
        }  
    }
}
