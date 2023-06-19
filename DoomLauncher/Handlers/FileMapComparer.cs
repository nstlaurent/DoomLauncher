using DoomLauncher.Interfaces;
using System.Collections.Generic;

namespace DoomLauncher.Handlers
{
    public class FileMapComparer : IComparer<IFileData>
    {
        public int Compare(IFileData x, IFileData y)
        {
            bool xMap = string.IsNullOrEmpty(x.Map);
            bool yMap = string.IsNullOrEmpty(y.Map);
            if (!xMap && yMap)
                return 0;

            if (xMap && !yMap)
                return -1;
            if (!xMap && yMap)
                return 1;

            return x.Map.CompareTo(y.Map);
        }
    }
}
