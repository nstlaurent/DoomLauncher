using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    class LaunchData
    {
        public LaunchData(IGameFile gameFile, IEnumerable<IGameFile> additionalGameFiles)
        {
            GameFile = gameFile;
            if (additionalGameFiles == null)
                AdditionalGameFiles = new List<IGameFile>();
            else
                AdditionalGameFiles = additionalGameFiles.ToList();
            Success = true;
        }

        public LaunchData(string errorTitle, string errorDescritpion)
        {
            Success = false;
            ErrorTitle = errorTitle;
            ErrorDescription = errorDescritpion;
        }

        public bool Success { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorDescription { get; set; }
        public IGameFile GameFile { get; set; }
        public List<IGameFile> AdditionalGameFiles { get; set; }
    }
}
