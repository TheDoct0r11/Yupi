/**
     Because i love chocolat...                                      
                                    88 88  
                                    "" 88  
                                       88  
8b       d8 88       88 8b,dPPYba,  88 88  
`8b     d8' 88       88 88P'    "8a 88 88  
 `8b   d8'  88       88 88       d8 88 ""  
  `8b,d8'   "8a,   ,a88 88b,   ,a8" 88 aa  
    Y88'     `"YbbdP'Y8 88`YbbdP"'  88 88  
    d8'                 88                 
   d8'                  88     
   
   Private Habbo Hotel Emulating System
   @author Claudio A. Santoro W.
   @author Kessiler R.
   @version dev-beta
   @license MIT
   @copyright Sulake Corporation Oy
   @observation All Rights of Habbo, Habbo Hotel, and all Habbo contents and it's names, is copyright from Sulake
   Corporation Oy. Yupi! has nothing linked with Sulake. 
   This Emulator is Only for DEVELOPMENT uses. If you're selling this you're violating Sulakes Copyright.
*/

using System;
using System.Data;
using MySql.Data.MySqlClient;
using Yupi.Core.Io;
using Yupi.Data.Base.Connections;
using Yupi.Data.Base.Sessions.Interfaces;

namespace Yupi.Data.Base.Sessions
{
    public class QueryAdapter : IRegularQueryAdapter
    {
        protected IDatabaseClient Client;
        protected MySqlCommand CommandMySql;

        public QueryAdapter(IDatabaseClient client)
        {
            Client = client;
        }

        private static bool DbEnabled => ConnectionManager.DbEnabled;

        public void AddParameter(string name, byte[] data)
        {
            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default: // MySQL
                    CommandMySql.Parameters.Add(new MySqlParameter(name, MySqlDbType.Blob, data.Length));
                    break;
            }
        }

        public void AddParameter(string parameterName, object val)
        {
            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default: // MySQL
                    CommandMySql.Parameters.AddWithValue(parameterName, val);
                    break;
            }
        }

        public bool FindsResult()
        {
            if (!DbEnabled)
                return false;

            bool hasRows;

            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default:
                    try
                    {
                        using (MySqlDataReader reader = CommandMySql.ExecuteReader())
                            hasRows = reader.HasRows;
                    }
                    catch (Exception exception)
                    {
                        Writer.LogQueryError(exception, CommandMySql.CommandText);
                        throw;
                    }
                    break;
            }

            return hasRows;
        }

        public int GetInteger()
        {
            if (!DbEnabled)
                return 0;

            return int.Parse(CommandMySql.ExecuteScalar().ToString());
        }

        public DataRow GetRow()
        {
            if (!DbEnabled)
                return null;

            DataRow row = null;

            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default:
                    try
                    {
                        DataSet dataSet = new DataSet();

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(CommandMySql))
                            adapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count == 1)
                            row = dataSet.Tables[0].Rows[0];
                    }
                    catch (Exception exception)
                    {
                        Writer.LogQueryError(exception, CommandMySql.CommandText);
                        throw;
                    }

                    break;
            }

            return row;
        }

        public string GetString()
        {
            if (!DbEnabled)
                return string.Empty;

            return CommandMySql.ExecuteScalar().ToString();
        }

        public DataTable GetTable()
        {
            DataTable dataTable = new DataTable();

            if (!DbEnabled)
                return dataTable;

            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default:
                    try
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(CommandMySql))
                            adapter.Fill(dataTable);
                    }
                    catch (Exception exception)
                    {
                        Writer.LogQueryError(exception, CommandMySql.CommandText);
                        throw;
                    }
                    break;
            }

            return dataTable;
        }

        public long InsertQuery()
        {
            if (!DbEnabled)
                return 0L;

            CommandMySql.ExecuteScalar();

            return CommandMySql.LastInsertedId;
        }

        public void RunFastQuery(string query)
        {
            if (!DbEnabled)
                return;

            SetQuery(query);

            RunQuery();
        }

        public void RunQuery()
        {
            if (!DbEnabled)
                return;

            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default:
                    try
                    {
                        CommandMySql.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Writer.LogQueryError(exception, CommandMySql.CommandText);
                        throw;
                    }
                    break;
            }
        }

        public void SetQuery(string query)
        {
            switch (ConnectionManager.DatabaseConnectionType.ToLower())
            {
                default: // MySQL
                    CommandMySql.Parameters.Clear();
                    CommandMySql.CommandText = query;
                    break;
            }
        }
    }
}