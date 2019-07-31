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
