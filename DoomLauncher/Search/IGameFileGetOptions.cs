using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public interface IGameFileGetOptions
    {
        GameFileFieldType[] SelectFields { get; set; }
        GameFileSearchField SearchField { get; set; }
        GameFileFieldType? OrderField { get; set; }
        OrderType? OrderBy { get; set; }
        int? Limit { get; set; }
    }
}
