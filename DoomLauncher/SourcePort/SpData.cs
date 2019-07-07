using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.SourcePort
{
    public class SpData
    {
        public SpData() { }

        public SpData(string value)
        {
            Value = value;
        }

        public SpData(string value, IGameFile gameFile, IEnumerable<IGameFile> addFiles)
        {
            Value = value;
            GameFile = gameFile;
            AdditionalFiles = addFiles.ToList();
        }

        public string Value { get; private set; }
        public IGameFile GameFile { get; private set; }
        public List<IGameFile> AdditionalFiles { get; private set; }
    }
}
