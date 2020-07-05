using DoomLauncher.Interfaces;
using System.Data.Common;
using System.Data.SQLite;

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
