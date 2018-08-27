using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    class DataAccess
    {
        public DataAccess(IDatabaseAdapter dbAdapter, string connectionString)
        {
            DbAdapter = dbAdapter;
            ConnectionString = connectionString;
        }

        public DataSet ExecuteSelect(string sql)
        {
            DbConnection conn = DbAdapter.CreateConnection(ConnectionString);
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            DataSet ds = new DataSet();
            DbDataAdapter adapter = DbAdapter.CreateAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(ds);

            conn.Close();

            return ds;
        }

        public DataSet ExecuteSelect(string sql, IEnumerable<DbParameter> parameters)
        {
            DbConnection conn = DbAdapter.CreateConnection(ConnectionString);
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            foreach (DbParameter dbParam in parameters)
            {
                cmd.Parameters.Add(dbParam);
            }

            DataSet ds = new DataSet();
            DbDataAdapter adapter = DbAdapter.CreateAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(ds);

            conn.Close();

            return ds;
        }

        public void ExecuteNonQuery(string sql)
        {
            DbConnection conn = DbAdapter.CreateConnection(ConnectionString);
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void ExecuteNonQuery(string sql, IEnumerable<DbParameter> parameters)
        {
            DbConnection conn = DbAdapter.CreateConnection(ConnectionString);
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            
            foreach(DbParameter dbParam in parameters)
            {
                cmd.Parameters.Add(dbParam);
            }

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public IDatabaseAdapter DbAdapter { get; private set; }
        public string ConnectionString { get; private set; }
    }
}
