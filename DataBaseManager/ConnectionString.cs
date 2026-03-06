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
        static public string? SourceConnection = null;
        static public string? DestinationConnection = null;

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
        //Parameters:  string tableName - name of the table
        //Returns:     bool - true if tableName is valid, otherwise false
        static public bool ValidateTableName(string tableName)
        {
          return string.IsNullOrEmpty(tableName);
        }
        //Method Name: BuildSourceConnectionString
        //Description: inputs the server and database names into the connection string
        //Parameters:  string server - name of the server
        //             string database - name of database
        //Returns:     void
        static public void BuildSourceConnectionString(string server, string database)
        {
            string connectionString = $"Data Source={server}; Initial Catalog={database}; Integrated Security=True;TrustServerCertificate=True;";

            SourceConnection = connectionString;
        }
        //Method Name: BuildDestinationConnectionString
        //Description: inputs the server and database names into the connection string
        //Parameters:  string server - name of the server
        //             string database - name of database
        //Returns:     void
        static public void BuildDestinationConnectionString(string server, string database)
        {
            string connectionString = $"Data Source={server}; Initial Catalog={database}; Integrated Security=True;TrustServerCertificate=True;";

            DestinationConnection = connectionString;
        }
        
    }
}
