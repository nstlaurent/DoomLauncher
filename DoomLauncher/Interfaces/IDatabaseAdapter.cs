using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace DoomLauncher.Interfaces
{
    public interface IDatabaseAdapter
    {
        DbConnection CreateConnection(string connectionString);
        DbDataAdapter CreateAdapter();
        DbParameter CreateParameter(string name, object value);
    }
}
