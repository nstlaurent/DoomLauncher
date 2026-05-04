using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

#pragma warning disable CA2100 //sql injection warning, these all use db parameters anyway

namespace DoomLauncher
{
    public class DataAccess
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
            ExecuteNonQuery(sql, new DbParameter[0], false);
        }

        public int ExecuteInsertionNonQuery(string sql, IEnumerable<DbParameter> parameters)
        {
            return ExecuteNonQuery(sql, parameters, true);
        }

        public int ExecuteInsertionNonQuery(string sql) => 
            ExecuteNonQuery(sql, new DbParameter[0], true);

        public void ExecuteNonQuery(string sql, IEnumerable<DbParameter> parameters) => 
            ExecuteNonQuery(sql, parameters, false);

        private int ExecuteNonQuery(string sql, IEnumerable<DbParameter> parameters, bool returnInsertedId)
        {
            DbConnection conn = DbAdapter.CreateConnection(ConnectionString);
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            foreach (DbParameter dbParam in parameters)
            {
                cmd.Parameters.Add(dbParam);
            }

            cmd.ExecuteNonQuery();

            int insertedId = -1;
            if (returnInsertedId)
            {
                insertedId = GetLastInsertedId(conn);
            }

            conn.Close();
            return insertedId;
        }

        private int GetLastInsertedId(DbConnection openConnection)
        {
            // Retrieve the last inserted FileID
            var sql = "SELECT last_insert_rowid();";

            DbCommand cmd = openConnection.CreateCommand();
            cmd.CommandText = sql;

            DataSet ds = new DataSet();
            DbDataAdapter adapter = DbAdapter.CreateAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(ds);

            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public IDatabaseAdapter DbAdapter { get; private set; }
        public string ConnectionString { get; private set; }
    }
}
