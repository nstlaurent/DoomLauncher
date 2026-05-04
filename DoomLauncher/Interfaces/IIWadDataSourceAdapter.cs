using System.Collections.Generic;

namespace DoomLauncher.Interfaces
{
    public interface IIWadDataSourceAdapter
    {
        IEnumerable<IIWadData> GetIWads();
        IIWadData GetIWad(int gameFileID);
        IIWadData GetIWadByIWadID(int iwadID);
        void InsertIWad(IIWadData iwad);
        void DeleteIWad(IIWadData iwad);
        void UpdateIWad(IIWadData iwad);
    }
}
