﻿/**
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

using System.Data;
using MySql.Data.MySqlClient;
using Yupi.Emulator.Data.Base.Adapters.Interfaces;
using Yupi.Emulator.Data.Base.Clients.Interfaces;

namespace Yupi.Emulator.Data.Base.Adapters
{
    class BasicQueryAdapter : IQueryAdapter
    {
        protected IDatabaseClient Client;

        protected MySqlCommand Command;

        public BasicQueryAdapter(IDatabaseClient client)
        {
            Client = client;

            Command = client.CreateCommand();
        }

        public void AddParameter(string parameterName, object value)
        {
            if (!Command.Parameters.Contains(parameterName))
                Command.Parameters.AddWithValue(parameterName, value.ToString());
        }

        public void AddParameter(string parameterName, byte[] data)
        {
            if (!Command.Parameters.Contains(parameterName))
                Command.Parameters.Add(new MySqlParameter(parameterName, MySqlDbType.Blob, data.Length));
        }

        public int GetInteger()
        {
            if (!Client.IsAvailable())
                return 0;

            int result;

            object integerResult = Command.ExecuteScalar();

            if (integerResult == null)
                return 0;

            int.TryParse(integerResult.ToString(), out result);

            return result;
        }

        public DataRow GetRow()
        {
            if (!Client.IsAvailable())
                return null;

            DataRow dataRow = null;

            DataSet dataSet = new DataSet();

            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(Command))
                dataAdapter.Fill(dataSet);

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count == 1)
                dataRow = dataSet.Tables[0].Rows[0];

            return dataRow;
        }

        public string GetString()
        {
            if (!Client.IsAvailable())
                return string.Empty;

            object stringResult = Command.ExecuteScalar();

            if (stringResult != null)
                return stringResult.ToString();

            return string.Empty;
        }

        public DataTable GetTable()
        {
            if (!Client.IsAvailable())
                return null;

            DataTable dataTable = new DataTable();

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(Command))
                adapter.Fill(dataTable);

            return dataTable;
        }

        public long InsertQuery()
        {
            if (!Client.IsAvailable())
                return 0L;

            Command.ExecuteScalar();

            return Command.LastInsertedId;
        }

        public void RunFastQuery(string query)
        {
            if (!Client.IsAvailable())
                return;

            SetQuery(query);

            RunQuery();
        }

        public void RunQuery()
        {
            if (!Client.IsAvailable())
                return;

            Command.ExecuteNonQuery();
        }

        public void SetQuery(string query)
        {
            Command.Parameters.Clear();

            Command.CommandText = query;
        }

        public void Dispose()
        {
            Command.Dispose();

            Client.ReportDone();
        }
    }
}
