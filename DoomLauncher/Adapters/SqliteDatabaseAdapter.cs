using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoomLauncher.Interfaces;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;

namespace DoomLauncher
{
    public class SqliteDatabaseAdapter : IDatabaseAdapter
    {
        public DbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        public DbDataAdapter CreateAdapter()
        {
            return new SQLiteDataAdapter();
        }

        public DbParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }
    }
}
