/*  
  FILE          : DataBaseService.cs 
  PROJECT       : PROG3070 - A03 – PROGRAMMING ABSTRACTIONS
  PROGRAMMER    : Nick Turco | 9056530
  FIRST VERSION : 2026-03-03 
  DESCRIPTION   : This class handles all the interactions between the source and destination databases. 
                  It contains the strings for the source and destination tables as well as the schemas
                  It contains the methods to determine if the database and tables exist, as well as the methods
                  to retrieve the schema, create a new table, and copy the data between the two.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Navigation;
using System.CodeDom;

namespace DataBaseManager
{
    class DataBaseServices
    {
        public static string SourceTableName = "";
        public static string DestinationTableName = "";
        public static string SourceTableSchema = "";


        //Method Name: DataBaseExists
        //Description: This method extracts the database name from the connection string
        //             It checks if it is null otherwise it first connects to the server
        //             It then queries the system for a count of databases matching the name
        //             It supplies the database with a parameter for the database name
        //Parameters:  string connectionString - connection string to the server
        //Returns:     bool - true if database name exists, otherwise false
        public static bool DataBaseExists(string connectionString)
        {
            
            SqlConnectionStringBuilder connString = new(connectionString);
            string dataBase = connString.InitialCatalog;        //extract the database name from the string

            if (string.IsNullOrEmpty(dataBase)) 
            {
                return false;               //check if database name is null
            }

            connString.InitialCatalog = "master";           //connect to master to check existence

            using SqlConnection conn = new(connString.ConnectionString);
            try
            {
                string query = "SELECT COUNT(1) FROM sys.databases WHERE name = @dbName";
                conn.Open();
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.Add("@dbName", SqlDbType.NVarChar).Value = dataBase;       //add database name as a parameter

                int count = (int)cmd.ExecuteScalar();
                return count > 0;                            // returns 1(true) if database if found otherwise 0 (false)
            }
            catch
            {
                return false;
            }
        }
        //Method Name: DatabaseTableExists
        //Description: This method checks if the table exists within a database
        //             It checks if it is null otherwise it first connects to the server
        //             It then queries the system for a count of tables matching the name
        //             It supplies the database with a parameter for the table name
        //Parameters:  string connectionString - connection string to the server
        //             string dbTable - name of the table being checked
        //Returns:     bool - true if datble name exists, otherwise false
        public static bool DatabaseTableExists(string dbTable, string connectionString)
        {
            if (string.IsNullOrEmpty(dbTable) || string.IsNullOrEmpty(connectionString))
            {
                return false;
            } 
            else
            {
                using SqlConnection connect = new(connectionString);   
                try
                {
                   
                    string query = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
                    connect.Open();
                    using SqlCommand cmd = new(query, connect);
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = dbTable;       //add table name as a parameter

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;                                // returns 1(true) if database if found otherwise 0 (false)
                } 
                catch
                {
                    return false;
                }
            }
        }
        //Method Name: GetSchema
        //Description: This method gets the schema of a given table in the database
        //             It connects to the database and searches for the table
        //             It then sends a command to get the schema for the table
        //             It stores the schema in a DataTable
        //Parameters:  string connectionString - connection string to the server
        //             string dbTable - name of the table that contains the schema    
        //Returns:     bool - true if successfully queries the schema, otherwise false. 
        public static bool GetSchema(string dbTable, string connectionString, out DataTable schema)
        {
            schema = new DataTable();
            if (string.IsNullOrEmpty(dbTable) || string.IsNullOrEmpty(connectionString))
            {
                return false;
            }
            else
            {
                using SqlConnection connect = new(connectionString);
                string query = $"SELECT * FROM dbo.{dbTable}";             // query db for table schema
                try
                {
                    connect.Open();

                    using SqlCommand command = new(query, connect);
                    using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
                    schema = reader.GetSchemaTable(); //store schema in DataTable
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
        //Method Name: CompareSchema
        //Description: This method compares the schema of the tables from the source and destination db
        //             It counts the number or rows in the schema
        //             It then iterates through each row and compares the name and data type
        //Parameters:  DataTable sourceSchema - schema of the source table
        //             DataTable destinationSchema - schema of the destination table
        //Returns:     bool - true if the schema's match, otherwise false. 
        public static bool CompareSchema(DataTable sourceSchema, DataTable destinationSchema)
        {
            if(sourceSchema.Rows.Count != destinationSchema.Rows.Count)      //check for correct number of rows
            {
                return false; 
            }
            for (int i = 0; i < sourceSchema.Rows.Count; i++) //go through each row and compare name and type
            {
                DataRow src = sourceSchema.Rows[i];
                DataRow dest = destinationSchema.Rows[i];

                string? srcName = src["ColumnName"].ToString();
                string? destName = dest["ColumnName"].ToString();

                Type srcType = (Type)src["DataType"];
                Type destType = (Type)dest["DataType"];

                if (srcName != destName)        //compare column names
                {
                    return false;
                }              
                if (srcType != destType)        //compare column data types
                {
                    return false;
                }
            }
            return true;
        }
        //Method Name: BuildCreateTableQuery
        //Description: This method creates the query that will create the table in the destination db
        //             It then iterates through each row and gets the name, datatype, size and null setting
        //             It then converts the .NET data types to SQL data types
        //             It checks if the row allows for null values
        //             it then stores the row in a list of strings
        //             Then, it concatonates the list into one query string
        //Parameters:  string tableName - name of the table that will be created in the destination db
        //             DataTable schema - schema of the table being created
        //Returns:     string - the final query string being sent to the destination db. 
        public static string BuildCreateTableQuery(string tableName, DataTable schema)
        {
            List<string> columns = [];               //create list to store schema

            foreach (DataRow row in schema.Rows)        //iterate through list and get the column names, data type, size and null option
            {
                string? columnName = row["ColumnName"].ToString();
                Type dataType = (Type)row["DataType"];
                int columnSize = row["ColumnSize"] != DBNull.Value ? (int)row["ColumnSize"] : 0;
                bool allowNull = row["AllowDBNull"] != DBNull.Value && (bool)row["AllowDBNull"];

                string sqlType = ConvertToSqlType(dataType, columnSize);            //converts to sql data types

                string nullText = allowNull ? "NULL" : "NOT NULL";

                columns.Add($"[{columnName}] {sqlType} {nullText}");        //add each rows data to the list
            }

            string columnList = string.Join(", ", columns);                 //convert the list to a single string to be sent to destination db

            return $"CREATE TABLE dbo.[{tableName}] ({columnList})";        //return the query string
        }

        //Method Name: ConvertToSqlType
        //Description: This method converts .NET data types into SQL data types
        //Parameters:  Type type - data type being checked
        //             int size - represents the column size
        //Returns:     string - the string format of the SQL data type 
        public static string ConvertToSqlType(Type type, int size)
        {
            if (type == typeof(int))
                return "INT";

            if (type == typeof(long))
                return "BIGINT";

            if (type == typeof(short))
                return "SMALLINT";

            if (type == typeof(bool))
                return "BIT";

            if (type == typeof(DateTime))
                return "DATETIME";

            if (type == typeof(decimal))
                return "DECIMAL(18,2)";

            if (type == typeof(double))
                return "FLOAT";

            if (type == typeof(string))
                return size > 0 ? $"NVARCHAR({size})" : "NVARCHAR(MAX)";

            return "NVARCHAR(MAX)";  //fall back if type is not recognized
        }

        //Method Name: CreateTableFromSchema
        //Description: This method creates the table from the schema
        //             It builds the query string with the table schema
        //             it connects to the db and sends the command to create the table
        //Parameters:  string tableName - name of the table being created
        //             DataTable schema - the table schema being used
        //             string connectionString - the connection string to the destination db
        //Returns:     bool - true if the table is successfully created, otherwise false
        public static bool CreateTableFromSchema(string tableName, DataTable schema, string connectionString)
        {
            string createQuery = BuildCreateTableQuery(tableName, schema); //create the query string
            try
            {
                using SqlConnection conn = new(connectionString);
                conn.Open();

                using SqlCommand cmd = new(createQuery, conn);
                cmd.ExecuteNonQuery();                              //execute query
                return true;    
            }
            catch 
            { 
                return false;
            }
                         
        }

        //Method Name: CopyTableData
        //Description: This method copies the data from one table to another.
        //             It connects to both the source and destination db
        //             Then it uses a transaction to perform the data transfer
        //             It get the data from the source table, the reader reads what comes back
        //             It then attempts to bulk copy the data to the destination table
        //             if it is successful, it will commit the change, otherwise it will roll back the query
        //Parameters:  takes no parameters
        //Returns:     bool - true if the table is successfully created, otherwise false
        public static bool CopyTableData()
        {
            using SqlConnection sourceConnection = new(ConnectionString.SourceConnection);               //connect to the source and destination
            using SqlConnection destinationConnection = new(ConnectionString.DestinationConnection);
            SqlTransaction? transaction = null;                                         //use transaction for rollback

            try
            {
                sourceConnection.Open();                            
                destinationConnection.Open();

                transaction = destinationConnection.BeginTransaction();             //begin the transaction

                string selectQuery = "SELECT * FROM dbo.[" + SourceTableName + "]";
                using SqlCommand selectCommand = new(selectQuery, sourceConnection);

                using SqlDataReader reader = selectCommand.ExecuteReader(); //read what the command brings back

                using SqlBulkCopy bulkCopy = new(destinationConnection, SqlBulkCopyOptions.Default, transaction); //setup the bulk copy

                bulkCopy.DestinationTableName = "dbo.[" + DestinationTableName + "]";

                bulkCopy.WriteToServer(reader);         //write the data to the recieving db

                transaction.Commit();                   //commit if everything goes well

                return true;
            }
            catch
            {
                transaction?.Rollback();         //only roll back if there is an actual transaction created

                sourceConnection.Close();
                destinationConnection.Close();

                return false;
            }
        }

    }
}
