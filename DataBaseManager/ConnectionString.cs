using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Navigation;
using Microsoft.Data.SqlClient;

namespace DataBaseManager
{
    class ConnectionString
    {

        //connection strings for source and destination databases
        static public string? SourceConnection {  get; set; }
        static public string? DestinationConnection { get; set; }

        //Method Name: ValidateServerName
        //Description: validates the source connection string inputs
        //Parameters:  string server - name of the server
        //Returns:     bool - true if server is valid, otherwise false
        static public bool ValidateServerName(string server)
        {
          return string.IsNullOrEmpty(server);
        }
        //Method Name: ValidateDatabaseName
        //Description: validates the source connection string inputs
        //Parameters:  string database - name of the database
        //Returns:     bool - true if server is valid, otherwise false
        static public bool ValidateDatabaseName(string database)
        {
            return string.IsNullOrEmpty(database);
        }
        //Method Name: ValidateTableName
        //Description: validates the source connection string inputs
        //Parameters:  string tableName - name of the server
        //Returns:     bool - true if tableName is valid, otherwise false
        static public bool ValidateTableName(string tableName)
        {
          return string.IsNullOrEmpty(tableName);
        }

    }
}
