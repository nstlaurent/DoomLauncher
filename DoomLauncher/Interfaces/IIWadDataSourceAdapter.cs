using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Interfaces
{
    public interface IIWadDataSourceAdapter
    {
        IEnumerable<IIWadData> GetIWads();
        IIWadData GetIWad(int gameFileID);
        void InsertIWad(IIWadData iwad);
        void DeleteIWad(IIWadData iwad);
        void UpdateIWad(IIWadData iwad);
    }
}
