using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class IWadData : IIWadData
    {
        public int IWadID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public int? GameFileID { get; set; }

        public override bool Equals(object obj)
        {
            IIWadData iwad = obj as IIWadData;

            if (iwad != null)
            {
                return iwad.FileName == FileName;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return FileName.GetHashCode();
        }
    }
}
