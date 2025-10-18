using DoomLauncher.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace DoomLauncher.SourcePort
{
    public class DoomsdaySourcePortFlavor : GenericSourcePortFlavor
    {
        public DoomsdaySourcePortFlavor(ISourcePortData sourcePortData)
            : base(sourcePortData)
        {

        }

        public override string IwadParameter(SpData data) =>
            string.Format(" -iwad \"{0}\" -game {1}", Path.GetDirectoryName(data.Value), GetGame(data));

        /**
            doom1-share	Shareware Doom v1.9
	        doom1	Registered Doom v1.9
	        doom1-ultimate	Ultimate Doom*
	        doom2	Doom 2
	        doom2-plut	Final Doom: Plutonia Experiment
	        doom2-tnt	Final Doom: TNT Evilution
	        chex	Chex Quest
	        hacx	HacX
            heretic-share	Shareware Heretic
	        heretic	Registered Heretic
	        heretic-ext	Heretic: Shadow of the Serpent Riders**
            hexen	Hexen v1.1
	        hexen-v10	Hexen v1.0
	        hexen-dk	Hexen: Death Kings of Dark Citadel
	        hexen-demo	The 4-level Hexen Demo 
         **/
        private string GetGame(SpData data)
        {
            string filename = Path.GetFileNameWithoutExtension(data.Value);

            if (filename.Equals("DOOM1", StringComparison.OrdinalIgnoreCase))
                return "doom1-share";
            if (filename.Equals("DOOM", StringComparison.OrdinalIgnoreCase))
            {
                if (data.GameFile.Map.Contains("E4M1"))
                    return "doom1-ultimate";
                else
                    return "doom1";
            }
            if (filename.Equals("DOOM2", StringComparison.OrdinalIgnoreCase))
                return "doom2";
            if (filename.Equals("PLUTONIA", StringComparison.OrdinalIgnoreCase))
                return "doom2-plut";
            if (filename.Equals("TNT", StringComparison.OrdinalIgnoreCase))
                return "doom2-tnt";
            if (filename.Equals("CHEX", StringComparison.OrdinalIgnoreCase))
                return "chex";
            if (filename.Equals("HACX", StringComparison.OrdinalIgnoreCase))
                return "hacx";
            if (filename.Equals("HERETIC", StringComparison.OrdinalIgnoreCase))
            {
                if (!data.GameFile.Map.Contains("E2M1"))
                    return "heretic-share";
                else if (data.GameFile.Map.Contains("E4M1"))
                    return "heretic-ext";
                else
                    return "heretic";
            }
            if (filename.Equals("HEXEN", StringComparison.OrdinalIgnoreCase) || filename.Equals("HEXDD", StringComparison.OrdinalIgnoreCase))
            {
                if (data.AdditionalFiles.Any(x => Path.GetFileNameWithoutExtension(x.FileName).Equals("HEXDD", StringComparison.OrdinalIgnoreCase)))
                    return "hexen-dk";
                if(data.GameFile.Map.Contains("MAP41")) //not sure why doomsday cares about this but v10 has unfinished maze map MAP41, v11 does not
                    return "hexen-v10";
                if (!data.GameFile.Map.Contains("MAP05"))
                    return "hexen-demo";
                return "hexen";
            }

            return "doom2";
        }

        public override bool Supported() => CheckFileNameWithoutExtension("doomsday");

        public override bool LoadSaveGameSupported() => false;
    }
}
