using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DataBaseManager
{
    class DataBaseServices
    {
        public static bool DataBaseExists(string connectionString)
        {
            
            SqlConnectionStringBuilder connString = new(connectionString);
            string dataBase = connString.InitialCatalog; //extract the database name from the string

            if (dataBase == null) {
                return false;  //check if database name is null
            }

            connString.InitialCatalog = "master"; //connect to master to check existence

            using (SqlConnection conn = new(connString.ConnectionString))
            {
                try
                {
                    string query = "SELECT COUNT(1) FROM sys.databases WHERE name = @dbName";
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dbName", dataBase); //add database name as a parameter

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; // returns 1(true) if database if found otherwise 0 (false)
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString()); 
                }
            }

            return true;
        }
    }
}
