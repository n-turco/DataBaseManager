using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Navigation;

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

                if (srcName != destName)
                {
                    return false;
                }              
                if (srcType != destType)
                {
                    return false;
                }
            }

            return true;

        }
    }
}
