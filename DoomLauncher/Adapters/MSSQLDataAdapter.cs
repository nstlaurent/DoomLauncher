using DoomLauncher.Interfaces;
using System.Data.Common;
using System.Data.SqlClient;

namespace DoomLauncher
{
    class MSSQLDataAdapter : IDatabaseAdapter
    {
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public DbDataAdapter CreateAdapter()
        {
            return new SqlDataAdapter();
        }

        public DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}
